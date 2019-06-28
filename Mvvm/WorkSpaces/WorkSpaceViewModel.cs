using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfUtilV2.Mvvm;

namespace NicoV5.Mvvm.WorkSpaces
{
    public class WorkSpaceViewModel : BindableBase
    {
        /// <summary>
        /// ﾀｲﾄﾙ
        /// </summary>
        public virtual string Title
        {
            get { return _Title; }
            set { SetProperty(ref _Title, value); }
        }
        private string _Title = default(string);

        /// <summary>
        /// ｱﾌﾟﾘｹｰｼｮﾝﾀｲﾄﾙ
        /// </summary>
        public string ApplicationTitle
        {
            get { return CreateTitle(); }
        }

        /// <summary>
        /// 画面表示時の処理
        /// </summary>
        public ICommand OnLoaded
        {
            get { return _OnLoaded = _OnLoaded ?? new RelayCommand(Loaded_Execute); }
        }
        private ICommand _OnLoaded;

        /// <summary>
        /// 画面表示時に発生するｲﾍﾞﾝﾄ
        /// </summary>
        public event EventHandler Loaded;

        /// <summary>
        /// 画面表示時の処理
        /// </summary>
        private void Loaded_Execute(object dummy)
        {
            Loaded?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// 画面ﾀｲﾄﾙを作成する。
        /// </summary>
        /// <returns>画面ﾀｲﾄﾙ</returns>
        private string CreateTitle()
        {
            // ｱｾﾝﾌﾞﾘ
            var assembly = this.GetType().Assembly;

            // ｱｾﾝﾌﾞﾘ名のみを取得
            var assemblyName = System.IO.Path.GetFileNameWithoutExtension(assembly.Location);

            // ﾊﾞｰｼﾞｮﾝを取得
            var version = assembly.GetName().Version;

            // ｱｾﾝﾌﾞﾘ名とﾊﾞｰｼﾞｮﾝを結合して画面ﾀｲﾄﾙを作成
            return $"{Title} Ver: {version.Major}.{version.Minor}.{version.Revision}.{version.Build}";
        }
    }
}
