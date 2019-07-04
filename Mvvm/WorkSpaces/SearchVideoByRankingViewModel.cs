using NicoV5.Mvvm.Combos;
using NicoV5.Mvvm.Components;
using NicoV5.Mvvm.Models;
using StatefulModel;
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

            Videos = Source.Videos.ToSyncedSynchronizationContextCollection(
                video => new VideoViewModel(video),
                AnonymousSynchronizationContext.Current
            );

            Genre = ComboRankGenreModel.Instance;
            Period = ComboRankPeriodModel.Instance;

            // ｺﾝﾎﾞﾎﾞｯｸｽにｲﾍﾞﾝﾄ割付
            Genre.AddOnPropertyChanged(this, Combo_ChangeSelectedItem);
            Period.AddOnPropertyChanged(this, Combo_ChangeSelectedItem);

            // 初期表示ｲﾍﾞﾝﾄ
            Loaded += Reload;

            Disposed += (sender, e) => Loaded -= Reload;
        }

        /// <summary>
        /// 処理用ｿｰｽ
        /// </summary>
        private SearchVideoByRankingModel Source { get; set; }

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
        public ComboRankGenreModel Genre { get; set; }

        /// <summary>
        /// 期間
        /// </summary>
        public ComboRankPeriodModel Period { get; set; }

        /// <summary>
        /// ｺﾝﾎﾞﾎﾞｯｸｽ選択変更ｲﾍﾞﾝﾄ
        /// </summary>
        private void Combo_ChangeSelectedItem(object sender, PropertyChangedEventArgs e)
        {
            var combo = sender as ComboboxModel;

            if (combo == null) return;

            if (e.PropertyName != nameof(combo.SelectedItem)) return;

            Reload(sender, e);
        }

        /// <summary>
        /// 再表示処理
        /// </summary>
        private async void Reload(object sender, EventArgs e)
        {
            await Source.GetRanking(Period.SelectedItem, Genre.SelectedItem);
        }
    }
}
