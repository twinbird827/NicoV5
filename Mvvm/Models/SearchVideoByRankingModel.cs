using NicoV5.Mvvm.Combos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUtilV2.Mvvm.Service;

namespace NicoV5.Mvvm.Models
{
    public class SearchVideoByRankingModel : SearchVideoModel
    {
        public SearchVideoByRankingModel() : base(false)
        {

        }

        public async Task<IEnumerable<VideoModel>> GetRanking(ComboboxItemModel period, ComboboxItemModel genre)
        {
            var results = new List<VideoModel>();
            var p = period.Value;
            var g = genre.Value;
            var t = "all";

            if (string.IsNullOrWhiteSpace(p) || string.IsNullOrWhiteSpace(g) || string.IsNullOrWhiteSpace(t))
            {
                ServiceFactory.MessageService.Error("検索ワードが入力されていません。");
                return results;
            }

            var url = $"http://www.nicovideo.jp/ranking/genre/{g}?tag={t}&term={p}&rss=2.0&lang=ja-jp";
            var xml = await GetXmlChannelAsync(url);

            foreach (var item in xml.Descendants("item"))
            {
                try
                {
                    results.Add(CreateVideoFromXml(
                        item,
                        "nico-info-total-view",
                        "nico-info-total-mylist",
                        "nico-info-total-res"
                    ));
                }
                catch (Exception ex)
                {
                    ServiceFactory.MessageService.Exception(ex);
                }
            }

            return results;
        }

    }
}
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
