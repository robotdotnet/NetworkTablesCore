using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static NetworkTablesCore.Native.Interop;

namespace NetworkTablesCore.Native
{
    public enum NT_Type
    {
        NT_UNASSIGNED = 0,
        NT_BOOLEAN = 0x01,
        NT_DOUBLE = 0x02,
        NT_STRING = 0x04,
        NT_RAW = 0x08,
        NT_BOOLEAN_ARRAY = 0x10,
        NT_DOUBLE_ARRAY = 0x20,
        NT_STRING_ARRAY = 0x40,
        NT_RPC = 0x80
    }  

    [StructLayout(LayoutKind.Sequential)]
    internal struct NT_String_Read
    {
        private readonly IntPtr str;
        private readonly UIntPtr len;

        public override string ToString()
        {
            byte[] arr = new byte[len.ToUInt64()];
            Marshal.Copy(str, arr, 0, (int)len.ToUInt64());
            return Encoding.UTF8.GetString(arr);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NT_String_Write : IDisposable
    {
        private readonly IntPtr str;
        private readonly UIntPtr len;

        public NT_String_Write(string v_str)
        {
            int bytes = Encoding.UTF8.GetByteCount(v_str);
            str = Marshal.AllocHGlobal(bytes * sizeof(byte));
            byte[] buffer = new byte[bytes];
            Encoding.UTF8.GetBytes(v_str, 0, v_str.Length, buffer, 0);
            Marshal.Copy(buffer, 0, str, bytes);
            len = (UIntPtr)bytes;
        }

        public override string ToString()
        {
            byte[] arr = new byte[len.ToUInt64()];
            Marshal.Copy(str, arr, 0, (int)len.ToUInt64());
            return Encoding.UTF8.GetString(arr);
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(str);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NT_EntryInfo
    {
        public NT_String_Read name;
        public NT_Type type;
        public uint flags;
        public ulong last_change;
    }


    //Looks like this will always be created for us by the library, so we do not have to write it.
    internal struct NT_ConnectionInfo
    {
        public NT_String_Read remote_id;
        public IntPtr remote_name;
        public uint remote_port;
        public ulong last_update;
        public uint protocol_version;

    }




}
