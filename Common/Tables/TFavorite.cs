using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NicoV5.Common.Tables
{
    public class TFavorite
    {
        public TFavorite(string mylist, long tick)
        {
            Mylist = mylist;
            Tick = tick;
        }

        public TFavorite(string mylist, DateTime date)
        {
            Mylist = mylist;
            Date = date;
        }

        public string Mylist { get; set; }

        public long Tick
        {
            get => Date.Ticks;
            set => Date = new DateTime(value);
        }

        public DateTime Date { get; set; }
    }
}
