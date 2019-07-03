using NicoV5.Common;
using NicoV5.Mvvm.Components;
using NicoV5.Mvvm.Main;
using NicoV5.Mvvm.Models;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfUtilV2.Mvvm;
using WpfUtilV2.Mvvm.Service;

namespace NicoV5.Mvvm.WorkSpaces
{
    public class SearchMylistViewModel : WorkSpaceViewModel
    {
        public SearchMylistViewModel()
        {
            Source = SearchMylistModel.Instance;

            Mylists = Source.Mylists.ToSyncedSynchronizationContextCollection(
                mylist => new MylistViewModel(mylist),
                AnonymousSynchronizationContext.Current
            );
        }

        private SearchMylistModel Source { get; set; }

        /// <summary>
        /// ﾒｲﾝ項目ﾘｽﾄ
        /// </summary>
        public SynchronizationContextCollection<MylistViewModel> Mylists
        {
            get { return _Mylists; }
            set { SetProperty(ref _Mylists, value); }
        }
        private SynchronizationContextCollection<MylistViewModel> _Mylists;

        /// <summary>
        /// ADDﾎﾞﾀﾝ押下時
        /// </summary>
        public ICommand OnFavoriteAdd
        {
            get
            {
                return _OnFavoriteAdd = _OnFavoriteAdd ?? new RelayCommand(
                async _ =>
                {
                    // URL入力欄を表示
                    string result = await MainViewModel.Instance.ShowInputAsync(
                        "ADD MYLIST",
                        "Please enter the URL of my list to be added.");

                    if (!string.IsNullOrEmpty(result))
                    {
                        await AddMylist(result);
                    }
                });
            }
        }
        public ICommand _OnFavoriteAdd;

        /// <summary>
        /// 共通の追加処理
        /// </summary>
        /// <param name="url">追加するURL</param>
        private async Task AddMylist(string url)
        {
            var mylist = NicoUtil.ToContentId(url);

            if (SearchMylistModel.Instance.Favorites.Any(f => f.Mylist == mylist))
            {
                return;
            }

            var model = new SearchVideoByMylistModel();

            await model.Reload(url);

            if (!model.Videos.Any())
            {
                ServiceFactory.MessageService.Error("ﾃﾞｰﾀ件数が0件");
            }
            else
            {
                await SearchMylistModel.Instance.AddFavorite(mylist);
            }
        }

    }
}
