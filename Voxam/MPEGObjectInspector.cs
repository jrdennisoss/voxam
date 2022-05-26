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

using Voxam.MPEG1ToolKit.Objects;

namespace Voxam
{
    public partial class MPEGObjectInspector : Form
    {
        private IMPEG1Object _object = null;
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

            _dataGridView.Rows.Add(new String[] { obj.Name, null, null });
            if (obj is MPEG1Picture) appendObjectDisplay((MPEG1Picture)obj);
            else if (obj is MPEG1GOP) appendObjectDisplay((MPEG1GOP)obj);
            else if (obj is MPEG1Sequence) appendObjectDisplay((MPEG1Sequence)obj);

            if (followParent) dispatchAppendObject(obj.Parent, true);
        }

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
            _dataGridView.Rows.Add(new String[] { null, "Temporal Sequence Number", picture.TemporalSequenceNumber.ToString() });
            _dataGridView.Rows.Add(new String[] { null, "Picture Type", pictureType });
            _dataGridView.Rows.Add(new String[] { null, "VBV Delay", picture.VBVDelay.ToString() });

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

            _dataGridView.Rows.Add(new String[] { null, "Full PEL Forward Vector", fullPELForwardVectorValue});
            _dataGridView.Rows.Add(new String[] { null, "Forward f_code", forwardFCodeValue});
            _dataGridView.Rows.Add(new String[] { null, "Full PEL Backward Vector", fullPELBackwardVectorValue});
            _dataGridView.Rows.Add(new String[] { null, "Backward f_code", backwardFCodeValue});
        }

        private void appendObjectDisplay(MPEG1GOP gop)
        {
            _dataGridView.Rows.Add(new String[] { null, "Drop Frame Flag", gop.DropFrame ? "True" : "False" });
            _dataGridView.Rows.Add(new String[] { null, "Timestamp HMS", String.Format("{0:d02}:{1:d02}:{2:d02}", gop.Hour, gop.Minute, gop.Second) });
            _dataGridView.Rows.Add(new String[] { null, "Timestamp Frame", gop.Frame.ToString() });
            _dataGridView.Rows.Add(new String[] { null, "Closed", gop.Closed ? "Yes" : "No" });
            _dataGridView.Rows.Add(new String[] { null, "Broken", gop.Broken ? "Yes" : "No" });
        }

        private void appendObjectDisplay(MPEG1Sequence sequence)
        {
            _dataGridView.Rows.Add(new String[] { null, "Horizontal Size", sequence.HorizontalSize.ToString() });
            _dataGridView.Rows.Add(new String[] { null, "Vertical Size", sequence.VerticalSize.ToString() });
            _dataGridView.Rows.Add(new String[] { null, "Aspect Ratio", String.Format("{0:d02}: {1:0.0000}", sequence.AspectRatioCode, sequence.AspectRatio) });
            _dataGridView.Rows.Add(new String[] { null, "Frame Rate", String.Format("{0:d02}: {1:0.000}", sequence.FrameRateCode, sequence.FrameRate) });
            _dataGridView.Rows.Add(new String[] { null, "Bitrate", sequence.Bitrate.ToString() });
            _dataGridView.Rows.Add(new String[] { null, "VBV Buffer Size", sequence.VBVBufferSize.ToString() });
            _dataGridView.Rows.Add(new String[] { null, "Constrained Parameters", sequence.ConstrainedParameters ? "Yes" : "No" });
            _dataGridView.Rows.Add(new String[] { null, "Load Intra Quantizer Matrix", sequence.HasCustomIntraQuantizerMatrix ? "Yes" : "No" });
            _dataGridView.Rows.Add(new String[] { null, "Load Non-Intra Quantizer Matrix", sequence.HasCustomNonIntraQuantizerMatrix ? "Yes" : "No" });
        }

    }
}
