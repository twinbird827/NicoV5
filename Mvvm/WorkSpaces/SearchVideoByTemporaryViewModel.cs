using NicoV5.Mvvm.Components;
using NicoV5.Mvvm.Models;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NicoV5.Mvvm.WorkSpaces
{
    public class SearchVideoByTemporaryViewModel : WorkSpaceViewModel
    {
        public SearchVideoByTemporaryViewModel()
        {
            Source = SearchVideoByTemporaryModel.Instance;

            Videos = Source.Videos.ToSyncedSynchronizationContextCollection(
                video => new VideoViewModel(video),
                AnonymousSynchronizationContext.Current
            );
        }

        private SearchVideoByTemporaryModel Source { get; set; }

        /// <summary>
        /// ﾒｲﾝ項目ﾘｽﾄ
        /// </summary>
        public SynchronizationContextCollection<VideoViewModel> Videos
        {
            get { return _Videos; }
            set { SetProperty(ref _Videos, value); }
        }
        private SynchronizationContextCollection<VideoViewModel> _Videos;

    }
}
