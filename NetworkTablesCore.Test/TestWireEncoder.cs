﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkTables.Native;
using NetworkTables;
using NUnit.Framework;

namespace NetworkTablesCore.Test
{
    [TestFixture]
    public class TestWireEncoder
    {
        const int BUFSIZE = 1024;

        [Test]
        public void TestWireEncoderWrite8()
        {
            int off = BUFSIZE - 1;
            WireEncoder e = new WireEncoder(0x0300);
            for (int i = 0; i < off; i++)
            {
                e.Write8(0);
            }
            e.Write8(5);
            unchecked
            {
                e.Write8((byte)0x101u);
            }
            e.Write8(0);

            Assert.That(e.Buffer.Length - off, Is.EqualTo(3));
            byte[] checkArray = { 0x05, 0x01, 0x00 };
            byte[] bufferArray = new byte[checkArray.Length];
            Array.Copy(e.Buffer, off, bufferArray, 0, checkArray.Length);
            Assert.That(bufferArray, Is.EquivalentTo(checkArray));
        }

        [Test]
        public void TestConstruct()
        {
            WireEncoder d = new WireEncoder(0x0300);
            Assert.That(d.Error, Is.Null);
            Assert.That(d.ProtoRev, Is.EqualTo(0x0300));
        }

        [Test]
        public void TestSetProtoRev()
        {
            WireEncoder d = new WireEncoder(0x0300);
            d.SetProtoRev(0x0200);
            Assert.That(d.ProtoRev, Is.EqualTo(0x0200));
        }

        [Test]
        public void TestWireEncoderWrite16()
        {
            int off = BUFSIZE - 2;
            WireEncoder e = new WireEncoder(0x0300);
            for (int i = 0; i < off; i++)
            {
                e.Write8(0);
            }
            e.Write16(5);
            unchecked
            {
                e.Write16((ushort)0x10001u);
            }
            e.Write16(0x4567);
            e.Write16(0);

            Assert.That(e.Buffer.Length - off, Is.EqualTo(8));
            byte[] checkArray = { 0x00, 0x05, 0x00, 0x01, 0x45, 0x67, 0x00, 0x00 };
            byte[] bufferArray = new byte[checkArray.Length];
            Array.Copy(e.Buffer, off, bufferArray, 0, checkArray.Length);
            Assert.That(bufferArray, Is.EquivalentTo(checkArray));
        }

        [Test]
        public void TestWireEncoderWrite32()
        {
            int off = BUFSIZE - 4;
            WireEncoder e = new WireEncoder(0x0300);
            for (int i = 0; i < off; i++)
            {
                e.Write8(0);
            }
            e.Write32(5);
            e.Write32(1);
            e.Write32(0xabcd);
            e.Write32(0x12345678);
            e.Write32(0);

            Assert.That(e.Buffer.Length - off, Is.EqualTo(20));
            byte[] checkArray = { 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0xab, 0xcd, 0x12, 0x34, 0x56, 0x78, 0x00, 0x00, 0x00, 0x00 };
            byte[] bufferArray = new byte[checkArray.Length];
            Array.Copy(e.Buffer, off, bufferArray, 0, checkArray.Length);
            Assert.That(bufferArray, Is.EquivalentTo(checkArray));
        }


        [Test]
        public void TestWireEncoderWriteDouble()
        {
            int off = BUFSIZE - 8;
            WireEncoder e = new WireEncoder(0x0300);
            for (int i = 0; i < off; i++)
            {
                e.Write8(0);
            }
            e.WriteDouble(0.0);
            e.WriteDouble(2.3e5);
            e.WriteDouble(double.PositiveInfinity);
            e.WriteDouble(2.2250738585072014e-308);//Minimum double size
            e.WriteDouble(double.MaxValue);

            Assert.That(e.Buffer.Length - off, Is.EqualTo(40));
            byte[] checkArray =
            {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x41, 0x0c, 0x13, 0x80, 0x00, 0x00, 0x00, 0x00,
                0x7f, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x7f, 0xef, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff
            };
            byte[] bufferArray = new byte[checkArray.Length];
            Array.Copy(e.Buffer, off, bufferArray, 0, checkArray.Length);
            Assert.That(bufferArray, Is.EquivalentTo(checkArray));
        }

        [Test]
        public void TestWireEncoderWriteLeb128()
        {
            int off = BUFSIZE - 2;
            WireEncoder e = new WireEncoder(0x0300);
            for (int i = 0; i < off; i++)
            {
                e.Write8(0);
            }
            e.WriteUleb128(0ul);
            e.WriteUleb128(0x7ful);
            e.WriteUleb128(0x80ul);

            Assert.That(e.Buffer.Length - off, Is.EqualTo(4));
            byte[] checkArray = { 0x00, 0x7f, 0x80, 0x01 };
            byte[] bufferArray = new byte[checkArray.Length];
            Array.Copy(e.Buffer, off, bufferArray, 0, checkArray.Length);
            Assert.That(bufferArray, Is.EquivalentTo(checkArray));
        }


        [Test]
        public void TestWireEncoderWriteType()
        {
            int off = BUFSIZE - 1;
            WireEncoder e = new WireEncoder(0x0300);
            for (int i = 0; i < off; i++)
            {
                e.Write8(0);
            }
            e.WriteType(NtType.Boolean);
            e.WriteType(NtType.Double);
            e.WriteType(NtType.String);
            e.WriteType(NtType.Raw);
            e.WriteType(NtType.BooleanArray);
            e.WriteType(NtType.DoubleArray);
            e.WriteType(NtType.StringArray);
            e.WriteType(NtType.Rpc);

            Assert.That(e.Buffer.Length - off, Is.EqualTo(8));
            byte[] checkArray = { 0x00, 0x01, 0x02, 0x03, 0x10, 0x11, 0x12, 0x20 };
            byte[] bufferArray = new byte[checkArray.Length];
            Array.Copy(e.Buffer, off, bufferArray, 0, checkArray.Length);
            Assert.That(bufferArray, Is.EquivalentTo(checkArray));
        }

        [Test]
        public void TestWriteTypeError()
        {
            WireEncoder e = new WireEncoder(0x0200);
            e.WriteType(NtType.Unassigned);
            Assert.That(e.Size(), Is.EqualTo(0));
            Assert.That(e.Error, Is.EqualTo("unrecognized Type"));

            e.Reset();
            e.WriteType(NtType.Raw);
            Assert.That(e.Size(), Is.EqualTo(0));
            Assert.That(e.Error, Is.EqualTo("raw type not supported in protocol < 3.0"));

            e.Reset();
            e.WriteType(NtType.Rpc);
            Assert.That(e.Size(), Is.EqualTo(0));
            Assert.That(e.Error, Is.EqualTo("RPC type not supported in protocol < 3.0"));
        }

        [Test]
        public void TestReset()
        {
            WireEncoder e = new WireEncoder(0x0300);
            e.WriteType(NtType.Unassigned);
            Assert.That(e.Error, Is.Not.Null);
            e.Reset();
            Assert.That(e.Error, Is.Null);

            e.Write8(0);
            Assert.That(e.Buffer.Length, Is.EqualTo(1));
            e.Reset();
            Assert.That(e.Buffer, Is.Empty);
        }

        private Value v_empty = Value.MakeEmpty();
        Value v_boolean = Value.MakeBoolean(true);
        Value v_double = Value.MakeDouble(1.0);
        Value v_string = Value.MakeString("hello");
        Value v_raw = Value.MakeRaw((byte)'h', (byte)'e', (byte)'l', (byte)'l', (byte)'o');
        Value v_rpc = Value.MakeRpc((byte)'h', (byte)'e', (byte)'l', (byte)'l', (byte)'o');
        Value v_boolArray = Value.MakeBooleanArray(false, true, false);
        Value v_boolArrayBig = Value.MakeBooleanArray(new bool[256]);
        Value v_doubleArray = Value.MakeDoubleArray(0.5, 0.25);
        Value v_doubleArrayBig = Value.MakeDoubleArray(new double[256]);

        Value v_stringArray = Value.MakeStringArray("hello", "goodbye");
        Value v_stringArrayBig;

        string s_normal = "hello";

        private string s_long;
        private string s_big;

        public TestWireEncoder()
        {
            List<string> sa = new List<string>();
            for (int i = 0; i < 256; i++)
            {
                sa.Add("a");
            }
            v_stringArrayBig = Value.MakeStringArray(sa.ToArray());

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
        public void TestGetValueSize2()
        {
            WireEncoder e = new WireEncoder(0x0200);
            Assert.That(e.GetValueSize(null), Is.EqualTo(0));
            Assert.That(e.GetValueSize(v_empty), Is.EqualTo(0));
            Assert.That(e.GetValueSize(v_boolean), Is.EqualTo(1));
            Assert.That(e.GetValueSize(v_double), Is.EqualTo(8));
            Assert.That(e.GetValueSize(v_string), Is.EqualTo(7));
            Assert.That(e.GetValueSize(v_raw), Is.EqualTo(0));
            Assert.That(e.GetValueSize(v_rpc), Is.EqualTo(0));

            Assert.That(e.GetValueSize(v_boolArray), Is.EqualTo(1 + 3));
            Assert.That(e.GetValueSize(v_boolArrayBig), Is.EqualTo(1 + 255));
            Assert.That(e.GetValueSize(v_doubleArray), Is.EqualTo(1 + 2 * 8));
            Assert.That(e.GetValueSize(v_doubleArrayBig), Is.EqualTo(1 + 255 * 8));
            Assert.That(e.GetValueSize(v_stringArray), Is.EqualTo(1 + 7 + 9));
            Assert.That(e.GetValueSize(v_stringArrayBig), Is.EqualTo(1 + 255 * 3));
        }

        [Test]
        public void TestWriteBooleanValue2()
        {
            WireEncoder e = new WireEncoder(0x0200);
            e.WriteValue(v_boolean);
            var v_false = Value.MakeBoolean(false);
            e.WriteValue(v_false);
            Assert.That(e.Error, Is.Null);
            Assert.That(e.Buffer.Length, Is.EqualTo(2));
            Assert.That(e.Buffer, Is.EquivalentTo(new byte[] { 0x01, 0x00 }));
        }

        [Test]
        public void TestWriteDoubleValue2()
        {
            WireEncoder e = new WireEncoder(0x0200);
            e.WriteValue(v_double);
            Assert.That(e.Error, Is.Null);
            Assert.That(e.Buffer.Length, Is.EqualTo(8));
            Assert.That(e.Buffer, Is.EquivalentTo(new byte[] { 0x3f, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }));
        }

        [Test]
        public void TestWriteStringValue2()
        {
            WireEncoder e = new WireEncoder(0x0200);
            e.WriteValue(v_string);
            Assert.That(e.Error, Is.Null);
            Assert.That(e.Buffer.Length, Is.EqualTo(7));
            Assert.That(e.Buffer, Is.EquivalentTo(new byte[] { 0x00, 0x05, (byte)'h', (byte)'e', (byte)'l', (byte)'l', (byte)'o' }));
        }

        [Test]
        public void TestWriteBooleanArrayValue2()
        {
            WireEncoder e = new WireEncoder(0x0200);
            e.WriteValue(v_boolArray);
            Assert.That(e.Error, Is.Null);
            Assert.That(e.Buffer.Length, Is.EqualTo(1 + 3));
            Assert.That(e.Buffer, Is.EquivalentTo(new byte[] { 0x03, 0x00, 0x01, 0x00 }));

            e.Reset();
            e.WriteValue(v_boolArrayBig);
            Assert.That(e.Error, Is.Null);
            Assert.That(e.Buffer.Length, Is.EqualTo(1 + 255));
            Assert.That(new[] { e.Buffer[0], e.Buffer[1] }, Is.EquivalentTo(new byte[] { 0xff, 0x00 }));
        }

        [Test]
        public void TestWriteDoubleArrayValue2()
        {
            WireEncoder e = new WireEncoder(0x0200);
            e.WriteValue(v_doubleArray);
            Assert.That(e.Error, Is.Null);
            Assert.That(e.Buffer.Length, Is.EqualTo(1 + 2 * 8));
            Assert.That(e.Buffer, Is.EquivalentTo(new byte[]
            {
                0x02, 0x3f, 0xe0, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x3f, 0xd0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            }));

            e.Reset();
            e.WriteValue(v_doubleArrayBig);
            Assert.That(e.Error, Is.Null);
            Assert.That(e.Buffer.Length, Is.EqualTo(1 + 255 * 8));
            Assert.That(new[] { e.Buffer[0], e.Buffer[1] }, Is.EquivalentTo(new byte[] { 0xff, 0x00 }));
        }

        [Test]
        public void TestWriteStringArrayValue2()
        {
            WireEncoder e = new WireEncoder(0x0200);
            e.WriteValue(v_stringArray);
            Assert.That(e.Error, Is.Null);
            Assert.That(e.Buffer.Length, Is.EqualTo(1 + 7 + 9));
            Assert.That(e.Buffer, Is.EquivalentTo(new byte[]
            {
                0x02, 0x00, 0x05, (byte)'h', (byte)'e', (byte)'l', (byte)'l', (byte)'o',
                0x00, 0x07, (byte)'g', (byte)'o', (byte)'o', (byte)'d', (byte)'b', (byte)'y', (byte)'e'
            }));

            e.Reset();
            e.WriteValue(v_stringArrayBig);
            Assert.That(e.Error, Is.Null);
            Assert.That(e.Buffer.Length, Is.EqualTo(1 + 255 * 3));
            Assert.That(new[] { e.Buffer[0], e.Buffer[1], e.Buffer[2] }, Is.EquivalentTo(new byte[] { 0xff, 0x00, 0x01 }));
        }

        [Test]
        public void TestGetStringSize2()
        {
            WireEncoder e = new WireEncoder(0x0200);
            Assert.That(e.GetStringSize(s_normal), Is.EqualTo(7));
            Assert.That(e.GetStringSize(s_long), Is.EqualTo(130));
            Assert.That(e.GetStringSize(s_big), Is.EqualTo(65537));
        }

        [Test]
        public void TestWriteString2()
        {
            WireEncoder e = new WireEncoder(0x0200);
            e.WriteString(s_normal);
            Assert.That(e.Error, Is.Null);
            Assert.That(e.Buffer.Length, Is.EqualTo(7));
            Assert.That(e.Buffer, Is.EquivalentTo(new byte[] { 0x00, 0x05, (byte)'h', (byte)'e', (byte)'l', (byte)'l', (byte)'o' }));

            e.Reset();
            e.WriteString(s_long);
            Assert.That(e.Error, Is.Null);
            Assert.That(e.Buffer.Length, Is.EqualTo(130));
            byte[] arr = new[] { e.Buffer[0], e.Buffer[1], e.Buffer[2], e.Buffer[3] };
            Assert.That(arr, Is.EquivalentTo(new byte[] { 0x00, 0x80, (byte)'*', (byte)'*' }));
            Assert.That(e.Buffer[128], Is.EqualTo((byte)'*'));
            Assert.That(e.Buffer[129], Is.EqualTo((byte)'x'));

            e.Reset();
            e.WriteString(s_big);
            Assert.That(e.Error, Is.Null);
            Assert.That(e.Buffer.Length, Is.EqualTo(65537));
            arr = new[] { e.Buffer[0], e.Buffer[1], e.Buffer[2], e.Buffer[3] };
            Assert.That(arr, Is.EquivalentTo(new byte[] { 0xff, 0xff, (byte)'*', (byte)'*' }));
            Assert.That(e.Buffer[65535], Is.EqualTo((byte)'*'));
            Assert.That(e.Buffer[65536], Is.EqualTo((byte)'x'));
        }

        [Test]
        public void TestWriteValueError2()
        {
            WireEncoder e = new WireEncoder(0x0200);
            e.WriteValue(v_empty);
            Assert.That(e.Buffer.Length, Is.EqualTo(0));
            Assert.That(e.Error, Is.Not.Null);

            e.Reset();
            e.WriteValue(v_raw);
            Assert.That(e.Size(), Is.EqualTo(0));
            Assert.That(e.Error, Is.Not.Null);

            e.Reset();
            e.WriteValue(v_rpc);
            Assert.That(e.Size(), Is.EqualTo(0));
            Assert.That(e.Error, Is.Not.Null);
        }

        [Test]
        public void TestWriteValueNull2()
        {
            WireEncoder e = new WireEncoder(0x0200);
            e.WriteValue(null);
            Assert.That(e.Buffer.Length, Is.EqualTo(0));
            Assert.That(e.Error, Is.Not.Null);
        }



        [Test]
        public void TestGetValueSize3()
        {
            WireEncoder e = new WireEncoder(0x0300);
            Assert.That(e.GetValueSize(null), Is.EqualTo(0));
            Assert.That(e.GetValueSize(v_empty), Is.EqualTo(0));
            Assert.That(e.GetValueSize(v_boolean), Is.EqualTo(1));
            Assert.That(e.GetValueSize(v_double), Is.EqualTo(8));
            Assert.That(e.GetValueSize(v_string), Is.EqualTo(6));
            Assert.That(e.GetValueSize(v_raw), Is.EqualTo(6));
            Assert.That(e.GetValueSize(v_rpc), Is.EqualTo(6));

            Assert.That(e.GetValueSize(v_boolArray), Is.EqualTo(1 + 3));
            Assert.That(e.GetValueSize(v_boolArrayBig), Is.EqualTo(1 + 255));
            Assert.That(e.GetValueSize(v_doubleArray), Is.EqualTo(1 + 2 * 8));
            Assert.That(e.GetValueSize(v_doubleArrayBig), Is.EqualTo(1 + 255 * 8));
            Assert.That(e.GetValueSize(v_stringArray), Is.EqualTo(1 + 6 + 8));
            Assert.That(e.GetValueSize(v_stringArrayBig), Is.EqualTo(1 + 255 * 2));
        }

        [Test]
        public void TestWriteBooleanValue3()
        {
            WireEncoder e = new WireEncoder(0x0300);
            e.WriteValue(v_boolean);
            var v_false = Value.MakeBoolean(false);
            e.WriteValue(v_false);
            Assert.That(e.Error, Is.Null);
            Assert.That(e.Buffer.Length, Is.EqualTo(2));
            Assert.That(e.Buffer, Is.EquivalentTo(new byte[] { 0x01, 0x00 }));
        }

        [Test]
        public void TestWriteDoubleValue3()
        {
            WireEncoder e = new WireEncoder(0x0300);
            e.WriteValue(v_double);
            Assert.That(e.Error, Is.Null);
            Assert.That(e.Buffer.Length, Is.EqualTo(8));
            Assert.That(e.Buffer, Is.EquivalentTo(new byte[] { 0x3f, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }));
        }

        [Test]
        public void TestWriteStringValue3()
        {
            WireEncoder e = new WireEncoder(0x0300);
            e.WriteValue(v_string);
            Assert.That(e.Error, Is.Null);
            Assert.That(e.Buffer.Length, Is.EqualTo(6));
            Assert.That(e.Buffer, Is.EquivalentTo(new byte[] { 0x05, (byte)'h', (byte)'e', (byte)'l', (byte)'l', (byte)'o' }));
        }

        [Test]
        public void TestWriteRawArray3()
        {
            WireEncoder e = new WireEncoder(0x0300);
            e.WriteValue(v_raw);
            Assert.That(e.Error, Is.Null);
            Assert.That(e.Buffer.Length, Is.EqualTo(6));
            Assert.That(e.Buffer, Is.EquivalentTo(new byte[] { 0x05, (byte)'h', (byte)'e', (byte)'l', (byte)'l', (byte)'o' }));
        }

        [Test]
        public void TestWriteRpcArray3()
        {
            WireEncoder e = new WireEncoder(0x0300);
            e.WriteValue(v_rpc);
            Assert.That(e.Error, Is.Null);
            Assert.That(e.Buffer.Length, Is.EqualTo(6));
            Assert.That(e.Buffer, Is.EquivalentTo(new byte[] { 0x05, (byte)'h', (byte)'e', (byte)'l', (byte)'l', (byte)'o' }));
        }

        [Test]
        public void TestWriteBooleanArrayValue3()
        {
            WireEncoder e = new WireEncoder(0x0300);
            e.WriteValue(v_boolArray);
            Assert.That(e.Error, Is.Null);
            Assert.That(e.Buffer.Length, Is.EqualTo(1 + 3));
            Assert.That(e.Buffer, Is.EquivalentTo(new byte[] { 0x03, 0x00, 0x01, 0x00 }));

            e.Reset();
            e.WriteValue(v_boolArrayBig);
            Assert.That(e.Error, Is.Null);
            Assert.That(e.Buffer.Length, Is.EqualTo(1 + 255));
            Assert.That(new[] { e.Buffer[0], e.Buffer[1] }, Is.EquivalentTo(new byte[] { 0xff, 0x00 }));
        }

        [Test]
        public void TestWriteDoubleArrayValue3()
        {
            WireEncoder e = new WireEncoder(0x0300);
            e.WriteValue(v_doubleArray);
            Assert.That(e.Error, Is.Null);
            Assert.That(e.Buffer.Length, Is.EqualTo(1 + 2 * 8));
            Assert.That(e.Buffer, Is.EquivalentTo(new byte[]
            {
                0x02, 0x3f, 0xe0, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x3f, 0xd0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            }));

            e.Reset();
            e.WriteValue(v_doubleArrayBig);
            Assert.That(e.Error, Is.Null);
            Assert.That(e.Buffer.Length, Is.EqualTo(1 + 255 * 8));
            Assert.That(new[] { e.Buffer[0], e.Buffer[1] }, Is.EquivalentTo(new byte[] { 0xff, 0x00 }));
        }

        [Test]
        public void TestWriteStringArrayValue3()
        {
            WireEncoder e = new WireEncoder(0x0300);
            e.WriteValue(v_stringArray);
            Assert.That(e.Error, Is.Null);
            Assert.That(e.Buffer.Length, Is.EqualTo(1 + 6 + 8));
            Assert.That(e.Buffer, Is.EquivalentTo(new byte[]
            {
                0x02, 0x05, (byte)'h', (byte)'e', (byte)'l', (byte)'l', (byte)'o',
                0x07, (byte)'g', (byte)'o', (byte)'o', (byte)'d', (byte)'b', (byte)'y', (byte)'e'
            }));

            e.Reset();
            e.WriteValue(v_stringArrayBig);
            Assert.That(e.Error, Is.Null);
            Assert.That(e.Buffer.Length, Is.EqualTo(1 + 255 * 2));
            Assert.That(new[] { e.Buffer[0], e.Buffer[1] }, Is.EquivalentTo(new byte[] { 0xff, 0x01 }));
        }

        [Test]
        public void TestGetStringSize3()
        {
            WireEncoder e = new WireEncoder(0x0300);
            Assert.That(e.GetStringSize(s_normal), Is.EqualTo(6));
            Assert.That(e.GetStringSize(s_long), Is.EqualTo(130));
            Assert.That(e.GetStringSize(s_big), Is.EqualTo(65540));
        }

        [Test]
        public void TestWriteString3()
        {
            WireEncoder e = new WireEncoder(0x0300);
            e.WriteString(s_normal);
            Assert.That(e.Error, Is.Null);
            Assert.That(e.Buffer.Length, Is.EqualTo(6));
            Assert.That(e.Buffer, Is.EquivalentTo(new byte[] { 0x05, (byte)'h', (byte)'e', (byte)'l', (byte)'l', (byte)'o' }));

            e.Reset();
            e.WriteString(s_long);
            Assert.That(e.Error, Is.Null);
            Assert.That(e.Buffer.Length, Is.EqualTo(130));
            byte[] arr = new[] { e.Buffer[0], e.Buffer[1], e.Buffer[2], e.Buffer[3] };
            Assert.That(arr, Is.EquivalentTo(new byte[] { 0x80, 0x01, (byte)'*', (byte)'*' }));
            Assert.That(e.Buffer[128], Is.EqualTo((byte)'*'));
            Assert.That(e.Buffer[129], Is.EqualTo((byte)'x'));

            e.Reset();
            e.WriteString(s_big);
            Assert.That(e.Error, Is.Null);
            Assert.That(e.Buffer.Length, Is.EqualTo(65540));
            arr = new[] { e.Buffer[0], e.Buffer[1], e.Buffer[2], e.Buffer[3] };
            Assert.That(arr, Is.EquivalentTo(new byte[] { 0x81, 0x80, 0x04, (byte)'*' }));
            Assert.That(e.Buffer[65536], Is.EqualTo((byte)'*'));
            Assert.That(e.Buffer[65537], Is.EqualTo((byte)'x'));
            Assert.That(e.Buffer[65538], Is.EqualTo((byte)'x'));
            Assert.That(e.Buffer[65539], Is.EqualTo((byte)'x'));
        }

        [Test]
        public void TestWriteValueError3()
        {
            WireEncoder e = new WireEncoder(0x0300);
            e.WriteValue(v_empty);
            Assert.That(e.Buffer.Length, Is.EqualTo(0));
            Assert.That(e.Error, Is.Not.Null);
        }

        [Test]
        public void TestWriteValueNull3()
        {
            WireEncoder e = new WireEncoder(0x0300);
            e.WriteValue(null);
            Assert.That(e.Buffer.Length, Is.EqualTo(0));
            Assert.That(e.Error, Is.Not.Null);
        }
    }
}
