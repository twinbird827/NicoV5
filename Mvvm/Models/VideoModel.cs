using NicoV5.Common;
using NicoV5.Common.Tables;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Media.Imaging;
using WpfUtilV2.Common;
using WpfUtilV2.Extensions;
using WpfUtilV2.Mvvm;

namespace NicoV5.Mvvm.Models
{
    public class VideoModel : SearchVideoModel
    {
        public VideoModel() : base(true)
        {

        }

        public VideoModel(string word) : base(true)
        {
            WpfUtil.BeginInvoke(async () => await Refresh(word));
        }

        /// <summary>
        /// ｺﾝﾃﾝﾂId (http://nico.ms/ の後に連結することでコンテンツへのURLになります)
        /// </summary>
        public string VideoUrl
        {
            get { return $"http://nico.ms/{VideoId}"; }
            set { VideoId = NicoUtil.ToContentId(value); }
        }

        /// <summary>
        /// 動画ID
        /// </summary>
        public string VideoId
        {
            get { return _VideoId; }
            set { SetProperty(ref _VideoId, value); OnPropertyChanged(nameof(VideoUrl)); }
        }
        private string _VideoId = null;

        /// <summary>
        /// ﾀｲﾄﾙ
        /// </summary>
        public string Title
        {
            get { return _Title; }
            set { SetProperty(ref _Title, HttpUtility.HtmlDecode(value)); }
        }
        private string _Title = null;

        /// <summary>
        /// ｺﾝﾃﾝﾂの説明文
        /// </summary>
        public string Description
        {
            get { return _Description; }
            set { SetProperty(ref _Description, HttpUtility.HtmlDecode(value)); }
        }
        private string _Description = null;

        /// <summary>
        /// ﾀｸﾞ (空白区切り)
        /// </summary>
        public string Tags
        {
            get { return _Tags; }
            set { SetProperty(ref _Tags, value); }
        }
        private string _Tags = null;

        /// <summary>
        /// ｶﾃｺﾞﾘﾀｸﾞ
        /// </summary>
        public string CategoryTag
        {
            get { return _CategoryTag; }
            set { SetProperty(ref _CategoryTag, value); }
        }
        private string _CategoryTag = null;

        /// <summary>
        /// 再生数
        /// </summary>
        public double ViewCounter
        {
            get { return _ViewCounter; }
            set { SetProperty(ref _ViewCounter, value); }
        }
        private double _ViewCounter = default(int);

        /// <summary>
        /// ﾏｲﾘｽﾄ数
        /// </summary>
        public double MylistCounter
        {
            get { return _MylistCounter; }
            set { SetProperty(ref _MylistCounter, value); }
        }
        private double _MylistCounter = default(int);

        /// <summary>
        /// ｺﾒﾝﾄ数
        /// </summary>
        public double CommentCounter
        {
            get { return _CommentCounter; }
            set { SetProperty(ref _CommentCounter, value); }
        }
        private double _CommentCounter = default(int);

        /// <summary>
        /// ｺﾝﾃﾝﾂの投稿時間
        /// </summary>
        public DateTime StartTime
        {
            get { return _StartTime; }
            set { SetProperty(ref _StartTime, value); }
        }
        private DateTime _StartTime = default(DateTime);

        /// <summary>
        /// 再生時間 (秒)
        /// </summary>
        public long LengthSeconds
        {
            get { return _LengthSeconds; }
            set { SetProperty(ref _LengthSeconds, value); }
        }
        private long _LengthSeconds = default(long);

        /// <summary>
        /// ｻﾑﾈｲﾙUrl
        /// </summary>
        public string ThumbnailUrl
        {
            get { return _ThumbnailUrl; }
            set { SetProperty(ref _ThumbnailUrl, value); var tmp = Thumbnail; }
        }
        private string _ThumbnailUrl = null;

        /// <summary>
        /// ｻﾑﾈｲﾙ
        /// </summary>
        public BitmapImage Thumbnail
        {
            get
            {
                if (_Thumbnail == null)
                {
                    WpfUtil.BeginInvoke(async () =>
                    {
                        Thumbnail = await NicoUtil.ToThumbnail(
                            (new[] { ".L", ".M", "" }).Select(s => $"{ThumbnailUrl}{s}")
                        );
                    });
                }
                return _Thumbnail;
            }
            set { SetProperty(ref _Thumbnail, value); }
        }
        private BitmapImage _Thumbnail;

        /// <summary>
        /// ﾕｰｻﾞ名
        /// </summary>
        public string Username
        {
            get { return _Username; }
            set { SetProperty(ref _Username, value); }
        }
        private string _Username = null;

        /// <summary>
        /// ｽﾃｰﾀｽ
        /// </summary>
        public VideoStatus Status
        {
            get { return _Status; }
            set { if (SetProperty(ref _Status, value)) OnPropertyChanged(nameof(StatusString)); }
        }
        private VideoStatus _Status = VideoStatus.None;

        /// <summary>
        /// ｽﾃｰﾀｽ文字
        /// </summary>
        public string StatusString => Status.GetLabel();

        /// <summary>
        /// ｺﾝﾃﾝﾂを開きます。
        /// </summary>
        /// <returns></returns>
        public async Task Open()
        {
            Process.Start(SettingModel.Instance.Browser, VideoUrl);

            // SEEﾘｽﾄに追加
            await SearchVideoByHistoryModel.Instance.AddHistory(VideoId);

            // ｽﾃｰﾀｽ変更
            Status = VideoStatus.See;

            // 概要欄に未視聴のﾃﾞｰﾀがある場合はTemporaryに追加する
            foreach (var id in Regex.Matches(Description, @"(?<id>sm[\d]+)")
                    .OfType<Match>()
                    .Select(m => m.Groups["id"].Value)
                    .Where(tmp => !SearchVideoByHistoryModel.Instance.IsSee(tmp))
                )
            {
                await SearchVideoByTemporaryModel.Instance.AddVideo(id);
            }
        }

        public static async Task<VideoModel> CreateInstance(VVideoHistory vvh)
        {
            var v = await CreateInstance(vvh.VideoId);

            v.StartTime = vvh.Date;

            return v;
        }

        public static async Task<VideoModel> CreateInstance(string url)
        {
            var v = new VideoModel();

            await v.Refresh(url);

            if (SearchVideoByHistoryModel.Instance.IsSee(v.VideoId))
            {
                v.Status = VideoStatus.See;
            }
            else if (SearchVideoByTemporaryModel.Instance.IsTemporary(v.VideoId))
            {
                v.Status = VideoStatus.Favorite;
            }

            return v;
        }

        public async Task Refresh(string url)
        {
            var txt = await GetStringAsync($"http://ext.nicovideo.jp/api/getthumbinfo/{NicoUtil.ToContentId(url)}");
            var xml = ToXml(txt).Descendants("thumb").FirstOrDefault();

            if (xml == null)
            {
                VideoUrl = url;
                Status = VideoStatus.Delete;
                return;
            }

            VideoUrl = (string)xml.Element("watch_url");
            Title = (string)xml.Element("title");
            Description = (string)xml.Element("description");
            ThumbnailUrl = (string)xml.Element("thumbnail_url");
            ViewCounter = (double)xml.Element("view_counter");
            CommentCounter = (double)xml.Element("comment_num");
            MylistCounter = (double)xml.Element("mylist_counter");
            StartTime = NicoUtil.ToDatetime((string)xml.Element("first_retrieve"));
            LengthSeconds = NicoUtil.ToLengthSeconds((string)xml.Element("length"));
            Tags = xml.Descendants("tags").First().Descendants("tag").Select(tag => (string)tag).GetString(" ");
            Username = (string)xml.Element("user_nickname");
            /*
<?xml version="1.0" encoding="UTF-8"?>
<nicovideo_thumb_response status="fail">
  <error>
    <code>DELETED</code>
    <description>deleted</description>
  </error>
</nicovideo_thumb_response>

            <nicovideo_thumb_response status="ok">
            <thumb>
            <video_id>sm1234567</video_id>
            <title>My Chemical Romance - Teenagers</title>
            <description>無いようなのでうｐしてみました。キリ番踏んどった。やるなマイケミ(笑)お祝いのコメあざぁ～す！！</description>
            <thumbnail_url>
            http://nicovideo.cdn.nimg.jp/thumbnails/1234567/1234567
            </thumbnail_url>
            <first_retrieve>2007-10-08T21:14:54+09:00</first_retrieve>
            <length>2:51</length>
            <movie_type>flv</movie_type>
            <size_high>6866820</size_high>
            <size_low>6732537</size_low>
            <view_counter>81493</view_counter>
            <comment_num>1877</comment_num>
            <mylist_counter>618</mylist_counter>
            <last_res_body>
            ID巡りです うぃー 1234567 1234567 グリーンデイのビリー 1234567から 1234567 ええやん 1234567! 1234567から 1234567 カオスなIDから 1234567厉害了 乗れる 1234...
            </last_res_body>
            <watch_url>https://www.nicovideo.jp/watch/sm1234567</watch_url>
            <thumb_type>video</thumb_type>
            <embeddable>1</embeddable>
            <no_live_play>0</no_live_play>
            <tags domain="jp">
            <tag>MyChemicalRomance</tag>
            <tag>マイケミ</tag>
            <tag>MCR</tag>
            <tag>音楽</tag>
            <tag>洋楽</tag>
            <tag>奇跡のsm1234567</tag>
            <tag>←sm1234</tag>
            <tag>ジェラルド・ウェイ</tag>
            <tag>カオスなIDシリーズ</tag>
            <tag>Teenagers</tag>
            </tags>
            <genre>未設定</genre>
            <user_id>1792891</user_id>
            <user_nickname>000000</user_nickname>
            <user_icon_url>
            https://secure-dcdn.cdn.nimg.jp/nicoaccount/usericon/defaults/blank_s.jpg
            </user_icon_url>
            </thumb>
            </nicovideo_thumb_response>
            */
            /*
            This XML file does not appear to have any style information associated with it. The document tree is shown below.
            <nicovideo_thumb_response status="ok">
            <thumb>
            <video_id>sm34525974</video_id>
            <title>【女子2人】初めてパンの気持ちを理解する実況【I am Bread】</title>
            <description>
            全パンの想いを背負いし者----------------------関西弁女子実況グループ『サイコロジカルサーカス』が、第13回実況者杯に参加します!フリー部門実況動画の部、謎部門にエントリー！再生数・コメント・マイリス数で順位が決まります！！応援よろよろ！( ˘ω˘ )ニコニ広告が可能です！よければ広告で宣伝もよろしくお願いします(強欲の壺)テーマは【初】この動画はフリー部門実況動画の部の動画です。 今回は映画風の始まりにしてみました！楽しんで見てもらえるとうれしい！(*^^*)大会が終了しました！感想動画はこちら⇒今回のプログラム⇒mylist/63232841大会詳細⇒sm33431601パンフレット⇒mylist/55555016舞台袖⇒co3253598プレイメンバー…ノイジーワールド(紫)・馬面なおと(桃) Twitter…https://twitter.com/rojikaru2525
            </description>
            <thumbnail_url>http://tn.smilevideo.jp/smile?i=34525974.59215</thumbnail_url>
            <first_retrieve>2019-01-25T18:11:01+09:00</first_retrieve>
            <length>20:04</length>
            <movie_type>mp4</movie_type>
            <size_high>1</size_high>
            <size_low>1</size_low>
            <view_counter>2087</view_counter>
            <comment_num>117</comment_num>
            <mylist_counter>12</mylist_counter>
            <last_res_body>
            隠せてねえよw 食えねえよw きめえw … えw なんだこれw パンw 意味不明w ちょw w うぽつ うぽつ 続きやるなら見たい!! 88888888888888 まないた汚いとかトラ 自分で言うて自分で受 「泣けるなあ...
            </last_res_body>
            <watch_url>https://www.nicovideo.jp/watch/sm34525974</watch_url>
            <thumb_type>video</thumb_type>
            <embeddable>1</embeddable>
            <no_live_play>0</no_live_play>
            <tags domain="jp">
            <tag category="1" lock="1">実況プレイ動画</tag>
            <tag lock="1">ゲーム</tag>
            <tag lock="1">サイコロジカルサーカス</tag>
            <tag lock="1">第13回実況者杯本選</tag>
            <tag lock="1">女性実況</tag>
            <tag lock="1">I_am_Bread</tag>
            <tag lock="1">女性実況単発リンク</tag>
            <tag lock="1">実況プレイ単発リンク</tag>
            <tag lock="1">ゲーム実況</tag>
            <tag>バカゲー</tag>
            </tags>
            <genre>ゲーム</genre>
            <user_id>49828637</user_id>
            <user_nickname>ノイジーワールド</user_nickname>
            <user_icon_url>
            https://secure-dcdn.cdn.nimg.jp/nicoaccount/usericon/s/4982/49828637.jpg?1467544909
            </user_icon_url>
            </thumb>
            </nicovideo_thumb_response>
             */
        }

        //public async Task Download(string path)
        //{
        //    using (var handler = new HttpClientHandler())
        //    using (var client = new HttpClient(handler))
        //    {
        //        client.Timeout = new TimeSpan(1, 0, 0);

        //        // ﾛｸﾞｲﾝｸｯｷｰ設定
        //        handler.CookieContainer = await SettingModel.Instance.GetCookies();

        //        // 対象動画にｼﾞｬﾝﾌﾟ
        //        await client.PostAsync("http://www.nicovideo.jp/watch/" + VideoId, null);

        //        // 動画URL全文を取得
        //        var flvurl = await client.GetStringAsync("http://flapi.nicovideo.jp/api/getflv/" + VideoId);

        //        flvurl = Uri.UnescapeDataString(flvurl);
        //        flvurl = Regex.Match(flvurl, @"&url=.*").Value.Replace("&url=", "");
        //        var res = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://flapi.nicovideo.jp/api/getflv/" + VideoId));

        //        //var bytes = await client.GetByteArrayAsync(flvurl);
        //        //File.WriteAllBytes(path, bytes);

        //        ServiceFactory.MessageService.Debug(flvurl);

        //    }
        //}

    }
}
