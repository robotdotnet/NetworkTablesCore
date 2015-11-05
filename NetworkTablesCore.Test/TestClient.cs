using System;
using System.Threading;
using NetworkTables;
using NetworkTables.Native;
using NetworkTables.Native.Exceptions;
using NUnit.Framework;

namespace NetworkTablesCore.Test
{
    [TestFixture]
    public class TestClient
    {
        [Test]
        public void Client()
        {
            NetworkTable.Shutdown();

            CoreMethods.SetLogger(((level, file, line, message) =>
            {
                //Console.Error.WriteLine(message);
            }), 0);

            NetworkTable.SetIPAddress("127.0.0.1");
            NetworkTable.SetPort(10000);
            NetworkTable.SetClientMode();
            NetworkTable nt = NetworkTable.GetTable("");

            Thread.Sleep(2000);

            try
            {
                Console.WriteLine("Got foo: " + nt.GetNumber("foo"));
            }
            catch (TableKeyNotDefinedException)
            {
            }

            nt.PutBoolean("bar", false);
            nt.SetFlags("bar", EntryFlags.Persistent);
            nt.PutBoolean("bar2", true);
            nt.PutBoolean("bar2", false);
            nt.PutBoolean("bar2", true);
            nt.PutString("str", "hello world");
            double[] nums = new[] { 0.5, 1.2, 3.0 };
            nt.PutNumberArray("numarray", nums);

            string[] strs = new[] {"Hello", "World"};
            nt.PutStringArray("strarray", strs);

            Thread.Sleep(1000);
        }
    }
}
