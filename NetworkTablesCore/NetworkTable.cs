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
            CheckInit();
            if (client)
            {
                StartClient(s_ipAddress, Port);
            }
            else
            {
                StartServer(s_persistentFilename, s_ipAddress, Port);
            }
            running = true;
        }

        public static void Shutdown()
        {
            if (!running)
                throw new InvalidOperationException("Network Table has not yet been initialized");
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
            CheckInit();
            client = true;
        }

        public static void SetServerMode()
        {
            CheckInit();
            client = false;
        }

        public static void SetTeam(int team)
        {
            SetIPAddress($"10.{(team / 100)}.{(team % 100)}.2");
        }

        public static void SetIPAddress(string address)
        {
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

        public static void SetPort(uint port)
        {
            CheckInit();
            Port = port;
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
            string subtablePrefix = path + key + PATH_SEPERATOR_CHAR;
            var array = GetEntryInfo(subtablePrefix, 0);
            return array.Length != 0;
            /*
            The issue with implementing this, is you can create the subtable, but nothing shows up until
            you actually assign something to the table. This will probably be changed at some point.
            */
        }

        public ITable GetSubTable(string key)
        {
            return new NetworkTable(path + PATH_SEPERATOR_CHAR + key);
        }

        public const int PERSISTENT = 1;

        public void SetFlags(string key, int flags)
        {
            SetEntryFlags(path + PATH_SEPERATOR_CHAR + key, flags);
        }

        public int GetFlags(string key)
        {
            return GetEntryFlags(path + PATH_SEPERATOR_CHAR + key);
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

        public void PutValue(string key, object value)
        {
            key = path + PATH_SEPERATOR_CHAR + key;
            if (value is double) PutNumber(key, (double)value);
            else if (value is string) PutString(key, (string)value);
            else if (value is bool) PutBoolean(key, (bool)value);
            else if (value is double[])
            {
                SetEntryDoubleArray(key, (double[])value);
            }
            else if (value is bool[])
            {
                SetEntryBooleanArray(key, (bool[])value);
            }
            else if (value is string[])
            {
                SetEntryStringArray(key, (string[])value);
            }
            else
            {
                throw new ArgumentException("Value is either null or an invalid type.");
            }
        }

        public void PutNumber(string key, double value)
        {
            SetEntryDouble(path + PATH_SEPERATOR_CHAR + key, value);
        }

        public double GetNumber(string key, double defaultValue)
        {
            return GetEntryDouble(path + PATH_SEPERATOR_CHAR + key, defaultValue);
        }

        public double GetNumber(string key)
        {
            return GetEntryDouble(path + PATH_SEPERATOR_CHAR + key);
        }

        public void PutString(string key, string value)
        {
            SetEntryString(path + PATH_SEPERATOR_CHAR + key, value);
        }

        public string GetString(string key, string defaultValue)
        {
            return GetEntryString(path + PATH_SEPERATOR_CHAR + key, defaultValue);
        }

        public string GetString(string key)
        {
            return GetEntryString(path + PATH_SEPERATOR_CHAR + key);
        }

        public void PutBoolean(string key, bool value)
        {
            SetEntryBoolean(path + PATH_SEPERATOR_CHAR + key, value);
        }

        public bool GetBoolean(string key, bool defaultValue)
        {
            return GetEntryBoolean(path + PATH_SEPERATOR_CHAR + key, defaultValue);
        }

        public bool GetBoolean(string key)
        {
            return GetEntryBoolean(path + PATH_SEPERATOR_CHAR + key);
        }

        public void PutStringArray(string key, string[] value)
        {
            SetEntryStringArray(path + PATH_SEPERATOR_CHAR + key, value);
        }

        public string[] GetStringArray(string key)
        {
            return GetEntryStringArray(path + PATH_SEPERATOR_CHAR + key);
        }

        public double[] GetNumberArray(string key)
        {
            return GetEntryDoubleArray(path + PATH_SEPERATOR_CHAR + key);
        }

        public void PutNumberArray(string key, double[] val)
        {
            SetEntryDoubleArray(path + PATH_SEPERATOR_CHAR + key, val);
        }

        private readonly Dictionary<ITableListener, List<int>> m_listenerMap = new Dictionary<ITableListener, List<int>>();

        public void AddTableListener(ITableListener listener, bool immediateNotify = false)
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

            int id = AddEntryListener(path + PATH_SEPERATOR_CHAR, func, immediateNotify);

            adapters.Add(id);
        }

        public void AddTableListener(string key, ITableListener listener, bool immediateNotify)
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

            int id = AddEntryListener(fullKey, func, immediateNotify);

            adapters.Add(id);
        }

        public void AddSubTableListener(ITableListener listener)
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
            int id = AddEntryListener(path + PATH_SEPERATOR_CHAR, func, true);

            adapters.Add(id);
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

        public bool IsServer() => !client;
    }
}
