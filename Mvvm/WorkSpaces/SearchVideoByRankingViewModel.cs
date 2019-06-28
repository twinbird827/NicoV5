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
    public class SearchVideoByRankingViewModel : WorkSpaceViewModel
    {
        public SearchVideoByRankingViewModel()
        {
            Source = new SearchVideoByRankingModel();
            Videos = new ObservableCollection<VideoViewModel>();
            Genre = ComboRankGenreModel.Instance;
            Period = ComboRankPeriodModel.Instance;
        }

        private SearchVideoByRankingModel Source { get; set; }

        public ObservableCollection<VideoViewModel> Videos
        {
            get { return _Videos; }
            set { SetProperty(ref _Videos, value); }
        }
        private ObservableCollection<VideoViewModel> _Videos;

        public ComboRankGenreModel Genre { get; set; }

        public ComboRankPeriodModel Period { get; set; }

        private async void Reload(object sender, EventArgs e)
        {
            Videos.Clear();
            foreach (var video in await Source.GetRanking(Period.SelectedItem, Genre.SelectedItem))
            {

            }
        }
    }
}
