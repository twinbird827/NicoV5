using NicoV5.Common.Databases;
using NicoV5.Common.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUtilV2.Common;
using WpfUtilV2.Mvvm;

namespace NicoV5.Mvvm.Models
{
    public class SearchVideoByHistoryModel : BindableBase
    {
        private SearchVideoByHistoryModel()
        {

        }

        /// <summary>
        /// ｼﾝｸﾞﾙﾄﾝﾊﾟﾀｰﾝ
        /// </summary>
        public static SearchVideoByHistoryModel Instance { get; private set; }

        /// <summary>
        /// 非同期ﾀｲﾏｰ
        /// </summary>
        public AsyncTimer Timer { get; set; }

        /// <summary>
        /// 内部保持情報
        /// </summary>
        private List<TVideoHistory> Histories { get; set; }

        /// <summary>
        /// SettingModelを初期化します。
        /// </summary>
        /// <param name="views"></param>
        public static async Task Initialize(List<TVideoHistory> views)
        {
            Instance = new SearchVideoByHistoryModel();

            await Instance.InitializePrivate(views);
        }

        private async Task InitializePrivate(List<TVideoHistory> views)
        {
            Histories = views;

            // 12分毎にﾘﾛｰﾄﾞするﾀｲﾏｰを設定
            Timer = new AsyncTimer();
            Timer.Interval = TimeSpan.FromMinutes(12);
            Timer.Tick += async (sender, e) =>
            {
                // 履歴に削除されたﾃﾞｰﾀがないか確認
                await RefreshAsync();

                // 処理完了
                Timer.Completed();
            };
            Timer.Start();

            // 履歴に削除されたﾃﾞｰﾀがないか確認
            await RefreshAsync();
        }

        /// <summary>
        /// Viewを追加します。
        /// </summary>
        /// <param name="id">video_id</param>
        public Task AddHistory(string id)
        {
            return AddHistory(id, DateTime.Now);
        }

        /// <summary>
        /// Viewを追加します。
        /// </summary>
        /// <param name="id">video_id</param>
        /// <param name="tick">追加日</param>
        public Task AddHistory(string id, long tick)
        {
            return AddHistory(id, new DateTime(tick));
        }

        /// <summary>
        /// Viewを追加します。
        /// </summary>
        /// <param name="id">video_id</param>
        /// <param name="date">追加日</param>
        public Task AddHistory(string id, DateTime date)
        {
            return AddHistory(new TVideoHistory(id, date));
        }

        /// <summary>
        /// Viewを追加します。
        /// </summary>
        /// <param name="view">view</param>
        public async Task AddHistory(TVideoHistory view)
        {
            Histories.Add(view);

            using (var accessor = DbAccessor.GetAccessor())
            using (var control = accessor.GetCommand())
            {
                await control.BeginTransaction();
                await control.InsertOrReplaceVideoHistory(view);
                await control.Commit();
            }
        }

        /// <summary>
        /// 視聴済であるか判定します。
        /// </summary>
        /// <param name="id">video_id</param>
        public bool IsSee(string id)
        {
            return Histories.Any(v => v.VideoId == id);
        }

        private async Task RefreshAsync()
        {
            var results = new List<string>();

            // 履歴に残っている動画のﾘｽﾄ(ID重複削除)
            var targets = Histories
                .Select(history => history.VideoId)
                .Distinct()
                .ToArray();

            foreach (var id in targets)
            {
                var v = new VideoModel();

                await v.Refresh(id);

                if (v.Status == VideoStatus.Delete)
                {
                    // 削除済の履歴はDBからも削除
                    results.Add(id);
                }
            }

            if (!results.Any())
            {
                return;
            }

            using (var accessor = DbAccessor.GetAccessor())
            using (var control = accessor.GetCommand())
            {
                await control.BeginTransaction();
                await control.DeleteVideoHistory(results.ToArray());
                await control.Commit();
            }
        }
    }
}
