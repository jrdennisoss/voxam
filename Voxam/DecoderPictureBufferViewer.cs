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


using System.Drawing;
using System.Windows.Forms;

using Voxam.MPEG1ToolKit;

namespace Voxam
{
    public partial class DecoderPictureBufferViewer : Form
    {
        private PLMPEG.AutoVESImageDecoder _decoder = null;

        public PLMPEG.AutoVESImageDecoder Decoder
        {
            get
            {
                return _decoder;
            }
            set
            {
                detachDecoder();
                _decoder = value;
                attachDecoder();
            }
        }


        public DecoderPictureBufferViewer()
        {
            InitializeComponent();
            this._decoder_DecoderChangedEvent(_decoder);
        }

        private void detachDecoder()
        {
            if (_decoder == null) return;
            _decoder.DecoderChangedEvent -= _decoder_DecoderChangedEvent;
            _decoder.PictureDecodedEvent -= _decoder_PictureDecodedEvent;
        }

        private void attachDecoder()
        {
            if (_decoder == null)
            {
                _decoder_DecoderChangedEvent(_decoder);
                return;
            }
            _decoder.DecoderChangedEvent += _decoder_DecoderChangedEvent;
            _decoder.PictureDecodedEvent += _decoder_PictureDecodedEvent;
            _decoder_DecoderChangedEvent(_decoder);
        }


        private void _decoder_DecoderChangedEvent(object sender)
        {
            if (sender != _decoder) return;

            _pbCurrent.Image = _pbForward.Image = _pbBackward.Image = null;
            if (_decoder != null)
            {
                _pbCurrent.Image = _decoder.CurrentPictureBuffer;
                _pbForward.Image = _decoder.ForwardPictureBuffer;
                _pbBackward.Image = _decoder.BackwardPictureBuffer;
            }

            if ((_pbCurrent.Image != null) && (_pbForward.Image != null) && (_pbBackward.Image != null))
            {
                //decoder is in a valid state... lay things out accordingly...
                _lblCurrent.Visible = _lblForward.Visible = _lblBackward.Visible = true;
                _pbCurrent.Visible = _pbForward.Visible = _pbBackward.Visible = true;
                _lblNoBuffers.Visible = false;

                _lblCurrent.Top = _lblForward.Top = _lblBackward.Top = 0;
                _pbCurrent.Top = _pbForward.Top = _pbBackward.Top = _lblCurrent.Bottom;
                _pbCurrent.Width = _pbForward.Width = _pbBackward.Width = _pbCurrent.Image.Width;
                _pbCurrent.Height = _pbForward.Height = _pbBackward.Height = _pbCurrent.Image.Height;

                _pbForward.Left = 0;
                _lblForward.Left = _pbForward.Left + (_pbForward.Width / 2) - (_lblForward.Width / 2);

                _pbCurrent.Left = _pbForward.Right;
                _lblCurrent.Left = _pbCurrent.Left + (_pbCurrent.Width / 2) - (_lblCurrent.Width / 2);

                _pbBackward.Left = _pbCurrent.Right;
                _lblBackward.Left = _pbBackward.Left + (_pbBackward.Width / 2) - (_lblBackward.Width / 2);

                _lblPointer.Top = _pbCurrent.Bottom;

                this.Size = SizeFromClientSize(new Size(_pbBackward.Right, _lblPointer.Bottom));

                updateCurrentPointerPosition();
            }
            else
            {
                //decoder is in an invalid state... lay things out accordingly...
                _lblCurrent.Visible = _lblForward.Visible = _lblBackward.Visible = false;
                _pbCurrent.Visible = _pbForward.Visible = _pbBackward.Visible = false;
                _lblPointer.Visible = false;

                _lblNoBuffers.Visible = true;

                this.Width = _lblNoBuffers.Width;
                this.Height = _lblNoBuffers.Height;
                _lblNoBuffers.Left = 0;
                _lblNoBuffers.Top = 0;
            }

            this.Invalidate();
        }

        private void _decoder_PictureDecodedEvent(object sender)
        {
            _pbCurrent.Invalidate();
            _pbForward.Invalidate();
            _pbBackward.Invalidate();
            updateCurrentPointerPosition();
        }


        private void updateCurrentPointerPosition()
        {
            PictureBox pbType = null;
            switch (_decoder.LastDecodePictureBufferType)
            {
                case PLMPEG.VESDec.DecoderPictureBuffer.Forward:  pbType = _pbForward;  break;
                case PLMPEG.VESDec.DecoderPictureBuffer.Current:  pbType = _pbCurrent;  break;
                case PLMPEG.VESDec.DecoderPictureBuffer.Backward: pbType = _pbBackward; break;
            }
            if (pbType == null)
            {
                _lblPointer.Visible = false;
                return;
            }
            _lblPointer.Visible = true;
            _lblPointer.Left = pbType.Left + (pbType.Width / 2) - (_lblPointer.Width / 2);
        }

        private void DecoderPictureBufferViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Decoder = null;
        }
    }
}
