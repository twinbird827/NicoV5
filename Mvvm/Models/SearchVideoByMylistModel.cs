using NicoV5.Common;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace NicoV5.Mvvm.Models
{
    public class SearchVideoByMylistModel : SearchVideoModel
    {
        public SearchVideoByMylistModel(string word) : this()
        {
            Reload(word).ConfigureAwait(false);
        }

        public SearchVideoByMylistModel() : base(false)
        {
            Videos = new ObservableSynchronizedCollection<VideoModel>();
        }

        /// <summary>
        /// ﾏｲﾘｽﾄUrl
        /// </summary>
        public string MylistUrl
        {
            get { return $"http://www.nicovideo.jp/mylist/{MylistId}?rss=2.0&numbers=1&sort={OrderBy}"; }
            set { MylistId = NicoUtil.ToContentId(value); }
        }

        /// <summary>
        /// ﾏｲﾘｽﾄId
        /// </summary>
        public string MylistId
        {
            get { return _MylistId; }
            set { SetProperty(ref _MylistId, value); OnPropertyChanged(nameof(MylistUrl)); }
        }
        private string _MylistId = null;

        /// <summary>
        /// ｿｰﾄ順
        /// </summary>
        public string OrderBy
        {
            get { return _OrderBy; }
            set { SetProperty(ref _OrderBy, value); }
        }
        private string _OrderBy = null;

        /// <summary>
        /// ﾀｲﾄﾙ
        /// </summary>
        public string MylistTitle
        {
            get { return _MylistTitle; }
            set { SetProperty(ref _MylistTitle, value); }
        }
        private string _MylistTitle = null;

        /// <summary>
        /// 作成者
        /// </summary>
        public string MylistCreator
        {
            get { return _MylistCreator; }
            set { SetProperty(ref _MylistCreator, value); }
        }
        private string _MylistCreator = null;

        /// <summary>
        /// ﾏｲﾘｽﾄ詳細
        /// </summary>
        public string MylistDescription
        {
            get { return _MylistDescription; }
            set { SetProperty(ref _MylistDescription, value); }
        }
        private string _MylistDescription = null;

        /// <summary>
        /// 作成者のID
        /// </summary>
        public string UserId
        {
            get { return _UserId; }
            set { SetProperty(ref _UserId, value); }
        }
        private string _UserId = null;

        /// <summary>
        /// 作成者のｻﾑﾈｲﾙ
        /// </summary>
        public BitmapImage UserThumbnail
        {
            get { return _UserThumbnail; }
            set { SetProperty(ref _UserThumbnail, value); }
        }
        private BitmapImage _UserThumbnail;

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime MylistDate
        {
            get { return _MylistDate; }
            set { SetProperty(ref _MylistDate, value); }
        }
        private DateTime _MylistDate = default(DateTime);

        /// <summary>
        /// 動画情報ﾘｽﾄ
        /// </summary>
        public ObservableSynchronizedCollection<VideoModel> Videos
        {
            get { return _Videos; }
            set { SetProperty(ref _Videos, value); }
        }
        private ObservableSynchronizedCollection<VideoModel> _Videos;

        /// <summary>
        /// 再読込
        /// </summary>
        public async Task Reload(string word)
        {
            MylistUrl = word;
            await Reload();
        }

        /// <summary>
        /// 再読込
        /// </summary>
        public async Task Reload()
        {
            var xml = await GetXmlChannelAsync(MylistUrl);

            // ﾏｲﾘｽﾄ情報を本ｲﾝｽﾀﾝｽのﾌﾟﾛﾊﾟﾃｨに転記
            MylistTitle = xml.Element("title").Value;
            MylistCreator = xml.Element(XName.Get("creator", "http://purl.org/dc/elements/1.1/")).Value;
            MylistDate = DateTime.Parse(xml.Element("lastBuildDate").Value);
            MylistDescription = xml.Element("description").Value;

            UserId = await GetUserId();
            UserThumbnail = await NicoUtil.ToThumbnail(await GetThumbnailUrl());

            Videos.Clear();

            foreach (var item in xml.Descendants("item"))
            {
                Videos.Add(CreateVideoFromXml(
                    item,
                    "nico-numbers-view",
                    "nico-numbers-mylist",
                    "nico-numbers-res"
                ));
            }
        }

        /// <summary>
        /// ﾏｲﾘｽﾄからﾕｰｻﾞIDを取得します。
        /// </summary>
        /// <returns>ﾕｰｻﾞID</returns>
        private async Task<string> GetUserId()
        {
            const string url = "http://www.nicovideo.jp/mylist/{0}";

            var txt = await GetStringAsync(string.Format(url, MylistId));
            var id = Regex.Match(txt, "user_id: (?<user_id>[\\d]+)").Groups["user_id"].Value;
            return id;
        }

        /// <summary>
        /// ﾕｰｻﾞIDからｻﾑﾈｲﾙUrlを取得します。
        /// </summary>
        /// <returns>ｻﾑﾈｲﾙUrl</returns>
        private async Task<string> GetThumbnailUrl()
        {
            const string userIframe = "http://ext.nicovideo.jp/thumb_user/{0}";
            const string url = "https://secure-dcdn.cdn.nimg.jp/nicoaccount/usericon/";

            var txt = await GetStringAsync(string.Format(userIframe, UserId));
            var thumbnail = Regex.Match(txt, url + "(?<url>[^\"]+)").Groups["url"].Value;
            return url + thumbnail;
        }

    }
}
