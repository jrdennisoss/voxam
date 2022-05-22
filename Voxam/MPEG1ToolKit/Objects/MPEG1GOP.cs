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
    public class MPEG1GOP : IMPEG1Object
    {
        public const byte STREAM_ID_TYPE = 0xB8;

        private readonly IMPEG1Object _parent;
        private readonly MPEG1ObjectSource _source;

        public readonly bool DropFrame;
        public readonly byte Hour;
        public readonly byte Minute;
        public readonly byte Second;
        public readonly byte Frame;
        public readonly bool Closed;
        public readonly bool Broken;


        public MPEG1GOP(IMPEG1Object parent, MPEG1ObjectSource source, bool dropFrame, byte hour, byte minute, byte second, byte frame, bool closed, bool broken)
        {
            _parent = parent;
            _source = source;

            DropFrame = dropFrame;
            Hour = hour;
            Minute = minute;
            Second = second;
            Frame = frame;
            Closed = closed;
            Broken = broken;
        }

        public static MPEG1GOP Marshal(MPEG1StreamObjectIterator iter, IMPEG1Object parent = null)
        {
            if (!iter.MPEGObjectValid) return null;
            if (iter.MPEGObjectType != STREAM_ID_TYPE) return null;

            int len = iter.MPEGObjectContentsBufferLength;
            if (len < 4) return null;
            BitStream bits = new BitStream(iter.MPEGObjectBuffer, iter.MPEGObjectContentsBufferOffset, len);

            bool dropFrame = bits.ReadBool(1);
            byte hour = bits.ReadByte(5);
            byte minute = bits.ReadByte(6);
            if (!bits.ReadBool(1)) return null;
            byte second = bits.ReadByte(6);
            byte frame = bits.ReadByte(6);
            bool closed = bits.ReadBool(1);
            bool broken = bits.ReadBool(1);

            return new MPEG1GOP(parent, new MPEG1ObjectSource(iter), dropFrame, hour, minute, second, frame, closed, broken);
        }


        public string Name => "MPEG-1 Group of Pictures";

        public IMPEG1Object Parent => _parent;

        public MPEG1ObjectSource Source => _source;

        public byte StreamIdType => STREAM_ID_TYPE;
    }
}
