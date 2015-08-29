using System;
using System.Runtime.InteropServices;
using System.Security;

namespace NetworkTablesCore.Native
{
    [SuppressUnmanagedCodeSecurity]
    internal class Interop
    {
        internal const string NTSharedFile = "ntcore";

        //Callback Typedefs
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void NT_EntryListenerCallback(
            uint uid, IntPtr data, IntPtr name, UIntPtr name_len, IntPtr value, int is_new);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void NT_ConnectionListenerCallback(
            uint uid, IntPtr data, int connected, ref NT_ConnectionInfo conn);

        //Table Functions
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_SetEntryFlags(byte[] name, UIntPtr name_len, uint flags);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint NT_GetEntryFlags(byte[] name, UIntPtr name_len);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_DeleteEntry(byte[] name, UIntPtr name_len);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_DeleteAllEntries();
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr NT_GetEntryInfo(byte[] prefix, UIntPtr prefix_len, uint types, ref UIntPtr count);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_Flush();

        //Callback Functions
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint NT_AddEntryListener(byte[] prefix, UIntPtr prefix_len, IntPtr data,
            NT_EntryListenerCallback callback, int immediate_notify);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_RemoveEntryListener(uint entry_listener_uid);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint NT_AddConnectionListener(IntPtr data, NT_ConnectionListenerCallback callback, int immediate_notify);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_RemoveConnectionListener(uint conn_listener_uid);

        //Ignoring RPC for now


        //Client/Server Functions
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_SetNetworkIdentity(byte[] name, UIntPtr name_len);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_StartServer(byte[] persist_filename, byte[] listen_address, uint port);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_StopServer();
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_StartClient(byte[] server_name, uint port);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_StopClient();
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_SetUpdateRate(double interval);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr NT_GetConnections(ref UIntPtr count);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void WarmFunction(UIntPtr line, IntPtr msg);

        //Persistent Functions
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr NT_SavePersistent(byte[] filename);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr NT_LoadPersistent(byte[] filename, WarmFunction warn);

        //Utility Functions
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_DisposeValue(IntPtr value);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_InitValue(IntPtr value);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_DisposeString(ref NT_String_Read str);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern NT_Type NT_GetType(byte[] name, UIntPtr name_len);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_DisposeConnectionInfoArray(IntPtr arr, UIntPtr count);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_DisposeEntryInfoArray(IntPtr arr, UIntPtr count);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong NT_Now();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void NT_LogFunc(uint level, IntPtr file, uint line, IntPtr msg);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_SetLogger(NT_LogFunc funct, uint min_level);

        //Interop Utility Functions
        //[DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        //public static extern NT_String NT_AllocateNTString(UIntPtr size);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr NT_AllocateCharArray(UIntPtr size);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_FreeBooleanArray(IntPtr arr);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_FreeDoubleArray(IntPtr arr);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_FreeCharArray(IntPtr arr);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_FreeStringArray(IntPtr arr, UIntPtr arr_size);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern NT_Type NT_GetValueType(IntPtr value);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NT_GetValueBoolean(IntPtr value, ref ulong last_change, ref int v_boolean);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NT_GetValueDouble(IntPtr value, ref ulong last_change, ref double v_double);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr NT_GetValueString(IntPtr value, ref ulong last_change, ref UIntPtr string_len);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr NT_GetValueRaw(IntPtr value, ref ulong last_change, ref UIntPtr raw_len);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr NT_GetValueBooleanArray(IntPtr value, ref ulong last_change, ref UIntPtr size);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr NT_GetValueDoubleArray(IntPtr value, ref ulong last_change, ref UIntPtr size);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr NT_GetValueStringArray(IntPtr value, ref ulong last_change, ref UIntPtr size);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]

        public static extern int NT_GetEntryBoolean(byte[] name, UIntPtr name_len, ref ulong last_change, ref int v_boolean);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NT_GetEntryDouble(byte[] name, UIntPtr name_len, ref ulong last_change, ref double v_double);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr NT_GetEntryString(byte[] name, UIntPtr name_len, ref ulong last_change, ref UIntPtr string_len);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr NT_GetEntryRaw(byte[] name, UIntPtr name_len, ref ulong last_change, ref UIntPtr raw_len);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr NT_GetEntryBooleanArray(byte[] name, UIntPtr name_len, ref ulong last_change, ref UIntPtr size);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr NT_GetEntryDoubleArray(byte[] name, UIntPtr name_len, ref ulong last_change, ref UIntPtr size);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr NT_GetEntryStringArray(byte[] name, UIntPtr name_len, ref ulong last_change, ref UIntPtr size);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NT_SetEntryBoolean(byte[] name, UIntPtr name_len, int v_boolean, int force);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NT_SetEntryDouble(byte[] name, UIntPtr name_len, double v_double, int force);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NT_SetEntryString(byte[] name, UIntPtr name_len, byte[] v_string, UIntPtr string_len, int force);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NT_SetEntryRaw(byte[] name, UIntPtr name_len, byte[] raw, UIntPtr raw_len, int force);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NT_SetEntryBooleanArray(byte[] name, UIntPtr name_len, int[] arr, UIntPtr size,
            int force);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NT_SetEntryDoubleArray(byte[] name, UIntPtr name_len, double[] arr, UIntPtr size,
            int force);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NT_SetEntryStringArray(byte[] name, UIntPtr name_len, NT_String_Write[] arr, UIntPtr size,
            int force);



        //RPC

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr NT_RPCCallback(
            IntPtr data, IntPtr name, UIntPtr name_len, IntPtr param, UIntPtr params_len, ref UIntPtr results_len);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_CreateRpc(byte[] name, UIntPtr name_len, byte[] def, UIntPtr def_len, IntPtr data,
            NT_RPCCallback callback);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_CreatePolledRpc(byte[] name, UIntPtr name_len, byte[] def, UIntPtr def_len);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NT_PollRpc(int blocking, ref NT_RpcCallInfo call_info);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_PostRpcResponse(uint rpc_id, uint call_uid, byte[] result, UIntPtr result_len);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint NT_CallRpc(byte[] name, UIntPtr name_len, byte[] param, UIntPtr params_len);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr NT_GetRpcResult(int blocking, uint call_uid, ref UIntPtr result_len);

    }
}
