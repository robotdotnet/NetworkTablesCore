//#define NativeDebug //Uncomment this to enable easy native debugging. Then change the DebugxWindows to be the native lib location
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace NetworkTables.Native
{
    enum OsType
    {
        Windows32,
        Windows64,
        Linux32,
        Linux64,
        MacOs32,
        MacOs64,
        ArmLinux,
        RoboRio
    }

    [ExcludeFromCodeCoverage]
    internal static class LoaderUtilities
    {

        [DllImport("libc")]
        static extern int uname(IntPtr buf);

        static bool IsRunningOnMac()
        {
            Utsname name;
            Uname.uname(out name);

            Console.WriteLine(name.ToString());

            if (name.sysname == "Darwin") return true;
            return false;
            /*
            IntPtr buf = IntPtr.Zero;
            try
            {
                buf = Marshal.AllocHGlobal(8192);
                // This is a hacktastic way of getting sysname from uname () 
                if (uname(buf) == 0)
                {
                    string os = Marshal.PtrToStringAnsi(buf);
                    if (os == "Darwin") return true;
                }
            }
            catch { }
            finally
            {
                if (buf != IntPtr.Zero) Marshal.FreeHGlobal(buf);
            }
            return false;
            */
        }

        internal static OsType GetOsType()
        {
            Console.WriteLine((int)Environment.OSVersion.Platform);

            var platform = (int)Environment.OSVersion.Platform;
            if (platform == 4 || platform == 6 || platform == 128)
            {
                //These 3 mean we are running on a unix based system
                //Check for RIO first
                if (File.Exists("/usr/local/frc/bin/frcRunRobot.sh")) return OsType.RoboRio;

                //TODO: Add check for Arm vs x86/amd64

                //Check for Bitness
                if (Environment.Is64BitProcess)
                {
                    //We are 64 bit.
                    if (IsRunningOnMac()) return OsType.MacOs64;
                    return OsType.Linux64;
                }
                else
                {
                    //We are 64 32 bit process.
                    if (IsRunningOnMac()) return OsType.MacOs32;
                    return OsType.Linux32;
                }


            }
            else
            {
                //Assume we are on windows otherwise
                return Environment.Is64BitProcess ? OsType.Windows64 : OsType.Windows32;
            }
        }

        internal static bool CheckOsValid(OsType type)
        {
            switch (type)
            {
                case OsType.Windows32:
                    return true;
                case OsType.Windows64:
                    return true;
                case OsType.Linux32:
                    return true;
                case OsType.Linux64:
                    return true;
                case OsType.MacOs32:
                    return true;
                case OsType.MacOs64:
                    return true;
                case OsType.ArmLinux:
                    return true;
                case OsType.RoboRio:
                    return true;
                default:
                    return false;
            }
        }

#if NativeDebug

        private const string Debug64 = @"C:\Users\thad\Documents\GitHub\PeterJohnson\ntcore\build\binaries\ntcoreSharedLibrary\x64\ntcore.dll";
        private const string Debug32 = @"C:\Users\thad\Documents\GitHub\PeterJohnson\ntcore\build\binaries\ntcoreSharedLibrary\x86\ntcore.dll";
        private const bool debug = true;
#else
        private const bool debug = false;
#endif


        internal static string ExtractLibrary(OsType type)
        {

#if NativeDebug
            if (debug)
            {
                switch (type)
                {
                    case OsType.Windows32:
                        return Debug32;
                        break;
                    case OsType.Windows64:
                        return Debug64;
                        break;
                }
            }
#endif


            string inputName;
            string outputName;
            switch (type)
            {
                case OsType.Windows32:
                    inputName = "NetworkTables.NativeLibraries.x86.ntcore.dll";
                    outputName = "ntcore.dll";
                    break;
                case OsType.Windows64:
                    inputName = "NetworkTables.NativeLibraries.amd64.ntcore.dll";
                    outputName = "ntcore.dll";
                    break;
                case OsType.Linux32:
                    inputName = "NetworkTables.NativeLibraries.x86.libntcore.so";
                    outputName = "libntcore.so";
                    break;
                case OsType.Linux64:
                    inputName = "NetworkTables.NativeLibraries.amd64.libntcore.so";
                    outputName = "libntcore.so";
                    break;
                case OsType.MacOs32:
                    inputName = "NetworkTables.NativeLibraries.x86.libntcore.dylib";
                    outputName = "libntcore.dylib";
                    break;
                case OsType.MacOs64:
                    inputName = "NetworkTables.NativeLibraries.amd64.libntcore.dylib";
                    outputName = "libntcore.dylib";
                    break;
                case OsType.RoboRio:
                case OsType.ArmLinux:
                    inputName = "NetworkTables.NativeLibraries.arm.libntcore.so";
                    outputName = "libntcore.so";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            outputName = Path.GetTempPath() + outputName;
            byte[] bytes;
            //Load our resource file into memory
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(inputName))
            {
                if (s == null || s.Length == 0)
                    return null;
                bytes = new byte[(int)s.Length];
                s.Read(bytes, 0, (int)s.Length);
            }
            bool isFileSame = true;

            //If file exists
            if (File.Exists(outputName))
            {
                //Load existing file into memory
                byte[] existingFile = File.ReadAllBytes(outputName);
                //If files are different length they are different,
                //and we can automatically assume they are different.
                if (existingFile.Length != bytes.Length)
                {
                    isFileSame = false;
                }
                else
                {
                    //Otherwise directly compare the files
                    //I first tried hashing, but that took 1.5-2.0 seconds,
                    //wheras this took 0.3 seconds.
                    for (int i = 0; i < existingFile.Length; i++)
                    {
                        if (bytes[i] != existingFile[i])
                        {
                            isFileSame = false;
                        }
                    }
                }
            }
            else
            {
                isFileSame = false;
            }

            //If file is different write the new file
            if (!isFileSame)
            {
                if (File.Exists(outputName))
                    File.Delete(outputName);
                File.WriteAllBytes(outputName, bytes);
            }
            //Force a garbage collection, since we just wasted about 12 MB of RAM.
            GC.Collect();

            return outputName;

        }

        internal static IntPtr LoadLibrary(string dllLoc, OsType type, out ILibraryLoader loader)
        {
            switch (type)
            {
                case OsType.Windows32:
                case OsType.Windows64:
                    loader = new WindowsLibraryLoader();
                    return loader.LoadLibrary(dllLoc);
                case OsType.Linux32:
                case OsType.Linux64:
                    loader = new LinuxLibraryLoader();
                    return loader.LoadLibrary(dllLoc);
                case OsType.MacOs32:
                case OsType.MacOs64:
                    loader = new MacOsLibraryLoader();
                    return loader.LoadLibrary(dllLoc);
                case OsType.RoboRio:
                    loader = new RoboRioLibraryLoader();
                    return loader.LoadLibrary(dllLoc);
                default:
                    loader = null;
                    return IntPtr.Zero;
            }
        }
    }
}
