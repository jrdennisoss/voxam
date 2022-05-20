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
    public class MPEG1Picture : IMPEG1Object
    {
        public const byte STREAM_ID_TYPE = 0x00;

        public enum PictureType
        {
            Invalid,
            IntraCoded,
            Predictive,
            Bipredictive,
            DirectCoded,
        }

        public readonly PictureType Type;




        private readonly IMPEG1Object _parent;
        private readonly MPEG1ObjectSource _source;

        public MPEG1Picture(IMPEG1Object parent, MPEG1ObjectSource source, PictureType type)
        {
            _parent = parent;
            _source = source;
            Type = type;
        }

        public static MPEG1Picture Marshal(MPEG1StreamObjectIterator iter, IMPEG1Object parent = null)
        {
            if (!iter.MPEGObjectValid) return null;
            if (iter.MPEGObjectType != STREAM_ID_TYPE) return null;

            return Marshal(iter.MPEGObjectBuffer, iter.MPEGObjectContentsBufferOffset, iter.MPEGObjectContentsBufferLength, iter, parent);

        }

        public static MPEG1Picture Marshal(byte[] buf, int off, int len)
        {
            return Marshal(buf, off, len, null, null);
        }


        private static MPEG1Picture Marshal(byte[] buf, int off, int len, MPEG1StreamObjectIterator iter, IMPEG1Object parent)
        {
            if (len < 4) return null;

            PictureType pictureType = decodePictureType((buf[off + 1] >> 3) & 0x07);
            if (pictureType == PictureType.Invalid) return null;

            if ((pictureType == PictureType.Predictive) || pictureType == PictureType.Bipredictive)
                if (len < 5) return null;

            return new MPEG1Picture(parent, (iter != null) ? new MPEG1ObjectSource(iter) : null, pictureType);
        }

        private static PictureType decodePictureType(int encodedPictureType)
        {
            switch (encodedPictureType)
            {
                case 1: return PictureType.IntraCoded;
                case 2: return PictureType.Predictive;
                case 3: return PictureType.Bipredictive;
                case 4: return PictureType.DirectCoded;
            }
            return PictureType.Invalid;
        }





        //
        //IMPEG1Object Interface:
        //
        public string Name => "MPEG-1 Picture";

        public IMPEG1Object Parent => _parent;

        public MPEG1ObjectSource Source => _source;

        public byte StreamIdType => STREAM_ID_TYPE;
    }
}
