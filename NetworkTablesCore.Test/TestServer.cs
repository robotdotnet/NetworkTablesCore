using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetworkTables;
using NetworkTables.Native;
using NUnit.Framework;

namespace NetworkTablesCore.Test
{
    [TestFixture]
    public class TestServer
    {
        [Test]
        public void Test()
        {
            NetworkTable.Shutdown();

            CoreMethods.SetLogger(((level, file, line, message) =>
            {
                Console.Error.WriteLine(message);
            }), 0);

            NetworkTable.SetIPAddress("127.0.0.1");
            NetworkTable.SetPort(10000);
            NetworkTable.SetServerMode();
            NetworkTable nt = NetworkTable.GetTable("");

            Thread.Sleep(1000);

            nt.PutNumber("foo", 0.5);
            nt.SetFlags("foo", EntryFlags.PERSISTENT);

            nt.PutNumber("foo2", 0.5);
            nt.PutNumber("foo2", 0.7);
            nt.PutNumber("foo2", 0.6);
            nt.PutNumber("foo2", 0.5);

            Thread.Sleep(1000);
        }
    }
}
