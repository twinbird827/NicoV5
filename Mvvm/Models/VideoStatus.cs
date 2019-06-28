using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUtilV2.Extensions;

namespace NicoV5.Mvvm.Models
{
    public enum VideoStatus
    {
        [Label("SEE")]
        See,

        [Label("NEW")]
        New,

        [Label("")]
        None,
    }
}
