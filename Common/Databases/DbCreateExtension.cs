using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUtilV2.Databases;

namespace NicoV5.Common.Databases
{
    public static class DbCreateExtension
    {
        public static async Task Initialize(this IDbControl control)
        {
            // 設定情報テーブルの作成
            await control.CreateSetting();

            // 設定情報テーブルに初期情報を登録する
            await control.InitializeSetting();

            // 視聴ﾃｰﾌﾞﾙの作成
            await control.CreateVideoHistory();
        }

        private static async Task CreateSetting(this IDbControl control)
        {
            var sql = new StringBuilder();

            sql.AppendLine($"CREATE TABLE IF NOT EXISTS setting (");
            sql.AppendLine($"    key   INTEGER NOT NULL,");
            sql.AppendLine($"    value TEXT    NOT NULL,");
            sql.AppendLine($"PRIMARY KEY (key)");
            sql.AppendLine($")");

            await control.ExecuteNonQueryAsync(sql.ToString());
        }

        private static async Task CreateVideoHistory(this IDbControl control)
        {
            var sql = new StringBuilder();

            sql.AppendLine($"CREATE TABLE IF NOT EXISTS video_history (");
            sql.AppendLine($"    id   TEXT    NOT NULL,");
            sql.AppendLine($"    tick INTEGER NOT NULL,");
            sql.AppendLine($"PRIMARY KEY (id, tick)");
            sql.AppendLine($")");

            await control.ExecuteNonQueryAsync(sql.ToString());
        }

        private static async Task CreateFavorite(this IDbControl control)
        {
            var sql = new StringBuilder();

            sql.AppendLine($"CREATE TABLE IF NOT EXISTS favorite (");
            sql.AppendLine($"    id   TEXT    NOT NULL,");
            sql.AppendLine($"    tick INTEGER NOT NULL,");
            sql.AppendLine($"PRIMARY KEY (id)");
            sql.AppendLine($")");

            await control.ExecuteNonQueryAsync(sql.ToString());
        }

    }
}
