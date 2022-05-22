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

using Voxam.MPEG1ToolKit.Objects;
using Voxam.MPEG1ToolKit.Streams;

namespace Voxam.MPEG1ToolKit
{
    public class MPEG1PictureBufferBuilder
    {
        private const int ALLOC_CHUNK_SIZE = 512 * 1024;
        byte[] _buffer = new byte[ALLOC_CHUNK_SIZE];
        int _length = 0;

        public byte[] Buffer { get { return _buffer; } }
        public int Length { get { return _length; } }

        public MPEG1PictureBufferBuilder(MPEG1StreamObjectIterator iter)
        {
            if (iter == null) throw new Exception("Iterator is null");
            if (!iter.MPEGObjectValid) throw new Exception("Iterator is not valid");
            if (iter.MPEGObjectType != MPEG1Picture.STREAM_ID_TYPE) throw new Exception("Iterator is not positioned on a picture.");

            copyInObject(iter);
            while(iter.Next())
            {
                if (iter.MPEGObjectValid) 
                    switch(iter.MPEGObjectType)
                    {
                        case MPEG1Picture.STREAM_ID_TYPE: return;
                        case MPEG1GOP.STREAM_ID_TYPE: return;
                        case MPEG1Sequence.STREAM_ID_TYPE: return;
                        case MPEG1SequenceEnd.STREAM_ID_TYPE: return;
                    }
                copyInObject(iter);
            }
        }

        private void copyInObject(MPEG1StreamObjectIterator iter)
        {
            while ((_buffer.Length - _length) < iter.AbsoluteBufferLength)
                extendBuffer();
            Array.Copy(iter.MPEGObjectBuffer, iter.AbsoluteBufferOffset, _buffer, _length, iter.AbsoluteBufferLength);
            _length += iter.AbsoluteBufferLength;
        }

        private void extendBuffer()
        {
            byte[] newBuffer = new byte[_buffer.Length + ALLOC_CHUNK_SIZE];
            Array.Copy(_buffer, newBuffer, _length);
            _buffer = newBuffer;
        }
    }
}
