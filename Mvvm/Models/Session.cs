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
            var mail = SettingModel.Instance.MailAddress;
            var password = SettingModel.Instance.Password;
            Cookies = Cookies ?? await LoginAsync(mail, password).ConfigureAwait(false);

            if (Cookies == null)
            {
                throw new ArgumentException("ﾛｸﾞｲﾝできませんでした。");
            }

            return Cookies;
        }

        /// <summary>
        /// 指定したﾒｰﾙｱﾄﾞﾚｽ、ﾊﾟｽﾜｰﾄﾞでﾛｸﾞｲﾝできるか確認します。
        /// </summary>
        /// <param name="mail">ﾒｰﾙ</param>
        /// <param name="password">ﾊﾟｽﾜｰﾄﾞ</param>
        public bool CanLogin(string mail, string password)
        {
            bool tmp = false;

            CanLoginAsync(mail, password)
                .ContinueWith(login => tmp = login.Result);

            return tmp;
        }

        /// <summary>
        /// 指定したﾒｰﾙｱﾄﾞﾚｽ、ﾊﾟｽﾜｰﾄﾞでﾛｸﾞｲﾝできるか確認します。
        /// </summary>
        /// <param name="mail">ﾒｰﾙ</param>
        /// <param name="password">ﾊﾟｽﾜｰﾄﾞ</param>
        public async Task<bool> CanLoginAsync(string mail, string password)
        {
            try
            {
                var cookies = await LoginAsync(mail, password).ConfigureAwait(false);
                return true;
            }
            catch
            {
                return false;
            }
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

                    await client.PostAsync(loginUrl, content).ConfigureAwait(false);

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
