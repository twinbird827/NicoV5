using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NicoV5.Common.Tables
{
    public class TSetting
    {
        public TSetting(SettingKeys key, string value)
        {
            Key = key;
            Value = value;
        }

        public SettingKeys Key { get; set; }

        public string Value { get; set; }

        public override bool Equals(object obj)
        {
            var tmp = obj as TSetting;

            if (tmp is null)
            {
                return false;
            }
            
            return Key == tmp.Key;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    }
}
