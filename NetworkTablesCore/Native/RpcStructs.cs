using System.Runtime.InteropServices;

namespace NetworkTables.Native
{
    internal class RpcValue
    {
        public NtType Type { get; }

        public object Value { get; }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static RpcValue MakeDouble(double val)
        {
            return new RpcValue(val);
        }

        public static RpcValue MakeBoolean(bool val)
        {
            return new RpcValue(val);
        }

        public static RpcValue MakeString(string val)
        {
            return new RpcValue(val);
        }

        public static RpcValue MakeRaw(byte[] val)
        {
            return new RpcValue(val);
        }

        public static RpcValue MakeBooleanArray(bool[] val)
        {
            return new RpcValue(val);
        }

        public static RpcValue MakeDoubleArray(double[] val)
        {
            return new RpcValue(val);
        }

        public static RpcValue MakeStringArray(string[] val)
        {
            return new RpcValue(val);
        }

        private RpcValue(string val)
        {
            Type = NtType.String;
            Value = val;
        }

        private RpcValue(byte[] val)
        {
            Type = NtType.Raw;
            Value = val;
        }

        private RpcValue(bool val)
        {
            Type = NtType.Boolean;
            Value = val;
        }

        private RpcValue(double val)
        {
            Type = NtType.Double;
            Value = val;
        }

        private RpcValue(string[] val)
        {
            Type = NtType.StringArray;
            Value = val;
        }

        private RpcValue(double[] val)
        {
            Type = NtType.DoubleArray;
            Value = val;
        }

        private RpcValue(bool[] val)
        {
            Type = NtType.BooleanArray;
            Value = val;
        }
    }

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
