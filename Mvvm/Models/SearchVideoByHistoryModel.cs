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
        /// 内部保持情報
        /// </summary>
        private List<TVideoHistory> Histories { get; set; }

        /// <summary>
        /// SettingModelを初期化します。
        /// </summary>
        /// <param name="views"></param>
        public static void Initialize(List<TVideoHistory> views)
        {
            Instance = new SearchVideoByHistoryModel();
            Instance.Histories = views;
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
    }
}
