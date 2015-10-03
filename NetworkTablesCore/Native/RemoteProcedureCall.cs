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
            RpcEncoder enc = new RpcEncoder();
            foreach (var value in values)
            {
                enc.WriteValue(value);
            }
            return enc.Buffer;
        }

        public static List<RpcValue> UnpackRpcValues(byte[] packed, params NtType[] types)
        {
            RpcDecoder dec = new RpcDecoder(packed);

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

        public static void CreateRpc(string name, NT_RpcDefinition def, RpcCallback callback)
        {
            Interop.NT_RPCCallback modCallback =
                (IntPtr data, IntPtr ptr, UIntPtr len, IntPtr intPtr, UIntPtr paramsLen, ref UIntPtr resultsLen) =>
                {
                    string retName = CoreMethods.ReadUTF8String(ptr, len);
                    byte[] param = CoreMethods.ReadUTF8StringToByteArray(intPtr, paramsLen);
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

        public static void CreatePolledPrc(string name, NT_RpcDefinition def)
        {
            UIntPtr packedLen;
            byte[] packed = PackRpcDefinition(def, out packedLen);
            UIntPtr nameLen;
            byte[] nameB = CoreMethods.CreateUTF8String(name, out nameLen);
            Interop.NT_CreatePolledRpc(nameB, nameLen, packed, packedLen);
        }

        public static bool PollRpc(bool blocking, ref NT_RpcCallInfo info)
        {
            int retVal = Interop.NT_PollRpc(blocking ? 1 : 0, ref info);
            return retVal != 0;
        }

        public static byte[] PackRpcDefinition(NT_RpcDefinition def, out UIntPtr packedLen)
        {
            RpcEncoder enc = new RpcEncoder();
            enc.Write8((byte)def.version);
            enc.WriteString(def.name);

            int paramsSize = def.paramsArray.Length;
            if (paramsSize > 0xff) paramsSize = 0xff;
            enc.Write8((byte)paramsSize);
            for (int i = 0; i < paramsSize; ++i)
            {
                enc.WriteType(def.paramsArray[i].value.Type);
                enc.WriteString(def.paramsArray[i].name);
                enc.WriteValue(def.paramsArray[i].value);
            }

            int resultsSize = def.resultsArray.Length;
            if (resultsSize > 0xff) resultsSize = 0xff;
            enc.Write8((byte)resultsSize);
            for (int i = 0; i < resultsSize; ++i)
            {
                enc.WriteType(def.resultsArray[i].type);
                enc.WriteString(def.resultsArray[i].name);
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
            return CoreMethods.ReadUTF8StringToByteArray(retVal, size);
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
