using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NetworkTables.Native.Rpc;

namespace NetworkTables.Native
{
    internal static class RemoteProcedureCall
    {
        public delegate byte[] RpcCallback(string name, byte[] paramsStr);

        public static byte[] PackRpcValues(params RpcValue[] values)
        {
            WireEncoder enc = new WireEncoder();
            foreach (var value in values)
            {
                enc.WriteValue(value);
            }
            return enc.Buffer;
        }

        public static List<RpcValue> UnpackRpcValues(byte[] packed, params NtType[] types)
        {
            WireDecoder dec = new WireDecoder(packed);

            List<RpcValue> values = new List<RpcValue>();
            foreach (var type in types)
            {
                var item = dec.ReadValue(type);
                if (item == null)
                {
                    values.Clear();
                    break;
                }
                values.Add(item);
            }
            return values;
        }

        private static readonly List<Interop.NT_RPCCallback> s_rpcCallbacks = new List<Interop.NT_RPCCallback>();

        public static void CreateRpc(string name, NtRpcDefinition def, RpcCallback callback)
        {
            Interop.NT_RPCCallback modCallback =
                (IntPtr data, IntPtr ptr, UIntPtr len, IntPtr intPtr, UIntPtr paramsLen, out UIntPtr resultsLen) =>
                {
                    string retName = CoreMethods.ReadUTF8String(ptr, len);
                    byte[] param = CoreMethods.GetRawDataFromPtr(intPtr, paramsLen);
                    byte[] cb = callback(retName, param);
                    resultsLen = (UIntPtr)cb.Length;
                    IntPtr retPtr = Interop.NT_AllocateCharArray(resultsLen);
                    Marshal.Copy(cb, 0, retPtr, cb.Length);
                    return retPtr;
                };

            UIntPtr packedLen;
            byte[] packed = PackRpcDefinition(def, out packedLen);
            UIntPtr nameLen;
            byte[] nameB = CoreMethods.CreateUTF8String(name, out nameLen);
            Interop.NT_CreateRpc(nameB, nameLen, packed, packedLen, IntPtr.Zero, modCallback);
            s_rpcCallbacks.Add(modCallback);
        }

        public static void CreatePolledPrc(string name, NtRpcDefinition def)
        {
            UIntPtr packedLen;
            byte[] packed = PackRpcDefinition(def, out packedLen);
            UIntPtr nameLen;
            byte[] nameB = CoreMethods.CreateUTF8String(name, out nameLen);
            Interop.NT_CreatePolledRpc(nameB, nameLen, packed, packedLen);
        }

        public static bool PollRpc(bool blocking, ref NtRpcCallInfo info)
        {
            int retVal = Interop.NT_PollRpc(blocking ? 1 : 0, ref info);
            return retVal != 0;
        }

        public static byte[] PackRpcDefinition(NtRpcDefinition def, out UIntPtr packedLen)
        {
            WireEncoder enc = new WireEncoder();
            enc.Write8((byte)def.Version);
            enc.WriteString(def.Name);

            int paramsSize = def.ParamsArray.Length;
            if (paramsSize > 0xff) paramsSize = 0xff;
            enc.Write8((byte)paramsSize);
            for (int i = 0; i < paramsSize; ++i)
            {
                enc.WriteType(def.ParamsArray[i].Value.Type);
                enc.WriteString(def.ParamsArray[i].Name);
                enc.WriteValue(def.ParamsArray[i].Value);
            }

            int resultsSize = def.ResultsArray.Length;
            if (resultsSize > 0xff) resultsSize = 0xff;
            enc.Write8((byte)resultsSize);
            for (int i = 0; i < resultsSize; ++i)
            {
                enc.WriteType(def.ResultsArray[i].Type);
                enc.WriteString(def.ResultsArray[i].Name);
            }
            packedLen = (UIntPtr)enc.Buffer.Length;
            return enc.Buffer;
        }

        public static byte[] GetRpcResult(bool blocking, uint callUid)
        {
            UIntPtr size = UIntPtr.Zero;
            IntPtr retVal = Interop.NT_GetRpcResult(blocking ? 1 : 0, callUid, ref size);
            if (retVal == IntPtr.Zero)
            {
                return new byte[0];

            }
            return CoreMethods.GetRawDataFromPtr(retVal, size);
        }

        public static uint CallRpc(string name, params RpcValue[] rpcValues)
        {
            UIntPtr size;
            byte[] nameB = CoreMethods.CreateUTF8String(name, out size);
            var vals = PackRpcValues(rpcValues);
            return Interop.NT_CallRpc(nameB, size, vals, (UIntPtr)vals.Length);
        }

    }
}
