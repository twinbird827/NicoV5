using NicoV5.Common.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUtilV2.Databases;
using WpfUtilV2.Extensions;

namespace NicoV5.Common.Databases
{
    public static class TemporaryHistoryExtension
    {
        public static async Task<TTemporaryHistory[]> GetTemporaryHistory(this IDbControl control)
        {
            var results = new List<TTemporaryHistory>();
            var sql = new StringBuilder();
            sql.AppendLine($"SELECT id, tick");
            sql.AppendLine($"FROM   temporary_history");

            using (var reader = await control.ExecuteReaderAsync(sql.ToString()))
            {
                while (await reader.ReadAsync())
                {
                    results.Add(new TTemporaryHistory(
                        reader.GetString(0),    // id
                        reader.GetInt64(1)      // tick
                    ));
                }
            }

            return results.ToArray();
        }

        public static async Task<int> InsertOrReplaceTemporaryHistory(this IDbControl control, params TTemporaryHistory[] temporaryHistories)
        {
            var columns = new[] { "id", "tick" };

            int result = 0;

            foreach (var chunk in temporaryHistories.Chunk(500))
            {
                var sql = new StringBuilder();

                sql.AppendLine($"INSERT OR REPLACE INTO temporary_history (");
                sql.AppendLine(columns.GetString(","));
                sql.AppendLine(")");
                sql.AppendLine(
                    chunk
                        .Select(data => $"SELECT '{data.VideoId}', {data.Tick}")
                        .GetString(" UNION ALL ")
                );

                // ﾊﾟﾗﾒｰﾀ設定
                result += await control.ExecuteNonQueryAsync(sql.ToString());
            }

            return result;
        }

    }
}
