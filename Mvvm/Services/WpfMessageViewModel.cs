using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WpfUtilV2.Common;
using WpfUtilV2.Mvvm;

namespace NicoV5.Mvvm.Services
{
    public class WpfMessageViewModel : ModalWindowViewModel
    {
        public string Title
        {
            get { return _Title; }
            set { SetProperty(ref _Title, value); }
        }
        private string _Title;

        public string Message
        {
            get { return _Message; }
            set { SetProperty(ref _Message, value); }
        }
        private string _Message;

        public WpfMessageType MessageType
        {
            get { return _MessageType; }
            set
            {
                SetProperty(ref _MessageType, value);
                OnPropertyChanged(nameof(Icon));
                OnPropertyChanged(nameof(IsOkOnly));
                OnPropertyChanged(nameof(IsOkCancel));
            }
        }
        private WpfMessageType _MessageType;

        public ImageSource Icon
        {
            get
            {
                switch (MessageType)
                {
                    case WpfMessageType.Info:
                        return WpfUtil.ToImageSource(SystemIcons.Information);
                    case WpfMessageType.Error:
                        return WpfUtil.ToImageSource(SystemIcons.Error);
                    case WpfMessageType.Confirm:
                        return WpfUtil.ToImageSource(SystemIcons.Question);
                    default:
                        return WpfUtil.ToImageSource(SystemIcons.Information);
                }
            }
        }

        public bool IsOkOnly
        {
            get { return MessageType != WpfMessageType.Confirm; }
        }

        public bool IsOkCancel
        {
            get { return !IsOkOnly; }
        }
    }
}
