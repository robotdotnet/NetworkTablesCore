using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace NetworkTables.Native.Rpc
{
    class RpcDecoder
    {
        public static double ReadDouble(byte[] buf, int start)
        {
            return BitConverter.Int64BitsToDouble(IPAddress.NetworkToHostOrder(BitConverter.ToInt64(buf, start)));
        }

        private readonly byte[] m_buffer;
        private int count = 0;

        public RpcDecoder(byte[] buffer)
        {
            m_buffer = buffer;
        }

        public RpcValue ReadValue(NT_Type type)
        {
            byte size = 0;
            byte[] buf;
            switch (type)
            {
                case NT_Type.NT_BOOLEAN:
                    byte vB = 0;
                    return !Read8(ref vB) ? null : RpcValue.MakeBoolean(vB != 0);
                case NT_Type.NT_DOUBLE:
                    double vD = 0;
                    return !ReadDouble(ref vD) ? null : RpcValue.MakeDouble(vD);
                case NT_Type.NT_RAW:
                    string vRa = "";
                    return !ReadString(ref vRa) ? null : RpcValue.MakeRaw(vRa);
                case NT_Type.NT_RPC:
                case NT_Type.NT_STRING:
                    string vS = "";
                    return !ReadString(ref vS) ? null : RpcValue.MakeString(vS);
                case NT_Type.NT_BOOLEAN_ARRAY:
                    if (!Read8(ref size)) return null;
                    buf = ReadArray(size);
                    if (buf == null) return null;
                    bool[] bBuf = new bool[buf.Length];
                    for (int i = 0; i < buf.Length; i++)
                    {
                        bBuf[i] = buf[i] != 0;
                    }
                    return RpcValue.MakeBooleanArray(bBuf);
                case NT_Type.NT_DOUBLE_ARRAY:
                    if (!Read8(ref size)) return null;
                    buf = ReadArray(size * 8);
                    if (buf == null) return null;
                    double[] dBuf = new double[size];
                    for (int i = 0; i < size; i++)
                    {
                        dBuf[i] = ReadDouble(buf, count);
                    }
                    return RpcValue.MakeDoubleArray(dBuf);
                    break;
                case NT_Type.NT_STRING_ARRAY:
                    if (!Read8(ref size)) return null;
                    buf = ReadArray(size * 8);
                    if (buf == null) return null;
                    string[] sBuf = new string[size];
                    for (int i = 0; i < size; i++)
                    {
                        if (!ReadString(ref sBuf[i])) return null;
                    }
                    return RpcValue.MakeStringArray(sBuf);
                default:
                    Console.WriteLine("invalid type when trying to read value");
                    return null;
            }
        }

        public byte[] ReadArray(int len)
        {
            List<byte> buf = new List<byte>(len);
            if (m_buffer.Length < count + len) return null;
            for (int i = count; i < count + len; i++)
            {
                buf.Add(m_buffer[i]);
            }
            return buf.ToArray();
        }

        public bool Read8(ref byte val)
        {
            if (m_buffer.Length < count + 1) return false;
            val = (byte)(m_buffer[count] & 0xff);
            count++;
            return true;
        }

        public bool Read16(ref short val)
        {
            if (m_buffer.Length < count + 2) return false;
            val = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(m_buffer, count));
            return true;
        }

        public bool Read32(ref int val)
        {
            if (m_buffer.Length < count + 4) return false;
            val = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(m_buffer, count));
            return true;
        }

        public bool ReadDouble(ref double val)
        {
            if (m_buffer.Length < count + 8) return false;
            val = ReadDouble(m_buffer, count);
            return true;
        }

        public bool ReadString(ref string val)
        {
            ulong v;
            if (Leb128.ReadUleb128(m_buffer, ref count, out v) == 0) return false;
            var len = (int)v;

            if (m_buffer.Length < count + len) return false;
            val = Encoding.UTF8.GetString(m_buffer, count, len);
            return true;
        }
    }
}
