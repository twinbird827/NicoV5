using NicoV5.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUtilV2.Mvvm;

namespace NicoV5.Mvvm.Combos
{
    public class ComboRankGenreModel : ComboboxModel
    {
        /// <summary>
        /// ｲﾝｽﾀﾝｽ (ｼﾝｸﾞﾙﾄﾝﾊﾟﾀｰﾝ)
        /// </summary>
        public static ComboRankGenreModel Instance { get; } = new ComboRankGenreModel();

        public override SettingKeys Key => SettingKeys.ComboRankGenre;

        private ComboRankGenreModel()
        {
            Items = new ObservableCollection<ComboboxItemModel>
            {
                new ComboboxItemModel() { Value = "all", Description = "全ジャンル" },
                new ComboboxItemModel() { Value = "hot_topic", Description = "話題" },
                new ComboboxItemModel() { Value = "entertainment", Description = "エンターテイメント" },
                new ComboboxItemModel() { Value = "radio", Description = "ラジオ" },
                new ComboboxItemModel() { Value = "music_sound", Description = "音楽・サウンド" },
                new ComboboxItemModel() { Value = "dance", Description = "ダンス" },
                new ComboboxItemModel() { Value = "animal", Description = "動物" },
                new ComboboxItemModel() { Value = "nature", Description = "自然" },
                new ComboboxItemModel() { Value = "cooking", Description = "料理" },
                new ComboboxItemModel() { Value = "traveling_outdoor", Description = "旅行・アウトドア" },
                new ComboboxItemModel() { Value = "vehicle", Description = "乗り物" },
                new ComboboxItemModel() { Value = "sports", Description = "スポーツ" },
                new ComboboxItemModel() { Value = "society_politics_news", Description = "社会・政治・時事" },
                new ComboboxItemModel() { Value = "technology_craft", Description = "技術・工作" },
                new ComboboxItemModel() { Value = "commentary_lecture", Description = "解説・講座" },
                new ComboboxItemModel() { Value = "anime", Description = "アニメ" },
                new ComboboxItemModel() { Value = "game", Description = "ゲーム" },
                new ComboboxItemModel() { Value = "other", Description = "その他" },
                new ComboboxItemModel() { Value = "r18", Description = "R-18" },
            };
        }

    }
}
