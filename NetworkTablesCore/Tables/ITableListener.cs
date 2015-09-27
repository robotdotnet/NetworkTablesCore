using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkTablesCore.Native;

namespace NetworkTablesCore.Tables
{
    public interface ITableListener
    {
        void ValueChanged(ITable source, string key, object value, NotifyFlags flags);
    }
}
