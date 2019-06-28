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
    public static class ViewExtension
    {
        public static async Task<List<TView>> GetView(this IDbControl control)
        {
            var results = new List<TView>();
            var sql = new StringBuilder();
            sql.AppendLine($"SELECT id, tick");
            sql.AppendLine($"FROM   view");

            using (var reader = await control.ExecuteReaderAsync(sql.ToString()))
            {
                while (await reader.ReadAsync())
                {
                    results.Add(new TView(
                        reader.GetString(0),    // id
                        reader.GetInt64(1)      // tick
                    ));
                }
            }

            return results;
        }

        public static async Task<int> InsertOrReplaceView(this IDbControl control, params TView[] views)
        {
            var columns = new[] { "id", "tick" };

            int result = 0;

            foreach (var chunk in views.Chunk(500))
            {
                var sql = new StringBuilder();

                sql.AppendLine($"INSERT OR REPLACE INTO setting (");
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
