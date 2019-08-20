using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WpfUtilV2.Extensions;
using WpfUtilV2.Mvvm.Service;

namespace NicoV5.Mvvm.Services
{
    public class WpfMessageService : ConsoleMessageService
    {
        /// <summary>
        /// ｲﾝﾌｫﾒｰｼｮﾝﾒｯｾｰｼﾞを画面に表示します。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        public override void Info(string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            base.Info(message, callerMemberName, callerFilePath, callerLineNumber);

            var window = new WpfMessageWindow("Information", message, WpfMessageType.Info);
            window.ShowModalWindow();
        }

        /// <summary>
        /// ｴﾗｰﾒｯｾｰｼﾞを画面に表示します。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        public override void Error(string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            base.Error(message, callerMemberName, callerFilePath, callerLineNumber);

            var window = new WpfMessageWindow("Error", message, WpfMessageType.Error);
            window.ShowModalWindow();
        }

        /// <summary>
        /// 確認ﾒｯｾｰｼﾞを画面に表示します。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        /// <returns></returns>
        public override bool Confirm(string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            base.Confirm(message, callerMemberName, callerFilePath, callerLineNumber);

            var window = new WpfMessageWindow("Confirm", message, WpfMessageType.Confirm);
            return window.ShowModalWindow();
        }

        /// <summary>
        /// 例外ﾒｯｾｰｼﾞを画面に表示します。
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        public override void Exception(Exception exception, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            base.Exception(exception, callerMemberName, callerFilePath, callerLineNumber);

            var window = new WpfMessageWindow("Exception", exception.ToString(), WpfMessageType.Error);
            window.ShowModalWindow();
        }
    }
}
