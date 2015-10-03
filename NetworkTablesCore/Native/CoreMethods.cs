using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using NetworkTables.Native.Exceptions;

namespace NetworkTables.Native
{
    /// <summary>
    /// This class contains all the methods to interface with the native library.
    /// </summary>
    /// <remarks>
    /// This is the equivelent of the NetworkTablesJNI.cpp class in the official library. It is the 
    /// bridge between the native methods called via P/Invoke and the NetworkTables classes.
    /// Most of these are internal, however some of them can be used publicly, so the class is public,
    /// and specific methods are public.
    /// </remarks>
    public static class CoreMethods
    {

        #region Setters
        internal static bool SetEntryBoolean(string name, bool value, bool force = false)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            int retVal = Interop.NT_SetEntryBoolean(namePtr, size, value ? 1 : 0, force ? 1 : 0);
            return retVal != 0;
        }

        internal static bool SetEntryDouble(string name, double value, bool force = false)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            int retVal = Interop.NT_SetEntryDouble(namePtr, size, value, force ? 1 : 0);
            return retVal != 0;
        }

        internal static bool SetEntryString(string name, string value, bool force = false)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr stringSize;
            byte[] stringPtr = CreateUTF8String(value, out stringSize);
            int retVal = Interop.NT_SetEntryString(namePtr, size, stringPtr, stringSize, force ? 1 : 0);
            return retVal != 0;
        }

        internal static bool SetEntryRaw(string name, string value, bool force = false)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr stringSize;
            byte[] stringPtr = CreateUTF8String(name, out stringSize);
            int retVal = Interop.NT_SetEntryRaw(namePtr, size, stringPtr, stringSize, force ? 1 : 0);
            return retVal != 0;
        }

        internal static bool SetEntryBooleanArray(string name, bool[] value, bool force = false)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);

            int[] valueIntArr = new int[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                valueIntArr[i] = value[i] ? 1 : 0;
            }

            int retVal = Interop.NT_SetEntryBooleanArray(namePtr, size, valueIntArr, (UIntPtr)valueIntArr.Length, force ? 1 : 0);

            return retVal != 0;
        }

        internal static bool SetEntryDoubleArray(string name, double[] value, bool force = false)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);

            int retVal = Interop.NT_SetEntryDoubleArray(namePtr, size, value, (UIntPtr)value.Length, force ? 1 : 0);

            return retVal != 0;
        }

        internal static bool SetEntryStringArray(string name, string[] value, bool force = false)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);

            NtStringWrite[] ntStrings = new NtStringWrite[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                ntStrings[i] = new NtStringWrite(value[i]);
            }

            int retVal = Interop.NT_SetEntryStringArray(namePtr, size, ntStrings, (UIntPtr)ntStrings.Length, force ? 1 : 0);

            foreach (var ntString in ntStrings)
            {
                ntString.Dispose();
            }

            return retVal != 0;
        }

        #endregion

        #region DefaultGetters
        internal static bool GetEntryBoolean(string name, bool defaultValue)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            int boolean = 0;
            ulong lc = 0;
            int status = Interop.NT_GetEntryBoolean(namePtr, size, ref lc, ref boolean);
            if (status == 0)
            {
                return defaultValue;
            }
            return boolean != 0;
        }

        internal static double GetEntryDouble(string name, double defaultValue)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            double retVal = 0;
            ulong lastChange = 0;
            int status = Interop.NT_GetEntryDouble(namePtr, size, ref lastChange, ref retVal);
            if (status == 0)
            {
                return defaultValue;
            }
            return retVal;
        }

        internal static string GetEntryString(string name, string defaultValue)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr stringSize = UIntPtr.Zero;
            ulong lastChange = 0;
            IntPtr ret = Interop.NT_GetEntryString(namePtr, size, ref lastChange, ref stringSize);
            if (ret == IntPtr.Zero)
            {
                return defaultValue;
            }
            else
            {
                string str = ReadUTF8String(ret, stringSize);
                Interop.NT_FreeCharArray(ret);
                return str;
            }
        }

        internal static string GetEntryRaw(string name, string defaultValue)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr stringSize = UIntPtr.Zero;
            ulong lastChange = 0;
            IntPtr ret = Interop.NT_GetEntryRaw(namePtr, size, ref lastChange, ref stringSize);
            if (ret == IntPtr.Zero)
            {
                return defaultValue;
            }
            else
            {
                string str = ReadUTF8String(ret, stringSize);
                Interop.NT_FreeCharArray(ret);
                return str;
            }
        }

        internal static double[] GetEntryDoubleArray(string name, double[] defaultValue)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr arrSize = UIntPtr.Zero;
            ulong lastChange = 0;
            IntPtr arrPtr = Interop.NT_GetEntryDoubleArray(namePtr, size, ref lastChange, ref arrSize);
            if (arrPtr == IntPtr.Zero)
            {
                return defaultValue;
            }
            double[] arr = GetDoubleArrayFromPtr(arrPtr, arrSize);
            Interop.NT_FreeDoubleArray(arrPtr);
            return arr;
        }

        internal static bool[] GetEntryBooleanArray(string name, bool[] defaultValue)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr arrSize = UIntPtr.Zero;
            ulong lastChange = 0;
            IntPtr arrPtr = Interop.NT_GetEntryBooleanArray(namePtr, size, ref lastChange, ref arrSize);
            if (arrPtr == IntPtr.Zero)
            {
                return defaultValue;
            }
            bool[] arr = GetBooleanArrayFromPtr(arrPtr, arrSize);
            Interop.NT_FreeBooleanArray(arrPtr);
            return arr;
        }

        internal static string[] GetEntryStringArray(string name, string[] defaultValue)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr arrSize = UIntPtr.Zero;
            ulong lastChange = 0;
            IntPtr arrPtr = Interop.NT_GetEntryStringArray(namePtr, size, ref lastChange, ref arrSize);
            if (arrPtr == IntPtr.Zero)
            {
                return defaultValue;
            }
            string[] arr = GetStringArrayFromPtr(arrPtr, arrSize);
            Interop.NT_FreeStringArray(arrPtr, arrSize);
            return arr;
        }
        #endregion

        #region Getters

        internal static bool GetEntryBoolean(string name)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            int boolean = 0;
            ulong lc = 0;
            int status = Interop.NT_GetEntryBoolean(namePtr, size, ref lc, ref boolean);
            if (status == 0)
            {
                throw new TableKeyNotDefinedException(name);//Change this to GetTableKeyNotDefined
            }
            return boolean != 0;
        }

        internal static double GetEntryDouble(string name)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            double retVal = 0;
            ulong lastChange = 0;
            int status = Interop.NT_GetEntryDouble(namePtr, size, ref lastChange, ref retVal);
            if (status == 0)
            {
                throw new TableKeyNotDefinedException(name);
            }
            return retVal;
        }

        internal static string GetEntryString(string name)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr stringSize = UIntPtr.Zero;
            ulong lastChange = 0;
            IntPtr ret = Interop.NT_GetEntryString(namePtr, size, ref lastChange, ref stringSize);
            if (ret == IntPtr.Zero)
            {
                throw new TableKeyNotDefinedException(name);
            }
            else
            {
                string str = ReadUTF8String(ret, stringSize);
                Interop.NT_FreeCharArray(ret);
                return str;
            }
        }

        internal static string GetEntryRaw(string name)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr stringSize = UIntPtr.Zero;
            ulong lastChange = 0;
            IntPtr ret = Interop.NT_GetEntryRaw(namePtr, size, ref lastChange, ref stringSize);
            if (ret == IntPtr.Zero)
            {
                throw new TableKeyNotDefinedException(name);
            }
            else
            {
                string str = ReadUTF8String(ret, stringSize);
                Interop.NT_FreeCharArray(ret);
                return str;
            }
        }

        internal static double[] GetEntryDoubleArray(string name)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr arrSize = UIntPtr.Zero;
            ulong lastChange = 0;
            IntPtr arrPtr = Interop.NT_GetEntryDoubleArray(namePtr, size, ref lastChange, ref arrSize);
            if (arrPtr == IntPtr.Zero)
            {
                throw new TableKeyNotDefinedException(name);//TODO: Change this to not defined exception
            }
            double[] arr = GetDoubleArrayFromPtr(arrPtr, arrSize);
            Interop.NT_FreeDoubleArray(arrPtr);
            return arr;
        }

        internal static bool[] GetEntryBooleanArray(string name)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr arrSize = UIntPtr.Zero;
            ulong lastChange = 0;
            IntPtr arrPtr = Interop.NT_GetEntryBooleanArray(namePtr, size, ref lastChange, ref arrSize);
            if (arrPtr == IntPtr.Zero)
            {
                throw new TableKeyNotDefinedException(name);//TODO: Change this to not defined exception
            }
            bool[] arr = GetBooleanArrayFromPtr(arrPtr, arrSize);
            Interop.NT_FreeBooleanArray(arrPtr);
            return arr;
        }

        internal static string[] GetEntryStringArray(string name)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr arrSize = UIntPtr.Zero;
            ulong lastChange = 0;
            IntPtr arrPtr = Interop.NT_GetEntryStringArray(namePtr, size, ref lastChange, ref arrSize);
            if (arrPtr == IntPtr.Zero)
            {
                throw new TableKeyNotDefinedException(name);
            }
            string[] arr = GetStringArrayFromPtr(arrPtr, arrSize);
            Interop.NT_FreeStringArray(arrPtr, arrSize);
            return arr;
        }
        #endregion

        #region EntryInfo

        internal static EntryInfo[] GetEntries(string prefix, EntryFlags types)
        {
            UIntPtr size;
            byte[] str = CreateUTF8String(prefix, out size);
            UIntPtr arrSize = UIntPtr.Zero;
            IntPtr arr = Interop.NT_GetEntryInfo(str, size, (uint)types, ref arrSize);
            int entryInfoSize = Marshal.SizeOf(typeof(NtEntryInfo));
            int arraySize = (int)arrSize.ToUInt64();
            EntryInfo[] entryArray = new EntryInfo[arraySize];

            for (int i = 0; i < arraySize; i++)
            {
                IntPtr data = new IntPtr(arr.ToInt64() + entryInfoSize * i);
                NtEntryInfo info = (NtEntryInfo)Marshal.PtrToStructure(data, typeof(NtEntryInfo));
                entryArray[i] = new EntryInfo(info.name.ToString(), info.type, (EntryFlags)info.flags, (long)info.last_change);
            }
            Interop.NT_DisposeEntryInfoArray(arr, arrSize);
            return entryArray;
        }
        #endregion

        #region ConnectionInfo

        internal static ConnectionInfo[] GetConnections()
        {
            UIntPtr count = UIntPtr.Zero;
            IntPtr connections = Interop.NT_GetConnections(ref count);
            int connectionInfoSize = Marshal.SizeOf(typeof(NtConnectionInfo));
            int arraySize = (int)count.ToUInt64();

            ConnectionInfo[] connectionsArray = new ConnectionInfo[arraySize];

            for (int i = 0; i < arraySize; i++)
            {
                IntPtr data = new IntPtr(connections.ToInt64() + connectionInfoSize * i);
                var con = (NtConnectionInfo)Marshal.PtrToStructure(data, typeof(NtConnectionInfo));
                connectionsArray[i] = new ConnectionInfo(con.remote_id.ToString(), ReadUTF8String(con.remote_name), (int)con.remote_port, (long)con.last_update, (int)con.protocol_version);
            }
            Interop.NT_DisposeConnectionInfoArray(connections, count);
            return connectionsArray;
        }
        #endregion

        #region EntryListeners
        private static readonly Dictionary<int, Interop.NT_EntryListenerCallback> s_entryCallbacks =
            new Dictionary<int, Interop.NT_EntryListenerCallback>();

        internal static int AddEntryListener(string prefix, EntryListenerFunction listener, NotifyFlags flags)
        {
            Interop.NT_EntryListenerCallback modCallback = (uid, data, name, len, value, flags_) =>
            {
                NtType type = Interop.NT_GetValueType(value);
                object obj;
                ulong lastChange = 0;
                UIntPtr size = UIntPtr.Zero;
                IntPtr ptr;
                switch (type)
                {
                    case NtType.Unassigned:
                        obj = null;
                        break;
                    case NtType.Boolean:
                        int boolean = 0;
                        Interop.NT_GetValueBoolean(value, ref lastChange, ref boolean);
                        obj = boolean != 0;
                        break;
                    case NtType.Double:
                        double val = 0;
                        Interop.NT_GetValueDouble(value, ref lastChange, ref val);
                        obj = val;
                        break;
                    case NtType.String:
                        ptr = Interop.NT_GetValueString(value, ref lastChange, ref size);
                        obj = ReadUTF8String(ptr, size);
                        Interop.NT_FreeCharArray(ptr);
                        break;
                    case NtType.Raw:
                        ptr = Interop.NT_GetValueRaw(value, ref lastChange, ref size);
                        obj = ReadUTF8String(ptr, size);
                        Interop.NT_FreeCharArray(ptr);
                        break;
                    case NtType.BooleanArray:
                        ptr = Interop.NT_GetValueBooleanArray(value, ref lastChange, ref size);
                        obj = GetBooleanArrayFromPtr(ptr, size);
                        Interop.NT_FreeBooleanArray(ptr);
                        break;
                    case NtType.DoubleArray:
                        ptr = Interop.NT_GetValueDoubleArray(value, ref lastChange, ref size);
                        obj = GetDoubleArrayFromPtr(ptr, size);
                        Interop.NT_FreeDoubleArray(ptr);
                        break;
                    case NtType.StringArray:
                        ptr = Interop.NT_GetValueStringArray(value, ref lastChange, ref size);
                        obj = GetStringArrayFromPtr(ptr, size);
                        Interop.NT_FreeStringArray(ptr, size);
                        break;
                    case NtType.Rpc:
                        obj = null;
                        break;
                    default:
                        obj = null;
                        break;
                }
                string key = ReadUTF8String(name, len);
                listener((int)uid, key, obj, (NotifyFlags)flags_);
            };
            UIntPtr prefixSize;
            byte[] prefixStr = CreateUTF8String(prefix, out prefixSize);
            int retVal = (int)Interop.NT_AddEntryListener(prefixStr, prefixSize, IntPtr.Zero, modCallback, (uint)flags);
            s_entryCallbacks.Add(retVal, modCallback);
            return retVal;
        }

        internal static void RemoveEntryListener(int uid)
        {
            Interop.NT_RemoveEntryListener((uint)uid);
            if (s_entryCallbacks.ContainsKey(uid))
            {
                s_entryCallbacks.Remove(uid);
            }
        }
        #endregion

        #region Connection Listeners
        private static readonly Dictionary<int, Interop.NT_ConnectionListenerCallback> s_connectionCallbacks =
            new Dictionary<int, Interop.NT_ConnectionListenerCallback>();

        internal static int AddConnectionListener(ConnectionListenerFunction callback, bool immediateNotify)
        {
            Interop.NT_ConnectionListenerCallback modCallback =
                (uint uid, IntPtr data, int connected, ref NtConnectionInfo conn) =>
                {
                    string remoteName = ReadUTF8String(conn.remote_name);
                    ConnectionInfo info = new ConnectionInfo(conn.remote_id.ToString(), remoteName, (int)conn.remote_port, (long)conn.last_update, (int)conn.protocol_version);
                    callback((int)uid, connected != 0, info);
                };

            int retVal = (int)Interop.NT_AddConnectionListener(IntPtr.Zero, modCallback, immediateNotify ? 1 : 0);
            s_connectionCallbacks.Add(retVal, modCallback);
            return retVal;
        }

        internal static void RemoveConnectionListener(int uid)
        {
            Interop.NT_RemoveConnectionListener((uint)uid);
            if (s_connectionCallbacks.ContainsKey(uid))
            {
                s_connectionCallbacks.Remove(uid);
            }
        }
        #endregion

        #region Server and Client Methods

        internal static void SetNetworkIdentity(string name)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            Interop.NT_SetNetworkIdentity(namePtr, size);
        }
        internal static void StartClient(string serverName, uint port)
        {
            if (serverName == null)
            {
                throw new ArgumentNullException(nameof(serverName), "Server cannot be null");
            }
            UIntPtr size;
            byte[] serverNamePtr = CreateUTF8String(serverName, out size);
            Interop.NT_StartClient(serverNamePtr, port);
        }

        internal static void StartServer(string fileName, string listenAddress, uint port)
        {
            UIntPtr size;
            var fileNamePtr = string.IsNullOrEmpty(fileName) ? new []{(byte)0} : CreateUTF8String(fileName, out size);
            var listenAddressPtr = string.IsNullOrEmpty(fileName) ? new[] { (byte)0 } : CreateUTF8String(listenAddress, out size);
            Interop.NT_StartServer(fileNamePtr, listenAddressPtr, port);
        }

        internal static void StopClient()
        {
            Interop.NT_StopClient();
        }

        internal static void StopServer()
        {
            Interop.NT_StopServer();
        }

        internal static void SetUpdateRate(double interval)
        {
            Interop.NT_SetUpdateRate(interval);
        }
        #endregion

        #region Persistent

        internal static void SavePersistent(string filename)
        {
            UIntPtr size;
            byte[] name = CreateUTF8String(filename, out size);
            IntPtr err = Interop.NT_SavePersistent(name);
            if (err != IntPtr.Zero) throw new Exception();//TODO: Figure out this exception
        }

        internal static string[] LoadPersistent(string filename)
        {
            UIntPtr size;
            byte[] name = CreateUTF8String(filename, out size);
            List<string> warns = new List<string>();
            IntPtr err = Interop.NT_LoadPersistent(name, (line, msg) =>
            {
                warns.Add($"{line.ToString()}: {ReadUTF8String(msg)}");
            });
            if (err != IntPtr.Zero) throw new Exception();//TODO: Figure out this exception
            return warns.ToArray();
        }
        #endregion

        #region Flags
        internal static void SetEntryFlags(string name, EntryFlags flags)
        {
            UIntPtr size;
            byte[] str = CreateUTF8String(name, out size);
            Interop.NT_SetEntryFlags(str, size, (uint)flags);
        }

        internal static EntryFlags GetEntryFlags(string name)
        {
            UIntPtr size;
            byte[] str = CreateUTF8String(name, out size);
            uint flags = Interop.NT_GetEntryFlags(str, size);
            return (EntryFlags)flags;
        }
        #endregion

        #region Utility

        internal static void DeleteEntry(string name)
        {
            UIntPtr size;
            byte[] str = CreateUTF8String(name, out size);
            Interop.NT_DeleteEntry(str, size);
        }

        internal static void DeleteAllEntries()
        {
            Interop.NT_DeleteAllEntries();
        }

        internal static void Flush()
        {
            Interop.NT_Flush();
        }

        internal static NtType GetType(string name)
        {
            UIntPtr size;
            byte[] str = CreateUTF8String(name, out size);
            NtType retVal = Interop.NT_GetType(str, size);
            return retVal;
        }

        internal static bool ContainsKey(string key)
        {
            return GetType(key) != NtType.Unassigned;
        }

        internal static long Now()
        {
            return (long)Interop.NT_Now();
        }
        #endregion

        #region Logger

        private static Interop.NT_LogFunc s_nativeLog;

        /// <summary>
        /// Assigns a method to be called whenever a log statement occurs in the internal
        /// network table library.
        /// </summary>
        /// <param name="func">The log function to assign.</param>
        /// <param name="minLevel">The minimum level to log.</param>
        public static void SetLogger(LoggerFunction func, LogLevel minLevel)
        {
            s_nativeLog = (level, file, line, msg) =>
            {
                string message = ReadUTF8String(msg);
                string fileName = ReadUTF8String(file);

                func((int)level, fileName, (int)line, message);
            };

            Interop.NT_SetLogger(s_nativeLog, (uint)minLevel);
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
            int ntStringSize = Marshal.SizeOf(typeof(NtStringRead));
            int arraySize = (int)size.ToUInt64();
            string[] strArray = new string[arraySize];

            for (int i = 0; i < arraySize; i++)
            {
                IntPtr data = new IntPtr(ptr.ToInt64() + ntStringSize * i);
                strArray[i] = Marshal.PtrToStructure(data, typeof(NtStringRead)).ToString();
            }
            return strArray;
        }
        #endregion

        #region IntPtrs To String Conversions
        //Must be null terminated
        internal static byte[] ReadUTF8StringToByteArray(IntPtr str, UIntPtr size)
        {
            int iSize = (int)size.ToUInt64();
            byte[] data = new byte[iSize];
            Marshal.Copy(str, data, 0, iSize);
            return data;
        }

        internal static byte[] CreateUTF8String(string str, out UIntPtr size)
        {
            var bytes = Encoding.UTF8.GetByteCount(str);

            var buffer = new byte[bytes + 1];
            size = (UIntPtr)bytes;
            Encoding.UTF8.GetBytes(str, 0, str.Length, buffer, 0);
            buffer[bytes] = 0;
            return buffer;
        }

        //Must be null terminated
        internal static string ReadUTF8String(IntPtr str, UIntPtr size)
        {
            int iSize = (int)size.ToUInt64();
            byte[] data = new byte[iSize];
            Marshal.Copy(str, data, 0, iSize);
            return Encoding.UTF8.GetString(data);
        }

        internal static string ReadUTF8String(IntPtr ptr)
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
