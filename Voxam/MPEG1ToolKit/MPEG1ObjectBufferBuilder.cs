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
    public class MPEG1ObjectBufferBuilder
    {
        private readonly byte[] _buffer;

        public byte[] Buffer { get { return _buffer; } }
        public int Length { get { return _buffer.Length; } }

        public MPEG1ObjectBufferBuilder(IMPEG1Object obj)
        {
            var iter = obj.Source.Iterator;
            if (iter == null) throw new Exception("Iterator is null");
            iter.SeekSourceTo(obj.Source.IteratorSourceStreamPosition);

            if (!iter.MPEGObjectValid) throw new Exception("Iterator is not valid");
            if (iter.MPEGObjectType != obj.StreamIdType) throw new Exception("Iterator object stream ID mismatch");
            if (iter.AbsoluteBufferLength != obj.Source.IteratorSourceStreamAbsoluteLength) throw new Exception("Iterator object absolute buffer length mismatch");

            _buffer = readCurrentObject(iter);
        }

        public MPEG1ObjectBufferBuilder(MPEG1StreamObjectIterator iter)
        {
            if (iter == null) throw new Exception("Iterator is null");
            if (!iter.MPEGObjectValid) throw new Exception("Iterator is not valid");

            _buffer = readCurrentObject(iter);
        }

        private static byte[] readCurrentObject(MPEG1StreamObjectIterator iter)
        {
            var rv = new byte[iter.AbsoluteBufferLength];
            Array.Copy(iter.MPEGObjectBuffer, iter.AbsoluteBufferOffset, rv, 0, rv.Length);
            return rv;
        }
    }
}
