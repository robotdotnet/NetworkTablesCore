using System;
using NetworkTables;
using NUnit.Framework;

namespace NetworkTablesCore.Test
{
    [TestFixture]
    public class TestEntries
    {
        private NetworkTable nt;

        public TestEntries()
        {
            nt = NetworkTable.GetTable("");
        }

        [Test]
        public void TestPutEmptyName()
        {
            bool retVal = nt.PutNumber("emptyName", 3.14);
            Assert.IsTrue(retVal);
        }

        [Test]
        public void TestPutNullName()
        {
            bool retVal = nt.PutNumber(null, 3.14);
            Assert.IsTrue(retVal);
        }
        [Test]
        public void TestPutNullString()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                nt.PutString("nullString", null);
            }, "Null string should not be allowed");

        }
        [Test]
        public void TestPutEmptyString()
        {
            bool retVal = nt.PutString("emptyString", "");
            Assert.IsTrue(retVal);
        }
        [Test]
        public void TestPutNullStringArray()
        {
            Assert.Throws<NullReferenceException>(() =>
            {
                nt.PutStringArray("nullStringArray", null);
            }, "Null array should not be allowed");
        }
        [Test]
        public void TestPutEmptyStringArray()
        {
            bool retVal = nt.PutStringArray("emptyStringArray", new string[0]);
            Assert.IsTrue(retVal);
        }
        [Test]
        public void TestPutNullBooleanArray()
        {
            Assert.Throws<NullReferenceException>(() =>
            {
                nt.PutBooleanArray("nullBoolArray", null);
            }, "Null array should not be allowed");
        }
        [Test]
        public void TestPutEmptyBooleanArray()
        {
            bool retVal = nt.PutBooleanArray("emptyBoolArray", new bool[0]);
            Assert.IsTrue(retVal);
        }
        [Test]
        public void TestPutNullDoubleArray()
        {
            Assert.Throws<NullReferenceException>(() =>
            {
                nt.PutNumberArray("nullNumberArray", null);
            }, "Null array should not be allowed");
        }
        [Test]
        public void TestPutEmptyDoubleArray()
        {
            bool retVal = nt.PutNumberArray("emptyNumberArray", new double[0]);
            Assert.IsTrue(retVal);
        }
        [Test]
        public void TestInvalidValueSet()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                nt.PutValue("randomValue", nt);
            });
        }
    }
}
