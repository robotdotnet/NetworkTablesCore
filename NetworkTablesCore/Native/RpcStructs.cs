using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTablesCore.Native
{
    internal class RPCValue
    {
        private readonly NT_Type type;
        private readonly object value;

        public NT_Type Type => type;
        public object Value => value;

        public RPCValue(string val)
        {
            type = NT_Type.NT_STRING;
            value = val;
        }

        public RPCValue(bool val)
        {
            type = NT_Type.NT_BOOLEAN;
            value = val;
        }

        public RPCValue(double val)
        {
            type = NT_Type.NT_DOUBLE;
            value = val;
        }

        public RPCValue(string[] val)
        {
            type = NT_Type.NT_STRING_ARRAY;
            value = val;
        }

        public RPCValue(double[] val)
        {
            type = NT_Type.NT_DOUBLE_ARRAY;
            value = val;
        }

        public RPCValue(bool[] val)
        {
            type = NT_Type.NT_BOOLEAN_ARRAY;
            value = val;
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
            this.paramsArray = p;
            this.resultsArray = r;
        }
    }


    internal class NT_RpcResultDef
    {
        public readonly string name;
        public readonly NT_Type type;

        public NT_RpcResultDef(string name, NT_Type type)
        {
            this.name = name;
            this.type = type;
        }
    }

    internal class NT_RpcParamDef
    {
        public readonly string name;
        public readonly RPCValue value;

        public NT_RpcParamDef(string name, RPCValue value)
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
