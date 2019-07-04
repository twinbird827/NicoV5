using NicoV5.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WpfUtilV2.Common;
using WpfUtilV2.Mvvm;

namespace NicoV5.Mvvm.Models
{
    public class SearchVideoModel : BindableBase
    {
        /// <summary>
        /// ｺﾝｽﾄﾗｸﾀ
        /// </summary>
        /// <param name="login">URLｱｸｾｽ時にﾛｸﾞｲﾝを必要とするか</param>
        protected SearchVideoModel(bool login)
        {
            Login = login;
        }

        /// <summary>
        /// URLｱｸｾｽ時にﾛｸﾞｲﾝを必要とするか
        /// </summary>
        private bool Login { get; set; }

        /// <summary>
        /// URLの内容を取得します。
        /// </summary>
        /// <param name="url">URL</param>
        /// <returns></returns>
        protected async Task<string> GetStringAsync(string url)
        {
            using (var handler = new HttpClientHandler())
            using (var client = new HttpClient(handler))
            {
                if (Login)
                {
                    handler.CookieContainer = await Session.Instance.GetCookies();
                }

                var txt = await client.GetStringAsync(url);

                //txt = txt.Replace("&copy;", "");
                //txt = txt.Replace("&nbsp;", " ");
                //txt = txt.Replace("&#x20;", " ");
                //txt = txt.Replace("&", "&amp;");

                return txt;
            }
        }

        /// <summary>
        /// URLの内容をJson形式で取得します。
        /// </summary>
        /// <param name="url">URL</param>
        protected async Task<dynamic> GetJsonAsync(string url)
        {
            return JsonConverter.Parse(await GetStringAsync(url));
        }

        /// <summary>
        /// URLの内容をXml形式で取得します。
        /// </summary>
        /// <param name="url">URL</param>
        protected async Task<XElement> GetXmlAsync(string url)
        {
            return ToXml(await GetStringAsync(url));
        }

        /// <summary>
        /// URLのchannelﾀｸﾞの内容をXml形式で取得します。
        /// </summary>
        /// <param name="url">URL</param>
        protected async Task<XElement> GetXmlChannelAsync(string url)
        {
            var xml = await GetXmlAsync(url);
            var tmp = xml.Descendants("channel");
            return tmp.First();
        }

        /// <summary>
        /// 文字列をXml形式に変換します。
        /// </summary>
        /// <param name="url">URL</param>
        protected XElement ToXml(string value)
        {
            return XDocument.Load(new StringReader(value)).Root;
        }

        /// <summary>
        /// Xmlからﾓﾃﾞﾙを作成します。
        /// </summary>
        /// <param name="item">Xml</param>
        /// <param name="view">viewのﾀｸﾞ名</param>
        /// <param name="mylist">mylistのﾀｸﾞ名</param>
        /// <param name="comment">commentのﾀｸﾞ名</param>
        /// <returns></returns>
        protected async Task<VideoModel> CreateVideoFromXml(XElement item, string view, string mylist, string comment)
        {
            // 明細部をXDocumentで読み込むために整形
            var descriptionString = item.Element("description").Value;

            descriptionString = descriptionString.Replace("&nbsp;", "&#x20;");
            //descriptionString = HttpUtility.HtmlDecode(descriptionString);
            //descriptionString = descriptionString.Replace("&", "&amp;");
            //descriptionString = descriptionString.Replace("'", "&apos;");

            // 明細部読み込み
            var desc = ToXml($"<root>{descriptionString}</root>");

            // 動画時間
            var lengthSecondsStr = (string)desc
                    .Descendants("strong")
                    .Where(x => (string)x.Attribute("class") == "nico-info-length")
                    .First();

            var video = new VideoModel();

            await video.Refresh(item.Element("link").Value);

            video.VideoUrl = item.Element("link").Value;
            video.Title = item.Element("title").Value;
            video.ViewCounter = NicoUtil.ToCounter(desc, view);
            video.MylistCounter = NicoUtil.ToCounter(desc, mylist);
            video.CommentCounter = NicoUtil.ToCounter(desc, comment);
            video.StartTime = NicoUtil.ToRankingDatetime(desc, "nico-info-date");
            video.ThumbnailUrl = (string)desc.Descendants("img").First().Attribute("src");
            video.LengthSeconds = NicoUtil.ToLengthSeconds(lengthSecondsStr);
            video.Description = (string)desc.Descendants("p").FirstOrDefault(x => (string)x.Attribute("class") == "nico-description");

            return video;
        }

    }
}
