using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUtilV2.Databases;

namespace NicoV5.Common.Databases
{
    public class DbAccessor : Sqlite3Accessor
    {
        private DbAccessor(string path, bool isReadOnly = false) : base(path, isReadOnly)
        {

        }

        public static DbAccessor GetAccessor(bool isReadOnly = false)
        {

            return GetAccessor(@"lib\database.sqlite3", false);
        }

        public static DbAccessor GetAccessor(string path, bool isReadOnly = false)
        {
            return new DbAccessor(path, false);
        }
    }
}
