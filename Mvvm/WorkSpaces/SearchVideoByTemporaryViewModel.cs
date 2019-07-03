using NicoV5.Mvvm.Components;
using NicoV5.Mvvm.Main;
using NicoV5.Mvvm.Models;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfUtilV2.Mvvm;
using WpfUtilV2.Mvvm.Service;

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

        /// <summary>
        /// ADDﾎﾞﾀﾝ押下時
        /// </summary>
        public ICommand OnTemporaryAdd
        {
            get
            {
                return _OnTemporaryAdd = _OnTemporaryAdd ?? new RelayCommand(async _ =>
                {
                    // URL入力欄を表示
                    string result = await MainViewModel.Instance.ShowInputAsync(
                        "ADD TEMPORARY",
                        "Please enter the URL of video to be added.");

                    if (string.IsNullOrEmpty(result))
                    {
                        ServiceFactory.MessageService.Error($"Please enter a URL.");
                        return;
                    }

                    var video = await VideoModel.CreateInstance(result);

                    if (video == null)
                    {
                        ServiceFactory.MessageService.Error($"{result} is not a valid URL.");
                        return;
                    }

                    await Source.AddVideo(video);
                });
            }
        }
        public ICommand _OnTemporaryAdd;

    }
}
