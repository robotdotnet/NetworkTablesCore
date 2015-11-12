using System;
using System.Diagnostics;
using NetworkTables.Native;
using NUnit.Framework;

namespace NetworkTablesCore.Test
{
    [TestFixture]
    [Category("Server")]
    public class TestRPC : ServerTestBase
    {
        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            CoreMethods.StopRpcServer();
        }

        private byte[] Callback1(string names, byte[] paramsStr)
        {
            var param = RemoteProcedureCall.UnpackRpcValues(paramsStr, NtType.Double);

            if (param.Count == 0)
            {
                Console.Error.WriteLine("Empty Params?");
                return new byte[] { 0 };
            }
            double val = (double) param[0].Value;
            //Console.WriteLine($"Called with {val}");

            return RemoteProcedureCall.PackRpcValues(RpcValue.MakeDouble(val + 1.2));
        }

        [Test]
        public void TestRpcLocal()
        {
            CoreMethods.SetLogger((level, file, line, message) =>
            {
                //Console.Error.WriteLine(message);
            }, 0);

            var def = new NtRpcDefinition(1, "myfunc1", new[] {new NtRpcParamDef("param1", RpcValue.MakeDouble(0.0))}, new[] {new NtRpcResultDef("result1", NtType.Double)});

            RemoteProcedureCall.CreateRpc("func1", def, Callback1);

            Console.WriteLine("Calling RPC");

            uint call1Uid = RemoteProcedureCall.CallRpc("func1", RpcValue.MakeDouble(2.0));

            Console.WriteLine("Waiting for RPC Result");
            byte[] result = RemoteProcedureCall.GetRpcResult(true, call1Uid);
            var call1Result = RemoteProcedureCall.UnpackRpcValues(result, NtType.Double);
            Assert.AreNotEqual(0, call1Result.Count, "RPC Result empty");

            Console.WriteLine(call1Result[0].ToString());
        }

        [Test]
        public void TestRpcSpeed()
        {
            CoreMethods.SetLogger((level, file, line, message) =>
            {
                //Console.Error.WriteLine(message);
            }, 0);

            var def = new NtRpcDefinition(1, "myfunc1", new[] { new NtRpcParamDef("param1", RpcValue.MakeDouble(0.0)) }, new[] { new NtRpcResultDef("result1", NtType.Double) });

            RemoteProcedureCall.CreateRpc("func1", def, Callback1);

            

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < 10000; ++i)
            {
                uint call1Uid = RemoteProcedureCall.CallRpc("func1", RpcValue.MakeDouble(i));
                byte[] call1Result = RemoteProcedureCall.GetRpcResult(true, call1Uid);
                var res = RemoteProcedureCall.UnpackRpcValues(call1Result, NtType.Double);
                Assert.AreNotEqual(0, res.Count, "RPC Result empty");
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed);

        }
    }


}
