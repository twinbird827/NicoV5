using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NicoV5.Common.Tables
{
    public class VVideoHistory
    {
        public VVideoHistory(string id, long tick, long count)
        {
            VideoId = id;
            Tick = tick;
            Count = count;
        }

        public VVideoHistory(string id, DateTime date, long count)
        {
            VideoId = id;
            Date = date;
            Count = count;
        }

        public string VideoId { get; set; }

        public long Tick
        {
            get => Date.Ticks;
            set => Date = new DateTime(value);
        }

        public DateTime Date { get; set; }

        public long Count { get; set; }
    }
}
