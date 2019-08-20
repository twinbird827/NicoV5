using NicoV5.Mvvm.Combos;
using StatefulModel;
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
            Videos = new ObservableSynchronizedCollection<VideoModel>();
        }

        public ObservableSynchronizedCollection<VideoModel> Videos
        {
            get { return _Videos; }
            set { SetProperty(ref _Videos, value); }
        }
        private ObservableSynchronizedCollection<VideoModel> _Videos;

        public async Task GetRanking(ComboboxItemModel period, ComboboxItemModel genre)
        {
            var p = period.Value;
            var g = genre.Value;
            var t = "all";

            if (string.IsNullOrWhiteSpace(p) || string.IsNullOrWhiteSpace(g) || string.IsNullOrWhiteSpace(t))
            {
                ServiceFactory.MessageService.Error("検索ワードが入力されていません。");
                return;
            }

            var url = $"http://www.nicovideo.jp/ranking/genre/{g}?tag={t}&term={p}&rss=2.0&lang=ja-jp";
            var xml = await GetXmlChannelAsync(url);

            Exception lastException = null;
            Videos.Clear();
            foreach (var item in xml.Descendants("item"))
            {
                try
                {
                    Videos.Add(await CreateVideoFromXml(
                        item,
                        "nico-info-total-view",
                        "nico-info-total-mylist",
                        "nico-info-total-res"
                    ));
                }
                catch (Exception ex)
                {
                    lastException = ex;
                }
            }

            if (lastException != null)
            {
                ServiceFactory.MessageService.Exception(lastException);
            }
        }
    }
}
