using NetworkTables;
using NetworkTables.Native.Exceptions;
using NUnit.Framework;

namespace NetworkTablesCore.Test
{
    public class TestEntries
    {
        public void InitClass()
        {
            NetworkTable.Shutdown();
            NetworkTable.SetServerMode();
            nt = NetworkTable.GetTable("nt");
        }

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
