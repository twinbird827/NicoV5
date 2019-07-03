using NicoV5.Mvvm.Main;
using NicoV5.Mvvm.Models;
using NicoV5.Mvvm.WorkSpaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfUtilV2.Mvvm;

namespace NicoV5.Mvvm.Components
{
    public class MylistViewModel : BindableBase
    {
        public MylistViewModel(SearchVideoByMylistModel source)
        {
            Source = source;
            // 初期値設定
            MylistTitle = MylistTitle;
            MylistCreator = MylistCreator;
            MylistDescription = MylistDescription;
            UserId = UserId;
            UserThumbnail = UserThumbnail;
            MylistDate = MylistDate;

            // ﾓﾃﾞﾙ側で変更があったら通知する
            Source.AddOnPropertyChanged(this, (sender, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(MylistTitle):
                        MylistTitle = Source.MylistTitle;
                        break;
                    case nameof(MylistCreator):
                        MylistCreator = Source.MylistCreator;
                        break;
                    case nameof(MylistDescription):
                        MylistDescription = Source.MylistDescription;
                        break;
                    case nameof(UserId):
                        UserId = Source.UserId;
                        break;
                    case nameof(UserThumbnail):
                        UserThumbnail = Source.UserThumbnail;
                        break;
                    case nameof(MylistDate):
                        MylistDate = Source.MylistDate;
                        break;
                }
            });

        }

        public SearchVideoByMylistModel Source { get; set; }

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
        /// 項目ﾀﾞﾌﾞﾙｸﾘｯｸ時ｲﾍﾞﾝﾄ
        /// </summary>
        public ICommand OnDoubleClick
        {
            get
            {
                return _OnDoubleClick = _OnDoubleClick ?? new RelayCommand(
                _ =>
                {
                    // 検索画面を出す
                    var vm = new SearchVideoByMylistViewModel();

                    vm.Word = Source.MylistUrl;
                    vm.OnSearch.Execute(null);

                    MainViewModel.Instance.Current = vm;
                });
            }
        }
        public ICommand _OnDoubleClick;

        /// <summary>
        /// ﾏｲﾘｽﾄ削除ｲﾍﾞﾝﾄ
        /// </summary>
        public ICommand OnFavoriteDel
        {
            get
            {
                return _OnFavoriteDel = _OnFavoriteDel ?? new RelayCommand(
                async _ =>
                {
                    // ﾏｲﾘｽﾄ削除
                    await SearchMylistModel.Instance.RemoveFavorite(Source.MylistId);
                });
            }
        }
        public ICommand _OnFavoriteDel;

        /// <summary>
        /// 項目ｷｰ入力時ｲﾍﾞﾝﾄ
        /// </summary>
        public ICommand OnKeyDown
        {
            get
            {
                return _OnKeyDown = _OnKeyDown ?? new RelayCommand<KeyEventArgs>(
                e =>
                {
                    switch (e.Key)
                    {
                        case Key.Enter:
                            // ﾀﾞﾌﾞﾙｸﾘｯｸと同じ処理
                            OnDoubleClick.Execute(null);
                            break;
                        case Key.Delete:
                            // ﾏｲﾘｽﾄ削除
                            OnFavoriteDel.Execute(null);
                            break;
                    }
                });
            }
        }
        public ICommand _OnKeyDown;

    }
}
