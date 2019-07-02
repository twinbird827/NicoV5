using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NicoV5.Common.Tables
{
    public class TVideoHistory
    {
        public TVideoHistory(string id, long tick)
        {
            VideoId = id;
            Tick = tick;
        }

        public TVideoHistory(string id, DateTime date)
        {
            VideoId = id;
            Date = date;
        }

        public string VideoId { get; set; }

        public long Tick
        {
            get => Date.Ticks;
            set => Date = new DateTime(value);
        }

        public DateTime Date { get; set; }
    }
}
