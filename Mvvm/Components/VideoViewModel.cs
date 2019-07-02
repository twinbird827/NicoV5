using NicoV5.Mvvm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfUtilV2.Extensions;
using WpfUtilV2.Mvvm;

namespace NicoV5.Mvvm.Components
{
    public class VideoViewModel : BindableBase
    {
        public VideoViewModel(VideoModel source)
        {
            Source = source;

            // 初期値設定
            Thumbnail = Source.Thumbnail;
            Status = Source.Status.GetLabel();
            LengthSeconds = TimeSpan.FromSeconds(Source.LengthSeconds);
            Title = Source.Title;
            Description = Source.Description;
            ViewCounter = Source.ViewCounter;
            MylistCounter = Source.MylistCounter;
            CommentCounter = Source.CommentCounter;
            StartDate = Source.StartTime;

            // ﾓﾃﾞﾙ側で変更があったら通知する
            Source.AddOnPropertyChanged(this, (sender, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(Thumbnail):
                        Thumbnail = Source.Thumbnail;
                        break;
                    case nameof(Status):
                        Status = Source.Status.GetLabel();
                        break;
                    case nameof(LengthSeconds):
                        LengthSeconds = TimeSpan.FromSeconds(Source.LengthSeconds);
                        break;
                    case nameof(Title):
                        Title = Source.Title;
                        break;
                    case nameof(Description):
                        Description = Source.Description;
                        break;
                    case nameof(ViewCounter):
                        ViewCounter = Source.ViewCounter;
                        break;
                    case nameof(MylistCounter):
                        MylistCounter = Source.MylistCounter;
                        break;
                    case nameof(CommentCounter):
                        CommentCounter = Source.CommentCounter;
                        break;
                    case nameof(StartDate):
                        StartDate = Source.StartTime;
                        break;
                }
            });
        }

        private VideoModel Source { get; set; }

        public BitmapImage Thumbnail
        {
            get { return _Thumbnail; }
            set { SetProperty(ref _Thumbnail, value); }
        }
        private BitmapImage _Thumbnail;

        public string Status
        {
            get { return _Status; }
            set { SetProperty(ref _Status, value); }
        }
        private string _Status;

        public TimeSpan LengthSeconds
        {
            get { return _LengthSeconds; }
            set { SetProperty(ref _LengthSeconds, value); }
        }
        private TimeSpan _LengthSeconds;

        public string Title
        {
            get { return _Title; }
            set { SetProperty(ref _Title, value); }
        }
        private string _Title;

        public string Description
        {
            get { return _Description; }
            set { SetProperty(ref _Description, value); }
        }
        private string _Description;

        public double ViewCounter
        {
            get { return _ViewCounter; }
            set { SetProperty(ref _ViewCounter, value); }
        }
        private double _ViewCounter;

        public double MylistCounter
        {
            get { return _MylistCounter; }
            set { SetProperty(ref _MylistCounter, value); }
        }
        private double _MylistCounter;

        public double CommentCounter
        {
            get { return _CommentCounter; }
            set { SetProperty(ref _CommentCounter, value); }
        }
        private double _CommentCounter;

        public DateTime StartDate
        {
            get { return _StartDate; }
            set { SetProperty(ref _StartDate, value); }
        }
        private DateTime _StartDate;

        /// <summary>
        /// 項目ﾀﾞﾌﾞﾙｸﾘｯｸ時ｲﾍﾞﾝﾄ
        /// </summary>
        public ICommand OnDoubleClick
        {
            get
            {
                return _OnDoubleClick = _OnDoubleClick ?? new RelayCommand(
                async _ =>
                {
                    // ﾌﾞﾗｳｻﾞ表示
                    await Source.Open();
                });
            }
        }
        public ICommand _OnDoubleClick;

        /// <summary>
        /// 項目ｷｰ入力時ｲﾍﾞﾝﾄ
        /// </summary>
        public ICommand OnKeyDown
        {
            get
            {
                return _OnKeyDown = _OnKeyDown ?? new RelayCommand<KeyEventArgs>(
                e =>
                {
                    switch (e.Key)
                    {
                        case Key.Enter:
                            // ﾀﾞﾌﾞﾙｸﾘｯｸと同じ処理
                            OnDoubleClick.Execute(null);
                            break;
                        case Key.Delete:
                            OnTemporaryDel.Execute(null);
                            break;
                    }
                });
            }
        }
        public ICommand _OnKeyDown;

        /// <summary>
        /// 項目をﾃﾝﾎﾟﾗﾘに追加する
        /// </summary>
        public ICommand OnTemporaryAdd
        {
            get
            {
                return _OnTemporaryAdd = _OnTemporaryAdd ?? new RelayCommand(
                async e =>
                {
                    // ﾃﾝﾎﾟﾗﾘに追加
                    await SearchVideoByTemporaryModel.Instance.AddVideo(Source);
                });
            }
        }
        public ICommand _OnTemporaryAdd;

        /// <summary>
        /// 項目をﾃﾝﾎﾟﾗﾘから削除する
        /// </summary>
        public ICommand OnTemporaryDel
        {
            get
            {
                return _OnTemporaryDel = _OnTemporaryDel ?? new RelayCommand(
                async e =>
                {
                    // ﾃﾝﾎﾟﾗﾘから削除
                    await SearchVideoByTemporaryModel.Instance.DeleteVideo(Source);
                });
            }
        }
        public ICommand _OnTemporaryDel;

        /// <summary>
        /// 動画をﾀﾞｳﾝﾛｰﾄﾞする。
        /// </summary>
        public ICommand OnDownload
        {
            get
            {
                return _OnDownload = _OnDownload ?? new RelayCommand(
                _ =>
                {
                    //string result = await MainWindowViewModel.Instance.ShowInputAsync(
                    //    Resources.L_DOWNLOAD,
                    //    Resources.L_DOWNLOAD);

                    //await Source.Download(result);
                    //DownloadModel.Instance.Downloads.Add(Source);
                });
            }
        }
        public ICommand _OnDownload;

    }
}
