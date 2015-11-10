using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetworkTables;
using NetworkTables.Native;
using NUnit.Framework;

namespace NetworkTablesCore.Test
{
    [TestFixture]
    [Category("Server")]
    public class TestEntryListener : ServerTestBase
    {
        [SetUp]
        public void SetUp()
        {
            CoreMethods.DeleteAllEntries();
        }

        [Test]
        public void TestAddEntryListener()
        {
            string key1 = "testKey";
            string toWrite1 = "written";
            CoreMethods.SetEntryString(key1, toWrite1);

            int count = 0;
            string recievedKey = "";
            object recievedValue = null;
            NotifyFlags receivedFlags = 0;

            NotifyFlags f = NotifyFlags.NotifyNew | NotifyFlags.NotifyUpdate;
            if (true)
                f |= NotifyFlags.NotifyLocal;

            int listener = CoreMethods.AddEntryListener(key1, (uid, key, value, flags) =>
            {
                count++;
                recievedKey = key;
                recievedValue = value;
                receivedFlags = flags;
            }, f);

            string toWrite2 = "NewNumber";
            CoreMethods.SetEntryString(key1, toWrite2);

            Thread.Sleep(20);

            Assert.That(count, Is.EqualTo(1));
            Assert.That(recievedKey, Is.EqualTo(key1));
            Assert.That(recievedValue, Is.Not.Null);

            string retValue = recievedValue as string;
            Assert.That(retValue, Is.Not.Null);
            Assert.That(retValue, Is.EqualTo(toWrite2));

            Assert.That(receivedFlags.HasFlag(NotifyFlags.NotifyLocal));

            CoreMethods.RemoveEntryListener(listener);
        }

        [Test]
        public void TestAddEntryListenerImmediateNotify()
        {
            string key1 = "testKey";
            string toWrite1 = "written";
            CoreMethods.SetEntryString(key1, toWrite1);

            int count = 0;
            string recievedKey = "";
            object recievedValue = null;
            NotifyFlags receivedFlags = 0;

            NotifyFlags f = NotifyFlags.NotifyNew | NotifyFlags.NotifyUpdate;
            if (true)
                f |= NotifyFlags.NotifyImmediate;

            int listener = CoreMethods.AddEntryListener(key1, (uid, key, value, flags) =>
            {
                count++;
                recievedKey = key;
                recievedValue = value;
                receivedFlags = flags;
            }, f);

            Thread.Sleep(20);

            Assert.That(count, Is.EqualTo(1));
            Assert.That(recievedKey, Is.EqualTo(key1));
            Assert.That(recievedValue, Is.Not.Null);

            string retValue = recievedValue as string;
            Assert.That(retValue, Is.Not.Null);
            Assert.That(retValue, Is.EqualTo(toWrite1));

            Assert.That(receivedFlags.HasFlag(NotifyFlags.NotifyImmediate));

            CoreMethods.RemoveEntryListener(listener);
        }

        [TestCase(true)]
        [TestCase(3.5)]
        [TestCase("MyString")]
        public void TestAddEntryListenerDifferentRawTypes(object val)
        {
            string key1 = "testKey";

            if (val is double) CoreMethods.SetEntryDouble(key1, (double)val);
            else if (val is bool)
                CoreMethods.SetEntryBoolean(key1, (bool)val);
            else if (val is string) CoreMethods.SetEntryString(key1, (string)val);

            int count = 0;
            string recievedKey = "";
            object recievedValue = null;
            NotifyFlags receivedFlags = 0;

            NotifyFlags f = NotifyFlags.NotifyNew | NotifyFlags.NotifyUpdate;
            if (true)
                f |= NotifyFlags.NotifyImmediate;

            int listener = CoreMethods.AddEntryListener(key1, (uid, key, value, flags) =>
            {
                count++;
                recievedKey = key;
                recievedValue = value;
                receivedFlags = flags;
            }, f);

            Thread.Sleep(20);

            Assert.That(count, Is.EqualTo(1));
            Assert.That(recievedKey, Is.EqualTo(key1));
            Assert.That(recievedValue, Is.Not.Null);

            if (val is double)
            {
                double retValue = (double)recievedValue;
                Assert.That(retValue, Is.EqualTo((double)val));
                Assert.That(receivedFlags.HasFlag(NotifyFlags.NotifyImmediate));
            }
            else if (val is bool)
            {
                bool retValue = (bool) recievedValue;
                Assert.That(retValue, Is.EqualTo((bool)val));

                Assert.That(receivedFlags.HasFlag(NotifyFlags.NotifyImmediate));
            }
            else if (val is string)
            {
                string retValue = recievedValue as string;
                Assert.That(retValue, Is.Not.Null);
                Assert.That(retValue, Is.EqualTo((string)val));

                Assert.That(receivedFlags.HasFlag(NotifyFlags.NotifyImmediate));
            }
            else
            {
                CoreMethods.RemoveEntryListener(listener);
                Assert.Fail("Unknown type");
                return;
            }

            CoreMethods.RemoveEntryListener(listener);
        }

        [Test]
        public void TestEntryListenerStringArray()
        {
            string key1 = "testKey";
            string[] toWrite1 = { "write1", "write2", "write3"};
            CoreMethods.SetEntryStringArray(key1, toWrite1);

            int count = 0;
            string recievedKey = "";
            object recievedValue = null;
            NotifyFlags receivedFlags = 0;

            NotifyFlags f = NotifyFlags.NotifyNew | NotifyFlags.NotifyUpdate;
            if (true)
                f |= NotifyFlags.NotifyImmediate;

            int listener = CoreMethods.AddEntryListener(key1, (uid, key, value, flags) =>
            {
                count++;
                recievedKey = key;
                recievedValue = value;
                receivedFlags = flags;
            }, f);

            Thread.Sleep(300);

            Assert.That(count, Is.EqualTo(1));
            Assert.That(recievedKey, Is.EqualTo(key1));
            Assert.That(recievedValue, Is.Not.Null);

            string[] retValue = recievedValue as string[];
            Assert.That(retValue, Is.Not.Null);

            for (int i = 0; i < retValue.Length; i++)
            {
                Assert.That(retValue[i], Is.EqualTo(toWrite1[i]));
            }
            //Assert.That(retValue, Is.EqualTo(toWrite1));

            Assert.That(receivedFlags.HasFlag(NotifyFlags.NotifyImmediate));

            CoreMethods.RemoveEntryListener(listener);
        }

        [Test]
        public void TestEntryListenerDoubleArray()
        {
            string key1 = "testKey";
            double[] toWrite1 = { 3.58, 6.825, 454.54};
            CoreMethods.SetEntryDoubleArray(key1, toWrite1);

            int count = 0;
            string recievedKey = "";
            object recievedValue = null;
            NotifyFlags receivedFlags = 0;

            NotifyFlags f = NotifyFlags.NotifyNew | NotifyFlags.NotifyUpdate;
            if (true)
                f |= NotifyFlags.NotifyImmediate;

            int listener = CoreMethods.AddEntryListener(key1, (uid, key, value, flags) =>
            {
                count++;
                recievedKey = key;
                recievedValue = value;
                receivedFlags = flags;
            }, f);

            Thread.Sleep(300);

            Assert.That(count, Is.EqualTo(1));
            Assert.That(recievedKey, Is.EqualTo(key1));
            Assert.That(recievedValue, Is.Not.Null);

            double[] retValue = recievedValue as double[];
            Assert.That(retValue, Is.Not.Null);

            for (int i = 0; i < retValue.Length; i++)
            {
                Assert.That(retValue[i], Is.EqualTo(toWrite1[i]));
            }
            //Assert.That(retValue, Is.EqualTo(toWrite1));

            Assert.That(receivedFlags.HasFlag(NotifyFlags.NotifyImmediate));

            CoreMethods.RemoveEntryListener(listener);
        }

        [Test]
        public void TestEntryListenerBooleanArray()
        {
            string key1 = "testKey";
            bool[] toWrite1 = { true, true, true };
            CoreMethods.SetEntryBooleanArray(key1, toWrite1);

            int count = 0;
            string recievedKey = "";
            object recievedValue = null;
            NotifyFlags receivedFlags = 0;

            NotifyFlags f = NotifyFlags.NotifyNew | NotifyFlags.NotifyUpdate;
            if (true)
                f |= NotifyFlags.NotifyImmediate;

            int listener = CoreMethods.AddEntryListener(key1, (uid, key, value, flags) =>
            {
                count++;
                recievedKey = key;
                recievedValue = value;
                receivedFlags = flags;
            }, f);

            Thread.Sleep(300);

            Assert.That(count, Is.EqualTo(1));
            Assert.That(recievedKey, Is.EqualTo(key1));
            Assert.That(recievedValue, Is.Not.Null);

            bool[] retValue = recievedValue as bool[];
            Assert.That(retValue, Is.Not.Null);

            for (int i = 0; i < retValue.Length; i++)
            {
                Assert.That(retValue[i], Is.EqualTo(toWrite1[i]));
            }
            //Assert.That(retValue, Is.EqualTo(toWrite1));

            Assert.That(receivedFlags.HasFlag(NotifyFlags.NotifyImmediate));

            CoreMethods.RemoveEntryListener(listener);
        }
    }
}
