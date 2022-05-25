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

namespace Voxam.MPEG1ToolKit.ReelMagic
{
    public class VideoConverter
    {
        private readonly VideoConverterSettings _settings;
        private readonly IMPEG1PictureCollection _pictures;
        private readonly int _firstPictureIndex; //belonging to sequence

        public VideoConverter(VideoConverterSettings settings, IMPEG1PictureCollection pictures, MagicalSequence sequence)
        {
            _settings = settings;
            _pictures = pictures;
            _firstPictureIndex = 0;
            for (int i = 0; i < _pictures.Count; ++i)
            {
                for (IMPEG1Object obj = _pictures[i]; obj != null; obj = obj.Parent)
                {
                    if (obj == sequence)
                    {
                        _firstPictureIndex = i;
                        return;
                    }
                }
            }
        }


        public bool PictureContainsTruthfulFCode(MPEG1Picture picture)
        {
            if (_settings.DecodeMode != VideoConverterSettings.Mode.SEEK_TRUTHFUL_FCODE) return false;
            if ((picture.Type != MPEG1Picture.PictureType.Predictive) && (picture.Type != MPEG1Picture.PictureType.Bipredictive))
                return false;
            switch (_settings.MagicKey)
            {
                case VideoConverterSettings.MAGIC_KEY_40044041:
                    if ((picture.TemporalSequenceNumber == 3) || (picture.TemporalSequenceNumber == 8))
                        return true;
                    break;
                case VideoConverterSettings.MAGIC_KEY_C39D7088:
                    if (picture.TemporalSequenceNumber == 4)
                        return true;
                    break;
            }

            return false;
        }

        private MPEG1Picture seekFirstTruthfulPicture()
        {
            for (int i = _firstPictureIndex; i < _pictures.Count; ++i)
            {
                var picture = _pictures[i];
                if (PictureContainsTruthfulFCode(picture))
                    return picture;
            }
            return null;
        }


        private int _cachedFirstTruthfulPictureSettingsSN = -1;
        private MPEG1Picture _cachedFirstTruthfulPicture = null;
        private MPEG1Picture seekCachedFirstTruthfulPicture()
        {
            if (_cachedFirstTruthfulPictureSettingsSN != _settings.LastUpdateSN)
            {
                _cachedFirstTruthfulPicture = seekFirstTruthfulPicture();
                _cachedFirstTruthfulPictureSettingsSN = _settings.LastUpdateSN;
            }
            return _cachedFirstTruthfulPicture;
        }

        public void PatchPicture(MPEG1Picture picture, byte[] buf, int off, int len)
        {
            //note: this routine expects buf[off] to be positioned at 00 00 01 00 ...
            switch (_settings.DecodeMode)
            {
                case VideoConverterSettings.Mode.SEEK_TRUTHFUL_FCODE:
                    patchPictureSeekTruthfulFCode(picture, buf, off, len);
                    return;
                case VideoConverterSettings.Mode.APPLY_STATIC_FCODE:
                    patchPictureApplyStaticFCode(picture, buf, off, len);
                    return;
                case VideoConverterSettings.Mode.CUSTOM:
                    _settings.InvokeCustomPatchPictureEvent(this, picture, buf, off, len);
                    return;
            }
        }
        private void patchPictureSeekTruthfulFCode(MPEG1Picture picture, byte[] buf, int off, int len)
        {
            if (len < 9) return;
            MPEG1Picture pictureOfTruth = seekCachedFirstTruthfulPicture();
            if (pictureOfTruth == null) return;

            byte fcodeOfTruth = pictureOfTruth.ForwardFCode;

            switch (picture.Type)
            {
                case MPEG1Picture.PictureType.Predictive:
                    writePFCode(buf, off, len, fcodeOfTruth);
                    break;
                case MPEG1Picture.PictureType.Bipredictive:
                    writeBFCodes(buf, off, len, fcodeOfTruth, fcodeOfTruth);
                    break;
            }
        }

        private void patchPictureApplyStaticFCode(MPEG1Picture picture, byte[] buf, int off, int len)
        {
            if (len < 9) return;

            switch (picture.Type)
            {
                case MPEG1Picture.PictureType.Predictive:
                    writePFCode(buf, off, len, _settings.StaticPForwardFCode);
                    break;
                case MPEG1Picture.PictureType.Bipredictive:
                    writeBFCodes(buf, off, len, _settings.StaticBForwardFCode, _settings.StaticBBackwardFCode);
                    break;
            }
        }

        private static void writePFCode(byte[] buf, int off, int len, int forwardValue)
        {
            forwardValue &= 0x07;
            buf[off + 7] &= 0xFC;
            buf[off + 8] &= 0x7F;
            buf[off + 7] |= (byte)(forwardValue >> 1);
            buf[off + 8] |= (byte)((forwardValue << 7) & 0x80);
        }
        private static void writeBFCodes(byte[] buf, int off, int len, int forwardValue, int backwardValue)
        {
            writePFCode(buf, off, len, forwardValue);
            backwardValue &= 0x07;
            buf[off + 8] &= 0xC7;
            buf[off + 8] |= (byte)(backwardValue << 3);
        }
    }
}
