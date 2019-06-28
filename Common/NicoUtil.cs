using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using WpfUtilV2.Common;

namespace NicoV5.Common
{
    public static class NicoUtil
    {
        /// <summary>
        /// 文字列を暗号化します。
        /// </summary>
        /// <param name="data">暗号化したい文字</param>
        public static string EncryptString(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return string.Empty;
            }
            else
            {
                return Encrypter.EncryptString(data, AppSetting.ApplicationId);
            }
        }

        /// <summary>
        /// 文字列を復号化します。
        /// </summary>
        /// <param name="data">復号化したい文字</param>
        public static string DecryptString(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return string.Empty;
            }
            else
            {
                return Encrypter.DecryptString(data, AppSetting.ApplicationId);
            }
        }

        /// <summary>
        /// 指定したﾜｰﾄﾞをIDに変換します。
        /// </summary>
        /// <param name="word">ﾜｰﾄﾞ</param>
        /// <returns>ID</returns>
        public static string ToContentId(string word)
        {
            return word?.Split('/').Last();
        }

        /// <summary>
        /// 指定した文字をlong値に変換します。
        /// </summary>
        /// <param name="value">文字</param>
        /// <returns>long値</returns>
        public static long ToLong(string value)
        {
            return long.Parse(value.Replace(",", ""));
        }

        /// <summary>
        /// XMLから所定の場所にある値を取得します。
        /// </summary>
        /// <param name="e">Xｴﾚﾒﾝﾄ</param>
        /// <param name="name">名前</param>
        /// <returns></returns>
        public static string GetData(XElement e, string name)
        {
            return (string)e
                .Descendants("strong")
                .Where(x => (string)x.Attribute("class") == name)
                .FirstOrDefault();
        }

        /// <summary>
        /// Xmlｴﾚﾒﾝﾄから指定した名前に紐付くｶｳﾝﾀを取得します。
        /// </summary>
        /// <param name="e">Xmlｴﾚﾒﾝﾄ</param>
        /// <param name="name">ｶｳﾝﾀ名</param>
        /// <returns>ｶｳﾝﾀ</returns>
        public static long ToCounter(XElement e, string name)
        {
            var s = GetData(e, name);

            if (string.IsNullOrEmpty(s))
            {
                return 0;
            }
            else
            {
                return ToLong(s);
            }
        }

        /// <summary>
        /// HH:mm:ss形式の文字を合算した秒に変換します。
        /// </summary>
        /// <param name="value">HH:mm:ss形式の文字</param>
        /// <returns>合算した秒</returns>
        public static long ToLengthSeconds(string value)
        {
            var lengthSecondsIndex = 0;
            var lengthSeconds = value
                    .Split(':')
                    .Select(s => long.Parse(s))
                    .Reverse()
                    .Select(l => l * (long)Math.Pow(60, lengthSecondsIndex++))
                    .Sum();
            return lengthSeconds;
        }

        /// <summary>
        /// Unix時間(long)からDateTimeに変換します。
        /// </summary>
        /// <param name="time">Unix時間(long)</param>
        /// <returns><code>DateTime</code></returns>
        public static DateTime FromUnixTime(long time)
        {
            DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            return UnixEpoch.AddSeconds(time).ToLocalTime();
        }

        /// <summary>
        /// 文字列をUrlｴﾝｺｰﾄﾞ文字列に変換します。
        /// </summary>
        /// <param name="txt">変換前文字列</param>
        /// <returns>変換後文字列</returns>
        public static string ToUrlEncode(string txt)
        {
            return HttpUtility.UrlEncode(txt);
        }

        /// <summary>
        /// 文字をDateTimeに変換します。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime ToDatetime(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return default(DateTime);
            }
            else
            {
                //return System.DateTime.ParseExact(value,
                //    "ddd, d MMM yyyy HH\\:mm\\:ss zzz",
                //    System.Globalization.DateTimeFormatInfo.InvariantInfo,
                //    System.Globalization.DateTimeStyles.None
                //);
                return DateTime.Parse(value);
            }
        }

        /// <summary>
        /// ﾗﾝｷﾝｸﾞ画面の日付をDateTimeに変換します。
        /// </summary>
        /// <param name="e">Xmlｴﾚﾒﾝﾄ</param>
        /// <param name="name">日付名</param>
        /// <returns>日付</returns>
        public static DateTime ToRankingDatetime(XElement e, string name)
        {
            // 2018年02月27日 20：00：00
            var s = GetData(e, name);

            return System.DateTime.ParseExact(s,
                "yyyy年MM月dd日 HH：mm：ss",
                System.Globalization.DateTimeFormatInfo.InvariantInfo,
                System.Globalization.DateTimeStyles.None
            );

        }

        /// <summary>
        /// 指定したURLのｻﾑﾈｲﾙを取得します。
        /// </summary>
        /// <param name="url">URL</param>
        /// <returns></returns>
        public static async Task<BitmapImage> ToThumbnail(string url)
        {
            try
            {
                byte[] bytes = default(byte[]);

                using (var client = new HttpClient())
                {
                    bytes = await client.GetByteArrayAsync(url);
                }

                using (WrappingStream stream = new WrappingStream(new MemoryStream(bytes)))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.DecodePixelWidth = 160 + 48 * 3;
                    bitmap.DecodePixelHeight = 120 + 36 * 3;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    if (bitmap.CanFreeze)
                    {
                        bitmap.Freeze();
                    }
                    return bitmap;
                }
            }
            catch
            {
                return null;
            }
        }

        public static async Task<BitmapImage> ToThumbnail(IEnumerable<string> urls)
        {
            foreach (var url in urls)
            {
                var thumnail = await ToThumbnail(url);
                if (thumnail != null)
                {
                    return thumnail;
                }
            }
            return null;
        }
    }
}
