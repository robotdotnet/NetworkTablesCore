using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTables.Native
{
    [AttributeUsage(
        AttributeTargets.Class |
        AttributeTargets.Delegate |
        AttributeTargets.Enum |
        AttributeTargets.Field |
        AttributeTargets.Struct)]
    internal class MapAttribute : Attribute
    {
        private string nativeType;
        private string suppressFlags;

        public MapAttribute()
        {
        }

        public MapAttribute(string nativeType)
        {
            this.nativeType = nativeType;
        }

        public string NativeType
        {
            get { return nativeType; }
        }

        public string SuppressFlags
        {
            get { return suppressFlags; }
            set { suppressFlags = value; }
        }
    }

    internal sealed class Utsname
        : IEquatable<Utsname>
    {
        public string sysname;
        public string nodename;
        public string release;
        public string version;
        public string machine;
        public string domainname;

        public override int GetHashCode()
        {
            return sysname.GetHashCode() ^ nodename.GetHashCode() ^
                release.GetHashCode() ^ version.GetHashCode() ^
                machine.GetHashCode() ^ domainname.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            Utsname u = (Utsname)obj;
            return Equals(u);
        }

        public bool Equals(Utsname value)
        {
            return value.sysname == sysname && value.nodename == nodename &&
                value.release == release && value.version == version &&
                value.machine == machine && value.domainname == domainname;
        }

        // Generate string in /etc/passwd format
        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4}",
                sysname, nodename, release, version, machine);
        }

        public static bool operator ==(Utsname lhs, Utsname rhs)
        {
            return Object.Equals(lhs, rhs);
        }

        public static bool operator !=(Utsname lhs, Utsname rhs)
        {
            return !Object.Equals(lhs, rhs);
        }
    }

    [Map]
    internal struct _Utsname
    {
        public IntPtr sysname;
        public IntPtr nodename;
        public IntPtr release;
        public IntPtr version;
        public IntPtr machine;
        public IntPtr domainname;
        public IntPtr _buf_;
    }

    [ExcludeFromCodeCoverage]
    internal class Uname
    {
        private static void CopyUtsname(ref Utsname to, ref _Utsname from)
        {
            try
            {
                to = new Utsname();
                to.sysname = Marshal.PtrToStringAnsi(from.sysname);
                to.nodename = Marshal.PtrToStringAnsi(from.nodename);
                to.release = Marshal.PtrToStringAnsi(from.release);
                to.version = Marshal.PtrToStringAnsi(from.version);
                to.machine = Marshal.PtrToStringAnsi(from.machine);
                to.domainname = Marshal.PtrToStringAnsi(from.domainname);
            }
            finally
            {
                free(from._buf_);
                from._buf_ = IntPtr.Zero;
            }
        }
        internal const string MPH = "MonoPosixHelper";
        internal const string LIBC = "libc";

        [DllImport(LIBC, CallingConvention = CallingConvention.Cdecl)]
        public static extern void free(IntPtr ptr);

        [DllImport(MPH, SetLastError = true,
                EntryPoint = "Mono_Posix_Syscall_uname")]
        private static extern int sys_uname(out _Utsname buf);

        public static int uname(out Utsname buf)
        {
            _Utsname _buf;
            int r = sys_uname(out _buf);
            buf = new Utsname();
            if (r == 0)
            {
                CopyUtsname(ref buf, ref _buf);
            }
            return r;
        }
    }
}
