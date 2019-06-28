using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUtilV2.Extensions;

namespace NicoV5.Mvvm.Services
{
    public enum WpfMessageType
    {
        [Label("ｲﾝﾌｫﾒｰｼｮﾝ")]
        Info,

        [Label("確認")]
        Confirm,

        [Label("ｴﾗｰ")]
        Error
    }
}
