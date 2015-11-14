using System;
using NetworkTables;
using NUnit.Framework;

namespace NetworkTablesCore.Test
{
    public class ClientTestBase
    {
        [SetUp]
        public void TestInit()
        {
            Console.WriteLine(Environment.Is64BitProcess ? "64 Bit Test" : "32 Bit Test");
        }

        [TestFixtureSetUp]
        public void ClassInit()
        {
            if (!GlobalTestConstants.Started)
            {
                NetworkTable.SetIPAddress("127.0.0.1");
                NetworkTable.SetPort(10000);
                NetworkTable.SetServerMode();
                NetworkTable.Initialize();
                GlobalTestConstants.Started = true;
                GlobalTestConstants.Server = true;
            }
            else
            {
                if (GlobalTestConstants.Server == false)
                {
                    throw new InvalidOperationException("Test can only run either client or Server. Not both.");
                }
            }
        }
    }
}
