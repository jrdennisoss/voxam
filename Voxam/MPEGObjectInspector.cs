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
using System.Drawing;
using System.Windows.Forms;

using Voxam.MPEG1ToolKit.Objects;

namespace Voxam
{
    public partial class MPEGObjectInspector : Form
    {
        private IMPEG1Object _object = null;
        private readonly Font _sectionFont;

        public IMPEG1Object InspectedObject
        {
            get => _object;
            set
            {
                _object = value;
                updateObjectDisplay();
            }
        }

        public MPEGObjectInspector(IMPEG1Object obj = null)
        {
            InitializeComponent();
            _sectionFont = new System.Drawing.Font(
                _dataGridView.Font.FontFamily,
                _dataGridView.Font.Size,
                System.Drawing.FontStyle.Bold,
                _dataGridView.Font.Unit,
                _dataGridView.Font.GdiCharSet
            );

            _object = obj;
            updateObjectDisplay();
        }

        private void updateObjectDisplay()
        {
            if (this.IsDisposed) return;

            _dataGridView.Rows.Clear();
            dispatchAppendObject(_object, true);
        }

        private void dispatchAppendObject(IMPEG1Object obj, bool followParent)
        {
            if (obj == null) return;

            _dataGridView.Rows.Add(new String[] { obj.Name, null });
            _dataGridView.Rows[_dataGridView.RowCount - 1].Cells[0].Style.Font = _sectionFont;


            if (obj is MPEG1Picture) appendObjectDisplay((MPEG1Picture)obj);
            else if (obj is MPEG1GOP) appendObjectDisplay((MPEG1GOP)obj);
            else if (obj is MPEG1Sequence) appendObjectDisplay((MPEG1Sequence)obj);

            if (followParent) dispatchAppendObject(obj.Parent, true);
        }

        private void appendKV(string key, string value)
        {
            _dataGridView.Rows.Add(new String[] { key, value });
        }

        private void appendKV(string key, int value) => appendKV(key, value.ToString());
        private void appendKV(string key, uint value) => appendKV(key, value.ToString());
        private void appendKV(string key, byte value) => appendKV(key, value.ToString());

        private void appendObjectDisplay(MPEG1Picture picture)
        {
            String pictureType = "Unknown";
            switch(picture.Type)
            {
                case MPEG1Picture.PictureType.IntraCoded:   pictureType = "I: Intra-Coded";   break;
                case MPEG1Picture.PictureType.Predictive:   pictureType = "P: Predictive";    break;
                case MPEG1Picture.PictureType.Bipredictive: pictureType = "B: Bi-Predictive"; break;
                case MPEG1Picture.PictureType.DirectCoded:  pictureType = "D: Direct-Coded";  break;
            }
            appendKV("Temporal Sequence Number", picture.TemporalSequenceNumber);
            appendKV("Picture Type", pictureType );
            appendKV("VBV Delay", picture.VBVDelay);

            string fullPELForwardVectorValue = "N/A";
            string forwardFCodeValue = "N/A";
            string fullPELBackwardVectorValue = "N/A";
            string backwardFCodeValue = "N/A";

            if ((picture.Type == MPEG1Picture.PictureType.Predictive) || (picture.Type == MPEG1Picture.PictureType.Bipredictive))
            {
                fullPELForwardVectorValue = picture.FullPELForwardVector ? "Yes" : "No";
                forwardFCodeValue = picture.ForwardFCode.ToString();


                if (picture.Type == MPEG1Picture.PictureType.Bipredictive)
                {
                    fullPELBackwardVectorValue = picture.FullPELBackwardVector ? "Yes" : "No";
                    backwardFCodeValue = picture.BackwardFCode.ToString();
                }
            }

            appendKV("Full PEL Forward Vector", fullPELForwardVectorValue);
            appendKV("Forward f_code", forwardFCodeValue);
            appendKV("Full PEL Backward Vector", fullPELBackwardVectorValue);
            appendKV("Backward f_code", backwardFCodeValue);
        }

        private void appendObjectDisplay(MPEG1GOP gop)
        {
            appendKV("Drop Frame Flag", gop.DropFrame ? "True" : "False" );
            appendKV("Timestamp", String.Format("{0:d02}:{1:d02}:{2:d02} Frame #{3:0}", gop.Hour, gop.Minute, gop.Second, gop.Frame));
            appendKV("Closed", gop.Closed ? "Yes" : "No" );
            appendKV("Broken", gop.Broken ? "Yes" : "No" );
        }

        private void appendObjectDisplay(MPEG1Sequence sequence)
        {
            appendKV("Horizontal Size", sequence.HorizontalSize);
            appendKV("Vertical Size", sequence.VerticalSize);
            appendKV("Aspect Ratio", String.Format("{0:d02}: {1:0.0000}", sequence.AspectRatioCode, sequence.AspectRatio));
            appendKV("Frame Rate", String.Format("{0:d02}: {1:0.000}", sequence.FrameRateCode, sequence.FrameRate));
            appendKV("Bitrate", sequence.Bitrate);
            appendKV("VBV Buffer Size", sequence.VBVBufferSize);
            appendKV("Constrained Parameters", sequence.ConstrainedParameters ? "Yes" : "No");
            appendKV("Load Intra Quantizer Matrix", sequence.HasCustomIntraQuantizerMatrix ? "Yes" : "No");
            appendKV("Load Non-Intra Quantizer Matrix", sequence.HasCustomNonIntraQuantizerMatrix ? "Yes" : "No");
        }

    }
}
