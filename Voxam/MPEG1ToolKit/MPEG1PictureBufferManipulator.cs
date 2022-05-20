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


using Voxam.MPEG1ToolKit.Objects;

namespace Voxam.MPEG1ToolKit
{
    public class MPEG1PictureBufferManipulator
    {
        private readonly byte[] _buf;
        private readonly int _off;
        private readonly int _len;
        private readonly MPEG1Picture _picture;

        public MPEG1PictureBufferManipulator(byte[] buf, int off, int len)
        {
            _buf = buf;
            _off = off + 4;
            _len = len - 4;
            _picture = MPEG1Picture.Marshal(_buf, _off, _len);
        }

        public void OverrideFCode(byte forwardValue, byte backwardValue)
        {
            if (_picture == null) return;

            forwardValue &= 0x07;
            backwardValue &= 0x07;

            if (_picture.Type == MPEG1Picture.PictureType.Bipredictive)
            {
                _buf[_off + 4] &= 0xC7;
                _buf[_off + 4] |= (byte)(backwardValue << 3);
            }
            if ((_picture.Type == MPEG1Picture.PictureType.Bipredictive) || (_picture.Type == MPEG1Picture.PictureType.Predictive))
            {
                _buf[_off + 3] &= 0xFC;
                _buf[_off + 4] &= 0x7F;
                _buf[_off + 3] |= (byte)(forwardValue >> 1);
                _buf[_off + 4] |= (byte)((forwardValue << 7) & 0x80);
            }
        }
    }
}
