using System;
using System.Threading;
using NetworkTables;
using NetworkTables.Native;
using NUnit.Framework;

namespace NetworkTablesCore.Test
{
    [TestFixture]
    [Category("Server")]
    public class TestServer
    {
        [Test]
        public void Test()
        {
            CoreMethods.SetLogger(((level, file, line, message) =>
            {
                //Console.Error.WriteLine(message);
            }), 0);

            NetworkTable nt = NetworkTable.GetTable("");

            Thread.Sleep(500);

            nt.PutNumber("foo", 0.5);
            nt.SetFlags("foo", EntryFlags.Persistent);
            nt.PutNumber("foo2", 0.5);
            nt.PutNumber("foo2", 0.7);
            nt.PutNumber("foo2", 0.6);
            nt.PutNumber("foo2", 0.5);

            Thread.Sleep(500);
        }
    }
}
