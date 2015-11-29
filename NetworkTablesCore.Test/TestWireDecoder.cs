using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkTables.Native;
using NetworkTables.Native.Rpc;
using NUnit.Framework;

namespace NetworkTablesCore.Test
{
    [TestFixture]
    public class TestWireDecoder
    {
        readonly RpcValue v_boolean = RpcValue.MakeBoolean(true);
        readonly RpcValue v_double = RpcValue.MakeDouble(1.0);
        readonly RpcValue v_string = RpcValue.MakeString("hello");
        readonly RpcValue v_raw = RpcValue.MakeRaw((byte)'h', (byte)'e', (byte)'l', (byte)'l', (byte)'o');
        readonly RpcValue v_boolArray = RpcValue.MakeBooleanArray(false, true, false);
        readonly RpcValue v_boolArrayBig = RpcValue.MakeBooleanArray(new bool[255]);
        readonly RpcValue v_doubleArray = RpcValue.MakeDoubleArray(0.5, 0.25);
        readonly RpcValue v_doubleArrayBig = RpcValue.MakeDoubleArray(new double[255]);

        readonly RpcValue v_stringArray = RpcValue.MakeStringArray("hello", "goodbye");
        readonly RpcValue v_stringArrayBig;


        readonly string s_normal = "hello";

        private readonly string s_long;
        private readonly string s_big;
        
        public TestWireDecoder()
        {
            List<string> sa = new List<string>();
            for (int i = 0; i < 255; i++)
            {
                sa.Add("h");
            }
            v_stringArrayBig = RpcValue.MakeStringArray(sa.ToArray());
            
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < 127; i++)
            {
                builder.Append('*');
            }
            builder.Append('x');
            s_long = builder.ToString();

            builder.Clear();
            for (int i = 0; i < 65534; i++)
            {
                builder.Append('*');
            }
            builder.Append('x');
            builder.Append('x');
            builder.Append('x');
            s_big = builder.ToString();
            
        }

        [Test]
        public void TestRead8()
        {
            byte[] rawData = { 0x05, 0x01, 0x00 };
            WireDecoder d = new WireDecoder(rawData);
            byte val = 0;
            Assert.That(d.Read8(ref val));
            Assert.That(val, Is.EqualTo(5));

            Assert.That(d.Read8(ref val));
            Assert.That(val, Is.EqualTo(1));

            Assert.That(d.Read8(ref val));
            Assert.That(val, Is.EqualTo(0));

            Assert.That(!d.Read8(ref val));

            Assert.That(d.Error, Is.Null);
        }

        [Test]
        public void TestRead16()
        {
            byte[] rawData = { 0x00, 0x05, 0x00, 0x01, 0x45, 0x67, 0x00, 0x00 };
            WireDecoder d = new WireDecoder(rawData);
            short val = 0;
            Assert.That(d.Read16(ref val));
            Assert.That(val, Is.EqualTo(5));

            Assert.That(d.Read16(ref val));
            Assert.That(val, Is.EqualTo(1));

            Assert.That(d.Read16(ref val));
            Assert.That(val, Is.EqualTo(0x4567));

            Assert.That(d.Read16(ref val));
            Assert.That(val, Is.EqualTo(0));

            Assert.That(!d.Read16(ref val));
            Assert.That(d.Error, Is.Null);
        }

        [Test]
        public void TestRead32()
        {
            byte[] rawData = { 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0xab, 0xcd, 0x12, 0x34, 0x56, 0x78, 0x00, 0x00, 0x00, 0x00 };
            WireDecoder d = new WireDecoder(rawData);
            int val = 0;
            Assert.That(d.Read32(ref val));
            Assert.That(val, Is.EqualTo(5));

            Assert.That(d.Read32(ref val));
            Assert.That(val, Is.EqualTo(1));

            Assert.That(d.Read32(ref val));
            Assert.That(val, Is.EqualTo(0xabcd));

            Assert.That(d.Read32(ref val));
            Assert.That(val, Is.EqualTo(0x12345678));

            Assert.That(d.Read32(ref val));
            Assert.That(val, Is.EqualTo(0));

            Assert.That(!d.Read32(ref val));

            Assert.That(d.Error, Is.Null);
        }

        [Test]
        public void TestReadDouble()
        {
            byte[] rawData =
            {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x41, 0x0c, 0x13, 0x80, 0x00, 0x00, 0x00, 0x00,
                0x7f, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x7f, 0xef, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff
            };
            WireDecoder d = new WireDecoder(rawData);
            double val = 0;
            Assert.That(d.ReadDouble(ref val));
            Assert.That(val, Is.EqualTo(0.0));

            Assert.That(d.ReadDouble(ref val));
            Assert.That(val, Is.EqualTo(2.3e5));

            Assert.That(d.ReadDouble(ref val));
            Assert.That(val, Is.EqualTo(double.PositiveInfinity));

            Assert.That(d.ReadDouble(ref val));
            Assert.That(val, Is.EqualTo(2.2250738585072014e-308));

            Assert.That(d.ReadDouble(ref val));
            Assert.That(val, Is.EqualTo(double.MaxValue));

            Assert.That(!d.ReadDouble(ref val));

            Assert.That(d.Error, Is.Null);
        }

        [Test]
        public void TestReadUleb128()
        {
            byte[] rawData = { 0x00, 0x7f, 0x80, 0x01, 0x80 };
            WireDecoder d = new WireDecoder(rawData);
            ulong val = 0;
            Assert.That(d.ReadUleb128(ref val));
            Assert.That(val, Is.EqualTo(0));

            Assert.That(d.ReadUleb128(ref val));
            Assert.That(val, Is.EqualTo(0x7f));

            Assert.That(d.ReadUleb128(ref val));
            Assert.That(val, Is.EqualTo(0x80));

            Assert.That(!d.ReadUleb128(ref val));

            Assert.That(d.Error, Is.Null);
        }

        [Test]
        public void TestReadType()
        {
            byte[] rawData = { 0x00, 0x01, 0x02, 0x03, 0x10, 0x11, 0x12, 0x20 };
            WireDecoder d = new WireDecoder(rawData);
            NtType val = 0;
            Assert.That(d.ReadType(ref val));
            Assert.That(val, Is.EqualTo(NtType.Boolean));

            Assert.That(d.ReadType(ref val));
            Assert.That(val, Is.EqualTo(NtType.Double));

            Assert.That(d.ReadType(ref val));
            Assert.That(val, Is.EqualTo(NtType.String));

            Assert.That(d.ReadType(ref val));
            Assert.That(val, Is.EqualTo(NtType.Raw));

            Assert.That(d.ReadType(ref val));
            Assert.That(val, Is.EqualTo(NtType.BooleanArray));

            Assert.That(d.ReadType(ref val));
            Assert.That(val, Is.EqualTo(NtType.DoubleArray));

            Assert.That(d.ReadType(ref val));
            Assert.That(val, Is.EqualTo(NtType.StringArray));

            Assert.That(d.ReadType(ref val));
            Assert.That(val, Is.EqualTo(NtType.Rpc));

            Assert.That(!d.ReadType(ref val));

            Assert.That(d.Error, Is.Null);
        }

        [Test]
        public void TestReadTypeError()
        {
            byte[] rawData = { 0x30 };
            WireDecoder d = new WireDecoder(rawData);
            NtType val = 0;

            Assert.That(!d.ReadType(ref val));

            Assert.That(d.Error, Is.Not.Null);
        }

        [Test]
        public void TestReset()
        {
            byte[] rawData = { 0x30 };
            WireDecoder d = new WireDecoder(rawData);
            NtType val = 0;

            Assert.That(!d.ReadType(ref val));

            Assert.That(d.Error, Is.Not.Null);
            d.Reset();
            Assert.That(d.Error, Is.Null);
        }

        [Test]
        public void TestBooleanValue()
        {
            byte[] rawData = new byte[] { 0x01, 0x00 };
            WireDecoder d = new WireDecoder(rawData);
            var val = d.ReadValue(NtType.Boolean);
            Assert.That(val.Type, Is.EqualTo(NtType.Boolean));
            Assert.That(val.Value, Is.EqualTo(v_boolean.Value));

            var vFalse = RpcValue.MakeBoolean(false);
            val = d.ReadValue(NtType.Boolean);
            Assert.That(val.Type, Is.EqualTo(NtType.Boolean));
            Assert.That(val.Value, Is.EqualTo(vFalse.Value));

            Assert.That(d.ReadValue(NtType.Boolean), Is.Null);
            Assert.That(d.Error, Is.Null);
        }

        [Test]
        public void TestDoubleValue()
        {
            byte[] rawData = new byte[]
            {
                0x3f, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x3f, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };
            WireDecoder d = new WireDecoder(rawData);
            var val = d.ReadValue(NtType.Double);
            Assert.That(val.Type, Is.EqualTo(NtType.Double));
            Assert.That(val.Value, Is.EqualTo(v_double.Value));

            val = d.ReadValue(NtType.Double);
            Assert.That(val.Type, Is.EqualTo(NtType.Double));
            Assert.That(val.Value, Is.EqualTo(v_double.Value));

            Assert.That(d.ReadValue(NtType.Double), Is.Null);
            Assert.That(d.Error, Is.Null);
        }

        [Test]
        public void TestStringValue()
        {
            byte[] rawData = new byte[]
            {
                0x05, (byte)'h', (byte)'e', (byte)'l', (byte)'l', (byte)'o',
                0x03, (byte)'b', (byte)'y', (byte)'e', 0x55,
            };
            WireDecoder d = new WireDecoder(rawData);
            var val = d.ReadValue(NtType.String);
            Assert.That(val.Type, Is.EqualTo(NtType.String));
            Assert.That(val.Value, Is.EqualTo(v_string.Value));

            var vFalse = RpcValue.MakeString("bye");
            val = d.ReadValue(NtType.String);
            Assert.That(val.Type, Is.EqualTo(NtType.String));
            Assert.That(val.Value, Is.EqualTo(vFalse.Value));

            byte b = 0;
            Assert.That(d.Read8(ref b));
            Assert.That(b, Is.EqualTo(0x55));

            Assert.That(d.ReadValue(NtType.String), Is.Null);
            Assert.That(d.Error, Is.Null);
        }

        [Test]
        public void TestRawValue()
        {
            byte[] rawData = new byte[]
            {
                0x05, (byte)'h', (byte)'e', (byte)'l', (byte)'l', (byte)'o',
                0x03, (byte)'b', (byte)'y', (byte)'e', 0x55,
            };
            WireDecoder d = new WireDecoder(rawData);
            var val = d.ReadValue(NtType.Raw);
            Assert.That(val.Type, Is.EqualTo(NtType.Raw));
            Assert.That(val.Value, Is.EqualTo(v_raw.Value));

            var vFalse = RpcValue.MakeRaw((byte)'b', (byte)'y', (byte)'e');
            val = d.ReadValue(NtType.Raw);
            Assert.That(val.Type, Is.EqualTo(NtType.Raw));
            Assert.That(val.Value, Is.EqualTo(vFalse.Value));

            byte b = 0;
            Assert.That(d.Read8(ref b));
            Assert.That(b, Is.EqualTo(0x55));

            Assert.That(d.ReadValue(NtType.Raw), Is.Null);
            Assert.That(d.Error, Is.Null);
        }

        [Test]
        public void TestReadBooleanArray()
        {
            byte[] b = { 0x03, 0x00, 0x01, 0x00, 0x02, 0x01, 0x00, 0xff };
            WireDecoder d = new WireDecoder(b);

            var val = d.ReadValue(NtType.BooleanArray);
            Assert.That(val.Type, Is.EqualTo(NtType.BooleanArray));
            Assert.That(val.Value, Is.EqualTo(v_boolArray.Value));

            var boolArray2 = RpcValue.MakeBooleanArray(true, false);
            val = d.ReadValue(NtType.BooleanArray);
            Assert.That(val.Type, Is.EqualTo(NtType.BooleanArray));
            Assert.That(val.Value, Is.EqualTo(boolArray2.Value));

            Assert.That(d.ReadValue(NtType.BooleanArray), Is.Null);
            Assert.That(d.Error, Is.Null);
        }

        [Test]
        public void ReadBooleanArrayBig()
        {
            List<byte> s = new List<byte>();
            s.Add(0xff);
            for (int i = 0; i < 255; i++)
            {
                s.Add(0x00);
            }
            WireDecoder d = new WireDecoder(s.ToArray());

            var val = d.ReadValue(NtType.BooleanArray);
            Assert.That(val.Type, Is.EqualTo(NtType.BooleanArray));
            Assert.That(val.Value, Is.EqualTo(v_boolArrayBig.Value));

            Assert.That(d.ReadValue(NtType.BooleanArray), Is.Null);
            Assert.That(d.Error, Is.Null);
        }



        [Test]
        public void TestReadDoubleArray()
        {
            byte[] b =
            {
                0x02, 0x3f, 0xe0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x3f, 0xd0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x55
            };
            WireDecoder d = new WireDecoder(b);

            var val = d.ReadValue(NtType.DoubleArray);
            Assert.That(val.Type, Is.EqualTo(NtType.DoubleArray));
            Assert.That(val.Value, Is.EqualTo(v_doubleArray.Value));

            byte by = 0;
            Assert.That(d.Read8(ref by));
            Assert.That(by, Is.EqualTo(0x55));

            Assert.That(d.ReadValue(NtType.DoubleArray), Is.Null);
            Assert.That(d.Error, Is.Null);
        }

        [Test]
        public void ReadDoubleArrayBig()
        {
            List<byte> s = new List<byte>();
            s.Add(0xff);
            for (int i = 0; i < 255 * 8; i++)
            {
                s.Add(0x00);
            }
            WireDecoder d = new WireDecoder(s.ToArray());

            var val = d.ReadValue(NtType.DoubleArray);
            Assert.That(val.Type, Is.EqualTo(NtType.DoubleArray));
            Assert.That(val.Value, Is.EqualTo(v_doubleArrayBig.Value));

            Assert.That(d.ReadValue(NtType.DoubleArray), Is.Null);
            Assert.That(d.Error, Is.Null);
        }

        [Test]
        public void TestReadStringArray()
        {
            List<byte> b = new List<byte>()
            {
                0x02,
                0x05
            };
            b.AddRange(Encoding.UTF8.GetBytes("hello"));
            b.Add(0x07);
            b.AddRange(Encoding.UTF8.GetBytes("goodbye"));
            b.Add(0x55);
            WireDecoder d = new WireDecoder(b.ToArray());

            var val = d.ReadValue(NtType.StringArray);
            Assert.That(val.Type, Is.EqualTo(NtType.StringArray));
            Assert.That(val.Value, Is.EqualTo(v_stringArray.Value));

            byte by = 0;
            Assert.That(d.Read8(ref by));
            Assert.That(by, Is.EqualTo(0x55));

            Assert.That(d.ReadValue(NtType.StringArray), Is.Null);
            Assert.That(d.Error, Is.Null);
        }

        [Test]
        public void ReadStringArrayBig()
        {
            List<byte> s = new List<byte>();
            s.Add(0xff);
            for (int i = 0; i < 255; i++)
            {
                s.Add(0x01);
                s.Add((byte) 'h');
            }
            WireDecoder d = new WireDecoder(s.ToArray());

            var val = d.ReadValue(NtType.StringArray);
            Assert.That(val.Type, Is.EqualTo(NtType.StringArray));
            Assert.That(val.Value, Is.EqualTo(v_stringArrayBig.Value));

            Assert.That(d.ReadValue(NtType.StringArray), Is.Null);
            Assert.That(d.Error, Is.Null);
        }

        [Test]
        public void ReadValueError()
        {
            WireDecoder d = new WireDecoder(new byte[0]);
            Assert.That(d.ReadValue(NtType.Unassigned), Is.Null);
            Assert.That(d.Error, Is.Not.Null);
        }

        [Test]
        public void TestReadString()
        {
            byte[] sNormalBytes = Encoding.UTF8.GetBytes(s_normal);
            byte[] sLongBytes = Encoding.UTF8.GetBytes(s_long);
            byte[] sBigBytes = Encoding.UTF8.GetBytes(s_big);
            List<byte> s = new List<byte>();
            s.Add(0x05);
            s.AddRange(sNormalBytes);
            s.AddRange(new byte[] { 0x80, 0x01 });
            s.AddRange(sLongBytes);
            s.AddRange(new byte[] { 0x81, 0x80, 0x04 });
            s.AddRange(sBigBytes);
            s.Add(0x55);

            WireDecoder d = new WireDecoder(s.ToArray());

            string outs = null;
            Assert.That(d.ReadString(ref outs));
            Assert.That(outs, Is.EquivalentTo(s_normal));

            Assert.That(d.ReadString(ref outs));
            Assert.That(outs, Is.EquivalentTo(s_long));

            Assert.That(d.ReadString(ref outs));
            Assert.That(outs, Is.EquivalentTo(s_big));

            byte b = 0;
            Assert.That(d.Read8(ref b));
            Assert.That(b, Is.EqualTo(0x55));

            Assert.That(d.ReadString(ref outs), Is.False);
            Assert.That(d.Error, Is.Null);
        }
    }
}
