using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NicoV5.Common;
using WpfUtilV2.Mvvm;

namespace NicoV5.Mvvm.Combos
{
    public class ComboRankPeriodModel : ComboboxModel
    {
        /// <summary>
        /// ｲﾝｽﾀﾝｽ (ｼﾝｸﾞﾙﾄﾝﾊﾟﾀｰﾝ)
        /// </summary>
        public static ComboRankPeriodModel Instance { get; } = new ComboRankPeriodModel();

        public override SettingKeys Key => SettingKeys.ComboRankPeriod;

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
        }
    }
}
