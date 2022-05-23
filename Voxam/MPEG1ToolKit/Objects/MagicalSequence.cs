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
    public class MagicalSequence : MPEG1Sequence
    {
        public override double FrameRate { get => LookupFrameRate((byte)(FrameRateCode & 0x07)); }

        public MagicalSequence(IMPEG1Object parent, MPEG1ObjectSource source, int horizontalSize, int verticalSize, byte aspectRatioCode, byte frameRateCode, int bitrate, int vbvBufferSize, bool constrainedParameters, bool hasCustomIntraQuantizerMatrix, bool hasCustomNonIntraQuantizerMatrix)
        :  base(parent, source, horizontalSize, verticalSize, aspectRatioCode, frameRateCode, bitrate, vbvBufferSize, constrainedParameters, hasCustomIntraQuantizerMatrix, hasCustomNonIntraQuantizerMatrix)
        {
            //
        }

        public static new MPEG1Sequence Marshal(MPEG1StreamObjectIterator iter, IMPEG1Object parent = null)
        {
            MPEG1Sequence rv = MPEG1Sequence.Marshal(iter, parent);
            if (rv.FrameRateCode > 8)
            {
                //assumed to be magical...
                rv = new MagicalSequence(rv.Parent, rv.Source, rv.HorizontalSize, rv.VerticalSize, rv.AspectRatioCode, rv.FrameRateCode, rv.Bitrate, rv.VBVBufferSize, rv.ConstrainedParameters, rv.HasCustomIntraQuantizerMatrix, rv.HasCustomNonIntraQuantizerMatrix);
            }
            return rv;
        }

        public override string Name => "Magical Sequence";
    }
}
