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
using System.Windows.Forms;

using Voxam.MPEG1ToolKit.ReelMagic;

namespace Voxam
{
    public partial class ReelMagicVideoConverterSettings : Form
    {
        private VideoConverterSettings _settings = null;
        public VideoConverterSettings Settings
        {
            get => _settings;
            set
            {
                if (_settings == value) return;
                _settings = value;
                if (this.IsDisposed) return;
                loadFromSettings();
            }
        }

        public ReelMagicVideoConverterSettings(VideoConverterSettings settings = null)
        {
            _settings = settings;

            InitializeComponent();

            _gbFCode.Location = _gbMagicKey.Location;
            loadFromSettings();
        }

        private void loadFromSettings()
        {
            if (_settings == null) return;
            switch (_settings.DecodeMode)
            {
                case VideoConverterSettings.Mode.NONE:
                    _cboDecodeMode.SelectedIndex = 0;
                    break;
                case VideoConverterSettings.Mode.SEEK_TRUTHFUL_FCODE:
                    _cboDecodeMode.SelectedIndex = 1;
                    break;
                case VideoConverterSettings.Mode.APPLY_STATIC_FCODE:
                    _cboDecodeMode.SelectedIndex = 2;
                    break;
            }

            switch(_settings.MagicKey)
            {
                case VideoConverterSettings.MAGIC_KEY_40044041:
                    _rbMagicKey40044041.Checked = true;
                    break;
                case VideoConverterSettings.MAGIC_KEY_C39D7088:
                    _rbMagicKeyC39D7088.Checked = true;
                    break;
            }

            _nudPPictureForwardFCode.Value = _settings.StaticPForwardFCode;
            _nudBPictureForwardFCode.Value = _settings.StaticBForwardFCode;
            _nudBPictureBackwardFCode.Value = _settings.StaticBBackwardFCode;
        }

        private void _cboDecodeMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            _gbMagicKey.Visible = false;
            _gbFCode.Visible = false;

            switch (_cboDecodeMode.SelectedIndex)
            {
                case 0:
                    if (_settings != null) _settings.DecodeMode = VideoConverterSettings.Mode.NONE;
                    break;
                case 1:
                    if (_settings != null) _settings.DecodeMode = VideoConverterSettings.Mode.SEEK_TRUTHFUL_FCODE;
                    _gbMagicKey.Visible = true;
                    break;
                case 2:
                    if (_settings != null) _settings.DecodeMode = VideoConverterSettings.Mode.APPLY_STATIC_FCODE;
                    _gbFCode.Visible = true;
                    break;
            }
        }

        private void _rbMagicKey_CheckedChanged(object sender, EventArgs e)
        {
            if (_settings == null) return;
            if (sender == _rbMagicKey40044041) _settings.MagicKey = VideoConverterSettings.MAGIC_KEY_40044041;
            if (sender == _rbMagicKeyC39D7088) _settings.MagicKey = VideoConverterSettings.MAGIC_KEY_C39D7088;
        }

        private void _nudFCode_ValueChanged(object sender, EventArgs e)
        {
            if (_settings == null) return;
            _settings.StaticPForwardFCode = (byte)_nudPPictureForwardFCode.Value;
            _settings.StaticBForwardFCode = (byte)_nudBPictureForwardFCode.Value;
            _settings.StaticBBackwardFCode = (byte)_nudBPictureBackwardFCode.Value;
        }
    }
}
