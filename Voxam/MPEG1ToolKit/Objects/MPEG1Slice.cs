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


using Voxam.MPEG1ToolKit.Streams;

namespace Voxam.MPEG1ToolKit.Objects
{
    public class MPEG1Slice : IMPEG1Object
    {
        private readonly IMPEG1Object _parent;
        private readonly MPEG1ObjectSource _source;
        private readonly byte _streamIdType;

        public string Name => "MPEG-1 Slice";
        public IMPEG1Object Parent => _parent;
        public MPEG1ObjectSource Source => _source;
        public byte StreamIdType => _streamIdType;
        public static bool IsSliceType(byte streamIdType)
        {
            return (streamIdType >= 0x01) && (streamIdType <= 0xAF);
        }

        public MPEG1Slice(IMPEG1Object parent, MPEG1ObjectSource source, byte streamIdType)
        {
            _parent = parent;
            _source = source;
            _streamIdType = streamIdType;
        }

        public static MPEG1Slice Marshal(MPEG1StreamObjectIterator iter, IMPEG1Object parent = null)
        {
            if (!iter.MPEGObjectValid) return null;
            byte streamIdType = iter.MPEGObjectType;
            if (!IsSliceType(streamIdType)) return null;
            return new MPEG1Slice(parent, new MPEG1ObjectSource(iter), streamIdType);
        }

    }
}
