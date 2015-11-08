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
        /// <summary>
        /// No type assigned
        /// </summary>
        Unassigned = 0,
        /// <summary>
        /// Boolean type
        /// </summary>
        Boolean = 0x01,
        /// <summary>
        /// Double type
        /// </summary>
        Double = 0x02,
        /// <summary>
        /// String type
        /// </summary>
        String = 0x04,
        /// <summary>
        /// Raw type
        /// </summary>
        Raw = 0x08,
        /// <summary>
        /// Boolean Array type
        /// </summary>
        BooleanArray = 0x10,
        /// <summary>
        /// Double Array type
        /// </summary>
        DoubleArray = 0x20,
        /// <summary>
        /// String Array type
        /// </summary>
        StringArray = 0x40,
        /// <summary>
        /// Rpc type
        /// </summary>
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
#pragma warning disable 649
        public readonly NtStringRead RemoteId;
        public readonly IntPtr RemoteName;
        public readonly uint RemotePort;
        public readonly ulong LastUpdate;
        public readonly uint ProtocolVersion;
#pragma warning restore 649
    }




}
