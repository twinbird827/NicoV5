using NicoV5.Common;
using NicoV5.Common.Databases;
using NicoV5.Common.Tables;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUtilV2.Common;
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

        public static async Task Initialize(IEnumerable<TFavorite> favorites)
        {
            Instance = new SearchMylistModel();

            Instance.InitializePrivate(favorites);

            await Task.Delay(1);
        }

        private void InitializePrivate(IEnumerable<TFavorite> favorites)
        {
            foreach (var favorite in favorites)
            {
                Favorites.Add(favorite);
            }

            // 5分毎にﾘﾛｰﾄﾞするﾀｲﾏｰを設定
            Timer = new AsyncTimer();
            Timer.Interval = TimeSpan.FromMinutes(5);
            Timer.Tick += async (sender, e) =>
            {
                // ﾏｲﾘｽﾄに新着がないか確認
                await Reload();

                // 処理完了
                Timer.Completed();
            };
            Timer.Start();
        }

        public AsyncTimer Timer { get; set; }

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

        public async Task AddFavorite(string id)
        {
            await AddFavorite(new TFavorite(NicoUtil.ToContentId(id), DateTime.Now));
        }

        public async Task AddFavorite(TFavorite favorite)
        {
            if (!Favorites.Any(f => f.Mylist == favorite.Mylist))
            {
                using (var accessor = DbAccessor.GetAccessor())
                using (var control = accessor.GetCommand())
                {
                    Favorites.Add(favorite);
                    await control.BeginTransaction();
                    await control.InsertOrReplaceFavorite(favorite);
                    await control.Commit();
                }
            }
        }

        public async Task RemoveFavorite(string id)
        {
            await RemoveFavorite(new TFavorite(NicoUtil.ToContentId(id), DateTime.Now));
        }

        public async Task RemoveFavorite(TFavorite favorite)
        {
            if (!Favorites.Any(f => f.Mylist == favorite.Mylist))
            {
                using (var accessor = DbAccessor.GetAccessor())
                using (var control = accessor.GetCommand())
                {
                    Favorites.Remove(Favorites.First(f => f.Mylist == favorite.Mylist));
                    await control.BeginTransaction();
                    await control.DeleteFavorite(favorite);
                    await control.Commit();
                }
            }
        }

        public async Task Reload()
        {
            using (var accessor = DbAccessor.GetAccessor())
            using (var control = accessor.GetCommand())
            {
                foreach (var favorite in Favorites)
                {
                    var mylist = Mylists.First(m => m.MylistId == favorite.Mylist);

                    await mylist.Reload();

                    var videos = mylist.Videos
                        .Where(video => favorite.Date < video.StartTime)
                        .Where(video => !SearchVideoByTemporaryModel.Instance.Videos.Any(v => v.VideoId == video.VideoId))
                        .ToArray();

                    if (!videos.Any())
                    {
                        continue;
                    }

                    foreach (var video in videos)
                    {
                        await SearchVideoByTemporaryModel.Instance.AddVideo(video);
                    }

                    favorite.Date = videos.Max(video => video.StartTime);
                }
                await control.BeginTransaction();
                await control.InsertOrReplaceFavorite(Favorites.ToArray());
                await control.Commit();
            }
        }
    }
}
