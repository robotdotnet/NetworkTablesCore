using System.Runtime.InteropServices;

namespace NetworkTables.Native
{
    internal class NtRpcDefinition
    {
        public readonly uint Version;
        public readonly string Name;
        public readonly NtRpcParamDef[] ParamsArray;
        public readonly NtRpcResultDef[] ResultsArray;

        public NtRpcDefinition(uint version, string name, NtRpcParamDef[] p, NtRpcResultDef[] r)
        {
            Version = version;
            Name = name;
            ParamsArray = p;
            ResultsArray = r;
        }
    }


    internal class NtRpcResultDef
    {
        public readonly string Name;
        public readonly NtType Type;

        public NtRpcResultDef(string name, NtType type)
        {
            Name = name;
            Type = type;
        }
    }

    internal class NtRpcParamDef
    {
        public readonly string Name;
        public readonly RpcValue Value;

        public NtRpcParamDef(string name, RpcValue value)
        {
            Value = value;
            Name = name;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NtRpcCallInfo
    {
#pragma warning disable 649
        public readonly uint RpcId;
        public readonly uint CallUid;
        public readonly NtStringRead Name;
        public readonly NtStringRead Param;
#pragma warning restore 649
    }
}
