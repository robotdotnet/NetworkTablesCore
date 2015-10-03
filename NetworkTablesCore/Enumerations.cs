using System;

namespace NetworkTables
{
    /// <summary>
    /// The flags avalible for TableListeners
    /// </summary>
    [Flags]
    public enum NotifyFlags
    {
        /// Notify nobody
        NotifyNone = 0x00,
        /// Initial listener addition
        NotifyImmediate = 0x01,
        /// Changed locally
        NotifyLocal = 0x02,    
        /// Newly created entry
        NotifyNew = 0x04,    
        /// Deleted entry
        NotifyDelete = 0x08,    
        /// Value changed for entry
        NotifyUpdate = 0x10,    
        /// Flags changed for entry
        NotifyFlagsChanged = 0x20      
    };

    /// <summary>
    /// The flags avalible for Entries
    /// </summary>
    [Flags]
    public enum EntryFlags
    {
        /// No flags
        None = 0x00,
        /// Set entry to be persistent
        Persistent = 0x01
    }

    /// <summary>
    /// The log level to use for the NT logger
    /// </summary>
    public enum LogLevel
    {
        ///
        LogCritical = 50,
        ///
        LogError = 40,
        ///
        LogWarning = 30,
        ///
        LogInfo = 20,
        ///
        LogDebug = 10,
        ///
        LogDebug1 = 9,
        ///
        LogDebug2 = 8,
        ///
        LogDebug3 = 7,
        ///
        LogDebug4 = 6
    }
}
