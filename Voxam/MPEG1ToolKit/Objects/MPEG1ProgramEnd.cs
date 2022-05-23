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
    public class MPEG1ProgramEnd : IMPEG1Object
    {
        public const byte STREAM_ID_TYPE = 0xB9;

        private readonly IMPEG1Object _parent;
        private readonly MPEG1ObjectSource _source;

        public MPEG1ProgramEnd(IMPEG1Object parent, MPEG1ObjectSource source)
        {
            _parent = parent;
            _source = source;
        }

        public static MPEG1ProgramEnd Marshal(MPEG1StreamObjectIterator iter, IMPEG1Object parent = null)
        {
            if (!iter.MPEGObjectValid) return null;
            if (iter.MPEGObjectType != STREAM_ID_TYPE) return null;

            return new MPEG1ProgramEnd(parent, new MPEG1ObjectSource(iter));
        }

        public string Name => "MPEG-1 Program End";

        public IMPEG1Object Parent => _parent;

        public MPEG1ObjectSource Source => _source;

        public byte StreamIdType => STREAM_ID_TYPE;
    }
}
