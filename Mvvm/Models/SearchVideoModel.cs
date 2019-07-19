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
            try
            {
                //// 明細部をXDocumentで読み込むために整形
                //var descriptionString = item.Element("description").Value;

                //descriptionString = descriptionString.Replace("&nbsp;", "&#x20;");
                ////descriptionString = HttpUtility.HtmlDecode(descriptionString);
                //descriptionString = descriptionString.Replace("&", "&amp;");
                ////descriptionString = descriptionString.Replace("'", "&apos;");

                //// 明細部読み込み
                //var desc = ToXml($"<root>{descriptionString}</root>");

                //// 動画時間
                //var lengthSecondsStr = (string)desc
                //        .Descendants("strong")
                //        .Where(x => (string)x.Attribute("class") == "nico-info-length")
                //        .First();

                var video = await VideoModel.CreateInstance(item.Element("link").Value);

                //video.VideoUrl = item.Element("link").Value;
                video.Title = item.Element("title").Value;
                //video.ViewCounter = NicoUtil.ToCounter(desc, view);
                //video.MylistCounter = NicoUtil.ToCounter(desc, mylist);
                //video.CommentCounter = NicoUtil.ToCounter(desc, comment);
                //video.StartTime = NicoUtil.ToRankingDatetime(desc, "nico-info-date");
                //video.ThumbnailUrl = (string)desc.Descendants("img").First().Attribute("src");
                //video.LengthSeconds = NicoUtil.ToLengthSeconds(lengthSecondsStr);
                //video.Description = (string)desc.Descendants("p").FirstOrDefault(x => (string)x.Attribute("class") == "nico-description");

                /*
                <item>
                  <title>第1位：【女子2人】初めてパンの気持ちを理解する実況【I am Bread】</title>
                  <link>http://www.nicovideo.jp/watch/sm34525974</link>
                  <guid isPermaLink="false">tag:nicovideo.jp,2019-01-25:/watch/sm34525974</guid>
                  <pubDate>Thu, 31 Jan 2019 07:06:01 +0900</pubDate>
                  <description><![CDATA[
                                      <p class="nico-thumbnail"><img alt="【女子2人】初めてパンの気持ちを理解する実況【I am Bread】" src="http://tn.smilevideo.jp/smile?i=34525974.59215" width="94" height="70" border="0"/></p>
                                                <p class="nico-description">全パンの想いを背負いし者----------------------関西弁女子実況グループ『サイコロジカルサーカス』が、第13回実況者杯に参加します!フリー部門実況動画の部、謎部門にエントリー！再生数・コメント・マイリス数で順位が決まります！！応援よろよろ！( ˘ω˘ )ニコニ広告が可能です！よければ広告で宣伝もよろしくお願いします(強欲の壺)テーマは【初】この動画はフリー部門実況動画の部の動画です。 今回は映画風の始まりにしてみました！楽しんで見てもらえるとうれしい！(*^^*)今回のプログラム⇒mylist/63232841大会詳細⇒sm33431601パンフレット⇒mylist/55555016舞台袖⇒co3253598プレイメンバー…ノイジーワールド(紫)・馬面なおと(桃) Twitter…https://twitter.com/rojikaru2525</p>
                                                <p class="nico-info"><small><strong class="nico-info-number">20,404</strong>pts.｜<strong class="nico-info-length">20:04</strong>｜<strong class="nico-info-date">2019年01月25日 18：11：01</strong> 投稿<br/><strong>合計</strong>  再生：<strong class="nico-info-total-view">729</strong>  コメント：<strong class="nico-info-total-res">59</strong>  マイリスト：<strong class="nico-info-total-mylist">9</strong><br/><strong>毎時</strong>  再生：<strong class="nico-info-hourly-view">4</strong>  コメント：<strong class="nico-info-hourly-res">0</strong>  マイリスト：<strong class="nico-info-hourly-mylist">0</strong><br/></small></p>
                                  ]]></description>
                </item>
                 * */
                return video;
            }
            catch
            {
                Console.WriteLine(item);
                throw;
            }
        }

    }
}
