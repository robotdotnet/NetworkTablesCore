using System;
using System.Collections.Generic;
using System.IO;
using NetworkTables;
using NetworkTables.Native;
using NetworkTables.Native.Exceptions;
using NetworkTables.Tables;
using NUnit.Framework;

namespace NetworkTablesCore.Test
{
    [TestFixture]
    [Category("Client")]
    public class TestNetworkTablesApi : ClientTestBase
    {
        [TestFixtureSetUp]
        public void FixtureSetup()
        {

        }

        private NetworkTable m_table;

        [SetUp]
        public void TestSetup()
        {
            CoreMethods.DeleteAllEntries();
            m_table = NetworkTable.GetTable("Table");
        }

        [Test]
        public void TestToString()
        {
            Assert.That(m_table.ToString(), Is.EqualTo("NetworkTable: /Table"));
        }

        [Test]
        public void TestContainsSubTableFalse()
        {
            Assert.That(!m_table.ContainsSubTable("SubTable"));
        }

        [Test]
        public void TestContainsSubTableTrue()
        {
            string subTableName = "SubTable";

            ITable subTable = m_table.GetSubTable(subTableName);
            subTable.PutBoolean("MyKey", true);

            Assert.That(m_table.ContainsSubTable(subTableName));
        }

        [Test]
        public void TestGetKeysAll()
        {
            string key1 = "FirstKey";
            string key2 = "SecondKey";

            m_table.PutString(key1, "String1");
            m_table.PutString(key2, "String2");

            HashSet<string> keys = m_table.GetKeys();

            Assert.That(keys.Contains(key1));
            Assert.That(keys.Contains(key2));
        }

        [Test]
        public void TestGetKeysOnlyPersistent()
        {
            string key1 = "FirstKey";
            string key2 = "SecondKey";

            m_table.PutString(key1, "String1");
            m_table.PutNumber(key2, 3.586);
            m_table.SetPersistent(key2);

            HashSet<string> keys = m_table.GetKeys(NtType.Double);

            Assert.That(!keys.Contains(key1));
            Assert.That(keys.Contains(key2));
        }

        [Test]
        public void TestGetSubTables()
        {
            string subTableName = "SubTable";

            ITable subTable = m_table.GetSubTable(subTableName);
            subTable.PutBoolean("MyKey", true);

            string subTableName2 = "SubTable2";

            ITable subTable2 = m_table.GetSubTable(subTableName2);
            subTable2.PutBoolean("MyKey2", true);

            HashSet<string> subTables = m_table.GetSubTables();

            Assert.That(subTables.Contains(subTableName));
            Assert.That(subTables.Contains(subTableName2));
        }

        [Test]
        public void TestSetPersistent()
        {
            string key2 = "SecondKey";

            m_table.PutString(key2, "String2");
            m_table.SetPersistent(key2);

            Assert.That(m_table.IsPersistent(key2));
        }

        [Test]
        public void TestClearPersistent()
        {
            string key2 = "SecondKey";

            m_table.PutString(key2, "String2");
            m_table.SetPersistent(key2);

            Assert.That(m_table.IsPersistent(key2));

            m_table.ClearPersistent(key2);

            Assert.That(!m_table.IsPersistent(key2));
        }

        [Test]
        public void TestSetFlags()
        {
            string key2 = "SecondKey";

            m_table.PutString(key2, "String2");
            m_table.SetFlags(key2, EntryFlags.Persistent);

            Assert.That(m_table.GetFlags(key2), Is.EqualTo(EntryFlags.Persistent));
        }

        [Test]
        public void TestClearFlags()
        {
            string key2 = "SecondKey";

            m_table.PutString(key2, "String2");
            m_table.SetFlags(key2, EntryFlags.Persistent);

            Assert.That(m_table.GetFlags(key2), Is.EqualTo(EntryFlags.Persistent));

            m_table.ClearFlags(key2, EntryFlags.Persistent);

            Assert.That(m_table.GetFlags(key2), Is.Not.EqualTo(EntryFlags.Persistent));
        }

        [Test]
        public void TestDeleteKey()
        {
            string key = "MyKey";

            m_table.PutString(key, "Value");

            Assert.That(m_table.GetString(key), Is.EqualTo("Value"));

            m_table.Delete(key);

            Assert.Throws<TableKeyNotDefinedException>(() => m_table.GetString(key));
        }

        [Test]
        public void TestDeleteAllEntries()
        {
            m_table.PutString("Test1", "val1");
            m_table.PutString("Test2", "val2");
            m_table.PutString("Test3", "val3");

            Assert.That(m_table.GetKeys().Count, Is.EqualTo(3));

            NetworkTable.GlobalDeleteAll();

            Assert.That(m_table.GetKeys().Count, Is.EqualTo(0));
        }

        [Test]
        public void TestSavePersistent()
        {
            string key1 = "key1";
            string toWrite1 = "val1";
            m_table.PutString(key1, toWrite1);
            m_table.SetFlags(key1, EntryFlags.Persistent);

            string key2 = "key2";
            m_table.PutBoolean(key2, true);
            m_table.SetFlags(key2, EntryFlags.Persistent);

            string fileName = "testfile.txt";
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            NetworkTable.SavePersistent(fileName);

            string[] lines = File.ReadAllLines(fileName);

            Assert.That(lines.Length, Is.GreaterThanOrEqualTo(3));
            Assert.That(lines[0], Contains.Substring("[NetworkTables Storage 3.0]"));
            Assert.That(lines[1], Contains.Substring($"string \"/Table/{key1}\"=\"{toWrite1}\""));
            Assert.That(lines[2], Contains.Substring($"boolean \"/Table/{key2}\"=true"));
        }

        [Test]
        public void TestLoadPersistent()
        {
            const string key1 = "key1";
            const string key2 = "key2";
            const string val1 = "val1";

            string[] toWrite = new[]
            {
                "[NetworkTables Storage 3.0]",
                $"string \"/Table/{key1}\"=\"{val1}\"",
                $"boolean \"/Table/{key2}\"=true",
                ""
            };

            string fileName = "testfile.txt";
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            File.WriteAllLines(fileName, toWrite);

            string[] errors = NetworkTable.LoadPersistent(fileName);

            Assert.That(errors.Length, Is.EqualTo(0));

            HashSet<string> entries = m_table.GetKeys();

            Assert.That(entries.Count, Is.EqualTo(2));

            Assert.That(m_table.GetString(key1, ""), Is.EqualTo(val1));
            Assert.That(m_table.GetBoolean(key2, false), Is.EqualTo(true));
        }

        [Test]
        public void TestPersistentLoadError()
        {
            const string key1 = "key1";
            const string key2 = "key2";
            const string val1 = "val1";

            string[] toWrite = new[]
            {
                "[NetworkTables Storage 3.0]",
                $"string \"/Table/{key1}\"=\"{val1}\"",
                $"boolean \"/Table/{key2}\"=invalid",
                ""
            };

            string fileName = "testfile.txt";
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            File.WriteAllLines(fileName, toWrite);

            string[] errors = NetworkTable.LoadPersistent(fileName);

            Assert.That(errors.Length, Is.EqualTo(1));
            Assert.That(errors[0], Contains.Substring("3: unrecognized boolean value, not 'true' or 'false'"));
        }

        [Test]
        [TestCase(3.56, Result = 3.56)]
        [TestCase(true, Result = true)]
        [TestCase("Hello", Result = "Hello")]
        [TestCase(new byte[] { 0, 5, 12, 0 }, Result = new byte[] { 0, 5, 12, 0 })]
        [TestCase(new double[] { 3.56, 5547.545 }, Result = new double[] { 3.56, 5547.545 })]
        [TestCase(new bool[] { false, true, true }, Result = new bool[] { false, true, true })]
        public object TestGetValue(object value)
        {
            string key = "key";
            if (value is double)
            {
                m_table.PutNumber(key, (double)value);
            }
            else if (value is string)
            {
                m_table.PutString(key, (string)value);
            }
            else if (value is bool)
            {
                m_table.PutBoolean(key, (bool)value);
            }
            else if (value is byte[])
            {
                m_table.PutValue(key, (byte[])value);
            }
            else if (value is double[])
            {
                m_table.PutNumberArray(key, (double[])value);
            }
            else if (value is bool[])
            {
                m_table.PutBooleanArray(key, (bool[])value);
            }
            else if (value is string[])
            {
                m_table.PutStringArray(key, (string[])value);
            }
            else
            {
                throw new ArgumentException("Value is either null or an invalid type.");
            }
            return m_table.GetValue(key);
        }

        [Test]
        public void TestGetValueStringArray()
        {
            string key = "key";
            string[] array = new[] { "Hello1", "Hello2" };

            m_table.PutStringArray(key, array);

            Assert.That(m_table.GetValue(key), Is.EquivalentTo(array));
        }

        [Test]
        public void TestGetValueInvalid()
        {
            Assert.Throws<TableKeyNotDefinedException>(() => m_table.GetValue("key"));
        }



        [Test]
        [TestCase(3.56, Result = 3.56)]
        [TestCase(true, Result = true)]
        [TestCase("Hello", Result = "Hello")]
        [TestCase(new byte[] { 0, 5, 12, 0 }, Result = new byte[] { 0, 5, 12, 0 })]
        [TestCase(new double[] { 3.56, 5547.545 }, Result = new double[] { 3.56, 5547.545 })]
        [TestCase(new bool[] { false, true, true }, Result = new bool[] { false, true, true })]
        public object TestGetValueDefault(object value)
        {
            string key = "key";
            if (value is double)
            {
                m_table.PutNumber(key, (double)value);
            }
            else if (value is string)
            {
                m_table.PutString(key, (string)value);
            }
            else if (value is bool)
            {
                m_table.PutBoolean(key, (bool)value);
            }
            else if (value is byte[])
            {
                m_table.PutValue(key, (byte[])value);
            }
            else if (value is double[])
            {
                m_table.PutNumberArray(key, (double[])value);
            }
            else if (value is bool[])
            {
                m_table.PutBooleanArray(key, (bool[])value);
            }
            else if (value is string[])
            {
                m_table.PutStringArray(key, (string[])value);
            }
            else
            {
                throw new ArgumentException("Value is either null or an invalid type.");
            }
            return m_table.GetValue(key, null);
        }

        [Test]
        public void TestGetValueStringArrayDefault()
        {
            string key = "key";
            string[] array = new[] { "Hello1", "Hello2" };

            m_table.PutStringArray(key, array);

            Assert.That(m_table.GetValue(key, null), Is.EquivalentTo(array));
        }

        [Test]
        public void TestGetValueInvalidDefault()
        {
            Assert.That(m_table.GetValue("key", "value"), Is.EqualTo("value"));
        }

        [Test]
        [TestCase(3.56, Result = 3.56)]
        [TestCase(true, Result = true)]
        [TestCase("Hello", Result = "Hello")]
        [TestCase(new byte[] {0, 5, 12, 0}, Result = new byte[] {0, 5, 12, 0})]
        [TestCase(new double[] {3.56, 5547.545}, Result = new double[] {3.56, 5547.545})]
        [TestCase(new bool[] {false, true, true}, Result = new bool[] {false, true, true})]
        public object TestPutValue(object value)
        {
            string key = "key";
            m_table.PutValue(key, value);
            return m_table.GetValue(key);
        }

        [Test]
        public void TestPutValueStringArray()
        {
            string key = "key";
            string[] array = new[] { "Hello1", "Hello2" };

            m_table.PutValue(key, array);

            Assert.That(m_table.GetValue(key), Is.EquivalentTo(array));
        }

        [Test]
        public void TestInvalidPutValue()
        {
            Assert.Throws<ArgumentException>(() => m_table.PutValue("key", this));
        }



    }
}
