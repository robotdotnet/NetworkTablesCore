using System;
using System.Collections.Generic;
using NetworkTables.Native;
using NetworkTables.Native.Exceptions;
using NetworkTables.Tables;

namespace NetworkTables
{
    public class NetworkTable : ITable, IRemote
    {
        public const char PATH_SEPERATOR_CHAR = '/';
        public const uint DEFAULT_PORT = 1735;
        private static uint Port = DEFAULT_PORT;
        private static string s_ipAddress = "";
        private static bool client = false;
        private static bool running = false;

        private static string s_persistentFilename = "networktables.ini";

        private static void CheckInit()
        {
            if (running)
                throw new InvalidOperationException("Network Tables has already been initialized");
        }

        public static void Initialize()
        {
            if (running)
                Shutdown();
            if (client)
            {
                CoreMethods.StartClient(s_ipAddress, Port);
            }
            else
            {
                CoreMethods.StartServer(s_persistentFilename, "", Port);
            }
            running = true;
        }

        public static void Shutdown()
        {
            if (!running)
                return;
            if (client)
            {
                CoreMethods.StopClient();
            }
            else
            {
                CoreMethods.StopServer();
            }
            running = false;
        }

        public static void SetClientMode()
        {
            if (client)
                return;
            CheckInit();
            client = true;
        }

        public static void SetServerMode()
        {
            if (!client)
                return;
            CheckInit();
            client = false;
        }

        public static void SetTeam(int team)
        {
            SetIPAddress($"10.{(team / 100)}.{(team % 100)}.2");
        }

        public static void SetIPAddress(string address)
        {
            if (s_ipAddress == address)
                return;
            CheckInit();
            s_ipAddress = address;
        }

        public static NetworkTable GetTable(string key)
        {
            if (!running) Initialize();
            if (key == "")
                return new NetworkTable(key);
            return new NetworkTable(PATH_SEPERATOR_CHAR + key);
        }

        public static void SetPort(int port)
        {
            if (port == Port)
                return;
            CheckInit();
            Port = (uint)port;
        }

        public static void SetPersistentFilename(string filename)
        {
            if (s_persistentFilename == filename)
                return;
            CheckInit();
            s_persistentFilename = filename;
        }

        public static void SetNetworkIdentity(string name)
        {
            CoreMethods.SetNetworkIdentity(name);
        }

        private readonly string path;

        private NetworkTable(string path)
        {
            this.path = path;
        }

        public override string ToString()
        {
            return $"NetworkTable: {path}";
        }

        public bool ContainsKey(string key)
        {
            return CoreMethods.ContainsKey(key);
        }

        public bool ContainsSubTable(string key)
        {
            EntryInfo[] array = CoreMethods.GetEntries(path + PATH_SEPERATOR_CHAR + key + PATH_SEPERATOR_CHAR, 0);
            return array.Length != 0;
        }

        public HashSet<string> GetKeys(int types)
        {
            HashSet<string> keys = new HashSet<string>();
            int prefixLen = path.Length + 1;
            foreach (EntryInfo entry in CoreMethods.GetEntries(path + PATH_SEPERATOR_CHAR, types))
            {
                string relativeKey = entry.Name.Substring(prefixLen);
                if (relativeKey.IndexOf(PATH_SEPERATOR_CHAR) != -1)
                    continue;
                keys.Add(relativeKey);
            }
            return keys;
        }

        public HashSet<string> GetKeys()
        {
            return GetKeys(0);
        }

        public HashSet<string> GetSubTables()
        {
            HashSet<string> keys = new HashSet<string>();
            int prefixLen = path.Length + 1;
            foreach (EntryInfo entry in CoreMethods.GetEntries(path + PATH_SEPERATOR_CHAR, 0))
            {
                string relativeKey = entry.Name.Substring(prefixLen);
                int endSubTable = relativeKey.IndexOf(PATH_SEPERATOR_CHAR);
                if (endSubTable == -1)
                    continue;
                keys.Add(relativeKey.Substring(0, endSubTable));
            }
            return keys;
        } 

        public ITable GetSubTable(string key)
        {
            return new NetworkTable(path + PATH_SEPERATOR_CHAR + key);
        }

        public void SetPersistent(string key)
        {
            SetFlags(key, EntryFlags.PERSISTENT);
        }

        public void ClearPersistent(string key)
        {
            ClearFlags(key, EntryFlags.PERSISTENT);
        }

        public bool IsPersistent(string key)
        {
            return (GetFlags(key) & EntryFlags.PERSISTENT) != 0;
        }

        public void SetFlags(string key, EntryFlags flags)
        {
            CoreMethods.SetEntryFlags(path + PATH_SEPERATOR_CHAR + key, GetFlags(key) | flags);
        }

        public void ClearFlags(string key, EntryFlags flags)
        {
            CoreMethods.SetEntryFlags(path + PATH_SEPERATOR_CHAR + key, GetFlags(key) & ~flags);
        }

        public EntryFlags GetFlags(string key)
        {
            return CoreMethods.GetEntryFlags(path + PATH_SEPERATOR_CHAR + key);
        }

        public void Delete(string key)
        {
            CoreMethods.DeleteEntry(path + PATH_SEPERATOR_CHAR + key);
        }

        public static void GlobalDeleteAll()
        {
            CoreMethods.DeleteAllEntries();
        }

        public static void Flush()
        {
            CoreMethods.Flush();
        }

        public static void SetUpdateRate(double interval)
        {
            CoreMethods.SetUpdateRate(interval);
        }

        public static void SavePersistent(string filename)
        {
            CoreMethods.SavePersistent(filename);
        }

        public static string[] LoadPersistent(string filename)
        {
            return CoreMethods.LoadPersistent(filename);
        }

        public object GetValue(string key)
        {
            string localPath = path + PATH_SEPERATOR_CHAR + key;
            NT_Type type;
            int status = 0;
            ulong lc = 0;
            type = CoreMethods.GetType(localPath);
            switch (type)
            {
                case NT_Type.NT_BOOLEAN:
                    return CoreMethods.GetEntryBoolean(localPath);
                case NT_Type.NT_DOUBLE:
                    return CoreMethods.GetEntryDouble(localPath);
                case NT_Type.NT_STRING:
                    return CoreMethods.GetEntryString(localPath);
                case NT_Type.NT_RAW:
                    return CoreMethods.GetEntryRaw(localPath);
                case NT_Type.NT_BOOLEAN_ARRAY:
                    return CoreMethods.GetEntryBooleanArray(localPath);
                case NT_Type.NT_DOUBLE_ARRAY:
                    return CoreMethods.GetEntryDoubleArray(localPath);
                case NT_Type.NT_STRING_ARRAY:
                    return CoreMethods.GetEntryStringArray(localPath);
                default:
                    throw new TableKeyNotDefinedException(localPath);
            }
        }

        public object GetValue(string key, object defaultValue)
        {
            string localPath = path + PATH_SEPERATOR_CHAR + key;
            NT_Type type;
            int status = 0;
            ulong lc = 0;
            type = CoreMethods.GetType(localPath);
            switch (type)
            {
                case NT_Type.NT_BOOLEAN:
                    return CoreMethods.GetEntryBoolean(localPath);
                case NT_Type.NT_DOUBLE:
                    return CoreMethods.GetEntryDouble(localPath);
                case NT_Type.NT_STRING:
                    return CoreMethods.GetEntryString(localPath);
                case NT_Type.NT_RAW:
                    return CoreMethods.GetEntryRaw(localPath);
                case NT_Type.NT_BOOLEAN_ARRAY:
                    return CoreMethods.GetEntryBooleanArray(localPath);
                case NT_Type.NT_DOUBLE_ARRAY:
                    return CoreMethods.GetEntryDoubleArray(localPath);
                case NT_Type.NT_STRING_ARRAY:
                    return CoreMethods.GetEntryStringArray(localPath);
                default:
                    return defaultValue;
            }
        }

        public bool PutValue(string key, object value)
        {
            key = path + PATH_SEPERATOR_CHAR + key;
            if (value is double) return PutNumber(key, (double)value);
            else if (value is string) return PutString(key, (string)value);
            else if (value is bool) return PutBoolean(key, (bool)value);
            else if (value is double[])
            {
                return CoreMethods.SetEntryDoubleArray(key, (double[])value);
            }
            else if (value is bool[])
            {
                return CoreMethods.SetEntryBooleanArray(key, (bool[])value);
            }
            else if (value is string[])
            {
                return CoreMethods.SetEntryStringArray(key, (string[])value);
            }
            else
            {
                throw new ArgumentException("Value is either null or an invalid type.");
            }
        }

        public bool PutNumber(string key, double value)
        {
            return CoreMethods.SetEntryDouble(path + PATH_SEPERATOR_CHAR + key, value);
        }

        public double GetNumber(string key, double defaultValue)
        {
            return CoreMethods.GetEntryDouble(path + PATH_SEPERATOR_CHAR + key, defaultValue);
        }

        public double GetNumber(string key)
        {
            return CoreMethods.GetEntryDouble(path + PATH_SEPERATOR_CHAR + key);
        }

        public bool PutString(string key, string value)
        {
            return CoreMethods.SetEntryString(path + PATH_SEPERATOR_CHAR + key, value);
        }

        public string GetString(string key, string defaultValue)
        {
            return CoreMethods.GetEntryString(path + PATH_SEPERATOR_CHAR + key, defaultValue);
        }

        public string GetString(string key)
        {
            return CoreMethods.GetEntryString(path + PATH_SEPERATOR_CHAR + key);
        }

        public bool PutBoolean(string key, bool value)
        {
            return CoreMethods.SetEntryBoolean(path + PATH_SEPERATOR_CHAR + key, value);
        }

        public bool GetBoolean(string key, bool defaultValue)
        {
            return CoreMethods.GetEntryBoolean(path + PATH_SEPERATOR_CHAR + key, defaultValue);
        }

        public bool GetBoolean(string key)
        {
            return CoreMethods.GetEntryBoolean(path + PATH_SEPERATOR_CHAR + key);
        }

        public bool PutStringArray(string key, string[] value)
        {
            return CoreMethods.SetEntryStringArray(path + PATH_SEPERATOR_CHAR + key, value);
        }

        public string[] GetStringArray(string key)
        {
            return CoreMethods.GetEntryStringArray(path + PATH_SEPERATOR_CHAR + key);
        }

        public string[] GetStringArray(string key, string[] defaultValue)
        {
            return CoreMethods.GetEntryStringArray(path + PATH_SEPERATOR_CHAR + key, defaultValue);
        }

        public bool PutNumberArray(string key, double[] val)
        {
            return CoreMethods.SetEntryDoubleArray(path + PATH_SEPERATOR_CHAR + key, val);
        }

        public double[] GetNumberArray(string key)
        {
            return CoreMethods.GetEntryDoubleArray(path + PATH_SEPERATOR_CHAR + key);
        }

        public double[] GetNumberArray(string key, double[] defaultValue)
        {
            return CoreMethods.GetEntryDoubleArray(path + PATH_SEPERATOR_CHAR + key, defaultValue);
        }

        public bool PutBooleanArray(string key, bool[] val)
        {
            return CoreMethods.SetEntryBooleanArray(path + PATH_SEPERATOR_CHAR + key, val);
        }

        public bool[] GetBooleanArray(string key)
        {
            return CoreMethods.GetEntryBooleanArray(path + PATH_SEPERATOR_CHAR + key);
        }

        public bool[] GetBooleanArray(string key, bool[] defaultValue)
        {
            return CoreMethods.GetEntryBooleanArray(path + PATH_SEPERATOR_CHAR + key, defaultValue);
        }

        private readonly Dictionary<ITableListener, List<int>> m_listenerMap = new Dictionary<ITableListener, List<int>>();

        public void AddTableListenerEx(ITableListener listener, NotifyFlags flags)
        {
            List<int> adapters;
            if (!m_listenerMap.TryGetValue(listener, out adapters))
            {
                adapters = new List<int>();
                m_listenerMap.Add(listener, adapters);
            }

            EntryListenerFunction func = (uid, key, value, flags_) =>
            {
                string relativeKey = key.Substring(path.Length + 1);
                if (relativeKey.IndexOf(PATH_SEPERATOR_CHAR) != -1)
                {
                    return;
                }
                listener.ValueChanged(this, relativeKey, value, flags_);
            };

            int id = CoreMethods.AddEntryListener(path + PATH_SEPERATOR_CHAR, func, flags);

            adapters.Add(id);
        }

        public void AddTableListenerEx(string key, ITableListener listener, NotifyFlags flags)
        {
            List<int> adapters;
            if (!m_listenerMap.TryGetValue(listener, out adapters))
            {
                adapters = new List<int>();
                m_listenerMap.Add(listener, adapters);
            }
            string fullKey = path + PATH_SEPERATOR_CHAR + key;
            EntryListenerFunction func = (uid, funcKey, value, flags_) =>
            {
                if (!funcKey.Equals(fullKey))
                    return;
                listener.ValueChanged(this, key, value, flags_);
            };

            int id = CoreMethods.AddEntryListener(fullKey, func, flags);

            adapters.Add(id);
        }

        public void AddSubTableListener(ITableListener listener, bool localNotify)
        {
            List<int> adapters;
            if (!m_listenerMap.TryGetValue(listener, out adapters))
            {
                adapters = new List<int>();
                m_listenerMap.Add(listener, adapters);
            }
            HashSet<string> notifiedTables = new HashSet<string>();
            EntryListenerFunction func = (uid, key, value, flags_) =>
            {
                string relativeKey = key.Substring(path.Length + 1);
                int endSubTable = relativeKey.IndexOf(PATH_SEPERATOR_CHAR);
                if (endSubTable == -1)
                    return;
                string subTableKey = relativeKey.Substring(0, endSubTable);
                if (notifiedTables.Contains(subTableKey))
                    return;
                notifiedTables.Add(subTableKey);
                listener.ValueChanged(this, subTableKey, GetSubTable(subTableKey), flags_);
            };
            NotifyFlags flags = NotifyFlags.NOTIFY_NEW | NotifyFlags.NOTIFY_UPDATE;
            if (localNotify)
                flags |= NotifyFlags.NOTIFY_LOCAL;
            int id = CoreMethods.AddEntryListener(path + PATH_SEPERATOR_CHAR, func, flags);

            adapters.Add(id);
        }

         public void AddTableListener(ITableListener listener, bool immediateNotify = false)
        {
            NotifyFlags flags = NotifyFlags.NOTIFY_NEW | NotifyFlags.NOTIFY_UPDATE;
            if (immediateNotify)
                flags |= NotifyFlags.NOTIFY_IMMEDIATE;
            AddTableListenerEx(listener, flags);
        }

        public void AddTableListener(string key, ITableListener listener, bool immediateNotify = false)
        {
            NotifyFlags flags = NotifyFlags.NOTIFY_NEW | NotifyFlags.NOTIFY_UPDATE;
            if (immediateNotify)
                flags |= NotifyFlags.NOTIFY_IMMEDIATE;
            AddTableListenerEx(key, listener, flags);
        }

        public void AddSubTableListener(ITableListener listener)
        {
            AddSubTableListener(listener, false);
        }

        public void RemoveTableListener(ITableListener listener)
        {
            List<int> adapters;
            if (m_listenerMap.TryGetValue(listener, out adapters))
            {
                foreach (int t in adapters)
                {
                    CoreMethods.RemoveEntryListener(t);
                }
                adapters.Clear();
            }
        }

        private readonly Dictionary<IRemoteConnectionListener, int> m_connectionListenerMap =
            new Dictionary<IRemoteConnectionListener, int>();

        public void AddConnectionListener(IRemoteConnectionListener listener, bool immediateNotify)
        {

            if (m_connectionListenerMap.ContainsKey(listener))
            {
                throw new ArgumentException("Cannot add the same listener twice", nameof(listener));
            }

            ConnectionListenerFunction func = (uid, connected, conn) =>
            {
                if (connected) listener.Connected(this);
                else listener.Disconnected(this);
            };

            int id = CoreMethods.AddConnectionListener(func, immediateNotify);

            m_connectionListenerMap.Add(listener, id);

        }

        public void RemoveConnectionListener(IRemoteConnectionListener listener)
        {
            int val;
            if (m_connectionListenerMap.TryGetValue(listener, out val))
            {
                CoreMethods.RemoveConnectionListener(val);
            }
        }

        public bool IsConnected()
        {
            ConnectionInfo[] conns = CoreMethods.GetConnections();
            return conns.Length > 0;
        }

        public static ConnectionInfo[] Connections()
        {
            return CoreMethods.GetConnections();
        }

        public bool IsServer() => !client;
    }
}
