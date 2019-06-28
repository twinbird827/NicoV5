using NicoV5.Common;
using NicoV5.Common.Databases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NicoV5.Mvvm.Models
{
    public class Session
    {
        public static Session Instance { get; private set; } = new Session();

        /// <summary>
        /// ﾛｸﾞｲﾝ時ｸｯｷｰｺﾝﾃﾅ
        /// </summary>
        private CookieContainer Cookies { get; set; }

        /// <summary>
        /// ﾛｸﾞｲﾝ用ｸｯｷｰを取得します。
        /// </summary>
        /// <returns></returns>
        public async Task<CookieContainer> GetCookies()
        {
            using (var accessor = DbAccessor.GetAccessor())
            using (var control = accessor.GetCommand())
            {
                var settings = await control.GetSetting();
                var mail = settings.First(s => s.Key == SettingKeys.MailAddress).Value;
                var password = NicoUtil.DecryptString(settings.First(s => s.Key == SettingKeys.Password).Value);
                Cookies = Cookies ?? await LoginAsync(mail, password);
            }


            if (Cookies == null)
            {
                throw new ArgumentException("ﾛｸﾞｲﾝできませんでした。");
            }

            return Cookies;
        }

        /// <summary>
        /// ﾛｸﾞｲﾝし、ｸｯｷｰを受け取ります。
        /// </summary>
        /// <returns>ﾛｸﾞｲﾝ用CookieContainer</returns>
        private async Task<CookieContainer> LoginAsync(string mail, string password)
        {
            const string loginUrl = "https://secure.nicovideo.jp/secure/login?site=niconico";

            if (string.IsNullOrWhiteSpace(mail) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            try
            {
                using (var handler = new HttpClientHandler())
                using (var client = new HttpClient(handler))
                {
                    var content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "next_url", string.Empty },
                        { "mail", mail },
                        { "password", password }
                    });

                    await client.PostAsync(loginUrl, content);

                    return handler.CookieContainer;
                }
            }
            catch
            {
                return null;
            }
        }

    }
}
