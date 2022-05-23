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
    public class MPEG1Sequence : IMPEG1Object
    {
        public const byte STREAM_ID_TYPE = 0xB3;


        private readonly IMPEG1Object _parent;
        private readonly MPEG1ObjectSource _source;

        public readonly int HorizontalSize;
        public readonly int VerticalSize;
        public readonly byte AspectRatioCode;
        public readonly byte FrameRateCode;
        public readonly int Bitrate;
        public readonly int VBVBufferSize;
        public readonly bool ConstrainedParameters;
        public readonly bool HasCustomIntraQuantizerMatrix;
        public readonly bool HasCustomNonIntraQuantizerMatrix;



        public virtual double AspectRatio { get => LookupAspectRatio(AspectRatioCode); }
        public virtual double FrameRate { get => LookupFrameRate(FrameRateCode); }


        public bool Equals(MPEG1Sequence other)
        {
            return 
                (HorizontalSize == other.HorizontalSize) &&
                (VerticalSize == other.VerticalSize) &&
                (AspectRatioCode == other.AspectRatioCode) &&
                (FrameRateCode == other.FrameRateCode) &&
                (Bitrate == other.Bitrate) &&
                (VBVBufferSize == other.VBVBufferSize) &&
                (ConstrainedParameters == other.ConstrainedParameters) &&
                (HasCustomIntraQuantizerMatrix == other.HasCustomIntraQuantizerMatrix) &&
                (HasCustomNonIntraQuantizerMatrix == other.HasCustomNonIntraQuantizerMatrix);
        }



        public MPEG1Sequence(IMPEG1Object parent, MPEG1ObjectSource source, int horizontalSize, int verticalSize, byte aspectRatioCode, byte frameRateCode, int bitrate, int vbvBufferSize, bool constrainedParameters, bool hasCustomIntraQuantizerMatrix, bool hasCustomNonIntraQuantizerMatrix)
        {
            _parent = parent;
            _source = source;
            HorizontalSize = horizontalSize;
            VerticalSize = verticalSize;
            AspectRatioCode = aspectRatioCode;
            FrameRateCode = frameRateCode;
            Bitrate = bitrate;
            VBVBufferSize = vbvBufferSize;
            ConstrainedParameters = constrainedParameters;
            HasCustomIntraQuantizerMatrix = hasCustomIntraQuantizerMatrix;
            HasCustomNonIntraQuantizerMatrix = hasCustomNonIntraQuantizerMatrix;
        }

        public static MPEG1Sequence Marshal(MPEG1StreamObjectIterator iter, IMPEG1Object parent = null)
        {
            if (!iter.MPEGObjectValid) return null;
            if (iter.MPEGObjectType != STREAM_ID_TYPE) return null;

            int len = iter.MPEGObjectContentsBufferLength;
            if (len < 8) return null;
            BitStream bits = new BitStream(iter.MPEGObjectBuffer, iter.MPEGObjectContentsBufferOffset, len);

            int horizontalSize = bits.ReadInt(12);
            int verticalSize = bits.ReadInt(12);
            byte aspectRatioCode = bits.ReadByte(4);
            byte frameRateCode = bits.ReadByte(4);
            int bitrate = bits.ReadInt(18);
            if (!bits.ReadBool()) return null;
            int vbvBufferSize = bits.ReadInt(10);
            bool constrainedParameters = bits.ReadBool(1);
            bool hasCustomIntraQuantizerMatrix = bits.ReadBool(1);
            bool hasCustomNonIntraQuantizerMatrix = bits.ReadBool(1);

            return new MPEG1Sequence(parent, new MPEG1ObjectSource(iter), horizontalSize, verticalSize, aspectRatioCode, frameRateCode, bitrate, vbvBufferSize, constrainedParameters, hasCustomIntraQuantizerMatrix, hasCustomNonIntraQuantizerMatrix);
        }




        public static double LookupAspectRatio(byte aspectRatioCode)
        {
            switch (aspectRatioCode)
            {
                case 1: return 1.00;
                case 2: return 0.6735;
                case 3: return 0.7031; // PAL 16:9
                case 4: return 0.7615;
                case 5: return 0.8055;
                case 6: return 0.8437; // NTSC 16:9
                case 7: return 0.8935;
                case 8: return 0.9375; // PAL 4:3
                case 9: return 0.9815;
                case 10: return 1.0255;
                case 11: return 1.0695;
                case 12: return 1.1250; // NTSC 4:3
                case 13: return 1.1575;
                case 14: return 1.2015;
            }
            return 0.00;
        }
        public static double LookupFrameRate(byte frameRateCode)
        {
            switch (frameRateCode)
            {
                case 1: return 24000.00 / 1001.00;
                case 2: return 24.00;
                case 3: return 25.00;
                case 4: return 30000.00 / 1001.00;
                case 5: return 30.00;
                case 6: return 50.00;
                case 7: return 60000.00 / 1001.00;
                case 8: return 60.00;
            }
            return 0.00;
        }




        public virtual string Name => "MPEG-1 Sequence";

        public IMPEG1Object Parent => _parent;

        public MPEG1ObjectSource Source => _source;

        public byte StreamIdType => STREAM_ID_TYPE;
    }
}
