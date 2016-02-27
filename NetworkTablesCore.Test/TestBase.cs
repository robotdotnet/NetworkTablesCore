using System;
using System.IO;
using NetworkTables;
using NetworkTables.Native;
using NUnit.Framework;

namespace NetworkTablesCore.Test
{
    public class TestBase
    {
        public static bool s_started = false;

        [TestFixtureSetUp]
        public void ClassSetUp()
        {

            CoreMethods.SetLogger(((level, file, line, msg) =>
            {
                if (level == (int)LogLevel.LogInfo)
                {
                    Console.Error.WriteLine($"NT: {msg}");
                }

                string levelmsg = "";
                if (level >= (int)LogLevel.LogCritical)
                    levelmsg = "CRITICAL";
                else if (level >= (int)LogLevel.LogError)
                    levelmsg = "ERROR";
                else if (level >= (int)LogLevel.LogWarning)
                    levelmsg = "WARNING";
                else
                    return;
                string fname = Path.GetFileName(file);
                Console.Error.WriteLine($"NT: {levelmsg}: {msg} ({fname}:{line})");
            }), 0);
        }
    }
}
