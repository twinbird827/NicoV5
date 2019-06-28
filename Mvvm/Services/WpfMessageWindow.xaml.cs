using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NicoV5.Mvvm.Services
{
    /// <summary>
    /// WpfMessageWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class WpfMessageWindow : MetroWindow
    {
        public WpfMessageWindow(string title, string message, WpfMessageType messageType)
        {
            InitializeComponent();

            DataContext = new WpfMessageViewModel()
            {
                Title = title,
                Message = message,
                MessageType = messageType
            };
        }
    }
}
