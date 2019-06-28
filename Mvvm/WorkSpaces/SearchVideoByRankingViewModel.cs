using NicoV5.Mvvm.Combos;
using NicoV5.Mvvm.Components;
using NicoV5.Mvvm.Models;
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
        public SearchVideoByRankingViewModel()
        {
            Source = new SearchVideoByRankingModel();
            Videos = new ObservableCollection<VideoViewModel>();
            Genre = ComboRankGenreModel.Instance;
            Period = ComboRankPeriodModel.Instance;

            Genre.AddOnPropertyChanged(this, Combo_ChangeSelectedItem);
            Period.AddOnPropertyChanged(this, Combo_ChangeSelectedItem);
            Loaded += async (sender, e) => await Reload(sender, e);
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

        private async void Combo_ChangeSelectedItem(object sender, PropertyChangedEventArgs e)
        {
            var combo = sender as ComboboxModel;

            if (combo == null) return;

            if (e.PropertyName != nameof(combo.SelectedItem)) return;

            await Reload(sender, e);
        }

        private async Task Reload(object sender, EventArgs e)
        {
            Videos.Clear();
            foreach (var video in await Source.GetRanking(Period.SelectedItem, Genre.SelectedItem))
            {
                Videos.Add(new VideoViewModel(video));
            }
        }
    }
}
