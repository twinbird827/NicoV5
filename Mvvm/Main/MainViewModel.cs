using MahApps.Metro.Controls.Dialogs;
using NicoV5.Common.Databases;
using NicoV5.Mvvm.Models;
using NicoV5.Mvvm.Services;
using NicoV5.Mvvm.WorkSpaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using WpfUtilV2.Common;
using WpfUtilV2.Mvvm;
using WpfUtilV2.Mvvm.Service;

namespace NicoV5.Mvvm.Main
{
    public class MainViewModel : BindableBase
    {
        /// <summary>
        /// ｺﾝｽﾄﾗｸﾀ
        /// </summary>
        public MainViewModel() : base()
        {
            if (!WpfUtil.IsDesignMode() && Instance != null)
            {
                throw new InvalidOperationException("本ViewModelは複数のｲﾝｽﾀﾝｽを作成することができません。");
            }
            Instance = this;

            // ﾒｯｾｰｼﾞｻｰﾋﾞｽの変更
            ServiceFactory.MessageService = new WpfMessageService();

            // 画面終了時ｲﾍﾞﾝﾄ
            Disposed += (sender, e) =>
            {
                // ｶﾚﾝﾄ破棄
                Current?.Dispose();
            };
        }

        /// ****************************************************************************************************
        /// 画面表示時
        /// ****************************************************************************************************

        /// <summary>
        /// 画面表示時の処理
        /// </summary>
        public ICommand OnLoaded
        {
            get { return _OnLoaded = _OnLoaded ?? new RelayCommand(Loaded_Command); }
        }
        private ICommand _OnLoaded;

        /// <summary>
        /// 画面表示時の処理
        /// </summary>
        private async void Loaded_Command(object dummy)
        {
            await ShowProgressAsync(async (pdc) =>
            {
                pdc.SetIndeterminate();
                pdc.SetTitle("初期化中");

                using (var accessor = DbAccessor.GetAccessor())
                using (var control = accessor.GetCommand())
                {
                    pdc.SetMessage("データベースの初期化中");

                    // 全体の初期化
                    await control.BeginTransaction();
                    await control.Initialize();
                    await control.Commit();

                    pdc.SetMessage("設定情報の初期化中");

                    // SettingModelの初期化
                    SettingModel.Initialize(await control.GetSetting());

                    pdc.SetMessage("履歴情報の初期化");

                    // ViewModelの初期化
                    await SearchVideoByHistoryModel.Initialize(await control.GetVideoHistory());

                    pdc.SetMessage("ﾃﾝﾎﾟﾗﾘの初期化");

                    // Temporaryの初期化
                    await SearchVideoByTemporaryModel.Initialize(await control.GetTemporaryHistory());

                    pdc.SetMessage("お気に入りの初期化");

                    // SearchMylistModelの初期化
                    SearchMylistModel.Initialize(await control.GetFavorite());

                    pdc.SetMessage("ｶﾚﾝﾄ設定");

                    // ｶﾚﾝﾄ設定
                    Current = new SearchVideoByRankingViewModel();
                }
            });

        }
        /// ****************************************************************************************************
        /// ﾌﾟﾛﾊﾟﾃｨ定義
        /// ****************************************************************************************************

        /// <summary>
        /// 本ｲﾝｽﾀﾝｽ(ｼﾝｸﾞﾙﾄﾝ)
        /// </summary>
        public static MainViewModel Instance { get; private set; }

        /// <summary>
        /// ﾀﾞｲｱﾛｸﾞ表示用ｲﾝｽﾀﾝｽ
        /// </summary>
        public IDialogCoordinator DialogCoordinator { get; set; }

        /// <summary>
        /// ｶﾚﾝﾄﾜｰｸｽﾍﾟｰｽ
        /// </summary>
        public WorkSpaceViewModel Current
        {
            get { return _Current; }
            set { SetProperty(ref _Current, value, true); }
        }
        private WorkSpaceViewModel _Current;

        public int TemporaryCount
        {
            get { return _TemporaryCount; }
            set { SetProperty(ref _TemporaryCount, value, true); }
        }
        private int _TemporaryCount;

        /// ****************************************************************************************************
        /// ﾒｿｯﾄﾞ定義
        /// ****************************************************************************************************

        /// <summary>
        /// ﾒｯｾｰｼﾞﾀﾞｲｱﾛｸﾞを表示します。
        /// </summary>
        /// <param name="title">ﾀｲﾄﾙ</param>
        /// <param name="message">ﾒｯｾｰｼﾞ</param>
        /// <param name="style">ﾀﾞｲｱﾛｸﾞｽﾀｲﾙ</param>
        /// <param name="settings">設定情報</param>
        /// <returns><code>MessageDialogResult</code></returns>
        public async Task<MessageDialogResult> ShowMessageAsync(string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings settings = null)
        {
            return await DialogCoordinator.ShowMessageAsync(this, title, message, style, settings);
        }

        /// <summary>
        /// 入力ﾀﾞｲｱﾛｸﾞを表示します。
        /// </summary>
        /// <param name="title">ﾀｲﾄﾙ</param>
        /// <param name="message">ﾒｯｾｰｼﾞ</param>
        /// <param name="settings">設定情報</param>
        /// <returns>入力値</returns>
        public async Task<string> ShowInputAsync(string title, string message, MetroDialogSettings settings = null)
        {
            return await DialogCoordinator.ShowInputAsync(this, title, message, settings);
        }

        /// <summary>
        /// 時間のかかる処理を実行します。
        /// </summary>
        /// <param name="method">時間のかかる処理</param>
        /// <param name="settings">設定情報</param>
        public async Task ShowProgressAsync(Func<ProgressDialogController, Task> method, MetroDialogSettings settings = null)
        {
            var pdc = await DialogCoordinator.ShowProgressAsync(this, null, null, false, settings);

            await method(pdc);

            await pdc.CloseAsync();
        }

        /// <summary>
        /// 時間のかかる処理を実行します。
        /// </summary>
        /// <param name="method">時間のかかる処理</param>
        /// <param name="settings">設定情報</param>
        public void ShowProgress(Func<ProgressDialogController, Task> method, MetroDialogSettings settings = null)
        {
            ShowProgressAsync(method, settings).ConfigureAwait(false);
        }

        /// <summary>
        /// ｶｽﾀﾑﾀﾞｲｱﾛｸﾞを表示します。
        /// </summary>
        /// <param name="dialog">ｶｽﾀﾑﾀﾞｲｱﾛｸﾞのｲﾝｽﾀﾝｽ</param>
        /// <param name="settings">設定情報</param>
        /// <returns><code>Task</code></returns>
        public Task ShowMetroDialogAsync(BaseMetroDialog dialog, MetroDialogSettings settings = null)
        {
            return DialogCoordinator.ShowMetroDialogAsync(this, dialog, settings);
        }

        /// <summary>
        /// ｶｽﾀﾑﾀﾞｲｱﾛｸﾞを非表示にします。
        /// </summary>
        /// <param name="dialog">ｶｽﾀﾑﾀﾞｲｱﾛｸﾞのｲﾝｽﾀﾝｽ</param>
        /// <param name="settings">設定情報</param>
        /// <returns><code>Task</code></returns>
        public Task HideMetroDialogAsync(BaseMetroDialog dialog, MetroDialogSettings settings = null)
        {
            return DialogCoordinator.HideMetroDialogAsync(this, dialog, settings);
        }

        /// ****************************************************************************************************
        /// ｺﾏﾝﾄﾞ定義
        /// ****************************************************************************************************

        /// <summary>
        /// ﾒﾆｭｰ処理
        /// </summary>
        public ICommand OnClickMenu
        {
            get { return _OnClickMenu = _OnClickMenu ?? new RelayCommand<MenuType>(Menu_Command); }
        }
        public ICommand _OnClickMenu;

        /// <summary>
        /// ﾒﾆｭｰ処理
        /// </summary>
        private void Menu_Command(MenuType type)
        {
            switch (type)
            {
                case MenuType.SearchByRanking:
                    Current = new SearchVideoByRankingViewModel();
                    break;
                case MenuType.SearchByTemporary:
                    Current = new SearchVideoByTemporaryViewModel();
                    break;
                case MenuType.SearchByHistory:
                    Current = new SearchVideoByHistoryViewModel();
                    break;
                case MenuType.SearchByMylist:
                    Current = new SearchVideoByMylistViewModel();
                    break;
                case MenuType.SearchMylist:
                    Current = new SearchMylistViewModel();
                    break;
                case MenuType.Setting:
                    Current = new SettingViewModel();
                    break;
            }
        }

    }
}
