using NicoV5.Mvvm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfUtilV2.Mvvm;

namespace NicoV5.Mvvm.Components
{
    public class VideoHistoryViewModel : VideoViewModel
    {
        public VideoHistoryViewModel(VideoModel source, long count) : base(source)
        {
            Count = count;
        }

        public long Count
        {
            get { return _Count; }
            set { SetProperty(ref _Count, value); }
        }
        private long _Count;

        public override ICommand OnDoubleClick
        {
            get
            {
                return _OnDoubleClick = _OnDoubleClick ?? new RelayCommand(_ =>
                {
                    // ｶｳﾝﾄ+
                    Count++;

                    // ﾌﾞﾗｳｻﾞ表示
                    base.OnDoubleClick.Execute(null);
                });
            }
        }
        private ICommand _OnDoubleClick;
    }
}
