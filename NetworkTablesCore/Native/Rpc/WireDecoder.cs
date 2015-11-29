using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace NetworkTables.Native.Rpc
{
    internal class WireDecoder
    {
        public static double ReadDouble(byte[] buf, int start)
        {
            return BitConverter.Int64BitsToDouble(IPAddress.NetworkToHostOrder(BitConverter.ToInt64(buf, start)));
        }

        private readonly byte[] m_buffer;
        private int m_count;

        public string Error { get; private set; } = null;

        public WireDecoder(byte[] buffer)
        {
            m_buffer = buffer;
        }

        public void Reset()
        {
            Error = null;
        }

        public bool ReadType(ref NtType type)
        {
            byte itype = 0;
            if (!Read8(ref itype)) return false;
            switch (itype)
            {
                case 0x00:
                    type = NtType.Boolean;
                    break;
                case 0x01:
                    type = NtType.Double;
                    break;
                case 0x02:
                    type = NtType.String;
                    break;
                case 0x03:
                    type = NtType.Raw;
                    break;
                case 0x10:
                    type = NtType.BooleanArray;
                    break;
                case 0x11:
                    type = NtType.DoubleArray;
                    break;
                case 0x12:
                    type = NtType.StringArray;
                    break;
                case 0x20:
                    type = NtType.Rpc;
                    break;
                default:
                    type = NtType.Unassigned;
                    Error = "unrecognized value type";
                    return false;
            }
            return true;
        }

        public RpcValue ReadValue(NtType type)
        {
            byte size = 0;
            byte[] buf;
            Error = null;
            switch (type)
            {
                case NtType.Boolean:
                    byte vB = 0;
                    return !Read8(ref vB) ? null : RpcValue.MakeBoolean(vB != 0);
                case NtType.Double:
                    double vD = 0;
                    return !ReadDouble(ref vD) ? null : RpcValue.MakeDouble(vD);
                case NtType.Raw:
                    byte[] vRa = null;
                    return !ReadRaw(ref vRa) ? null : RpcValue.MakeRaw(vRa);
                case NtType.Rpc:
                case NtType.String:
                    string vS = "";
                    return !ReadString(ref vS) ? null : RpcValue.MakeString(vS);
                case NtType.BooleanArray:
                    if (!Read8(ref size)) return null;
                    buf = ReadArray(size);
                    if (buf == null) return null;
                    bool[] bBuf = new bool[buf.Length];
                    for (int i = 0; i < buf.Length; i++)
                    {
                        bBuf[i] = buf[i] != 0;
                    }
                    return RpcValue.MakeBooleanArray(bBuf);
                case NtType.DoubleArray:
                    if (!Read8(ref size)) return null;
                    buf = ReadArray(size * 8);
                    if (buf == null) return null;
                    double[] dBuf = new double[size];
                    for (int i = 0; i < size; i++)
                    {
                        dBuf[i] = ReadDouble(buf, i * 8);
                    }
                    return RpcValue.MakeDoubleArray(dBuf);
                case NtType.StringArray:
                    if (!Read8(ref size)) return null;
                    string[] sBuf = new string[size];
                    for (int i = 0; i < size; i++)
                    {
                        if (!ReadString(ref sBuf[i])) return null;
                    }
                    return RpcValue.MakeStringArray(sBuf);
                default:
                    Error = "invalid type when trying to read value";
                    Console.WriteLine("invalid type when trying to read value");
                    return null;
            }
        }

        public byte[] ReadArray(int len)
        {
            List<byte> buf = new List<byte>(len);
            if (m_buffer.Length < m_count + len) return null;
            int readTo = m_count + len;
            for (; m_count < readTo; m_count++)
            {
                buf.Add(m_buffer[m_count]);
            }
            return buf.ToArray();
        }

        public bool Read8(ref byte val)
        {
            if (m_buffer.Length < m_count + 1) return false;
            val = (byte)(m_buffer[m_count] & 0xff);
            m_count++;
            return true;
        }

        public bool Read16(ref short val)
        {
            if (m_buffer.Length < m_count + 2) return false;
            val = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(m_buffer, m_count));
            m_count += 2;
            return true;
        }

        public bool Read32(ref int val)
        {
            if (m_buffer.Length < m_count + 4) return false;
            val = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(m_buffer, m_count));
            m_count += 4;
            return true;
        }

        public bool ReadDouble(ref double val)
        {
            if (m_buffer.Length < m_count + 8) return false;
            val = ReadDouble(m_buffer, m_count);
            m_count += 8;
            return true;
        }

        public bool ReadString(ref string val)
        {
            ulong v;
            if (Leb128.ReadUleb128(m_buffer, ref m_count, out v) == 0) return false;
            var len = (int)v;

            if (m_buffer.Length < m_count + len) return false;
            val = Encoding.UTF8.GetString(m_buffer, m_count, len);
            m_count += len;
            return true;
        }

        public bool ReadRaw(ref byte[] val)
        {
            ulong v;
            if (Leb128.ReadUleb128(m_buffer, ref m_count, out v) == 0) return false;
            var len = (int)v;

            if (m_buffer.Length < m_count + len) return false;
            val = new byte[len];
            Array.Copy(m_buffer, m_count, val, 0, len);

            m_count += len;
            return true;
        }

        public bool ReadUleb128(ref ulong val)
        {
            return Leb128.ReadUleb128(m_buffer, ref m_count, out val) != 0;
        }
    }
}
