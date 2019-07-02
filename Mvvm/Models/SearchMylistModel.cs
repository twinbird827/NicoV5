using NicoV5.Common;
using NicoV5.Common.Tables;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUtilV2.Mvvm;

namespace NicoV5.Mvvm.Models
{
    public class SearchMylistModel : BindableBase
    {
        private SearchMylistModel()
        {
            // お気に入りﾘｽﾄ
            Favorites = new ObservableSynchronizedCollection<TFavorite>();
            // ﾏｲﾘｽﾄ
            Mylists = Favorites.ToSyncedSynchronizationContextCollection(
                video => new SearchVideoByMylistModel(video.Mylist),
                AnonymousSynchronizationContext.Current
            );
        }

        public static SearchMylistModel Instance { get; private set; }

        public static void Initialize(IEnumerable<TFavorite> favorites)
        {
            Instance = new SearchMylistModel();

            foreach (var favorite in favorites)
            {
                Instance.Favorites.Add(favorite);
            }
        }

        public ObservableSynchronizedCollection<TFavorite> Favorites
        {
            get { return _Favorites; }
            set { SetProperty(ref _Favorites, value); }
        }
        private ObservableSynchronizedCollection<TFavorite> _Favorites;

        public SynchronizationContextCollection<SearchVideoByMylistModel> Mylists
        {
            get { return _Mylists; }
            set { SetProperty(ref _Mylists, value); }
        }
        private SynchronizationContextCollection<SearchVideoByMylistModel> _Mylists;

        public void AddFavorite(string id)
        {
            AddFavorite(new TFavorite(NicoUtil.ToContentId(id), DateTime.Now));
        }

        public void AddFavorite(TFavorite favorite)
        {
            if (!Favorites.Any(f => f.Mylist == favorite.Mylist))
            {
                Favorites.Add(favorite);
            }
        }

        public void RemoveFavorite(string id)
        {
            RemoveFavorite(new TFavorite(NicoUtil.ToContentId(id), DateTime.Now));
        }

        public void RemoveFavorite(TFavorite favorite)
        {
            if (!Favorites.Any(f => f.Mylist == favorite.Mylist))
            {
                Favorites.Remove(Favorites.First(f => f.Mylist == favorite.Mylist));
            }
        }

        public async Task Reload()
        {
            foreach (var favorite in Favorites)
            {
                var mylist = Mylists.First(m => m.MylistId == favorite.Mylist);

                await mylist.Reload();

                var videos = mylist.Videos
                    .Where(video => favorite.Date < video.StartTime)
                    .Where(video => !SearchVideoByTemporaryModel.Instance.Videos.Any(v => v.VideoId == video.VideoId));

                foreach (var video in videos)
                {
                    await SearchVideoByTemporaryModel.Instance.AddVideo(video);
                }

                favorite.Date = videos.Max(video => video.StartTime);
            }
        }
    }
}
