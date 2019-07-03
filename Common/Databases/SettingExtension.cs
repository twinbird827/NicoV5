using NicoV5.Common.Tables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUtilV2.Databases;
using WpfUtilV2.Extensions;

namespace NicoV5.Common.Databases
{
    public static class SettingExtension
    {
        public static async Task<int> InitializeSetting(this IDbControl control)
        {
            // 既に登録されている設定情報を取得
            var settings1 = await control.GetSetting();

            // 未登録時に登録する設定情報
            var settings2 = new[]
            {
                new TSetting(SettingKeys.MailAddress, ""),
                new TSetting(SettingKeys.Password, ""),
                new TSetting(SettingKeys.Browser, @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe"),
                new TSetting(SettingKeys.ComboHistorySort, "0"),
                new TSetting(SettingKeys.ComboRankGenre, "0"),
                new TSetting(SettingKeys.ComboRankPeriod, "0"),
                new TSetting(SettingKeys.ComboVMylistSort, "0"),
            };

            // 結合する
            var settings3 = settings1
                .Concat(settings2.Where(s2 => !settings1.Contains(s2)))
                .ToArray();

            // 全て登録
            return await control.InsertOrReplaceSetting(settings3);
        }

        public static async Task<int> InsertOrReplaceSetting(this IDbControl control, params TSetting[] settings)
        {
            var columns = new[] { "key", "value", "description" };

            int result = 0;

            foreach (var chunk in settings.Chunk(500))
            {
                var sql = new StringBuilder();

                sql.AppendLine($"INSERT OR REPLACE INTO setting (");
                sql.AppendLine(columns.GetString(","));
                sql.AppendLine(")");
                sql.AppendLine(
                    chunk
                        .Select(data => $"SELECT {(int)data.Key}, '{data.Value}', '{data.Description}'")
                        .GetString(" UNION ALL ")
                );

                // ﾊﾟﾗﾒｰﾀ設定
                result += await control.ExecuteNonQueryAsync(sql.ToString());
            }

            return result;
        }

        public static async Task<TSetting[]> GetSetting(this IDbControl control)
        {
            var results = new List<TSetting>();
            var sql = new StringBuilder();
            sql.AppendLine($"SELECT cast(key as text), value");
            sql.AppendLine($"FROM   setting");

            using (var reader = await control.ExecuteReaderAsync(sql.ToString()))
            {
                while (await reader.ReadAsync())
                {
                    results.Add(new TSetting(
                        (SettingKeys)Enum.Parse(typeof(SettingKeys), reader.GetString(0)),    // key
                        reader.GetString(1)                                                   // value
                    ));
                }
            }

            return results.ToArray();
        }

        public static async Task<TSetting> GetSetting(this IDbControl control, SettingKeys key)
        {
            var sql = new StringBuilder();
            sql.AppendLine($"SELECT cast(key as text), value");
            sql.AppendLine($"FROM   setting");
            sql.AppendLine($"WHERE  key = ?");

            using (var reader = await control.ExecuteReaderAsync(sql.ToString(), Sqlite3Util.CreateParameter(DbType.Int32, (int)key)))
            {
                if (await reader.ReadAsync())
                {
                    return new TSetting(
                        (SettingKeys)Enum.Parse(typeof(SettingKeys), reader.GetString(0)),    // key
                        reader.GetString(1)                                                   // value
                    );
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
