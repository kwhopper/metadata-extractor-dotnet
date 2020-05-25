﻿// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.IO;

namespace MetadataExtractor.Tests.IO
{
    internal class NonSeekableStream : Stream
    {
        Stream m_stream;
        internal NonSeekableStream(Stream baseStream)
        {
            m_stream = baseStream;
        }

        public override bool CanRead => m_stream.CanRead;

        public override bool CanSeek => false;

        public override bool CanWrite => m_stream.CanWrite;

        public override long Length => throw new NotSupportedException();

        public override long Position { get => m_stream.Position; set => throw new NotSupportedException(); }

        public override void Flush()
        {
            m_stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return m_stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            m_stream.Write(buffer, offset, count);
        }
    }
}