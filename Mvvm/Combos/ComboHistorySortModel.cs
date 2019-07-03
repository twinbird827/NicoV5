using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NicoV5.Mvvm.Combos
{
    public class ComboHistorySortModel : ComboboxModel
    {        /// <summary>
             /// ｲﾝｽﾀﾝｽ (ｼﾝｸﾞﾙﾄﾝﾊﾟﾀｰﾝ)
             /// </summary>
        public static ComboHistorySortModel Instance { get; } = new ComboHistorySortModel();

        private ComboHistorySortModel()
        {
            Items = new ObservableCollection<ComboboxItemModel>
            {
                new ComboboxItemModel() { Value = "2", Description = "DATE" },
                new ComboboxItemModel() { Value = "1", Description = "ID" },
                new ComboboxItemModel() { Value = "3", Description = "COUNT" },
            };
        }
    }
}
