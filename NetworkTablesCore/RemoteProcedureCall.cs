﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NetworkTables.Native;

namespace NetworkTables
{
    internal static class RemoteProcedureCall
    {
        public delegate byte[] RpcCallback(string name, byte[] paramsStr);

        public static byte[] PackRpcDefinition(RpcDefinition def)
        {
            WireEncoder enc = new WireEncoder(0x0300);
            enc.Write8((byte)def.Version);
            enc.WriteString(def.Name);

            int paramsSize = def.Params.Count;
            if (paramsSize > 0xff) paramsSize = 0xff;
            enc.Write8((byte)paramsSize);
            for (int i = 0; i < paramsSize; i++)
            {
                enc.WriteType(def.Params[i].DefValue.Type);
                enc.WriteString(def.Params[i].Name);
                enc.WriteValue(def.Params[i].DefValue);
            }

            int resultsSize = def.Results.Count;
            if (resultsSize > 0xff) resultsSize = 0xff;
            enc.Write8((byte)resultsSize);
            for (int i = 0; i < resultsSize; i++)
            {
                enc.WriteType(def.Results[i].Type);
                enc.WriteString(def.Results[i].Name);
            }
            return enc.Buffer;
        }

        public static bool UnpackRpcDefinition(byte[] packed, ref RpcDefinition def)
        {
            RawMemoryStream iStream = new RawMemoryStream(packed, packed.Length);
            WireDecoder dec = new WireDecoder(iStream, 0x0300);
            byte ref8 = 0;
            ushort ref16 = 0;
            ulong ref32 = 0;
            string str = "";

            if (!dec.Read8(ref ref8)) return false;
            def.Version = ref8;
            if (!dec.ReadString(ref str)) return false;
            def.Name = str;

            int paramsSize = 0;
            if (!dec.Read8(ref ref8)) return false;
            paramsSize = ref8;
            def.Params.Clear();
            NtType type = 0;
            for (int i = 0; i < paramsSize; i++)
            {

                if (!dec.ReadType(ref type)) return false;
                if (!dec.ReadString(ref str)) return false;
                var val = dec.ReadValue(type);
                if (val == null) return false;
                def.Params.Add(new RpcParamDef(str, val));
            }

            int resultsSize = 0;
            if (!dec.Read8(ref ref8)) return false;
            resultsSize = ref8;
            def.Results.Clear();
            for (int i = 0; i < resultsSize; i++)
            {
                type = 0;
                if (!dec.ReadType(ref type)) return false;
                if (!dec.ReadString(ref str)) return false;
                def.Results.Add(new RpcResultsDef(str, type));
            }

            return true;
        }

        public static byte[] PackRpcValues(params Value[] values)
        {
            WireEncoder enc = new WireEncoder(0x0300);
            foreach (var value in values)
            {
                enc.WriteValue(value);
            }
            return enc.Buffer;
        }

        public static byte[] PackRpcValues(List<Value> values)
        {
            WireEncoder enc = new WireEncoder(0x0300);
            foreach (var value in values)
            {
                enc.WriteValue(value);
            }
            return enc.Buffer;
        }

        public static List<Value> UnpackRpcValues(byte[] packed, params NtType[] types)
        {
            RawMemoryStream iStream = new RawMemoryStream(packed, packed.Length);
            WireDecoder dec = new WireDecoder(iStream, 0x0300);
            List<Value> vec = new List<Value>();
            foreach (var ntType in types)
            {
                var item = dec.ReadValue(ntType);
                if (item == null) return new List<Value>();
                vec.Add(item);
            }
            return vec;
        }

        public static List<Value> UnpackRpcValues(byte[] packed, List<NtType> types)
        {
            RawMemoryStream iStream = new RawMemoryStream(packed, packed.Length);
            WireDecoder dec = new WireDecoder(iStream, 0x0300);
            List<Value> vec = new List<Value>();
            foreach (var ntType in types)
            {
                var item = dec.ReadValue(ntType);
                if (item == null) return new List<Value>();
                vec.Add(item);
            }
            return vec;
        }

        private static readonly List<Interop.NT_RPCCallback> s_rpcCallbacks = new List<Interop.NT_RPCCallback>();

        public static void CreateRpc(string name, RpcDefinition def, RpcCallback callback)
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
            byte[] packed = PackRpcDefinition(def);
            UIntPtr nameLen;
            byte[] nameB = CoreMethods.CreateUTF8String(name, out nameLen);
            Interop.NT_CreateRpc(nameB, nameLen, packed, (UIntPtr)packed.Length, IntPtr.Zero, modCallback);
            s_rpcCallbacks.Add(modCallback);
        }

        public static void CreateRpc(string name, byte[] def, RpcCallback callback)
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
            UIntPtr nameLen;
            byte[] nameB = CoreMethods.CreateUTF8String(name, out nameLen);
            Interop.NT_CreateRpc(nameB, nameLen, def, (UIntPtr)def.Length, IntPtr.Zero, modCallback);
            s_rpcCallbacks.Add(modCallback);
        }

        public static void CreatePolledPrc(string name, RpcDefinition def)
        {
            byte[] packed = PackRpcDefinition(def);
            UIntPtr nameLen;
            byte[] nameB = CoreMethods.CreateUTF8String(name, out nameLen);
            Interop.NT_CreatePolledRpc(nameB, nameLen, packed, (UIntPtr)packed.Length);
        }

        public static void CreatePolledPrc(string name, byte[] def)
        {
            byte[] packed = def;
            UIntPtr nameLen;
            byte[] nameB = CoreMethods.CreateUTF8String(name, out nameLen);
            Interop.NT_CreatePolledRpc(nameB, nameLen, packed, (UIntPtr)packed.Length);
        }

        public static bool PollRpc(bool blocking, ref RpcCallInfo info)
        {
            NtRpcCallInfo nativeInfo = new NtRpcCallInfo();
            int retVal = Interop.NT_PollRpc(blocking ? 1 : 0, ref nativeInfo);
            info = nativeInfo.ToManaged();
            return retVal != 0;
        }

        public static bool GetRpcResult(bool blocking, long callUid, ref byte[] result)
        {
            UIntPtr size = UIntPtr.Zero;
            IntPtr retVal = Interop.NT_GetRpcResult(blocking ? 1 : 0, (uint)callUid, ref size);
            if (retVal == IntPtr.Zero)
            {
                return false;
            }
            result = CoreMethods.GetRawDataFromPtr(retVal, size);
            return true;
        }

        public static void PostRpcResonse(long rpcId, long callUid, params byte[] result)
        {
            Interop.NT_PostRpcResponse((uint)rpcId, (uint)callUid, result, (UIntPtr)result.Length);
        }

        public static long CallRpc(string name, params Value[] rpcValues)
        {
            UIntPtr size;
            byte[] nameB = CoreMethods.CreateUTF8String(name, out size);
            var vals = PackRpcValues(rpcValues);
            return Interop.NT_CallRpc(nameB, size, vals, (UIntPtr)vals.Length);
        }

        public static long CallRpc(string name, params byte[] rpcValues)
        {
            UIntPtr size;
            byte[] nameB = CoreMethods.CreateUTF8String(name, out size);
            return Interop.NT_CallRpc(nameB, size, rpcValues, (UIntPtr)rpcValues.Length);
        }

    }
}
