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
using Voxam.MPEG1ToolKit.Streams;

namespace Voxam.MPEG1ToolKit.Objects
{
    public class MPEG1Object : IMPEG1Object
    {
        private readonly IMPEG1Object _parent;
        private readonly MPEG1ObjectSource _source;
        private readonly byte _streamIdType;

        public MPEG1Object(IMPEG1Object parent, MPEG1ObjectSource source, byte streamIdType)
        {
            _parent = parent;
            _source = source;
            _streamIdType = streamIdType;
        }

        public static IMPEG1Object Marshal(MPEG1StreamObjectIterator iter, IMPEG1Object parent = null)
        {
            if (!iter.MPEGObjectValid) return null;

            IMPEG1Object rv = null;
            byte streamIdType = iter.MPEGObjectType;
            if (MPEG1PESPacket.IsPESType(streamIdType))
                rv = MPEG1PESPacket.Marshal(iter, parent);
            else if (MPEG1Slice.IsSliceType(streamIdType))
                rv = MPEG1Slice.Marshal(iter, parent);
            else {
                switch (streamIdType)
                {
                    case MPEG1GOP.STREAM_ID_TYPE: rv = MPEG1GOP.Marshal(iter, parent); break;
                    case MPEG1PackHeader.STREAM_ID_TYPE: rv = MPEG1PackHeader.Marshal(iter, parent); break;
                    case MPEG1Picture.STREAM_ID_TYPE: rv = MPEG1Picture.Marshal(iter, parent); break;
                    case MPEG1ProgramEnd.STREAM_ID_TYPE: rv = MPEG1ProgramEnd.Marshal(iter, parent); break;
                    case MPEG1Sequence.STREAM_ID_TYPE: rv = MPEG1Sequence.Marshal(iter, parent); break;
                    case MPEG1SequenceEnd.STREAM_ID_TYPE: rv = MPEG1SequenceEnd.Marshal(iter, parent); break;
                    case MPEG1SystemHeader.STREAM_ID_TYPE: rv = MPEG1SystemHeader.Marshal(iter, parent); break;
                }
            }

            if (rv == null) rv = new MPEG1Object(parent, new MPEG1ObjectSource(iter), streamIdType);
            return rv;
        }

        public string Name => String.Format("MPEG-1 Object ID: 0x{0:2X}", _streamIdType);

        public IMPEG1Object Parent => _parent;

        public MPEG1ObjectSource Source => _source;

        public byte StreamIdType => _streamIdType;
    }
}
