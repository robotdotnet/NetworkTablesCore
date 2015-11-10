using NetworkTables.Native;
using NUnit.Framework;

namespace NetworkTablesCore.Test
{
    [TestFixture]
    [Category("Client")]
    public class TestNativeSetters : ClientTestBase
    {
        [Test]
        public void TestMultipleStringArraySet()
        {
            string key = "MyKey";

            string[] firstWrite = new[]
            {
                "FirstString",
                "SecondString",
                "ThirdString"
            };
            CoreMethods.SetEntryStringArray(key, firstWrite);


            string[] secondWrite = new[]
            {
                "FirstNewString",
                "SecondNewString",
                "ThirdNewString"
            };
            CoreMethods.SetEntryStringArray(key, secondWrite);
        }
    }
}
