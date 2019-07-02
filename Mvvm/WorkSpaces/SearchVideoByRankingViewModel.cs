using NicoV5.Mvvm.Combos;
using NicoV5.Mvvm.Components;
using NicoV5.Mvvm.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NicoV5.Mvvm.WorkSpaces
{
    public class SearchVideoByRankingViewModel : WorkSpaceViewModel
    {
        /// <summary>
        /// ｺﾝｽﾄﾗｸﾀ
        /// </summary>
        public SearchVideoByRankingViewModel()
        {
            // 各変数の初期化
            Source = new SearchVideoByRankingModel();
            Videos = new ObservableCollection<VideoViewModel>();
            Genre = ComboRankGenreModel.Instance;
            Period = ComboRankPeriodModel.Instance;

            // ｺﾝﾎﾞﾎﾞｯｸｽにｲﾍﾞﾝﾄ割付
            Genre.AddOnPropertyChanged(this, Combo_ChangeSelectedItem);
            Period.AddOnPropertyChanged(this, Combo_ChangeSelectedItem);

            // 初期表示ｲﾍﾞﾝﾄ
            Loaded += async (sender, e) => await Reload(sender, e);
        }

        /// <summary>
        /// 処理用ｿｰｽ
        /// </summary>
        private SearchVideoByRankingModel Source { get; set; }

        /// <summary>
        /// 一覧
        /// </summary>
        public ObservableCollection<VideoViewModel> Videos
        {
            get { return _Videos; }
            set { SetProperty(ref _Videos, value); }
        }
        private ObservableCollection<VideoViewModel> _Videos;

        /// <summary>
        /// ｼﾞｬﾝﾙ
        /// </summary>
        public ComboRankGenreModel Genre { get; set; }

        /// <summary>
        /// 期間
        /// </summary>
        public ComboRankPeriodModel Period { get; set; }

        /// <summary>
        /// ｺﾝﾎﾞﾎﾞｯｸｽ選択変更ｲﾍﾞﾝﾄ
        /// </summary>
        private async void Combo_ChangeSelectedItem(object sender, PropertyChangedEventArgs e)
        {
            var combo = sender as ComboboxModel;

            if (combo == null) return;

            if (e.PropertyName != nameof(combo.SelectedItem)) return;

            await Reload(sender, e);
        }

        /// <summary>
        /// 再表示処理
        /// </summary>
        private async Task Reload(object sender, EventArgs e)
        {
            Videos.Clear();
            foreach (var video in await Source.GetRanking(Period.SelectedItem, Genre.SelectedItem))
            {
                Videos.Add(new VideoViewModel(video));
            }
        }
    }
}
