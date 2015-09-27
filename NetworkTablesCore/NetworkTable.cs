using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NetworkTablesCore.Native;
using NetworkTablesCore.Native.Exceptions;
using NetworkTablesCore.Tables;
using static NetworkTablesCore.Native.CoreMethods;

namespace NetworkTablesCore
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
                StartClient(s_ipAddress, Port);
            }
            else
            {
                StartServer(s_persistentFilename, "", Port);
            }
            running = true;
        }

        public static void Shutdown()
        {
            if (!running)
                return;
            if (client)
            {
                StopClient();
            }
            else
            {
                StopServer();
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
            EntryInfo[] array = GetEntries(path + PATH_SEPERATOR_CHAR + key + PATH_SEPERATOR_CHAR, 0);
            return array.Length != 0;
        }

        public HashSet<string> GetKeys(int types)
        {
            HashSet<string> keys = new HashSet<string>();
            int prefixLen = path.Length + 1;
            foreach (EntryInfo entry in GetEntries(path + PATH_SEPERATOR_CHAR, types))
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
            foreach (EntryInfo entry in GetEntries(path + PATH_SEPERATOR_CHAR, 0))
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

        public const int PERSISTENT = 1;

        public void SetPersistent(string key)
        {
            SetFlags(key, PERSISTENT);
        }

        public void ClearPersistent(string key)
        {
            ClearFlags(key, PERSISTENT);
        }

        public bool IsPersistent(string key)
        {
            return (GetFlags(key) & PERSISTENT) != 0;
        }

        public void SetFlags(string key, int flags)
        {
            SetEntryFlags(path + PATH_SEPERATOR_CHAR + key, GetFlags(key) | flags);
        }

        public void ClearFlags(string key, int flags)
        {
            SetEntryFlags(path + PATH_SEPERATOR_CHAR + key, GetFlags(key) & ~flags);
        }

        public int GetFlags(string key)
        {
            return GetEntryFlags(path + PATH_SEPERATOR_CHAR + key);
        }

        public void Delete(string key)
        {
            DeleteEntry(path + PATH_SEPERATOR_CHAR + key);
        }

        public static void GlobalDeleteAll()
        {
            DeleteAllEntries();
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
                    return GetEntryBoolean(localPath);
                case NT_Type.NT_DOUBLE:
                    return GetEntryDouble(localPath);
                case NT_Type.NT_STRING:
                    return GetEntryString(localPath);
                case NT_Type.NT_RAW:
                    return GetEntryRaw(localPath);
                case NT_Type.NT_BOOLEAN_ARRAY:
                    return GetEntryBooleanArray(localPath);
                case NT_Type.NT_DOUBLE_ARRAY:
                    return GetEntryDoubleArray(localPath);
                case NT_Type.NT_STRING_ARRAY:
                    return GetEntryStringArray(localPath);
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
                    return GetEntryBoolean(localPath);
                case NT_Type.NT_DOUBLE:
                    return GetEntryDouble(localPath);
                case NT_Type.NT_STRING:
                    return GetEntryString(localPath);
                case NT_Type.NT_RAW:
                    return GetEntryRaw(localPath);
                case NT_Type.NT_BOOLEAN_ARRAY:
                    return GetEntryBooleanArray(localPath);
                case NT_Type.NT_DOUBLE_ARRAY:
                    return GetEntryDoubleArray(localPath);
                case NT_Type.NT_STRING_ARRAY:
                    return GetEntryStringArray(localPath);
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
                return SetEntryDoubleArray(key, (double[])value);
            }
            else if (value is bool[])
            {
                return SetEntryBooleanArray(key, (bool[])value);
            }
            else if (value is string[])
            {
                return SetEntryStringArray(key, (string[])value);
            }
            else
            {
                throw new ArgumentException("Value is either null or an invalid type.");
            }
        }

        public bool PutNumber(string key, double value)
        {
            return SetEntryDouble(path + PATH_SEPERATOR_CHAR + key, value);
        }

        public double GetNumber(string key, double defaultValue)
        {
            return GetEntryDouble(path + PATH_SEPERATOR_CHAR + key, defaultValue);
        }

        public double GetNumber(string key)
        {
            return GetEntryDouble(path + PATH_SEPERATOR_CHAR + key);
        }

        public bool PutString(string key, string value)
        {
            return SetEntryString(path + PATH_SEPERATOR_CHAR + key, value);
        }

        public string GetString(string key, string defaultValue)
        {
            return GetEntryString(path + PATH_SEPERATOR_CHAR + key, defaultValue);
        }

        public string GetString(string key)
        {
            return GetEntryString(path + PATH_SEPERATOR_CHAR + key);
        }

        public bool PutBoolean(string key, bool value)
        {
            return SetEntryBoolean(path + PATH_SEPERATOR_CHAR + key, value);
        }

        public bool GetBoolean(string key, bool defaultValue)
        {
            return GetEntryBoolean(path + PATH_SEPERATOR_CHAR + key, defaultValue);
        }

        public bool GetBoolean(string key)
        {
            return GetEntryBoolean(path + PATH_SEPERATOR_CHAR + key);
        }

        public bool PutStringArray(string key, string[] value)
        {
            return SetEntryStringArray(path + PATH_SEPERATOR_CHAR + key, value);
        }

        public string[] GetStringArray(string key)
        {
            return GetEntryStringArray(path + PATH_SEPERATOR_CHAR + key);
        }

        public string[] GetStringArray(string key, string[] defaultValue)
        {
            return GetEntryStringArray(path + PATH_SEPERATOR_CHAR + key, defaultValue);
        }

        public bool PutNumberArray(string key, double[] val)
        {
            return SetEntryDoubleArray(path + PATH_SEPERATOR_CHAR + key, val);
        }

        public double[] GetNumberArray(string key)
        {
            return GetEntryDoubleArray(path + PATH_SEPERATOR_CHAR + key);
        }

        public double[] GetNumberArray(string key, double[] defaultValue)
        {
            return GetEntryDoubleArray(path + PATH_SEPERATOR_CHAR + key, defaultValue);
        }

        public bool PutBooleanArray(string key, bool[] val)
        {
            return SetEntryBooleanArray(path + PATH_SEPERATOR_CHAR + key, val);
        }

        public bool[] GetBooleanArray(string key)
        {
            return GetEntryBooleanArray(path + PATH_SEPERATOR_CHAR + key);
        }

        public bool[] GetBooleanArray(string key, bool[] defaultValue)
        {
            return GetEntryBooleanArray(path + PATH_SEPERATOR_CHAR + key, defaultValue);
        }

        private readonly Dictionary<ITableListener, List<int>> m_listenerMap = new Dictionary<ITableListener, List<int>>();

        public void AddTableListener(ITableListener listener, bool immediateNotify, bool localNotify)
        {
            List<int> adapters;
            if (!m_listenerMap.TryGetValue(listener, out adapters))
            {
                adapters = new List<int>();
                m_listenerMap.Add(listener, adapters);
            }

            Delegates.EntryListenerFunction func = (uid, key, value, isNew) =>
            {
                string relativeKey = key.Substring(path.Length + 1);
                if (relativeKey.IndexOf(PATH_SEPERATOR_CHAR) != -1)
                {
                    return;
                }
                listener.ValueChanged(this, relativeKey, value, isNew);
            };

            int id = AddEntryListener(path + PATH_SEPERATOR_CHAR, func, immediateNotify, localNotify);

            adapters.Add(id);
        }

        public void AddTableListener(string key, ITableListener listener, bool immediateNotify, bool localNotify)
        {
            List<int> adapters;
            if (!m_listenerMap.TryGetValue(listener, out adapters))
            {
                adapters = new List<int>();
                m_listenerMap.Add(listener, adapters);
            }
            string fullKey = path + PATH_SEPERATOR_CHAR + key;
            Delegates.EntryListenerFunction func = (uid, funcKey, value, isNew) =>
            {
                if (!funcKey.Equals(fullKey))
                    return;
                listener.ValueChanged(this, key, value, isNew);
            };

            int id = AddEntryListener(fullKey, func, immediateNotify, localNotify);

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
            Delegates.EntryListenerFunction func = (uid, key, value, isNew) =>
            {
                string relativeKey = key.Substring(path.Length + 1);
                int endSubTable = relativeKey.IndexOf(PATH_SEPERATOR_CHAR);
                if (endSubTable == -1)
                    return;
                string subTableKey = relativeKey.Substring(0, endSubTable);
                if (notifiedTables.Contains(subTableKey))
                    return;
                notifiedTables.Add(subTableKey);
                listener.ValueChanged(this, subTableKey, GetSubTable(subTableKey), true);
            };
            int id = AddEntryListener(path + PATH_SEPERATOR_CHAR, func, true, localNotify);

            adapters.Add(id);
        }

         public void AddTableListener(ITableListener listener, bool immediateNotify = false)
        {
            AddTableListener(listener, immediateNotify, false);
        }

        public void AddTableListener(string key, ITableListener listener, bool immediateNotify = false)
        {
            AddTableListener(key, listener, immediateNotify, false);
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
                    RemoveEntryListener(t);
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

            Delegates.ConnectionListenerFunction func = (uid, connected, conn) =>
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
            ConnectionInfo[] conns = GetConnections();
            return conns.Length > 0;
        }

        public static ConnectionInfo[] Connections()
        {
            return GetConnections();
        }

        public bool IsServer() => !client;
    }
}
