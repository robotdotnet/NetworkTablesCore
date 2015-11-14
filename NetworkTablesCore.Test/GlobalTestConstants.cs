using NetworkTables.Native;
using NUnit.Framework;

namespace NetworkTablesCore.Test
{
    [SetUpFixture]
    public class GlobalTestConstants
    {
        internal static bool Started = false;
        internal static bool Server = false;

        [TearDown]
        public void TearDown()
        {
            CoreMethods.StopClient();
            CoreMethods.StopServer();

            CoreMethods.StopNotifier();
            CoreMethods.StopRpcServer();
        }
    }
}
