using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUtilV2.Mvvm;

namespace NicoV5.Mvvm.Combos
{
    public class ComboRankPeriodModel : BindableBase
    {
        /// <summary>
        /// ｲﾝｽﾀﾝｽ (ｼﾝｸﾞﾙﾄﾝﾊﾟﾀｰﾝ)
        /// </summary>
        public static ComboRankPeriodModel Instance { get; } = new ComboRankPeriodModel();

        /// <summary>
        /// ｿｰﾄﾘｽﾄ構成
        /// </summary>
        public ObservableCollection<ComboboxItemModel> Items
        {
            get { return _Items; }
            set { SetProperty(ref _Items, value); }
        }
        private ObservableCollection<ComboboxItemModel> _Items;

        /// <summary>
        /// 選択中の構成
        /// </summary>
        public ComboboxItemModel SelectedItem
        {
            get { return _SelectedItem; }
            set { SetProperty(ref _SelectedItem, value); }
        }
        private ComboboxItemModel _SelectedItem;

        private ComboRankPeriodModel()
        {
            Items = new ObservableCollection<ComboboxItemModel>
            {
                new ComboboxItemModel() { Value = "hourly",  Description = "hourly" },
                new ComboboxItemModel() { Value = "daily",   Description = "daily" },
                new ComboboxItemModel() { Value = "weekly",  Description = "weekly" },
                new ComboboxItemModel() { Value = "monthly", Description = "monthly" },
                new ComboboxItemModel() { Value = "total",   Description = "total" },
            };
            SelectedItem = Items.First();
        }
    }
}
