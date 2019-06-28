using NicoV5.Common;
using NicoV5.Common.Databases;
using NicoV5.Common.Tables;
using NicoV5.Mvvm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfUtilV2.Mvvm;

namespace NicoV5.Mvvm.WorkSpaces
{
    public class SettingViewModel : WorkSpaceViewModel
    {
        public SettingViewModel()
        {
            MailAddress = SettingModel.Instance.MailAddress;
            Password = SettingModel.Instance.Password;
        }

        /// <summary>
        /// メールアドレス
        /// </summary>
        public string MailAddress
        {
            get { return _MailAddress; }
            set { SetProperty(ref _MailAddress, value); }
        }
        private string _MailAddress = default(string);

        /// <summary>
        /// パスワード
        /// </summary>
        public string Password
        {
            get { return _Password; }
            set { SetProperty(ref _Password, value); }
        }
        private string _Password = default(string);

        /// <summary>
        /// ブラウザ
        /// </summary>
        public string Browser
        {
            get { return _Browser; }
            set { SetProperty(ref _Browser, value); }
        }
        private string _Browser = default(string);

        /// <summary>
        /// ログイン処理
        /// </summary>
        public ICommand OnClickLogin
        {
            get
            {
                return _OnClickLogin = _OnClickLogin ?? new RelayCommand(
                async _ => 
                {
                    using (var accessor = DbAccessor.GetAccessor())
                    using (var control = accessor.GetCommand())
                    {
                        await control.BeginTransaction();
                        await control.InsertOrReplaceSetting(
                            new TSetting(SettingKeys.MailAddress, MailAddress),
                            new TSetting(SettingKeys.Password, NicoUtil.EncryptString(Password))
                        );
                        await control.Commit();
                    }
                },
                _ => 
                {
                    return !string.IsNullOrEmpty(MailAddress) && !string.IsNullOrEmpty(Password);
                });
            }
        }
        private ICommand _OnClickLogin;

    }
}