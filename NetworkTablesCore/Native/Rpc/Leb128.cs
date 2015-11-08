using System.Collections.Generic;

namespace NetworkTables.Native.Rpc
{
    internal static class Leb128
    {
        public static int SizeUleb128(ulong val)
        {
            int count = 0;
            do
            {
                val >>= 7;
                ++count;
            } while (val != 0);
            return count;
        }

        public static int WriteUleb128(List<byte> addr, ulong val)
        {
            int count = 0;
            do
            {
                byte b = (byte)(val & 0x7f);
                val >>= 7;

                if (val != 0)
                    b |= 0x80;
                addr.Add(b);
                count++;
            } while (val != 0);
            return count;
        }

        public static int ReadUleb128(byte[] addr, ref int start, out ulong ret)
        {
            ulong result = 0;
            int shift = 0;
            int count = 0;

            while (true)
            {
                byte b = addr[start];
                start++;
                count++;
                result |= (uint)((byte)(b & 0x7f) << shift);
                shift += 7;
                if ((b & 0x80) == 0) break;
            }
            ret = result;
            return count;
        }
    }
}
