using NetworkTables.Native;

namespace NetworkTables
{
    /// <summary>
    /// This class contains all info for a given entry.
    /// </summary>
    public class EntryInfo
    {
        /// Gets the Name of the entry.
        public string Name { get; }
        /// Gets the Type of the entry.
        public NtType Type { get; }
        /// Gets the Flags attached to the entry.
        public EntryFlags Flags { get; }
        /// Gets the last change time of the entry.
        public long LastChange { get; }

        internal EntryInfo(string name, NtType type, EntryFlags flags, long lastChange)
        {
            Name = name;
            Type = type;
            Flags = flags;
            LastChange = lastChange;
        }
    }
}
