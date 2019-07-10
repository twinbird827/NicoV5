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
    public static class VideoHistoryExtension
    {
        public static async Task<List<TVideoHistory>> GetVideoHistory(this IDbControl control)
        {
            var results = new List<TVideoHistory>();
            var sql = new StringBuilder();
            sql.AppendLine($"SELECT id, tick");
            sql.AppendLine($"FROM   video_history");

            using (var reader = await control.ExecuteReaderAsync(sql.ToString()))
            {
                while (await reader.ReadAsync())
                {
                    results.Add(new TVideoHistory(
                        reader.GetString(0),    // id
                        reader.GetInt64(1)      // tick
                    ));
                }
            }

            return results;
        }

        public static async Task<List<VVideoHistory>> GetVideoHistoryView(this IDbControl control, int order)
        {
            var results = new List<VVideoHistory>();
            var sql = new StringBuilder();
            sql.AppendLine($"SELECT id, tick, count");
            sql.AppendLine($"FROM (");
            sql.AppendLine($"    SELECT ifnull(id, '') id, ifnull(max(tick), 0) tick, count(id) count");
            sql.AppendLine($"    FROM   video_history");
            sql.AppendLine($"    GROUP BY id");
            sql.AppendLine($")");
            sql.AppendLine($"ORDER BY {order} desc");

            using (var reader = await control.ExecuteReaderAsync(sql.ToString()))
            {
                while (await reader.ReadAsync())
                {
                    results.Add(new VVideoHistory(
                        reader.GetString(0),    // id
                        reader.GetInt64(1),     // tick
                        reader.GetInt64(2)      // count
                    ));
                }
            }

            return results;
        }

        public static async Task<int> InsertOrReplaceVideoHistory(this IDbControl control, params TVideoHistory[] views)
        {
            var columns = new[] { "id", "tick" };

            int result = 0;

            foreach (var chunk in views.Chunk(500))
            {
                var sql = new StringBuilder();

                sql.AppendLine($"INSERT OR REPLACE INTO video_history (");
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

        public static async Task<int> DeleteVideoHistory(this IDbControl control, params string[] views)
        {
            var columns = new[] { "id", "tick" };

            int result = 0;

            foreach (var chunk in views.Chunk(500))
            {
                var sql = new StringBuilder();

                sql.AppendLine($"DELETE FROM video_history ");
                sql.AppendLine($"WHERE       id IN (");
                sql.AppendLine(chunk.Select(id => $"'{id}'").GetString(","));
                sql.AppendLine($")");

                // ﾊﾟﾗﾒｰﾀ設定
                result += await control.ExecuteNonQueryAsync(sql.ToString());
            }

            return result;
        }
    }
}
