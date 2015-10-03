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

        public static RpcValue MakeRaw(string val)
        {
            return new RpcValue(val, true);
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

        private RpcValue(string val, bool raw)
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

    internal class NT_RpcDefinition
    {
        public readonly uint version;
        public readonly string name;
        public readonly NT_RpcParamDef[] paramsArray;
        public readonly NT_RpcResultDef[] resultsArray;

        public NT_RpcDefinition(uint version, string name, NT_RpcParamDef[] p, NT_RpcResultDef[] r)
        {
            this.version = version;
            this.name = name;
            paramsArray = p;
            resultsArray = r;
        }
    }


    internal class NT_RpcResultDef
    {
        public readonly string name;
        public readonly NtType type;

        public NT_RpcResultDef(string name, NtType type)
        {
            this.name = name;
            this.type = type;
        }
    }

    internal class NT_RpcParamDef
    {
        public readonly string name;
        public readonly RpcValue value;

        public NT_RpcParamDef(string name, RpcValue value)
        {
            this.value = value;
            this.name = name;
        }
    }

    internal struct NT_RpcCallInfo
    {
        private uint rpc_id;
        private uint call_uid;
        private NtStringRead name;
        private NtStringRead param;
    }
}
