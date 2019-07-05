using NicoV5.Common;
using NicoV5.Mvvm.Components;
using NicoV5.Mvvm.Main;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WpfUtilV2.Extensions;

namespace NicoV5.Mvvm.Models
{
    public class SearchVideoByTemporaryModel : SearchVideoModel
    {
        private SearchVideoByTemporaryModel() : base(true)
        {
            Videos = new ObservableSynchronizedCollection<VideoModel>();
        }

        public static SearchVideoByTemporaryModel Instance { get; private set; }

        public ObservableSynchronizedCollection<VideoModel> Videos
        {
            get { return _Videos; }
            set { SetProperty(ref _Videos, value); }
        }
        private ObservableSynchronizedCollection<VideoModel> _Videos;

        public static async Task Initialize()
        {
            Instance = new SearchVideoByTemporaryModel();

            if (string.IsNullOrEmpty(SettingModel.Instance.MailAddress) || string.IsNullOrEmpty(SettingModel.Instance.Password))
            {
                using (var vm = new MailPassViewModel())
                {
                    var dialog = new MailPassWindow(vm);

                    dialog.ShowModalWindow();

                    SettingModel.Instance.MailAddress = vm.MailAddress;
                    SettingModel.Instance.Password = vm.Password;
                }
            }

            await Instance.ReloadTemporary();
        }

        public async Task ReloadTemporary()
        {
            const string url = "http://www.nicovideo.jp/api/deflist/list";

            var json = await GetJsonAsync(url);

            Videos.Clear();

            foreach (dynamic item in json["mylistitem"])
            {
                var video = new VideoModel();

                await video.Refresh(item["item_data"]["video_id"]);

                //video.VideoUrl = item["item_data"]["video_id"];
                //video.Title = item["item_data"]["title"];
                //video.Description = item["description"];
                ////video.Tags = data["tags"];
                ////video.CategoryTag = data["categoryTags"];
                //video.ViewCounter = long.Parse(item["item_data"]["view_counter"]);
                //video.MylistCounter = long.Parse(item["item_data"]["mylist_counter"]);
                //video.CommentCounter = long.Parse(item["item_data"]["num_res"]);
                //video.StartTime = NicoUtil.FromUnixTime((long)item["item_data"]["first_retrieve"]);
                ////video.LastCommentTime = Converter.item(data["lastCommentTime"]);
                //video.LengthSeconds = long.Parse(item["item_data"]["length_seconds"]);
                //video.ThumbnailUrl = item["item_data"]["thumbnail_url"];
                ////video.LastResBody = item["item_data"]["last_res_body"];
                ////video.CommunityIcon = data["communityIcon"];

                Videos.Add(video);
            }

            MainViewModel.Instance.TemporaryCount = Videos.Count;
        }

        public async Task AddVideo(VideoModel video)
        {
            await AddVideo(video.VideoId);
        }

        public async Task AddVideo(string id)
        {
            const string url = "http://www.nicovideo.jp/api/deflist/add?item_type=0&item_id={0}&description={1}&token={2}";

            if (!Videos.Any(v => v.VideoId == id))
            {
                // URLに追加
                var txt = await GetStringAsync(string.Format(url, await GetItemId(id), "", await GetToken()));

                // ﾘﾌﾚｯｼｭ
                var video = new VideoModel();

                await video.Refresh(id);

                // 自身に追加
                Videos.Insert(0, video);

                MainViewModel.Instance.TemporaryCount = Videos.Count;
            }
        }

        public async Task DeleteVideo(VideoModel video)
        {
            const string url = "http://www.nicovideo.jp/api/deflist/delete?id_list[0][]={0}&token={1}";

            if (Videos.Any(v => v.VideoId == video.VideoId))
            {
                // URLに追加
                var txt = await GetStringAsync(string.Format(url, await GetItemId(video.VideoId), await GetToken()));

                // 自身に追加
                Videos.Remove(Videos.First(v => v.VideoId == video.VideoId));

                MainViewModel.Instance.TemporaryCount = Videos.Count;
            }
        }

        public async Task DeleteVideo(string id)
        {
            const string url = "http://www.nicovideo.jp/api/deflist/delete?id_list[0][]={0}&token={1}";

            var video = Videos.FirstOrDefault(v => v.VideoId == id);

            if (video != null)
            {
                // 削除用URLを作成
                var txt = await GetStringAsync(string.Format(url, await GetItemId(id), await GetToken()));

                // 自身から削除
                Videos.Remove(video);

                MainViewModel.Instance.TemporaryCount = Videos.Count;
            }
        }

        private async Task<string> GetToken()
        {
            const string url = "http://www.nicovideo.jp/my/top";
            var txt = await GetStringAsync(url);
            return Regex.Match(txt, "data-csrf-token=\"(?<token>[^\"]+)\"").Groups["token"].Value;
        }

        public async Task<string> GetItemId(string id)
        {
            const string url = "http://www.nicovideo.jp/api/deflist/list";

            var json = await GetJsonAsync(url);

            foreach (dynamic item in json["mylistitem"])
            {
                if (id == item["item_data"]["video_id"])
                {
                    return item["item_id"];
                }
            }
            return id;
        }
    }
}
/*
<item type="object">
  <item_type type="string">0</item_type>
  <item_id type="string">1367028098</item_id>
  <description type="string"></description>
  <item_data type="object">
    <video_id type="string">sm20707987</video_id>
    <title type="string">【FF14ニコ超2】許されたシーン（意味深）</title>
    <thumbnail_url type="string">http://tn.smilevideo.jp/smile?i=20707987</thumbnail_url>
    <first_retrieve type="number">1367028098</first_retrieve>
    <update_time type="number">1547605203</update_time>
    <view_counter type="string">148267</view_counter>
    <mylist_counter type="string">1311</mylist_counter>
    <num_res type="string">4116</num_res>
    <group_type type="string">default</group_type>
    <length_seconds type="string">461</length_seconds>
    <deleted type="string">0</deleted>
    <last_res_body type="string">天丼するなw 逃げたシーンwwww 取り逃げwwww これは奇跡 ネタじゃない事実だぞ HQ正直でよろしい 草ァwwww オンラインつながりd あっ! ありましたね～ ふんふんふんふん 取り逃げの... </last_res_body>
    <watch_id type="string">sm20707987</watch_id>
  </item_data>
  <watch type="number">0</watch>
  <create_time type="number">1548886893</create_time>
  <update_time type="number">1548886893</update_time>
</item>
*/
