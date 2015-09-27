using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTablesCore.Native
{
    public class Delegates
    {
        public delegate void LoggerFunction(int level, string file, int line, string message);

        public delegate void EntryListenerFunction(int uid, string key, object value, NotifyFlags flags);

        public delegate void ConnectionListenerFunction(int uid, bool connected, ConnectionInfo conn);

        public delegate byte[] RpcFunction(string name, byte[] paramsString);
    }
}
