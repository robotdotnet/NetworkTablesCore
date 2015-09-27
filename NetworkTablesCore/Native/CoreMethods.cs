using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using NetworkTablesCore.Native.Exceptions;
using static NetworkTablesCore.Native.Interop;

namespace NetworkTablesCore.Native
{
    public static class CoreMethods
    {

        #region Setters
        public static bool SetEntryBoolean(string name, bool value, bool force = false)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            int retVal = NT_SetEntryBoolean(namePtr, size, value ? 1 : 0, force ? 1 : 0);
            return retVal != 0;
        }

        public static bool SetEntryDouble(string name, double value, bool force = false)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            int retVal = NT_SetEntryDouble(namePtr, size, value, force ? 1 : 0);
            return retVal != 0;
        }

        public static bool SetEntryString(string name, string value, bool force = false)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr stringSize;
            byte[] stringPtr = CreateUTF8String(value, out stringSize);
            int retVal = NT_SetEntryString(namePtr, size, stringPtr, stringSize, force ? 1 : 0);
            return retVal != 0;
        }

        public static bool SetEntryRaw(string name, string value, bool force = false)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr stringSize;
            byte[] stringPtr = CreateUTF8String(name, out stringSize);
            int retVal = NT_SetEntryRaw(namePtr, size, stringPtr, stringSize, force ? 1 : 0);
            return retVal != 0;
        }

        public static bool SetEntryBooleanArray(string name, bool[] value, bool force = false)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);

            int[] valueIntArr = new int[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                valueIntArr[i] = value[i] ? 1 : 0;
            }

            int retVal = NT_SetEntryBooleanArray(namePtr, size, valueIntArr, (UIntPtr)valueIntArr.Length, force ? 1 : 0);

            return retVal != 0;
        }

        public static bool SetEntryDoubleArray(string name, double[] value, bool force = false)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);

            int retVal = NT_SetEntryDoubleArray(namePtr, size, value, (UIntPtr)value.Length, force ? 1 : 0);

            return retVal != 0;
        }

        public static bool SetEntryStringArray(string name, string[] value, bool force = false)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);

            NT_String_Write[] ntStrings = new NT_String_Write[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                ntStrings[i] = new NT_String_Write(value[i]);
            }

            int retVal = NT_SetEntryStringArray(namePtr, size, ntStrings, (UIntPtr)ntStrings.Length, force ? 1 : 0);

            foreach (var ntString in ntStrings)
            {
                ntString.Dispose();
            }

            return retVal != 0;
        }

        #endregion

        #region DefaultGetters
        public static bool GetEntryBoolean(string name, bool defaultValue)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            int boolean = 0;
            ulong lc = 0;
            int status = NT_GetEntryBoolean(namePtr, size, ref lc, ref boolean);
            if (status == 0)
            {
                return defaultValue;
            }
            return boolean != 0;
        }

        public static double GetEntryDouble(string name, double defaultValue)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            double retVal = 0;
            ulong last_change = 0;
            int status = NT_GetEntryDouble(namePtr, size, ref last_change, ref retVal);
            if (status == 0)
            {
                return defaultValue;
            }
            return retVal;
        }

        public static string GetEntryString(string name, string defaultValue)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr stringSize = UIntPtr.Zero;
            ulong last_change = 0;
            IntPtr ret = NT_GetEntryString(namePtr, size, ref last_change, ref stringSize);
            if (ret == IntPtr.Zero)
            {
                return defaultValue;
            }
            else
            {
                string str = ReadUTF8String(ret, stringSize);
                NT_FreeCharArray(ret);
                return str;
            }
        }

        public static string GetEntryRaw(string name, string defaultValue)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr stringSize = UIntPtr.Zero;
            ulong last_change = 0;
            IntPtr ret = NT_GetEntryRaw(namePtr, size, ref last_change, ref stringSize);
            if (ret == IntPtr.Zero)
            {
                return defaultValue;
            }
            else
            {
                string str = ReadUTF8String(ret, stringSize);
                NT_FreeCharArray(ret);
                return str;
            }
        }

        public static double[] GetEntryDoubleArray(string name, double[] defaultValue)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr arrSize = UIntPtr.Zero;
            ulong last_change = 0;
            IntPtr arrPtr = NT_GetEntryDoubleArray(namePtr, size, ref last_change, ref arrSize);
            if (arrPtr == IntPtr.Zero)
            {
                return defaultValue;
            }
            double[] arr = GetDoubleArrayFromPtr(arrPtr, arrSize);
            NT_FreeDoubleArray(arrPtr);
            return arr;
        }

        public static bool[] GetEntryBooleanArray(string name, bool[] defaultValue)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr arrSize = UIntPtr.Zero;
            ulong last_change = 0;
            IntPtr arrPtr = NT_GetEntryBooleanArray(namePtr, size, ref last_change, ref arrSize);
            if (arrPtr == IntPtr.Zero)
            {
                return defaultValue;
            }
            bool[] arr = GetBooleanArrayFromPtr(arrPtr, arrSize);
            NT_FreeBooleanArray(arrPtr);
            return arr;
        }

        public static string[] GetEntryStringArray(string name, string[] defaultValue)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr arrSize = UIntPtr.Zero;
            IntPtr strLen = IntPtr.Zero;
            ulong last_change = 0;
            IntPtr arrPtr = NT_GetEntryStringArray(namePtr, size, ref last_change, ref arrSize);
            if (arrPtr == IntPtr.Zero)
            {
                return defaultValue;
            }
            string[] arr = GetStringArrayFromPtr(arrPtr, arrSize);
            NT_FreeStringArray(arrPtr, arrSize);
            return arr;
        }
        #endregion

        #region Getters

        public static bool GetEntryBoolean(string name)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            int boolean = 0;
            ulong lc = 0;
            int status = NT_GetEntryBoolean(namePtr, size, ref lc, ref boolean);
            if (status == 0)
            {
                throw new TableKeyNotDefinedException(name);//Change this to GetTableKeyNotDefined
            }
            return boolean != 0;
        }

        public static double GetEntryDouble(string name)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            double retVal = 0;
            ulong last_change = 0;
            int status = NT_GetEntryDouble(namePtr, size, ref last_change, ref retVal);
            if (status == 0)
            {
                throw new TableKeyNotDefinedException(name);//Change this to GetTableKeyNotDefined
            }
            return retVal;
        }

        public static string GetEntryString(string name)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr stringSize = UIntPtr.Zero;
            ulong last_change = 0;
            IntPtr ret = NT_GetEntryString(namePtr, size, ref last_change, ref stringSize);
            if (ret == IntPtr.Zero)
            {
                throw new TableKeyNotDefinedException(name);
            }
            else
            {
                string str = ReadUTF8String(ret, stringSize);
                NT_FreeCharArray(ret);
                return str;
            }
        }

        public static string GetEntryRaw(string name)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr stringSize = UIntPtr.Zero;
            ulong last_change = 0;
            IntPtr ret = NT_GetEntryRaw(namePtr, size, ref last_change, ref stringSize);
            if (ret == IntPtr.Zero)
            {
                throw new TableKeyNotDefinedException(name);
            }
            else
            {
                string str = ReadUTF8String(ret, stringSize);
                NT_FreeCharArray(ret);
                return str;
            }
        }

        public static double[] GetEntryDoubleArray(string name)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr arrSize = UIntPtr.Zero;
            ulong last_change = 0;
            IntPtr arrPtr = NT_GetEntryDoubleArray(namePtr, size, ref last_change, ref arrSize);
            if (arrPtr == IntPtr.Zero)
            {
                throw new TableKeyNotDefinedException(name);//TODO: Change this to not defined exception
            }
            double[] arr = GetDoubleArrayFromPtr(arrPtr, arrSize);
            NT_FreeDoubleArray(arrPtr);
            return arr;
        }

        public static bool[] GetEntryBooleanArray(string name)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr arrSize = UIntPtr.Zero;
            ulong last_change = 0;
            IntPtr arrPtr = NT_GetEntryBooleanArray(namePtr, size, ref last_change, ref arrSize);
            if (arrPtr == IntPtr.Zero)
            {
                throw new TableKeyNotDefinedException(name);//TODO: Change this to not defined exception
            }
            bool[] arr = GetBooleanArrayFromPtr(arrPtr, arrSize);
            NT_FreeBooleanArray(arrPtr);
            return arr;
        }

        public static string[] GetEntryStringArray(string name)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr arrSize = UIntPtr.Zero;
            ulong last_change = 0;
            IntPtr arrPtr = NT_GetEntryStringArray(namePtr, size, ref last_change, ref arrSize);
            if (arrPtr == IntPtr.Zero)
            {
                throw new TableKeyNotDefinedException(name);//TODO: Change this to not defined exception
            }
            string[] arr = GetStringArrayFromPtr(arrPtr, arrSize);
            NT_FreeStringArray(arrPtr, arrSize);
            return arr;
        }
        #endregion

        #region EntryInfo

        public static EntryInfo[] GetEntries(string prefix, int types)
        {
            UIntPtr size;
            byte[] str = CreateUTF8String(prefix, out size);
            UIntPtr arrSize = UIntPtr.Zero;
            IntPtr arr = NT_GetEntryInfo(str, size, (uint)types, ref arrSize);
            int entryInfoSize = Marshal.SizeOf(typeof(NT_EntryInfo));
            int arraySize = (int)arrSize.ToUInt64();
            EntryInfo[] entryArray = new EntryInfo[arraySize];

            for (int i = 0; i < arraySize; i++)
            {
                IntPtr data = new IntPtr(arr.ToInt64() + entryInfoSize * i);
                NT_EntryInfo info = (NT_EntryInfo)Marshal.PtrToStructure(data, typeof(NT_EntryInfo));
                entryArray[i] = new EntryInfo(info.name.ToString(), info.type, (int)info.flags, (long)info.last_change);
            }
            NT_DisposeEntryInfoArray(arr, arrSize);
            return entryArray;
        }
        #endregion

        #region ConnectionInfo

        public static ConnectionInfo[] GetConnections()
        {
            UIntPtr count = UIntPtr.Zero;
            IntPtr connections = NT_GetConnections(ref count);
            int connectionInfoSize = Marshal.SizeOf(typeof(NT_ConnectionInfo));
            int arraySize = (int)count.ToUInt64();

            ConnectionInfo[] connectionsArray = new ConnectionInfo[arraySize];

            for (int i = 0; i < arraySize; i++)
            {
                IntPtr data = new IntPtr(connections.ToInt64() + connectionInfoSize * i);
                var con = (NT_ConnectionInfo)Marshal.PtrToStructure(data, typeof(NT_ConnectionInfo));
                connectionsArray[i] = new ConnectionInfo(con.remote_id.ToString(), ReadUTF8String(con.remote_name), (int)con.remote_port, (long)con.last_update, (int)con.protocol_version);
            }
            NT_DisposeConnectionInfoArray(connections, count);
            return connectionsArray;
        }
        #endregion

        #region EntryListeners
        private static readonly Dictionary<int, NT_EntryListenerCallback> s_entryCallbacks =
            new Dictionary<int, NT_EntryListenerCallback>();

        public static int AddEntryListener(string prefix, Delegates.EntryListenerFunction listener, bool immediateNotify, bool localNotify)
        {
            NT_EntryListenerCallback modCallback = (uid, data, name, len, value, isNew) =>
            {
                NT_Type type = NT_GetValueType(value);
                object obj;
                ulong lastChange = 0;
                UIntPtr size = UIntPtr.Zero;
                IntPtr ptr;
                switch (type)
                {
                    case NT_Type.NT_UNASSIGNED:
                        obj = null;
                        break;
                    case NT_Type.NT_BOOLEAN:
                        int boolean = 0;
                        NT_GetValueBoolean(value, ref lastChange, ref boolean);
                        obj = boolean != 0;
                        break;
                    case NT_Type.NT_DOUBLE:
                        double val = 0;
                        NT_GetValueDouble(value, ref lastChange, ref val);
                        obj = val;
                        break;
                    case NT_Type.NT_STRING:
                        ptr = NT_GetValueString(value, ref lastChange, ref size);
                        obj = ReadUTF8String(ptr, size);
                        NT_FreeCharArray(ptr);
                        break;
                    case NT_Type.NT_RAW:
                        ptr = NT_GetValueRaw(value, ref lastChange, ref size);
                        obj = ReadUTF8String(ptr, size);
                        NT_FreeCharArray(ptr);
                        break;
                    case NT_Type.NT_BOOLEAN_ARRAY:
                        ptr = NT_GetValueBooleanArray(value, ref lastChange, ref size);
                        obj = GetBooleanArrayFromPtr(ptr, size);
                        NT_FreeBooleanArray(ptr);
                        break;
                    case NT_Type.NT_DOUBLE_ARRAY:
                        ptr = NT_GetValueDoubleArray(value, ref lastChange, ref size);
                        obj = GetDoubleArrayFromPtr(ptr, size);
                        NT_FreeDoubleArray(ptr);
                        break;
                    case NT_Type.NT_STRING_ARRAY:
                        ptr = NT_GetValueStringArray(value, ref lastChange, ref size);
                        obj = GetStringArrayFromPtr(ptr, size);
                        NT_FreeStringArray(ptr, size);
                        break;
                    case NT_Type.NT_RPC:
                        obj = null;
                        break;
                    default:
                        obj = null;
                        break;
                }
                string key = ReadUTF8String(name, len);
                listener((int)uid, key, obj, isNew != 0);
            };
            UIntPtr prefixSize;
            byte[] prefixStr = CreateUTF8String(prefix, out prefixSize);
            int retVal = (int)NT_AddEntryListener(prefixStr, prefixSize, IntPtr.Zero, modCallback, immediateNotify ? 1 : 0, localNotify ? 1 : 0);
            s_entryCallbacks.Add(retVal, modCallback);
            return retVal;
        }

        public static void RemoveEntryListener(int uid)
        {
            NT_RemoveEntryListener((uint)uid);
            if (s_entryCallbacks.ContainsKey(uid))
            {
                s_entryCallbacks.Remove(uid);
            }
        }
        #endregion

        #region Connection Listeners
        private static readonly Dictionary<int, NT_ConnectionListenerCallback> s_connectionCallbacks =
            new Dictionary<int, NT_ConnectionListenerCallback>();

        public static int AddConnectionListener(Delegates.ConnectionListenerFunction callback, bool immediateNotify)
        {
            NT_ConnectionListenerCallback modCallback =
                (uint uid, IntPtr data, int connected, ref NT_ConnectionInfo conn) =>
                {
                    string remoteName = ReadUTF8String(conn.remote_name);
                    ConnectionInfo info = new ConnectionInfo(conn.remote_id.ToString(), remoteName, (int)conn.remote_port, (long)conn.last_update, (int)conn.protocol_version);
                    callback((int)uid, connected != 0, info);
                };

            int retVal = (int)NT_AddConnectionListener(IntPtr.Zero, modCallback, immediateNotify ? 1 : 0);
            s_connectionCallbacks.Add(retVal, modCallback);
            return retVal;
        }

        public static void RemoveConnectionListener(int uid)
        {
            NT_RemoveConnectionListener((uint)uid);
            if (s_connectionCallbacks.ContainsKey(uid))
            {
                s_connectionCallbacks.Remove(uid);
            }
        }
        #endregion

        #region Server and Client Methods

        public static void SetNetworkIdentity(string name)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            NT_SetNetworkIdentity(namePtr, size);
        }
        public static void StartClient(string serverName, uint port)
        {
            if (serverName == null)
            {
                throw new ArgumentNullException(nameof(serverName), "Server cannot be null");
            }
            UIntPtr size;
            byte[] serverNamePtr = CreateUTF8String(serverName, out size);
            NT_StartClient(serverNamePtr, port);
        }

        public static void StartServer(string fileName, string listenAddress, uint port)
        {
            UIntPtr size = UIntPtr.Zero;
            var fileNamePtr = string.IsNullOrEmpty(fileName) ? new []{(byte)0} : CreateUTF8String(fileName, out size);
            var listenAddressPtr = string.IsNullOrEmpty(fileName) ? new[] { (byte)0 } : CreateUTF8String(listenAddress, out size);
            NT_StartServer(fileNamePtr, listenAddressPtr, port);
        }

        public static void StopClient()
        {
            NT_StopClient();
        }

        public static void StopServer()
        {
            NT_StopServer();
        }

        public static void SetUpdateRate(double interval)
        {
            NT_SetUpdateRate(interval);
        }
        #endregion

        #region Persistent

        public static void SavePersistent(string filename)
        {
            UIntPtr size;
            byte[] name = CreateUTF8String(filename, out size);
            IntPtr err = NT_SavePersistent(name);
            if (err != IntPtr.Zero) throw new Exception();//TODO: Figure out this exception
        }

        public static string[] LoadPersistent(string filename)
        {
            UIntPtr size;
            byte[] name = CreateUTF8String(filename, out size);
            List<string> warns = new List<string>();
            IntPtr err = NT_LoadPersistent(name, (line, msg) =>
            {
                warns.Add($"{line.ToString()}: {ReadUTF8String(msg)}");
            });
            if (err != IntPtr.Zero) throw new Exception();//TODO: Figure out this exception
            return warns.ToArray();
        }
        #endregion

        #region Flags
        public static void SetEntryFlags(string name, int flags)
        {
            UIntPtr size;
            byte[] str = CreateUTF8String(name, out size);
            NT_SetEntryFlags(str, size, (uint)flags);
        }

        public static int GetEntryFlags(string name)
        {
            UIntPtr size;
            byte[] str = CreateUTF8String(name, out size);
            uint flags = NT_GetEntryFlags(str, size);
            return (int)flags;
        }
        #endregion

        #region Utility

        public static void DeleteEntry(string name)
        {
            UIntPtr size;
            byte[] str = CreateUTF8String(name, out size);
            NT_DeleteEntry(str, size);
        }

        public static void DeleteAllEntries()
        {
            NT_DeleteAllEntries();
        }

        public static void Flush()
        {
            NT_Flush();
        }

        public static NT_Type GetType(string name)
        {
            UIntPtr size;
            byte[] str = CreateUTF8String(name, out size);
            NT_Type retVal = NT_GetType(str, size);
            return retVal;
        }

        public static bool ContainsKey(string key)
        {
            return GetType(key) != NT_Type.NT_UNASSIGNED;
        }

        public static long Now()
        {
            return (long)NT_Now();
        }
        #endregion

        #region Logger

        private static NT_LogFunc nativeLog;

        public static void SetLogger(Delegates.LoggerFunction func, int minLevel)
        {
            nativeLog = (level, file, line, msg) =>
            {
                string message = ReadUTF8String(msg);
                string fileName = ReadUTF8String(file);

                func((int)level, fileName, (int)line, message);
            };

            NT_SetLogger(nativeLog, (uint)minLevel);
        }

        #endregion

        #region IntPtr to Array Conversions
        private static double[] GetDoubleArrayFromPtr(IntPtr ptr, UIntPtr size)
        {
            double[] arr = new double[size.ToUInt64()];
            Marshal.Copy(ptr, arr, 0, arr.Length);
            return arr;
        }

        private static bool[] GetBooleanArrayFromPtr(IntPtr ptr, UIntPtr size)
        {
            int iSize = (int)size.ToUInt64();
            int[] arr = new int[size.ToUInt64()];

            bool[] bArr = new bool[iSize];
            for (int i = 0; i < iSize; i++)
            {
                IntPtr data = new IntPtr(ptr.ToInt64() + sizeof(int) * i);
                bArr[i] = ((int)Marshal.PtrToStructure(data, typeof(int))) != 0;
            }
            return bArr;
        }

        private static string[] GetStringArrayFromPtr(IntPtr ptr, UIntPtr size)
        {
            int ntStringSize = Marshal.SizeOf(typeof(NT_String_Read));
            int arraySize = (int)size.ToUInt64();
            string[] strArray = new string[arraySize];

            for (int i = 0; i < arraySize; i++)
            {
                IntPtr data = new IntPtr(ptr.ToInt64() + ntStringSize * i);
                strArray[i] = Marshal.PtrToStructure(data, typeof(NT_String_Read)).ToString();
            }
            return strArray;
        }
        #endregion

        #region IntPtrs To String Conversions
        //Must be null terminated
        public static byte[] ReadUTF8StringToByteArray(IntPtr str, UIntPtr size)
        {
            int iSize = (int)size.ToUInt64();
            byte[] data = new byte[iSize];
            Marshal.Copy(str, data, 0, iSize);
            return data;
        }

        public static byte[] CreateUTF8String(string str, out UIntPtr size)
        {
            var bytes = Encoding.UTF8.GetByteCount(str);

            var buffer = new byte[bytes + 1];
            size = (UIntPtr)bytes;
            Encoding.UTF8.GetBytes(str, 0, str.Length, buffer, 0);
            buffer[bytes] = 0;
            return buffer;
        }

        //Must be null terminated
        public static string ReadUTF8String(IntPtr str, UIntPtr size)
        {
            int iSize = (int)size.ToUInt64();
            byte[] data = new byte[iSize];
            Marshal.Copy(str, data, 0, iSize);
            return Encoding.UTF8.GetString(data);
        }

        public static string ReadUTF8String(IntPtr ptr)
        {
            var data = new List<byte>();
            var off = 0;
            while (true)
            {
                var ch = Marshal.ReadByte(ptr, off++);
                if (ch == 0)
                {
                    break;
                }
                data.Add(ch);
            }
            return Encoding.UTF8.GetString(data.ToArray());
        }
        #endregion

    }
}
