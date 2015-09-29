using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkTables.Native;
using NUnit.Framework;

namespace NetworkTablesCore.Test
{
    [TestFixture]
    public class TestRPC
    {
        public byte[] callback1(string names, byte[] params_str)
        {
            var param = RemoteProcedureCall.UnpackRpcValues(params_str, NT_Type.NT_DOUBLE);

            if (param.Count == 0)
            {
                Console.Error.WriteLine("Empty Params?");
                return new byte[] { 0 };
            }
            double val = (double) param[0].Value;
            //Console.WriteLine($"Called with {val}");

            return RemoteProcedureCall.PackRpcValues(RpcValue.MakeDouble(val + 1.2));
        }
        public void TestRpcLocal()
        {
            CoreMethods.SetLogger(((level, file, line, message) =>
            {
                Console.Error.WriteLine(message);
            }), 0);

            var def = new NT_RpcDefinition(1, "myfunc1", new NT_RpcParamDef[] {new NT_RpcParamDef("param1", RpcValue.MakeDouble(0.0))}, new NT_RpcResultDef[] {new NT_RpcResultDef("result1", NT_Type.NT_DOUBLE)});

            RemoteProcedureCall.CreateRpc("func1", def, callback1);

            Console.WriteLine("Calling RPC");

            uint call1UID = RemoteProcedureCall.CallRpc("func1", RpcValue.MakeDouble(2.0));

            Console.WriteLine("Waiting for RPC Result");
            byte[] result = RemoteProcedureCall.GetRpcResult(true, call1UID);
            var call1Result = RemoteProcedureCall.UnpackRpcValues(result, NT_Type.NT_DOUBLE);
            Assert.AreNotEqual(0, call1Result.Count, "RPC Result empty");

            Console.WriteLine(call1Result[0].ToString());
        }


        public void TestRpcSpeed()
        {
            CoreMethods.SetLogger(((level, file, line, message) =>
            {
                Console.Error.WriteLine(message);
            }), 0);

            var def = new NT_RpcDefinition(1, "myfunc1", new NT_RpcParamDef[] { new NT_RpcParamDef("param1", RpcValue.MakeDouble(0.0)) }, new NT_RpcResultDef[] { new NT_RpcResultDef("result1", NT_Type.NT_DOUBLE) });

            RemoteProcedureCall.CreateRpc("func1", def, callback1);

            

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < 10000; ++i)
            {
                uint call1UID = RemoteProcedureCall.CallRpc("func1", RpcValue.MakeDouble(i));
                byte[] call1Result = RemoteProcedureCall.GetRpcResult(true, call1UID);
                var res = RemoteProcedureCall.UnpackRpcValues(call1Result, NT_Type.NT_DOUBLE);
                Assert.AreNotEqual(0, res.Count, "RPC Result empty");
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed);

        }
    }


}
