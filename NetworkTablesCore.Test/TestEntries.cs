using NetworkTables;
using NetworkTables.Native.Exceptions;
using NUnit.Framework;

namespace NetworkTablesCore.Test
{
    public class TestEntries
    {
        private NetworkTable nt;

        public void TestDoublePutNullName()
        {
            Assert.Throws<TableKeyNotDefinedException>(() =>
            {
                nt.PutNumber(null, 3.14);
            });
        }
    }
}
