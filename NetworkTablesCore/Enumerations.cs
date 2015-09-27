using System;

namespace NetworkTables
{
    [Flags]
    public enum NotifyFlags
    {
        NOTIFY_NONE = 0x00,
        NOTIFY_IMMEDIATE = 0x01, /* initial listener addition */
        NOTIFY_LOCAL = 0x02,     /* changed locally */
        NOTIFY_NEW = 0x04,       /* newly created entry */
        NOTIFY_DELETE = 0x08,    /* deleted */
        NOTIFY_UPDATE = 0x10,    /* value changed */
        NOTIFY_FLAGS = 0x20      /* flags changed */
    };

    [Flags]
    public enum EntryFlags
    {
        NONE = 0x00,
        PERSISTENT = 0x01
    }

    public enum LogLevel
    {
        LOG_CRITICAL = 50,
        LOG_ERROR = 40,
        LOG_WARNING = 30,
        LOG_INFO = 20,
        LOG_DEBUG = 10,
        LOG_DEBUG1 = 9,
        LOG_DEBUG2 = 8,
        LOG_DEBUG3 = 7,
        LOG_DEBUG4 = 6
    }
}
