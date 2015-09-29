namespace NetworkTables.Native
{
    public class RpcValue
    {
        public NT_Type Type { get; }

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
            Type = NT_Type.NT_STRING;
            Value = val;
        }

        private RpcValue(string val, bool raw)
        {
            Type = NT_Type.NT_RAW;
            Value = val;
        }

        private RpcValue(bool val)
        {
            Type = NT_Type.NT_BOOLEAN;
            Value = val;
        }

        private RpcValue(double val)
        {
            Type = NT_Type.NT_DOUBLE;
            Value = val;
        }

        private RpcValue(string[] val)
        {
            Type = NT_Type.NT_STRING_ARRAY;
            Value = val;
        }

        private RpcValue(double[] val)
        {
            Type = NT_Type.NT_DOUBLE_ARRAY;
            Value = val;
        }

        private RpcValue(bool[] val)
        {
            Type = NT_Type.NT_BOOLEAN_ARRAY;
            Value = val;
        }
    }

    public class NT_RpcDefinition
    {
        public readonly uint version;
        public readonly string name;
        public readonly NT_RpcParamDef[] paramsArray;
        public readonly NT_RpcResultDef[] resultsArray;

        public NT_RpcDefinition(uint version, string name, NT_RpcParamDef[] p, NT_RpcResultDef[] r)
        {
            this.version = version;
            this.name = name;
            this.paramsArray = p;
            this.resultsArray = r;
        }
    }


    public class NT_RpcResultDef
    {
        public readonly string name;
        public readonly NT_Type type;

        public NT_RpcResultDef(string name, NT_Type type)
        {
            this.name = name;
            this.type = type;
        }
    }

    public class NT_RpcParamDef
    {
        public readonly string name;
        public readonly RpcValue value;

        public NT_RpcParamDef(string name, RpcValue value)
        {
            this.value = value;
            this.name = name;
        }
    }

    public struct NT_RpcCallInfo
    {
        private uint rpc_id;
        private uint call_uid;
        private NT_String_Read name;
        private NT_String_Read param;
    }
}
