using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace NetworkTables.Native.Rpc
{
    internal class RpcEncoder
    {
        private readonly List<byte> m_buffer = new List<byte>(1024);

        public byte[] Buffer => m_buffer.ToArray();

        public void WriteDouble(double val)
        {
            m_buffer.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(BitConverter.DoubleToInt64Bits(val))));
        }

        public void Write8(byte val)
        {
            m_buffer.Add(val);
        }

        public void Write16(short val)
        {
            m_buffer.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(val)));
        }

        public void Write32(int val)
        {
            m_buffer.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(val)));
        }

        public void WriteUleb128(ulong val)
        {
            Leb128.WriteUleb128(m_buffer, val);
        }

        public void WriteType(NT_Type type)
        {
            switch (type)
            {
                case NT_Type.NT_BOOLEAN:
                    m_buffer.Add(0x00);
                    break;
                case NT_Type.NT_DOUBLE:
                    m_buffer.Add(0x01);
                    break;
                case NT_Type.NT_STRING:
                    m_buffer.Add(0x02);
                    break;
                case NT_Type.NT_RAW:
                    //We will only ever call this from a 3.0 protocol. So we don't need to check;
                    m_buffer.Add(0x03);
                    break;
                case NT_Type.NT_BOOLEAN_ARRAY:
                    m_buffer.Add(0x10);
                    break;
                case NT_Type.NT_DOUBLE_ARRAY:
                    m_buffer.Add(0x11);
                    break;
                case NT_Type.NT_STRING_ARRAY:
                    m_buffer.Add(0x12);
                    break;
                case NT_Type.NT_RPC:
                    //We will only ever call this from a 3.0 protocol. So we don't need to check;
                    m_buffer.Add(0x20);
                    break;
                default:
                    Console.WriteLine("Unrecognized Type");
                    return;
            }
        }
        public int GetValueSize(RPCValue value)
        {
            if (value == null) return -1;
            int size;
            switch (value.Type)
            {
                case NT_Type.NT_BOOLEAN:
                    return 1;
                case NT_Type.NT_DOUBLE:
                    return 8;
                case NT_Type.NT_RPC:
                case NT_Type.NT_RAW:
                case NT_Type.NT_STRING:
                    return GetStringSize((string)value.Value);
                case NT_Type.NT_BOOLEAN_ARRAY:
                    size = ((bool[])value.Value).Length;
                    if (size > 0xff) size = 0xff;
                    return 1 + size;
                case NT_Type.NT_DOUBLE_ARRAY:
                    size = ((double[])value.Value).Length;
                    if (size > 0xff) size = 0xff;
                    return 1 + size * 8;
                case NT_Type.NT_STRING_ARRAY:
                    string[] v = (string[])value.Value;
                    size = v.Length;
                    if (size > 0xff) size = 0xff;
                    int len = 1;
                    for (int i = 0; i < size; i++)
                    {
                        len += GetStringSize(v[i]);
                    }
                    return len;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void WriteValue(RPCValue value)
        {
            if (value == null) return;
            switch (value.Type)
            {
                case NT_Type.NT_BOOLEAN:
                    Write8((bool)value.Value ? (byte)1 : (byte)0);
                    break;
                case NT_Type.NT_DOUBLE:
                    WriteDouble((double)value.Value);
                    break;
                case NT_Type.NT_RAW:
                case NT_Type.NT_STRING:
                case NT_Type.NT_RPC:
                    WriteString((string)value.Value);
                    break;
                case NT_Type.NT_BOOLEAN_ARRAY:
                    var vB = (bool[])value.Value;
                    int sizeB = vB.Length;
                    if (sizeB > 0xff) sizeB = 0xff;
                    Write8((byte)sizeB);
                    for (int i = 0; i < sizeB; i++)
                    {
                        Write8(vB[i] ? (byte)1 : (byte)0);
                    }
                    break;
                case NT_Type.NT_DOUBLE_ARRAY:
                    var vD = (double[])value.Value;
                    int sizeD = vD.Length;
                    if (sizeD > 0xff) sizeD = 0xff;
                    for (int i = 0; i < sizeD; i++)
                    {
                        WriteDouble(vD[i]);
                    }
                    break;
                case NT_Type.NT_STRING_ARRAY:
                    var vS = (string[])value.Value;
                    int sizeS = vS.Length;
                    if (sizeS > 0xff) sizeS = 0xff;
                    for (int i = 0; i < sizeS; i++)
                    {
                        WriteString(vS[i]);
                    }
                    break;
                default:
                    Console.WriteLine("unrecognized type when writing value");
                    return;
            }
        }

        public int GetStringSize(string str)
        {
            return Leb128.SizeUleb128((ulong)str.Length) + str.Length;
        }

        public void WriteString(string str)
        {
            int len = str.Length;
            WriteUleb128((ulong)len);

            m_buffer.AddRange(Encoding.UTF8.GetBytes(str));
        }
    }
}
