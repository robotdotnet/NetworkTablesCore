using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetworkTables;
using NUnit.Framework;

namespace NetworkTablesCore.Test
{
    [TestFixture]
    [Ignore("Still trying to debug multiple shutdowns. They certainly are not liked.")]
    public class TestNetworkTablesStaticApi : TestBase
    {
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            NetworkTable.Shutdown();
        }

        [TestFixtureTearDown]
        public void FixtureTeardown()
        {
            Thread.Sleep(10);
            NetworkTable.Shutdown();
            NetworkTable.SetIPAddress("127.0.0.1");
            NetworkTable.SetPort(10000);
            NetworkTable.SetServerMode();
            Thread.Sleep(10);
            NetworkTable.Initialize();
        }

        [SetUp]
        public void Setup()
        {
            NetworkTable.Shutdown();
            NetworkTable.SetIPAddress("127.0.0.1");
        }

        [Test]
        public void TestDoubleInitializeServer()
        {
            NetworkTable.SetServerMode();
            Assert.That(NetworkTable.Client, Is.False);
            NetworkTable.Initialize();
            Assert.DoesNotThrow(NetworkTable.Initialize);
        }

        [Test]
        public void TestDoubleInitializeClient()
        {
            NetworkTable.SetClientMode();
            Assert.That(NetworkTable.Client, Is.True);
            NetworkTable.Initialize();
            Assert.DoesNotThrow(NetworkTable.Initialize);
        }

        [Test]
        public void TestClientShutdown()
        {
            NetworkTable.SetClientMode();
            NetworkTable.Initialize();
            Assert.That(NetworkTable.Running, Is.True);
            NetworkTable.Shutdown();
            Assert.That(NetworkTable.Running, Is.False);
        }

        [Test]
        public void TestServerShutdown()
        {
            NetworkTable.SetServerMode();
            NetworkTable.Initialize();
            Assert.That(NetworkTable.Running, Is.True);
            NetworkTable.Shutdown();
            Assert.That(NetworkTable.Running, Is.False);
        }

        [Test]
        public void TestSetClientModeWhileStopped()
        {
            NetworkTable.SetServerMode();
            Assert.That(NetworkTable.Client, Is.False);
            NetworkTable.SetClientMode();
            Assert.That(NetworkTable.Client, Is.True);
            NetworkTable.SetClientMode();
            Assert.That(NetworkTable.Client, Is.True);
        }

        [Test]
        public void TestSetClientModeWhileRunningServer()
        {
            NetworkTable.SetServerMode();
            Assert.That(NetworkTable.Client, Is.False);
            NetworkTable.Initialize();
            Assert.Throws<InvalidOperationException>(NetworkTable.SetClientMode);
        }

        [Test]
        public void TestSetTeam()
        {
            NetworkTable.SetTeam(1234);
            Assert.That(NetworkTable.IPAddress, Is.EqualTo("roboRIO-1234-FRC.local"));
        }

        [Test]
        public void TestSetIpAddress()
        {
            NetworkTable.SetIPAddress("127.0.0.1");
            NetworkTable.SetIPAddress("10.12.34.2");
            Assert.That(NetworkTable.IPAddress, Is.EqualTo("10.12.34.2"));
        }
    }
}
