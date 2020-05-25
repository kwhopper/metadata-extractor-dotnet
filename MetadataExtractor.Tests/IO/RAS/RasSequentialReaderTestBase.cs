// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Text;
using MetadataExtractor.IO;
using Xunit;

namespace MetadataExtractor.Tests.IO
{
    /// <summary>Base class for testing implementations of <see cref="RandomAccessStream"/> using sequential reading.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    public abstract class RasSequentialReaderTestBase
    {
        protected abstract ReaderInfo CreateReader(byte[] bytes);

        [Fact]
        public void DefaultEndianness()
        {
            Assert.True(CreateReader(new byte[1]).IsMotorolaByteOrder);
        }

        [Fact]
        public void GetSByte()
        {
            var buffer = new byte[] { 0x00, 0x01, 0x7F, 0xFF };
            var reader = CreateReader(buffer);
            Assert.Equal(0, reader.GetSByte());
            Assert.Equal(1, reader.GetSByte());
            Assert.Equal(127, reader.GetSByte());
            Assert.Equal(-1, reader.GetSByte());
        }

        [Fact]
        public void GetByte()
        {
            var buffer = new byte[] { 0x00, 0x01, 0x7F, 0xFF };
            var reader = CreateReader(buffer);
            Assert.Equal(0, reader.GetByte());
            Assert.Equal(1, reader.GetByte());
            Assert.Equal(127, reader.GetByte());
            Assert.Equal(255, reader.GetByte());
        }

        [Fact]
        public void GetByte_OutOfBounds()
        {
            var reader = CreateReader(new byte[1]);
            reader.GetByte();
            var ex = Assert.Throws<BufferBoundsException>(() => reader.GetByte());
            Assert.Equal("Attempt to read from beyond end of underlying data source (requested index: 1, requested count: 1, max index: 0)", ex.Message);
        }

        [Fact]
        public void GetInt16()
        {
            Assert.Equal(-1, CreateReader(new[] { (byte)0xff, (byte)0xff }).GetInt16());

            var buffer = new byte[] { 0x00, 0x01, 0x7F, 0xFF };

            var reader = CreateReader(buffer);

            Assert.Equal(0x0001, reader.GetInt16());
            Assert.Equal(0x7FFF, reader.GetInt16());

            reader = CreateReader(buffer);
            reader.IsMotorolaByteOrder = false;

            Assert.Equal(0x0100, reader.GetInt16());
            Assert.Equal(unchecked((short)0xFF7F), reader.GetInt16());
        }

        [Fact]
        public void GetUInt16()
        {
            var buffer = new byte[] { 0x00, 0x01, 0x7F, 0xFF };

            var reader = CreateReader(buffer);

            Assert.Equal(0x0001, reader.GetUInt16());
            Assert.Equal(0x7FFF, reader.GetUInt16());

            reader = CreateReader(buffer);
            reader.IsMotorolaByteOrder = false;

            Assert.Equal(0x0100, reader.GetUInt16());
            Assert.Equal(0xFF7F, reader.GetUInt16());
        }

        [Fact]
        public void GetUInt16_OutOfBounds()
        {
            var reader = CreateReader(new byte[1]);
            var ex = Assert.Throws<BufferBoundsException>(() => reader.GetUInt16());
            Assert.Equal("Attempt to read from beyond end of underlying data source (requested index: 0, requested count: 2, max index: 0)", ex.Message);
        }

        [Fact]
        public void GetInt32()
        {
            Assert.Equal(-1, CreateReader(new[] { (byte)0xff, (byte)0xff, (byte)0xff, (byte)0xff }).GetInt32());

            var buffer = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 };

            var reader = CreateReader(buffer);

            Assert.Equal(0x00010203, reader.GetInt32());
            Assert.Equal(0x04050607, reader.GetInt32());

            reader = CreateReader(buffer);
            reader.IsMotorolaByteOrder = false;

            Assert.Equal(0x03020100, reader.GetInt32());
            Assert.Equal(0x07060504, reader.GetInt32());
        }

        [Fact]
        public void GetUInt32()
        {
            Assert.Equal(4294967295u, CreateReader(new byte[] { 0xff, 0xff, 0xff, 0xff }).GetUInt32());

            var buffer = new byte[] { 0xFF, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 };

            var reader = CreateReader(buffer);

            Assert.Equal(0xFF000102u, reader.GetUInt32());
            Assert.Equal(0x03040506u, reader.GetUInt32());

            reader = CreateReader(buffer);
            reader.IsMotorolaByteOrder = false;

            Assert.Equal(0x020100FFu, reader.GetUInt32());
            // 0x0010200FF
            Assert.Equal(0x06050403u, reader.GetUInt32());
        }

        [Fact]
        public void GetInt32_OutOfBounds()
        {
            var reader = CreateReader(new byte[3]);
            var ex = Assert.Throws<BufferBoundsException>(() => reader.GetInt32());
            Assert.Equal("Attempt to read from beyond end of underlying data source (requested index: 0, requested count: 4, max index: 2)", ex.Message);
        }

        [Fact]
        public void GetInt64()
        {
            var buffer = new byte[] { 0xFF, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 };

            var reader = CreateReader(buffer);

            Assert.Equal(unchecked((long)0xFF00010203040506UL), (object)reader.GetInt64());

            reader = CreateReader(buffer);
            reader.IsMotorolaByteOrder = false;

            Assert.Equal(0x06050403020100FFL, (object)reader.GetInt64());
        }

        [Fact]
        public void GetInt64_OutOfBounds()
        {
            var reader = CreateReader(new byte[7]);
            var ex = Assert.Throws<BufferBoundsException>(() => reader.GetInt64());
            Assert.Equal("Attempt to read from beyond end of underlying data source (requested index: 0, requested count: 8, max index: 6)", ex.Message);
        }

        [Fact]
        public void GetUInt64()
        {
            var buffer = new byte[] { 0xFF, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 };

            var reader = CreateReader(buffer);

            Assert.Equal(0xFF00010203040506UL, (object)reader.GetUInt64());

            reader = CreateReader(buffer);
            reader.IsMotorolaByteOrder = false;

            Assert.Equal(0x06050403020100FFUL, (object)reader.GetUInt64());
        }

        [Fact]
        public void GetUInt64_OutOfBounds()
        {
            var reader = CreateReader(new byte[7]);
            var ex = Assert.Throws<BufferBoundsException>(() => reader.GetUInt64());
            Assert.Equal("Attempt to read from beyond end of underlying data source (requested index: 0, requested count: 8, max index: 6)", ex.Message);
        }

        [Fact]
        public void GetFloat32()
        {
            const int nanBits = 0x7fc00000;
            Assert.True(float.IsNaN(BitConverter.ToSingle(BitConverter.GetBytes(nanBits), 0)));

            var reader = CreateReader(new byte[] { 0x7f, 0xc0, 0x00, 0x00 });
            Assert.True(float.IsNaN(reader.GetFloat32()));
        }

        [Fact]
        public void GetFloat64()
        {
            const long nanBits = unchecked((long)0xfff0000000000001L);
            Assert.True(double.IsNaN(BitConverter.Int64BitsToDouble(nanBits)));

            var reader = CreateReader(new byte[] { 0xff, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 });
            Assert.True(double.IsNaN(reader.GetDouble64()));
        }

        [Fact]
        public void GetNullTerminatedString()
        {
            var bytes = new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47 };

            // Test max length
            for (var i = 0; i < bytes.Length; i++)
                Assert.Equal("ABCDEFG".Substring(0, i - 0), CreateReader(bytes).GetNullTerminatedString(i));

            Assert.Equal(string.Empty, CreateReader(new byte[] { 0 }).GetNullTerminatedString(10));
            Assert.Equal("A", CreateReader(new byte[] { 0x41, 0 }).GetNullTerminatedString(10));
            Assert.Equal("AB", CreateReader(new byte[] { 0x41, 0x42, 0 }).GetNullTerminatedString(10));
            Assert.Equal("AB", CreateReader(new byte[] { 0x41, 0x42, 0, 0x43 }).GetNullTerminatedString(10));
        }

        [Fact]
        public void GetString()
        {
            var bytes = new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47 };
            var expected = Encoding.UTF8.GetString(bytes);

            Assert.Equal(bytes.Length, expected.Length);

            for (var i = 0; i < bytes.Length; i++)
                Assert.Equal("ABCDEFG".Substring(0, i - 0), CreateReader(bytes).GetString(i, Encoding.UTF8));
        }

        [Fact]
        public void GetBytes()
        {
            var bytes = new byte[] { 0, 1, 2, 3, 4, 5 };
            for (var i = 0; i < bytes.Length; i++)
            {
                var reader = CreateReader(bytes);
                var readBytes = reader.GetBytes(i);
                Assert.Equal(bytes.Take(i).ToArray(), readBytes);
            }
        }

        [Fact]
        public void OverflowBoundsCalculation()
        {
            var reader = CreateReader(new byte[10]);
            var ex = Assert.Throws<BufferBoundsException>(() => reader.GetBytes(15));
            Assert.Equal("Attempt to read from beyond end of underlying data source (requested index: 0, requested count: 15, max index: 9)", ex.Message);
        }

        [Fact]
        public void GetBytesEof()
        {
            CreateReader(new byte[50]).GetBytes(50);

            var reader = CreateReader(new byte[50]);
            reader.GetBytes(25);
            reader.GetBytes(25);

            Assert.Throws<BufferBoundsException>(() => CreateReader(new byte[50]).GetBytes(51));
        }

        [Fact]
        public void GetByteEof()
        {
            CreateReader(new byte[1]).GetByte();

            var reader = CreateReader(new byte[2]);
            reader.GetByte();
            reader.GetByte();

            reader = CreateReader(new byte[1]);
            reader.GetByte();
            Assert.Throws<BufferBoundsException>(() => reader.GetByte());
        }

        [Fact]
        public void SkipEof()
        {
            CreateReader(new byte[1]).Skip(1);

            var reader = CreateReader(new byte[2]);
            reader.Skip(1);
            reader.Skip(1);

            reader = CreateReader(new byte[1]);
            reader.Skip(1);
            Assert.Throws<BufferBoundsException>(() => reader.Skip(1));
        }

        [Fact]
        public void TrySkipEof()
        {
            Assert.True(CreateReader(new byte[1]).TrySkip(1));

            var reader = CreateReader(new byte[2]);
            Assert.True(reader.TrySkip(1));
            Assert.True(reader.TrySkip(1));
            Assert.False(reader.TrySkip(1));
        }
    }
}