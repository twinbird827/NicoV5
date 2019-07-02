using NicoV5.Mvvm.Combos;
using NicoV5.Mvvm.Components;
using NicoV5.Mvvm.Models;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfUtilV2.Mvvm;

namespace NicoV5.Mvvm.WorkSpaces
{
    public class SearchVideoByMylistViewModel : WorkSpaceViewModel
    {
        public SearchVideoByMylistViewModel()
        {
            Source = new SearchVideoByMylistModel();

            Sort = ComboVMylistSortModel.Instance;

            Videos = Source.Videos.ToSyncedSynchronizationContextCollection(
                video => new VideoViewModel(video),
                AnonymousSynchronizationContext.Current
            );
            MylistTitle = Source.MylistTitle;
            MylistCreator = Source.MylistCreator;
            MylistDescription = Source.MylistDescription;
            UserId = Source.UserId;
            UserThumbnail = Source.UserThumbnail;
            MylistDate = Source.MylistDate;

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
        public SearchVideoByMylistModel Source { get; private set; }

        /// <summary>
        /// ﾒｲﾝ項目ﾘｽﾄ
        /// </summary>
        public SynchronizationContextCollection<VideoViewModel> Videos
        {
            get { return _Videos; }
            set { SetProperty(ref _Videos, value); }
        }
        private SynchronizationContextCollection<VideoViewModel> _Videos;

        /// <summary>
        /// ｼﾞｬﾝﾙ
        /// </summary>
        public ComboVMylistSortModel Sort { get; set; }

        /// <summary>
        /// 検索ﾜｰﾄﾞ
        /// </summary>
        public string Word
        {
            get { return _Word; }
            set { SetProperty(ref _Word, value); }
        }
        private string _Word = null;

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
        private BitmapImage _UserThumbnail = null;

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
        /// 作成者情報を表示するか
        /// </summary>
        public bool IsCreatorVisible => Videos.Any();

        /// <summary>
        /// 検索処理
        /// </summary>
        public ICommand OnSearch
        {
            get
            {
                return _OnSearch = _OnSearch ?? new RelayCommand(async _ =>
                {
                    // 入力値をﾓﾃﾞﾙにｾｯﾄ
                    Source.MylistUrl = this.Word;
                    Source.OrderBy = Sort.SelectedItem.Value;

                    // 検索実行
                    await Source.Reload();

                    // ｵｰﾅｰ情報を表示するかどうか
                    OnPropertyChanged(nameof(IsCreatorVisible));
                },
                _ => {
                    return !string.IsNullOrWhiteSpace(Word);
                });
            }
        }
        public ICommand _OnSearch;

        /// <summary>
        /// 追加処理
        /// </summary>
        public ICommand OnAdd
        {
            get
            {
                return _OnAdd = _OnAdd ?? new RelayCommand(
                _ =>
                {
                    MylistStatusModel.Instance.AddFavorites(Source.MylistId);
                },
                _ => {
                    return IsCreatorVisible;
                });
            }
        }
        public ICommand _OnAdd;

    }
}
