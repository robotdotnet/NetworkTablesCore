using NetworkTables.Native;

namespace NetworkTables
{
    public class EntryInfo
    {
        public string Name { get; }
        public NT_Type Type { get; }
        public EntryFlags Flags { get; }
        public long LastChange { get; }

        public EntryInfo(string name, NT_Type type, EntryFlags flags, long lastChange)
        {
            this.Name = name;
            this.Type = type;
            this.Flags = flags;
            this.LastChange = lastChange;
        }
    }
}
