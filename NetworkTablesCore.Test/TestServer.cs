using System;
using System.Threading;
using NetworkTables;
using NetworkTables.Native;
using NUnit.Framework;

namespace NetworkTablesCore.Test
{
    [TestFixture]
    public class TestServer
    {
        [Test]
        //[Ignore("Test is failing because Shutdown() has a deadlock in ntcore. Fixed in library, but need to update library.")]
        [Timeout(10000)]
        public void Test()
        {
            CoreMethods.SetLogger(((level, file, line, message) =>
            {
                Console.Error.WriteLine(message);
            }), 0);
            
            NetworkTable.Shutdown();
            
            NetworkTable.SetIPAddress("127.0.0.1");
            NetworkTable.SetPort(10000);
            NetworkTable.SetServerMode();
            NetworkTable nt = NetworkTable.GetTable("");

            Thread.Sleep(1000);

            nt.PutNumber("foo", 0.5);
            nt.SetFlags("foo", EntryFlags.Persistent);
            nt.PutNumber("foo2", 0.5);
            nt.PutNumber("foo2", 0.7);
            nt.PutNumber("foo2", 0.6);
            nt.PutNumber("foo2", 0.5);

            Thread.Sleep(1000);
        }
    }
}
