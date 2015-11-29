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

        public static RpcValue MakeRaw(params byte[] val)
        {
            return new RpcValue(val);
        }

        public static RpcValue MakeBooleanArray(params bool[] val)
        {
            return new RpcValue(val);
        }

        public static RpcValue MakeDoubleArray(params double[] val)
        {
            return new RpcValue(val);
        }

        public static RpcValue MakeStringArray(params string[] val)
        {
            return new RpcValue(val);
        }

        internal static RpcValue MakeEmpty()
        {
            return new RpcValue();
        }

        private RpcValue()
        {
            Type = NtType.Unassigned;
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
}