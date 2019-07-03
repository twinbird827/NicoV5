using NicoV5.Common.Databases;
using NicoV5.Mvvm.Combos;
using NicoV5.Mvvm.Components;
using NicoV5.Mvvm.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NicoV5.Mvvm.WorkSpaces
{
    public class SearchVideoByHistoryViewModel : WorkSpaceViewModel
    {
        public SearchVideoByHistoryViewModel()
        {
            Videos = new ObservableCollection<VideoHistoryViewModel>();

            Loaded += async (sender, e) => await Reload(sender, e);
        }

        /// <summary>
        /// ｼﾞｬﾝﾙ
        /// </summary>
        public ComboHistorySortModel Sort { get; set; }

        /// <summary>
        /// 一覧
        /// </summary>
        public ObservableCollection<VideoHistoryViewModel> Videos
        {
            get { return _Videos; }
            set { SetProperty(ref _Videos, value); }
        }
        private ObservableCollection<VideoHistoryViewModel> _Videos;

        /// <summary>
        /// 再表示処理
        /// </summary>
        private async Task Reload(object sender, EventArgs e)
        {
            Videos.Clear();
            using (var accessor = DbAccessor.GetAccessor())
            using (var control = accessor.GetCommand())
            {
                foreach (var vvh in await control.GetVideoHistoryView(int.Parse(Sort.SelectedItem.Value)))
                {
                    Videos.Add(new VideoHistoryViewModel(await VideoModel.CreateInstance(vvh), vvh.Count));
                }
            }
        }

    }
}
