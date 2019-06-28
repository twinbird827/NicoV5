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
    public class ViewModel : BindableBase
    {
        private ViewModel()
        {

        }

        /// <summary>
        /// ｼﾝｸﾞﾙﾄﾝﾊﾟﾀｰﾝ
        /// </summary>
        public static ViewModel Instance { get; private set; }

        /// <summary>
        /// 内部保持情報
        /// </summary>
        private List<TView> Views { get; set; }

        /// <summary>
        /// SettingModelを初期化します。
        /// </summary>
        /// <param name="views"></param>
        public static void Initialize(List<TView> views)
        {
            Instance = new ViewModel();
            Instance.Views = views;
        }

        /// <summary>
        /// Viewを追加します。
        /// </summary>
        /// <param name="id">video_id</param>
        public Task AddView(string id)
        {
            return AddView(id, DateTime.Now);
        }

        /// <summary>
        /// Viewを追加します。
        /// </summary>
        /// <param name="id">video_id</param>
        /// <param name="tick">追加日</param>
        public Task AddView(string id, long tick)
        {
            return AddView(id, new DateTime(tick));
        }

        /// <summary>
        /// Viewを追加します。
        /// </summary>
        /// <param name="id">video_id</param>
        /// <param name="date">追加日</param>
        public Task AddView(string id, DateTime date)
        {
            return AddView(new TView(id, date));
        }

        /// <summary>
        /// Viewを追加します。
        /// </summary>
        /// <param name="view">view</param>
        public async Task AddView(TView view)
        {
            Views.Add(view);

            using (var accessor = DbAccessor.GetAccessor())
            using (var control = accessor.GetCommand())
            {
                await control.BeginTransaction();
                await control.InsertOrReplaceView(view);
                await control.Commit();
            }
        }

        /// <summary>
        /// 視聴済であるか判定します。
        /// </summary>
        /// <param name="id">video_id</param>
        public bool IsSee(string id)
        {
            return Views.Any(v => v.VideoId == id);
        }
    }
}
