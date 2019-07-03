using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NicoV5.Common;

namespace NicoV5.Mvvm.Combos
{
    public class ComboVMylistSortModel : ComboboxModel
    {
        /// <summary>
        /// ｲﾝｽﾀﾝｽ (ｼﾝｸﾞﾙﾄﾝﾊﾟﾀｰﾝ)
        /// </summary>
        public static ComboVMylistSortModel Instance { get; } = new ComboVMylistSortModel();

        public override SettingKeys Key => SettingKeys.ComboVMylistSort;

        private ComboVMylistSortModel()
        {
            Items = new ObservableCollection<ComboboxItemModel>
            {
                new ComboboxItemModel() { Value = "0", Description = "SORT_MYREG0" },
                new ComboboxItemModel() { Value = "1", Description = "SORT_MYREG1" },
                new ComboboxItemModel() { Value = "2", Description = "SORT_MYCOMMENT0" },
                new ComboboxItemModel() { Value = "3", Description = "SORT_MYCOMMENT1" },
                new ComboboxItemModel() { Value = "4", Description = "SORT_TITLE0" },
                new ComboboxItemModel() { Value = "5", Description = "SORT_TITLE1" },
                new ComboboxItemModel() { Value = "6", Description = "SORT_UPLOAD0" },
                new ComboboxItemModel() { Value = "7", Description = "SORT_UPLOAD1" },
                new ComboboxItemModel() { Value = "8", Description = "SORT_VIEWRES0" },
                new ComboboxItemModel() { Value = "9", Description = "SORT_VIEWRES1" },
                new ComboboxItemModel() { Value = "10", Description = "SORT_COMMENT0" },
                new ComboboxItemModel() { Value = "11", Description = "SORT_COMMENT1" },
                new ComboboxItemModel() { Value = "12", Description = "SORT_COMMENTRES0" },
                new ComboboxItemModel() { Value = "13", Description = "SORT_COMMENTRES1" },
                new ComboboxItemModel() { Value = "14", Description = "SORT_MYREGRES0" },
                new ComboboxItemModel() { Value = "15", Description = "SORT_MYREGRES1" },
                new ComboboxItemModel() { Value = "16", Description = "SORT_LENGTH0" },
                new ComboboxItemModel() { Value = "17", Description = "SORT_LENGTH1" },
            };
        }
    }
}
