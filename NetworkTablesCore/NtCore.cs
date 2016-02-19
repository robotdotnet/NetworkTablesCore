using NetworkTables.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTables
{
 /*   public static class NtCore
    {
        public static Value GetEntryValue(string name)
        {
            return Storage.Instance.GetEntryValue(name);
        }

        public static bool SetEntryValue(string name, Value value)
        {
            return Storage.Instance.SetEntryValue(name, value);
        }

        public static void SetEntryTypeValue(string name, Value value)
        {
            Storage.Instance.SetEntryTypeValue(name, value);
        }

        public static void SetEntryFlags(string name, EntryFlags flags)
        {
            Storage.Instance.SetEntryFlags(name, flags);
        }

        public static EntryFlags GetEntryFlags(string name)
        {
            return Storage.Instance.GetEntryFlags(name);
        }

        public static void DeleteEntry(string name)
        {
            CoreMethods.DeleteEntry(name);
        }

        public static void DeleteAllEntries()
        {
            CoreMethods.DeleteAllEntries();
        }

        public static List<EntryInfo> GetEntryInfo(string prefix, NtType types)
        {
            return CoreMethods.GetEntries(prefix, types);
        }

        public static void Flush()
        {
            CoreMethods.Flush();
        }



        public static int AddEntryListener(string prefix, EntryListenerCallback callback, NotifyFlags flags)
        {
            CoreMethods.AddEntryListener(prefix, callback, flags);

            Notifier notifier = Notifier.Instance;
            int uid = notifier.AddEntryListener(prefix, callback, flags);
            notifier.Start();
            if ((flags & NotifyFlags.NotifyImmediate) != 0)
                Storage.Instance.NotifyEntries(prefix, callback);
            return uid;
        }

        public static void RemoveEntryListener(int uid)
        {
            Notifier.Instance.RemoveEntryListener(uid);
        }

        public static int AddConnectionListener(ConnectionListenerCallback callback, bool immediateNotify)
        {
            Notifier notifier = Notifier.Instance;
            int uid = notifier.AddConnectionListener(callback);
            notifier.Start();
            if (immediateNotify) Dispatcher.Instance.NotifyConnections(callback);
            return uid;
        }

        public static void RemoveConnectionListener(int uid)
        {
            Notifier.Instance.RemoveConnectionListener(uid);
        }


        public static void SetNetworkIdentity(string name)
        {
            CoreMethods.SetNetworkIdentity(name);
        }

        public static void StartServer(string persistFilename, string listenAddress, int port)
        {
            CoreMethods.StartServer(persistFilename, listenAddress, (uint)port);
        }

        public static void StopServer()
        {
            CoreMethods.StopServer();
        }

        public static void StartClient(string serverName, int port)
        {
            CoreMethods.StartClient(serverName, (uint)port);
        }

        public static void StopClient()
        {
            CoreMethods.StopClient();
        }

        public static void StopRpcServer()
        {
            CoreMethods.StopRpcServer();
        }

        public static void StopNotifier()
        {
            CoreMethods.StopNotifier();
        }

        public static void SetUpdateRate(double interval)
        {
            CoreMethods.SetUpdateRate(interval);
        }

        public static List<ConnectionInfo> GetConnections()
        {
            return CoreMethods.GetConnections();
        }

        public static string SavePersistent(string filename)
        {
            return CoreMethods.SavePersistent(filename, false);
        }

        public static string LoadPersistent(string filename, Action<int, string> warn)
        {
            return CoreMethods.LoadPersistent(filename, warn);
        }

        public static long Now()
        {
            return CoreMethods.Now();
        }

        public static void SetLogger(LogFunc func, LogLevel minLevel)
        {
            CoreMethods.SetLogger(func, minLevel);
        }
    }
    */
}
