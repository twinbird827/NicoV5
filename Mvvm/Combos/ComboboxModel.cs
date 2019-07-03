using NicoV5.Common;
using NicoV5.Common.Databases;
using NicoV5.Common.Tables;
using NicoV5.Mvvm.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUtilV2.Mvvm;

namespace NicoV5.Mvvm.Combos
{
    public class ComboboxModel : BindableBase
    {
        /// <summary>
        /// ｿｰﾄﾘｽﾄ構成
        /// </summary>
        public ObservableCollection<ComboboxItemModel> Items
        {
            get { return _Items; }
            set { if (SetProperty(ref _Items, value)) SelectedItem = Items[GetInitializeIndex()]; }
        }
        private ObservableCollection<ComboboxItemModel> _Items;

        /// <summary>
        /// 選択中の構成
        /// </summary>
        public ComboboxItemModel SelectedItem
        {
            get { return _SelectedItem; }
            set { if (SetProperty(ref _SelectedItem, value)) Change_SelectedItem(); }
        }
        private ComboboxItemModel _SelectedItem;

        public virtual SettingKeys Key { get; }

        private int GetInitializeIndex()
        {
            var index = int.Parse(SettingModel.Instance.GetSetting(Key));

            if (index < Items.Count)
            {
                return index;
            }
            else
            {
                return 0;
            }
        }

        private async void Change_SelectedItem()
        {
            var index = Items.IndexOf(SelectedItem).ToString();

            if (SettingModel.Instance.GetSetting(Key) == index)
            {
                return;
            }

            using (var accessor = DbAccessor.GetAccessor())
            using (var control = accessor.GetCommand())
            {
                await control.BeginTransaction();
                await control.InsertOrReplaceSetting(new TSetting(Key, index));
                await control.Commit();
            }
        }
    }
}
