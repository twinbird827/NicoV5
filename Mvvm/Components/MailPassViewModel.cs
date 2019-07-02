using NicoV5.Mvvm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUtilV2.Mvvm;
using WpfUtilV2.Mvvm.Service;

namespace NicoV5.Mvvm.Components
{
    public class MailPassViewModel : ModalWindowViewModel
    {
        public MailPassViewModel()
        {
            MailAddress = SettingModel.Instance.MailAddress;
            Password = SettingModel.Instance.Password;
        }

        protected override bool CanClickOK<T>(T parameter)
        {
            if (string.IsNullOrEmpty(MailAddress) && string.IsNullOrEmpty(Password))
            {
                ServiceFactory.MessageService.Error("ﾒｰﾙｱﾄﾞﾚｽとﾊﾟｽﾜｰﾄﾞを入力してください。");
                return false;
            }
            else if (!Session.Instance.CanLogin(MailAddress, Password))
            {
                ServiceFactory.MessageService.Error("指定したﾒｰﾙｱﾄﾞﾚｽとﾊﾟｽﾜｰﾄﾞではﾛｸﾞｲﾝできませんでした。");
                return false;
            }
            return true;
        }

        public string MailAddress
        {
            get { return _MailAddress; }
            set { SetProperty(ref _MailAddress, value); }
        }
        private string _MailAddress;

        public string Password
        {
            get { return _Password; }
            set { SetProperty(ref _Password, value); }
        }
        private string _Password;
    }
}
