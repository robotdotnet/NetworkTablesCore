using System;
using System.Threading;
using NetworkTables;
using NetworkTables.Native;
using NUnit.Framework;

namespace NetworkTablesCore.Test
{
    public class TestServer
    {
        public void Test()
        {
            CoreMethods.SetLogger(((level, file, line, message) =>
            {
                Console.Error.WriteLine(message);
            }), LogLevel.LogDebug4);
            
            Console.WriteLine("BeforeShuttingDown");
            NetworkTable.Shutdown();
            Console.WriteLine("Shutting Down");
            

            NetworkTable.SetIPAddress("127.0.0.1");
            Console.WriteLine("IP");
            NetworkTable.SetPort(10000);
            Console.WriteLine("Port");
            NetworkTable.SetServerMode();
            Console.WriteLine("Server");
            NetworkTable nt = NetworkTable.GetTable("");
            Console.WriteLine("GetTable");

            Thread.Sleep(1000);
            Console.WriteLine("FirstSleep");

            nt.PutNumber("foo", 0.5);
            Console.WriteLine("foo .5");
            nt.SetFlags("foo", EntryFlags.Persistent);
            Console.WriteLine("Persistent");
            nt.PutNumber("foo2", 0.5);
            Console.WriteLine("Foo 2 .5");
            nt.PutNumber("foo2", 0.7);
            Console.WriteLine("Foo 2 .7");
            nt.PutNumber("foo2", 0.6);
            Console.WriteLine("Foo 2 .6");
            nt.PutNumber("foo2", 0.5);
            Console.WriteLine("Foo 2 .5 2");

            Thread.Sleep(1000);
            Console.WriteLine("2ndSleep");
        }
    }
}
