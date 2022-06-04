/*
 *  Copyright (C) 2022 Jon Dennis
 *
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.
 */

using System;
using System.IO;

using Voxam.MPEG1ToolKit.Objects;

namespace Voxam.MPEG1ToolKit.Streams
{
    public class IterWriter : IDisposable
    {
        private readonly MPEG1StreamObjectIterator _originalSource;
        private readonly MPEG1StreamObjectIterator _source;
        private readonly FileStream _output;

        public IterWriter(MPEG1StreamObjectIterator source, FileStream output)
        {
            _originalSource = source;
            _source = _originalSource.Duplicate(0);
            _output = output;
        }

        public void Dispose()
        {
            _source.Dispose();
            _output.Dispose();
        }

        public void Overwrite(IMPEG1Object obj, byte[] buf, int off, int len)
        {
            if (len > obj.Source.IteratorSourceStreamAbsoluteLength) throw new Exception("len > object max len");

            foreach (var segment in new MPEG1ObjectSuperstreamPositionMap(_originalSource, obj))
            {
                if (len < 1) break;

                int writeLen = len;
                if (writeLen > segment.Length) writeLen = segment.Length;

                WriteUntil(segment.Offset);
                _output.Write(buf, off, writeLen);
                _source.SeekSourceTo(writeLen, SeekOrigin.Current);

                off += writeLen;
                len -= writeLen;
            }
        }

        public void WriteUntil(IMPEG1Object obj)
        {
            foreach (var segment in new MPEG1ObjectSuperstreamPositionMap(_originalSource, obj))
            {
                WriteUntil(segment.Offset);
                break;
            }
        }

        public void WriteUntil(long sourceStreamPosition)
        {
            if (_source.MPEGObjectSourceStreamPosition > sourceStreamPosition) throw new Exception("Source Stream Position Beyond Written");
            while ((!_source.EndOfStream) && (_source.MPEGObjectSourceStreamPosition < sourceStreamPosition))
            {
                int writeSize = _source.AbsoluteBufferLength;
                if (writeSize > (sourceStreamPosition - _source.MPEGObjectSourceStreamPosition))
                    writeSize = (int)(sourceStreamPosition - _source.MPEGObjectSourceStreamPosition);

                _output.Write(_source.MPEGObjectBuffer, _source.AbsoluteBufferOffset, writeSize);
                _source.SeekSourceTo(writeSize, SeekOrigin.Current);
            }
        }

        public void WriteRemaining()
        {
            for (; !_source.EndOfStream; _source.Next())
                _output.Write(_source.MPEGObjectBuffer, _source.AbsoluteBufferOffset, _source.AbsoluteBufferLength);
        }
    }
}
