using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using NetworkTables;
using NetworkTables.Tables;

namespace TestProject
{
    class Program
    {
        static void Main(string[] args)
        {
            NetworkTables.NetworkTable.SetServerMode();
            NetworkTables.NetworkTable.Initialize();


            var t = NetworkTable.GetTable("test");

            int count = 0;
            while (true)
            {
                Console.WriteLine(count);
                t.PutNumber("key", count);
                count++;
                Thread.Sleep(500);
            }
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
