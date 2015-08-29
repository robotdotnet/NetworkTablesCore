using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetworkTablesCore.Native;
using NetworkTablesCore.Native.Exceptions;

namespace NetworkTablesCore
{
    class program
    {
        static void Main(string[] args)
        {
            CoreMethods.SetLogger(((level, file, line, message) =>
            {
                Console.Error.WriteLine(message);
            }), 0);

            NetworkTable.SetIPAddress("127.0.0.1");
            NetworkTable.SetPort(10000);
            NetworkTable.SetServerMode();
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
            nt.SetFlags("bar", NetworkTable.PERSISTENT);
            nt.PutBoolean("bar2", true);
            nt.PutBoolean("bar2", false);
            nt.PutBoolean("bar2", true);
            nt.PutString("str", "hello world");
            double[] nums = new[] { 0.5, 1.2, 3.0 };
            nt.PutNumberArray("numarray", nums);

            string[] strs = new[] { "Hello", "World" };
            nt.PutStringArray("strarray", strs);

            Console.WriteLine(nt.GetBoolean("bar"));
            Console.WriteLine(nt.GetBoolean("bar2"));

            Console.WriteLine(nt.GetString("str"));

            foreach (var d in nt.GetNumberArray("numarray"))
            {
                Console.WriteLine(d);
            }

            foreach (var s in nt.GetStringArray("strarray"))
            {
                Console.WriteLine(s);
            }

            Console.ReadKey();
        }
    }
}
