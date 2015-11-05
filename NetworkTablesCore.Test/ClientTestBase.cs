using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkTables;
using NUnit.Framework;

namespace NetworkTablesCore.Test
{
    public class ClientTestBase
    {
        [SetUp]
        public void Init()
        {
            if (!GlobalTestConstants.started)
            {
                NetworkTable.SetIPAddress("127.0.0.1");
                NetworkTable.SetPort(10000);
                NetworkTable.SetClientMode();
                NetworkTable.Initialize();
                GlobalTestConstants.started = true;
                GlobalTestConstants.server = false;
            }
            else
            {
                if (GlobalTestConstants.server == true)
                {
                    throw new InvalidOperationException("Test can only run either client or server. Not both.");
                }
            }
        }
    }
}
