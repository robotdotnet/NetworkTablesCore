using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkTables.Native;
using NUnit.Framework;

namespace NetworkTablesCore.Test
{
    [TestFixture]
    [Category("Client")]
    public class TestNativeStructs : ClientTestBase
    {
        [Test]
        public void TestNtStringWriteCreateAndDispose()
        {
            NtStringWrite write = new NtStringWrite("TestString");
            write.Dispose();
        }

        [Test]
        public void TestNtStringWriteToString()
        {
            const string testStr = "TestString";
            using (NtStringWrite write = new NtStringWrite(testStr))
            {
                Assert.AreEqual(testStr, write.ToString());
            }
        }

        [Test]
        public void TestNtStringReadToString()
        {
            const string testStr = "TestString";
            using (NtStringWrite write = new NtStringWrite(testStr))
            {
                NtStringRead read = new NtStringRead(write.str, write.len);
                Assert.AreEqual(testStr, read.ToString());
                GC.KeepAlive(write);
            }
        }
    }
}
