using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkTables;
using NetworkTables.Native;
using NetworkTables.Native.Exceptions;
using NUnit.Framework;

namespace NetworkTablesCore.Test
{
    [TestFixture]
    [Category("Client")]
    public class TestNativeGettersNoDefaultParameter : ClientTestBase
    {
        [SetUp]
        public void SetUp()
        {
            CoreMethods.DeleteAllEntries();
        }

        [Test]
        public void TestGetStringNonExistant()
        {
            string key = "mykey";
            string testVal = "Hello Test";
            Assert.Throws<TableKeyNotDefinedException>(() => CoreMethods.GetEntryString(key));
        }

        [Test]
        public void TestGetBooleanNonExistant()
        {
            string key = "mykey";
            bool testVal = true;
            Assert.Throws<TableKeyNotDefinedException>(() =>CoreMethods.GetEntryBoolean(key));
        }

        [Test]
        public void TestGetDoubleNonExistant()
        {
            string key = "mykey";
            double testVal = 5.86;
            Assert.Throws<TableKeyNotDefinedException>(() =>CoreMethods.GetEntryDouble(key));
        }

        [Test]
        public void TestGetRawNonExistant()
        {
            string key = "mykey";
            byte[] testVal = { 56, 88, 92, 0, 0, 1 };
            Assert.Throws<TableKeyNotDefinedException>(() =>CoreMethods.GetEntryRaw(key));
        }

        [Test]
        public void TestGetDoubleArrayNonExistant()
        {
            string key = "mykey";
            double[] testVal = { 3.58, 6.825, 454.54 };
            Assert.Throws<TableKeyNotDefinedException>(() =>CoreMethods.GetEntryDoubleArray(key));
        }

        [Test]
        public void TestGetStringArrayNonExistant()
        {
            string key = "mykey";
            string[] testVal = { "write1", "write2", "write3" };
            Assert.Throws<TableKeyNotDefinedException>(() =>CoreMethods.GetEntryStringArray(key));
        }

        [Test]
        public void TestGetBooleanArrayNonExistant()
        {
            string key = "mykey";
            bool[] testVal = { true, false, true };
            Assert.Throws<TableKeyNotDefinedException>(() =>CoreMethods.GetEntryBooleanArray(key));
        }



        [Test]
        public void TestGetStringWrongType()
        {
            string key = "mykey";
            string testVal = "Hello Test";
            CoreMethods.SetEntryBoolean(key, true);

            Assert.Throws<TableKeyNotDefinedException>(() =>CoreMethods.GetEntryString(key));
        }

        [Test]
        public void TestGetBooleanWrongType()
        {
            string key = "mykey";
            bool testVal = true;
            CoreMethods.SetEntryDouble(key, 5.86);

            Assert.Throws<TableKeyNotDefinedException>(() =>CoreMethods.GetEntryBoolean(key));
        }

        [Test]
        public void TestGetDoubleWrongType()
        {
            string key = "mykey";
            double testVal = 5.86;
            CoreMethods.SetEntryBoolean(key, true);

            Assert.Throws<TableKeyNotDefinedException>(() =>CoreMethods.GetEntryDouble(key));
        }

        [Test]
        public void TestGetRawWrongType()
        {
            string key = "mykey";
            byte[] testVal = { 56, 88, 92, 0, 0, 1 };
            CoreMethods.SetEntryBoolean(key, true);

            Assert.Throws<TableKeyNotDefinedException>(() =>CoreMethods.GetEntryRaw(key));
        }

        [Test]
        public void TestGetDoubleArrayWrongType()
        {
            string key = "mykey";
            double[] testVal = { 3.58, 6.825, 454.54 };
            CoreMethods.SetEntryBoolean(key, true);

            Assert.Throws<TableKeyNotDefinedException>(() =>CoreMethods.GetEntryDoubleArray(key));
        }

        [Test]
        public void TestGetStringArrayWrongType()
        {
            string key = "mykey";
            string[] testVal = { "write1", "write2", "write3" };
            CoreMethods.SetEntryBoolean(key, true);

            Assert.Throws<TableKeyNotDefinedException>(() =>CoreMethods.GetEntryStringArray(key));
        }

        [Test]
        public void TestGetBooleanArrayWrongType()
        {
            string key = "mykey";
            bool[] testVal = { true, false, true };
            CoreMethods.SetEntryBoolean(key, true);

            Assert.Throws<TableKeyNotDefinedException>(() => CoreMethods.GetEntryBooleanArray(key));
        }




        [Test]
        public void TestGetStringValid()
        {
            string key = "mykey";
            string testVal = "Hello Test";
            string actualVal = "Im Valid";
            CoreMethods.SetEntryString(key, actualVal);

            Assert.That(CoreMethods.GetEntryString(key), Is.EqualTo(actualVal));
        }

        [Test]
        public void TestGetBooleanValid()
        {
            string key = "mykey";
            bool testVal = true;
            bool actualVal = false;
            CoreMethods.SetEntryBoolean(key, actualVal);

            Assert.That(CoreMethods.GetEntryBoolean(key), Is.EqualTo(actualVal));
        }

        [Test]
        public void TestGetDoubleValid()
        {
            string key = "mykey";
            double testVal = 5.86;
            double actualVal = 1213.323;
            CoreMethods.SetEntryDouble(key, actualVal);

            Assert.That(CoreMethods.GetEntryDouble(key), Is.EqualTo(actualVal));
        }

        [Test]
        public void TestGetRawValid()
        {
            string key = "mykey";
            byte[] testVal = { 56, 88, 92, 0, 0, 1 };

            byte[] actualVal = { 42, 28, 142, 0, 22, 0, 132 };
            CoreMethods.SetEntryRaw(key, actualVal);

            byte[] retVal = CoreMethods.GetEntryRaw(key);

            Assert.That(retVal.Length, Is.EqualTo(actualVal.Length));

            for (int i = 0; i < actualVal.Length; i++)
            {
                Assert.That(retVal[i], Is.EqualTo(actualVal[i]));
            }
        }

        [Test]
        public void TestGetDoubleArrayValid()
        {
            string key = "mykey";
            double[] testVal = { 3.58, 6.825, 454.54 };

            double[] actualVal = { 3.54538, 43536.825, 34534454.34554, 3423413.23432 };
            CoreMethods.SetEntryDoubleArray(key, actualVal);

            double[] retVal = CoreMethods.GetEntryDoubleArray(key);

            Assert.That(retVal.Length, Is.EqualTo(actualVal.Length));

            for (int i = 0; i < actualVal.Length; i++)
            {
                Assert.That(retVal[i], Is.EqualTo(actualVal[i]));
            }
        }

        [Test]
        public void TestGetStringArrayValid()
        {
            string key = "mykey";
            string[] testVal = { "write1", "write2", "write3" };

            string[] actualVal = { "writeas1", "wriasdte2", "writasde3", "adsadsd4" };
            CoreMethods.SetEntryStringArray(key, actualVal);

            string[] retVal = CoreMethods.GetEntryStringArray(key);

            Assert.That(retVal.Length, Is.EqualTo(actualVal.Length));

            for (int i = 0; i < actualVal.Length; i++)
            {
                Assert.That(retVal[i], Is.EqualTo(actualVal[i]));
            }
        }

        [Test]
        public void TestGetBooleanArrayValid()
        {
            string key = "mykey";
            bool[] testVal = { true, false, true };

            bool[] actualVal = { false, false, true, true, false };
            CoreMethods.SetEntryBooleanArray(key, actualVal);

            bool[] retVal = CoreMethods.GetEntryBooleanArray(key);

            Assert.That(retVal.Length, Is.EqualTo(actualVal.Length));

            for (int i = 0; i < actualVal.Length; i++)
            {
                Assert.That(retVal[i], Is.EqualTo(actualVal[i]));
            }
        }
    }
}
