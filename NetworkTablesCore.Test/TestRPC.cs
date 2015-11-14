using System;
using System.Diagnostics;
using NetworkTables.Native;
using NUnit.Framework;

namespace NetworkTablesCore.Test
{
    [TestFixture]
    public class TestRPC : TestBase
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
            double val = (double)param[0].Value;
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

            var def = new NtRpcDefinition(1, "myfunc1", new[] { new NtRpcParamDef("param1", RpcValue.MakeDouble(0.0)) }, new[] { new NtRpcResultDef("result1", NtType.Double) });

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

















        private byte[] Callback2(string names, byte[] paramsStr)
        {
            var param = RemoteProcedureCall.UnpackRpcValues(paramsStr, NtType.Boolean, NtType.BooleanArray, NtType.Double, NtType.DoubleArray, NtType.Raw, NtType.String, NtType.StringArray);

            if (param.Count == 0)
            {
                Console.Error.WriteLine("Empty Params?");
                return new byte[] { 0 };
            }

            return RemoteProcedureCall.PackRpcValues(RpcValue.MakeBoolean(true), RpcValue.MakeBooleanArray(new[] { true, false }), RpcValue.MakeDouble(2.2), RpcValue.MakeDoubleArray(new[] { 2.8, 6.876 }),
                RpcValue.MakeRaw(new byte[] { 52, 0, 89, 0, 0, 98 }), RpcValue.MakeString("NewString"), RpcValue.MakeStringArray(new[] { "String1", "String2" }));
        }

        [Test]
        public void TestRpcAllTypes()
        {
            CoreMethods.SetLogger((level, file, line, message) =>
            {
                //Console.Error.WriteLine(message);
            }, 0);

            var def = new NtRpcDefinition(1, "myfunc1", new[]
            {
                new NtRpcParamDef("param1", RpcValue.MakeBoolean(true)),
                new NtRpcParamDef("param2", RpcValue.MakeBooleanArray(new[] { true, false })),
                new NtRpcParamDef("param3", RpcValue.MakeDouble(0.0)),
                new NtRpcParamDef("param4", RpcValue.MakeDoubleArray(new[] { 2.8, 6.87 })),
                new NtRpcParamDef("param5", RpcValue.MakeRaw(new byte[] { 52, 0, 89, 0, 0, 98 })),
                new NtRpcParamDef("param6", RpcValue.MakeString("NewString")),
                new NtRpcParamDef("param7", RpcValue.MakeStringArray(new[] { "String1", "String2" })),
            }, new[]
            {
                new NtRpcResultDef("result1", NtType.Boolean),
                new NtRpcResultDef("result2", NtType.BooleanArray),
                new NtRpcResultDef("result3", NtType.Double),
                new NtRpcResultDef("result4", NtType.DoubleArray),
                new NtRpcResultDef("result5", NtType.Raw),
                new NtRpcResultDef("result6", NtType.String),
                new NtRpcResultDef("result7", NtType.StringArray),
            });

            RemoteProcedureCall.CreateRpc("func1", def, Callback2);

            Console.WriteLine("Calling RPC");

            uint call1Uid = RemoteProcedureCall.CallRpc("func1", RpcValue.MakeBoolean(true), RpcValue.MakeBooleanArray(new[] { true, false }), RpcValue.MakeDouble(2.2), RpcValue.MakeDoubleArray(new[] { 2.8, 6.87 }),
                RpcValue.MakeRaw(new byte[] { 52, 0, 89, 0, 0, 98 }), RpcValue.MakeString("NewString"), RpcValue.MakeStringArray(new[] { "String1", "String2" }));

            Console.WriteLine("Waiting for RPC Result");
            byte[] result = RemoteProcedureCall.GetRpcResult(true, call1Uid);
            var call1Result = RemoteProcedureCall.UnpackRpcValues(result, NtType.Boolean, NtType.BooleanArray, NtType.Double, NtType.DoubleArray, NtType.Raw, NtType.String, NtType.StringArray);
            Assert.AreNotEqual(0, call1Result.Count, "RPC Result empty");

            Console.WriteLine(call1Result[0].ToString());
        }
    }


}
