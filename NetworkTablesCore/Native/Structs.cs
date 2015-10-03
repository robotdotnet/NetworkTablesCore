using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NetworkTables.Native
{
    /// <summary>
    /// An enumeration of all types allowed in the NetworkTables.
    /// </summary>
    public enum NtType
    {
        ///No type assigned
        Unassigned = 0,
        ///Boolean type
        Boolean = 0x01,
        ///Double type
        Double = 0x02,
        ///String type
        String = 0x04,
        ///Raw type
        Raw = 0x08,
        ///Boolean Array type
        BooleanArray = 0x10,
        ///Double Array type
        DoubleArray = 0x20,
        ///String Array type
        StringArray = 0x40,
        ///Rpc type
        Rpc = 0x80
    }  

    [StructLayout(LayoutKind.Sequential)]
    internal struct NtStringRead
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
    internal struct NtStringWrite : IDisposable
    {
        private readonly IntPtr str;
        private readonly UIntPtr len;

        public NtStringWrite(string vStr)
        {
            int bytes = Encoding.UTF8.GetByteCount(vStr);
            str = Marshal.AllocHGlobal(bytes * sizeof(byte));
            byte[] buffer = new byte[bytes];
            Encoding.UTF8.GetBytes(vStr, 0, vStr.Length, buffer, 0);
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
    internal struct NtEntryInfo
    {
        public NtStringRead name;
        public NtType type;
        public uint flags;
        public ulong last_change;
    }


    //Looks like this will always be created for us by the library, so we do not have to write it.
    internal struct NtConnectionInfo
    {
        public NtStringRead remote_id;
        public IntPtr remote_name;
        public uint remote_port;
        public ulong last_update;
        public uint protocol_version;

    }




}
