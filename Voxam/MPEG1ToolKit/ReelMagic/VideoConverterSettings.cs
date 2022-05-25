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

namespace Voxam.MPEG1ToolKit.ReelMagic
{
    public class VideoConverterSettings
    {
        public delegate void CustomPatchPictureEventHandler(VideoConverter converter, MPEG1Picture picture, byte[] buf, int off, int len);
        public event CustomPatchPictureEventHandler CustomPatchPictureEvent;
        public void InvokeCustomPatchPictureEvent(VideoConverter converter, MPEG1Picture picture, byte[] buf, int off, int len)
        {
            CustomPatchPictureEvent?.Invoke(converter, picture, buf, off, len);
        }

        public enum Mode
        {
            NONE,
            SEEK_TRUTHFUL_FCODE,
            APPLY_STATIC_FCODE,
            CUSTOM,
        }

        private Mode _mode = Mode.SEEK_TRUTHFUL_FCODE;
        public Mode DecodeMode
        {
            get => _mode;
            set
            {
                if (_mode == value) return;
                _mode = value;
                updated();
            }
        }

        public const UInt32 MAGIC_KEY_40044041 = 0x40044041;
        public const UInt32 MAGIC_KEY_C39D7088 = 0xC39D7088;

        private UInt32 _magicKey = MAGIC_KEY_40044041;
        public UInt32 MagicKey
        {
            get => _magicKey;
            set
            {
                if (_magicKey == value) return;
                _magicKey = value;
                updated();
            }
        }

        private byte _staticPForwardFCode = 0x01;
        private byte _staticBForwardFCode = 0x01;
        private byte _staticBBackwardFCode = 0x01;

        public byte StaticAllFCode
        {
            set
            {
                _staticPForwardFCode = value;
                _staticBForwardFCode = value;
                _staticBBackwardFCode = value;
                updated();
            }
        }
        public byte StaticPForwardFCode
        {
            get => _staticPForwardFCode;
            set { _staticPForwardFCode = value; updated(); }
        }
        public byte StaticBForwardFCode
        {
            get => _staticBForwardFCode;
            set { _staticBForwardFCode = value; updated(); }
        }
        public byte StaticBBackwardFCode
        {
            get => _staticBBackwardFCode;
            set { _staticBBackwardFCode = value; updated(); }
        }


        private int _lastUpdateSN = 0;
        public int LastUpdateSN { get => _lastUpdateSN; }
        private void updated() => ++_lastUpdateSN;
    }
}
