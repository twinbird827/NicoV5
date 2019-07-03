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
    public static class FavoriteExtension
    {
        public static async Task<TFavorite[]> GetFavorite(this IDbControl control)
        {
            var results = new List<TFavorite>();
            var sql = new StringBuilder();
            sql.AppendLine($"SELECT id, tick");
            sql.AppendLine($"FROM   favorite");

            using (var reader = await control.ExecuteReaderAsync(sql.ToString()))
            {
                while (await reader.ReadAsync())
                {
                    results.Add(new TFavorite(
                        reader.GetString(0),    // id
                        reader.GetInt64(1)      // tick
                    ));
                }
            }

            return results.ToArray();
        }

        public static async Task<int> InsertOrReplaceFavorite(this IDbControl control, params TFavorite[] favorites)
        {
            var columns = new[] { "id", "tick" };

            int result = 0;

            foreach (var chunk in favorites.Chunk(500))
            {
                var sql = new StringBuilder();

                sql.AppendLine($"INSERT OR REPLACE INTO favorite (");
                sql.AppendLine(columns.GetString(","));
                sql.AppendLine(")");
                sql.AppendLine(
                    chunk
                        .Select(data => $"SELECT '{data.Mylist}', {data.Tick}")
                        .GetString(" UNION ALL ")
                );

                // ﾊﾟﾗﾒｰﾀ設定
                result += await control.ExecuteNonQueryAsync(sql.ToString());
            }

            return result;
        }

        public static async Task<int> DeleteFavorite(this IDbControl control, params TFavorite[] favorites)
        {
            int result = 0;

            foreach (var chunk in favorites.Chunk(500))
            {
                var sql = new StringBuilder();

                sql.AppendLine($"DELETE FROM favorite (");
                sql.AppendLine($"WHERE id IN (");
                sql.AppendLine(
                    chunk
                        .Select(data => $"'{data.Mylist}'")
                        .GetString(", ")
                );
                sql.AppendLine($")");

                // ﾊﾟﾗﾒｰﾀ設定
                result += await control.ExecuteNonQueryAsync(sql.ToString());
            }

            return result;
        }

    }
}
