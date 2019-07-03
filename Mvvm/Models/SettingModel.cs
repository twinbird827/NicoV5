using NicoV5.Common;
using NicoV5.Common.Databases;
using NicoV5.Common.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUtilV2.Mvvm;

namespace NicoV5.Mvvm.Models
{
    public class SettingModel : BindableBase
    {
        private SettingModel()
        {

        }

        /// <summary>
        /// ｼﾝｸﾞﾙﾄﾝﾊﾟﾀｰﾝ
        /// </summary>
        public static SettingModel Instance { get; private set; }

        /// <summary>
        /// 内部保持情報
        /// </summary>
        private List<TSetting> Settings { get; set; }

        /// <summary>
        /// SettingModelを初期化します。
        /// </summary>
        /// <param name="settings"></param>
        public static void Initialize(IEnumerable<TSetting> settings)
        {
            Instance = new SettingModel();
            Instance.Settings = settings.ToList();
        }

        /// <summary>
        /// 設定情報を取得します。
        /// </summary>
        /// <param name="key">取得するｷｰ</param>
        public string GetSetting(SettingKeys key)
        {
            return Settings?.FirstOrDefault(s => s.Key == key)?.Value;
        }

        /// <summary>
        /// 設定情報を設定します。
        /// </summary>
        /// <param name="key">設定するｷｰ</param>
        /// <param name="value">設定値</param>
        /// <returns>同値ならfalse, それ以外はtrue</returns>
        private bool SetSetting(SettingKeys key, string value)
        {
            if (GetSetting(key) == value)
            {
                return false;
            }
            else if (Settings.Any(s => s.Key == key))
            {
                Settings.First(s => s.Key == key).Value = value;
            }
            else
            {
                Settings.Add(new TSetting(key, value));
            }
            return true;
        }

        /// <summary>
        /// ﾒｰﾙｱﾄﾞﾚｽ
        /// </summary>
        public string MailAddress
        {
            get { return GetSetting(SettingKeys.MailAddress); }
            set { if (SetSetting(SettingKeys.MailAddress, value)) OnPropertyChanged(); }
        }

        /// <summary>
        /// ﾊﾟｽﾜｰﾄﾞ
        /// </summary>
        public string Password
        {
            get { return NicoUtil.DecryptString(GetSetting(SettingKeys.Password)); }
            set { if (SetSetting(SettingKeys.Password, NicoUtil.EncryptString(value))) OnPropertyChanged(); }
        }

        /// <summary>
        /// ﾌﾞﾗｳｻﾞ
        /// </summary>
        public string Browser
        {
            get { return GetSetting(SettingKeys.Browser); }
            set { if (SetSetting(SettingKeys.Browser, value)) OnPropertyChanged(); }
        }

    }
}
