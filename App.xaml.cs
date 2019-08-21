using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WpfUtilV2.Mvvm.Service;

namespace NicoV5
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        // 例外をキャッチするイベントハンドラ
        public static void Application_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            if (ex == null)
            {
                return;
            }

            // 画面にﾒｯｾｰｼﾞ表示
            ServiceFactory.MessageService.Exception(ex);

            // ｲﾍﾞﾝﾄﾋﾞｭｰｱにﾒｯｾｰｼﾞ表示
            using (var log = new EventLog())
            {
                log.Source = typeof(App).FullName;
                log.WriteEntry(ex.ToString(), EventLogEntryType.Error);
            }

        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += Application_UnhandledException;
        }
    }
}
