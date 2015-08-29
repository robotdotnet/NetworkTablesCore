using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkTablesCore.Native;

namespace NetworkTablesCore
{
    public class EntryInfo
    {
        public string Name { get; }
        public NT_Type Type { get; }
        public int Flags { get; }
        public long LastChange { get; }

        public EntryInfo(string name, NT_Type type, int flags, long lastChange)
        {
            this.Name = name;
            this.Type = type;
            this.Flags = flags;
            this.LastChange = lastChange;
        }
    }
}
