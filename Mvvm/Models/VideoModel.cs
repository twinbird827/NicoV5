using NicoV5.Common;
using NicoV5.Common.Tables;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
            set
            {
                if (!SetProperty(ref _ThumbnailUrl, value) || Thumbnail != null)
                {
                    return;
                }

                WpfUtil.BeginInvoke(async () =>
                {
                    Thumbnail = await NicoUtil.ToThumbnail(
                        (new[] { ".L", ".M", "" }).Select(s => $"{value}{s}")
                    );
                });
            }
        }
        private string _ThumbnailUrl = null;

        /// <summary>
        /// ｻﾑﾈｲﾙ
        /// </summary>
        public BitmapImage Thumbnail
        {
            get { return _Thumbnail; }
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

            return v;
        }

        public async Task Refresh(string url)
        {
            var txt = await GetStringAsync($"http://ext.nicovideo.jp/api/getthumbinfo/{NicoUtil.ToContentId(url)}");
            var xml = ToXml(txt).Descendants("thumb").First();

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
