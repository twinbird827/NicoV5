using NicoV5.Common.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUtilV2.Databases;

namespace NicoV5.Common.Databases
{
    public static class FavoriteExtension
    {
        public static async Task<TFavorite[]> GetFavorite(this IDbControl control)
        {
            var results = new List<TFavorite>();
            var sql = new StringBuilder();
            sql.AppendLine($"SELECT id, tick");
            sql.AppendLine($"FROM   setting");

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

    }
}
