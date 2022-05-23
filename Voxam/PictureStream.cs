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

using Voxam.MPEG1ToolKit;
using Voxam.MPEG1ToolKit.Objects;

namespace Voxam
{
    public partial class PictureStream : UserControl
    {
        public class PictureStreamObjectClickEventArgs : EventArgs
        {
            public readonly MouseEventArgs MouseEventArgs;
            public readonly IMPEG1Object MPEG1Object;
            public readonly int PictureIndex;
            public PictureStreamObjectClickEventArgs(MouseEventArgs mouseEventArgs, IMPEG1Object MPEG1Object_ = null, int pictureIndex = -1)
            {
                MouseEventArgs = mouseEventArgs;
                MPEG1Object = MPEG1Object_;
                PictureIndex = pictureIndex;
            }
        }
        public delegate void PictureStreamObjectClickEventHandler(object sender, PictureStreamObjectClickEventArgs args);
        public event PictureStreamObjectClickEventHandler PictureStreamObjectClickEvent;


        private Size _pictureMargin = new Size(2, 4);
        private Size _pictureSize = new Size(80, 60);
        private Pen _pictureBorderPen = new Pen(Color.Red);
        private SolidBrush _pictureFillBrush = new SolidBrush(Color.DarkBlue);
        private SolidBrush _pictureTextBrush = new SolidBrush(Color.YellowGreen);
        private Size _totalPictureSize;
        private int _pictureYOffset;

        private bool _predictionArrowsVisible = true;
        private bool _predictionArrowsCompleteDependencyChain = true;
        private int _predictionArrowsHeight = 30;
        private Pen _predictionArrowsPen = new Pen(Color.Yellow);
        private SolidBrush _predictionArrowsFillBrush = new SolidBrush(Color.Yellow);

        private bool _gopVisible = true;
        private int _gopHeight = 20;
        private int _gopHeightMargin = 4;
        private Pen _gopBorderPen = new Pen(Color.Red);
        private SolidBrush _gopFillBrush = new SolidBrush(Color.DarkBlue);
        private SolidBrush _gopTextBrush = new SolidBrush(Color.Gray);
        private int _totalGopHeight;
        private int _gopYOffset;

        private bool _sequenceVisible = true;
        private int _sequenceHeight = 20;
        private int _sequenceHeightMargin = 4;
        private Pen _sequenceBorderPen = new Pen(Color.Red);
        private SolidBrush _sequenceFillBrush = new SolidBrush(Color.DarkBlue);
        private SolidBrush _sequenceTextBrush = new SolidBrush(Color.Gray);
        private int _totalSequenceHeight;
        private int _sequenceYOffset;



        public Size PictureMargin
        {
            get { return _pictureMargin; }
            set { _pictureMargin = value; viewConfigUpdate(); }
        }
        public Size PictureSize
        {
            get { return _pictureSize; }
            set { _pictureSize = value; viewConfigUpdate(); }
        }

        public Color PictureBorderColor
        {
            get { return _pictureBorderPen.Color; }
            set { _pictureBorderPen.Color = value; viewConfigUpdate(); }
        }

        public Color PictureSolidFillColor
        {
            get { return _pictureFillBrush.Color; }
            set { _pictureFillBrush.Color = value; viewConfigUpdate(); }
        }

        public Color PictureTextColor
        {
            get { return _pictureTextBrush.Color; }
            set { _pictureTextBrush.Color = value; viewConfigUpdate(); }
        }

        public bool PredictionArrowsVisible
        {
            get { return _predictionArrowsVisible; }
            set { _predictionArrowsVisible = value; viewConfigUpdate(); }
        }
        public bool PredictionArrowsCompleteDependencyChain
        {
            get { return _predictionArrowsCompleteDependencyChain; }
            set { _predictionArrowsCompleteDependencyChain = value; viewConfigUpdate(); }
        }
        public int PredictionArrowsHeight
        {
            get { return _predictionArrowsHeight; }
            set { _predictionArrowsHeight = value; viewConfigUpdate(); }
        }
        public Color PredictionArrowsColor
        {
            get { return _predictionArrowsPen.Color; }
            set { _predictionArrowsFillBrush.Color = _predictionArrowsPen.Color = value; viewConfigUpdate(); }
        }



        public bool GOPVisible
        {
            get { return _gopVisible; }
            set { _gopVisible = value; viewConfigUpdate(); }
        }
        public int GOPHeight
        {
            get { return _gopHeight; }
            set { _gopHeight = value; viewConfigUpdate(); }
        }
        public int GOPHeightMargin
        {
            get { return _gopHeightMargin; }
            set { _gopHeightMargin = value; viewConfigUpdate(); }
        }
        public Color GOPBorderColor
        {
            get { return _gopBorderPen.Color; }
            set { _gopBorderPen.Color = value; viewConfigUpdate(); }
        }
        public Color GOPSolidFillColor
        {
            get { return _gopFillBrush.Color; }
            set { _gopFillBrush.Color = value; viewConfigUpdate(); }
        }
        public Color GOPTextColor
        {
            get { return _gopTextBrush.Color; }
            set { _gopTextBrush.Color = value; viewConfigUpdate(); }
        }



        public bool SequenceVisible
        {
            get { return _sequenceVisible; }
            set { _sequenceVisible = value; viewConfigUpdate(); }
        }
        public int SequenceHeight
        {
            get { return _sequenceHeight; }
            set { _sequenceHeight = value; viewConfigUpdate(); }
        }
        public int SequenceHeightMargin
        {
            get { return _sequenceHeightMargin; }
            set { _sequenceHeightMargin = value; viewConfigUpdate(); }
        }
        public Color SequenceBorderColor
        {
            get { return _sequenceBorderPen.Color; }
            set { _sequenceBorderPen.Color = value; viewConfigUpdate(); }
        }
        public Color SequenceSolidFillColor
        {
            get { return _sequenceFillBrush.Color; }
            set { _sequenceFillBrush.Color = value; viewConfigUpdate(); }
        }
        public Color SequenceTextColor
        {
            get { return _sequenceTextBrush.Color; }
            set { _sequenceTextBrush.Color = value; viewConfigUpdate(); }
        }



        private IMPEG1PictureCollection _sourceData = null;
        private int _selectedPictureIndex;
        public IMPEG1PictureCollection SourceData
        {
            get { return _sourceData; }
            set
            {
                _sourceData = value;
                _selectedPictureIndex = -1;
                viewConfigUpdate();
            }
        }
        public int PictureCount
        {
            get
            {
                if (_sourceData == null) return 0;
                return _sourceData.Count;
            }
        }
        public int SelectedPictureIndex
        {
            get { return _selectedPictureIndex; }
            set { _selectedPictureIndex = value; Invalidate(); }
        }
        public MPEG1Picture SelectedPicture
        {
            get
            {
                if (SourceData == null) return null;
                if (SelectedPictureIndex == -1) return null;
                if (SelectedPictureIndex >= SourceData.Count) return null;
                return SourceData[SelectedPictureIndex];
            }
        }

        public int MaximumHeight { get => this.ClientRectangle.Height - _pictureYOffset; }
            

        public PictureStream()
        {
            InitializeComponent();
            viewConfigUpdate();
        }

        public void SetFocusToIndex(int pictureIndex)
        {
            if (pictureIndex < 0) return;
            if (pictureIndex >= PictureCount) return;
            int scrollOffset = pictureIndex * _totalPictureSize.Width;
            scrollOffset -= this.ClientRectangle.Width / 2;
            scrollOffset += _totalPictureSize.Width / 2;
            if (scrollOffset < 0) scrollOffset = 0;
            if (scrollOffset > _scrollbar.Maximum) scrollOffset = _scrollbar.Maximum;
            _scrollbar.Value = scrollOffset;
        }

        private void viewConfigUpdate()
        {

            //build things from the bottom up...
            int yBottom = ClientRectangle.Height;

            //scroll bar
            yBottom -= _scrollbar.Height;
            _scrollbar.Top = yBottom;
            _scrollbar.Left = 0;
            _scrollbar.Width = ClientRectangle.Width;

            //sequence
            if (_sequenceVisible)
            {
                _totalSequenceHeight = _sequenceHeight + (_sequenceHeightMargin * 2);
                yBottom -= _totalSequenceHeight;
                _sequenceYOffset = yBottom + _sequenceHeightMargin;
            }

            //GOP
            if (_gopVisible)
            {
                _totalGopHeight = _gopHeight + (_gopHeightMargin * 2);
                yBottom -= _totalGopHeight;
                _gopYOffset = yBottom + _gopHeightMargin;
            }
            
            //prediction arrows
            if (_predictionArrowsVisible)
            {
                yBottom -= _predictionArrowsHeight;
            }

            //pictures
            _totalPictureSize = new Size(
                _pictureSize.Width + (_pictureMargin.Width * 2),
                _pictureSize.Height + (_pictureMargin.Height * 2)
                );
            yBottom -= _totalPictureSize.Height;
            _pictureYOffset = yBottom + _pictureMargin.Height;


            //
            //set the scroll parameters after all visual sizes have been computed...
            //
            //
            //why does the simplest shit need to be so hard?
            //setting small and large change fucks thigns up and
            //the scroll bar often does not make it to the end...
            //_scrollbar.SmallChange = _totalPictureSize.Width / 4;
            //_scrollbar.LargeChange = _totalPictureSize.Width * 2;
            int scrollMax = (_totalPictureSize.Width * PictureCount) - this.ClientRectangle.Width + 1;
            if (scrollMax < 0) scrollMax = 0;
            _scrollbar.Maximum = scrollMax;

            this.Invalidate();
        }

        private readonly MPEG1PredictionTracker _predictionTracker = new MPEG1PredictionTracker();

        private void PictureStream_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(this.BackColor);

            if (PictureCount == 0) return;

            MPEG1Sequence currentSequence = null;
            int currentSequence_firstPictureIndex = 0;

            MPEG1GOP currentGop = null;
            int currentGop_firstPictureIndex = 0;



            int i = computeFirstDrawablePictureIndex();
            int maxi = computeMaxDrawablePictureIndex();
            if (_predictionArrowsVisible) _predictionTracker.Clear(i);
            for (; i < maxi; ++i)
            {
                MPEG1Picture picture = _sourceData[i];
                drawSinglePicture(g, picture, i);

                MPEG1Sequence sequence;
                MPEG1GOP gop;
                getPictureHierarchy(out sequence, out gop, picture);
                if (gop != currentGop)
                {
                    drawSingleGop(g, currentGop, currentGop_firstPictureIndex, i - 1);
                    currentGop = gop;
                    currentGop_firstPictureIndex = i;
                }
                if (sequence != currentSequence)
                {
                    drawSingleSequence(g, currentSequence, currentSequence_firstPictureIndex, i - 1);
                    currentSequence = sequence;
                    currentSequence_firstPictureIndex = i;
                }
                if (_predictionArrowsVisible) _predictionTracker.TrackNewPicture(picture);
            }
            drawPredictionArrows(g);
            drawSingleGop(g, currentGop, currentGop_firstPictureIndex, maxi - 1);
            drawSingleSequence(g, currentSequence, currentSequence_firstPictureIndex, maxi - 1);
        }

        private static void getPictureHierarchy(out MPEG1Sequence sequence, out MPEG1GOP gop, MPEG1Picture picture)
        {
            sequence = null;
            gop = null;
            if (picture.Parent is MPEG1GOP)
            {
                gop = (MPEG1GOP)picture.Parent;
                if (gop.Parent is MPEG1Sequence) sequence = (MPEG1Sequence)gop.Parent;
            }
            else if (picture.Parent is MPEG1Sequence)
            {
                sequence = (MPEG1Sequence)picture.Parent;
            }
        }

        private int computeFirstDrawablePictureIndex()
        {
            int rv = _scrollbar.Value;
            rv /= _totalPictureSize.Width;
            return rv;
        }

        private int computeMaxDrawablePictureIndex()
        {
            int rv = _scrollbar.Value + this.ClientRectangle.Width;
            rv /= _totalPictureSize.Width;
            rv += 1;
            if (rv > PictureCount) rv = PictureCount;
            return rv;
        }


        //
        //picture drawing...
        //

        private void drawSinglePicture(Graphics g, MPEG1Picture picture, int pictureIndex)
        {
            String pictureTypeStr = "";
            switch(picture.Type)
            {
                case MPEG1Picture.PictureType.IntraCoded:   pictureTypeStr = "I"; break;
                case MPEG1Picture.PictureType.Predictive:   pictureTypeStr = "P"; break;
                case MPEG1Picture.PictureType.Bipredictive: pictureTypeStr = "B"; break;
                case MPEG1Picture.PictureType.DirectCoded:  pictureTypeStr = "D"; break;
            }

            String picText = "# " + pictureIndex.ToString() + "\r\n" + pictureTypeStr;

            int xoff = _totalPictureSize.Width;
            xoff *= pictureIndex;
            xoff += _pictureMargin.Width;
            xoff -= _scrollbar.Value;
            
            if (_pictureFillBrush.Color != Color.Transparent)
              g.FillRectangle(_pictureFillBrush, xoff, _pictureYOffset, _pictureSize.Width, _pictureSize.Height);
            if ((pictureIndex == _selectedPictureIndex) && (_pictureBorderPen.Color != Color.Transparent))
              g.DrawRectangle(_pictureBorderPen, xoff, _pictureYOffset, _pictureSize.Width, _pictureSize.Height);

            if (picText.Length > 0)
            {
                var strfont = this.Font;
                var strdimens = g.MeasureString(picText, strfont);

                float strxoff = _totalPictureSize.Width;
                strxoff /= 2f;
                strxoff += xoff;
                strxoff -= strdimens.Width / 2f;

                float stryoff = _totalPictureSize.Height;
                stryoff /= 2f;
                stryoff += _pictureYOffset;
                stryoff -= strdimens.Height / 2f;

                g.DrawString(picText, strfont, _pictureTextBrush, strxoff, stryoff);
            }
        }


        //
        // prediction arrows drawing...
        //
        private void drawPredictionArrows(Graphics g)
        {
            if (!_predictionArrowsVisible) return;
            var selectedPredictionNode = _predictionTracker.LookupPicturePredictionNode(SelectedPicture);
            if (selectedPredictionNode == null) return;

            if (selectedPredictionNode.ForwardDependency != null)
            {
                drawSinglePredictionArrow(g, selectedPredictionNode.ForwardDependency, PredictionArrowTakeOffType.CENTER,
                    selectedPredictionNode, PredictionArrowTakeOffType.INNER_LEFT, selectedPredictionNode.BackwardDependency != null);
            }
            if (selectedPredictionNode.BackwardDependency != null)
            {
                drawSinglePredictionArrow(g, selectedPredictionNode.BackwardDependency, PredictionArrowTakeOffType.INNER_RIGHT,
                    selectedPredictionNode, PredictionArrowTakeOffType.OUTER_LEFT, false);
            }

            if (_predictionArrowsCompleteDependencyChain)
            {
                drawForwardDependencyChains(g, selectedPredictionNode.ForwardDependency);
                drawForwardDependencyChains(g, selectedPredictionNode.BackwardDependency);
            }

            foreach (var dependentNode in selectedPredictionNode.Dependents)
            {
                bool forward = dependentNode.ForwardDependency == selectedPredictionNode;
                drawSinglePredictionArrow(g, selectedPredictionNode, forward ? PredictionArrowTakeOffType.CENTER : PredictionArrowTakeOffType.INNER_RIGHT,
                    dependentNode, PredictionArrowTakeOffType.CENTER, forward);
            }
        }
        private void drawForwardDependencyChains(Graphics g, MPEG1PredictionTracker.PicturePredictionNode node)
        {
            if (node == null) return;
            for (; node.ForwardDependency != null; node = node.ForwardDependency)
            {
                drawSinglePredictionArrow(g, node.ForwardDependency, PredictionArrowTakeOffType.INNER_RIGHT,
                    node, PredictionArrowTakeOffType.INNER_LEFT, false);
            }
        }
        private void drawSinglePredictionArrow(Graphics g,
            MPEG1PredictionTracker.PicturePredictionNode startNode, PredictionArrowTakeOffType startNodeArrowTakeOffType,
            MPEG1PredictionTracker.PicturePredictionNode endNode, PredictionArrowTakeOffType endNodeArrowTakeOffType,
            bool low)
        {
            drawSinglePredictionArrow(g,
                translatePredictionArrowXTakeOffPosition(getPictureCenterXOffset(startNode.TrackIndex), startNodeArrowTakeOffType),
                translatePredictionArrowXTakeOffPosition(getPictureCenterXOffset(endNode.TrackIndex), endNodeArrowTakeOffType),
                low);
        }
        private void drawSinglePredictionArrow(Graphics g, int startX, int endX, bool low)
        {
            int topY = _pictureYOffset + _pictureSize.Height;
            int height = _predictionArrowsHeight;
            if (!low) height /= 2;
            int bottomY = topY + height;

            int bottomStartX = startX;
            int bottomEndX = endX;
            if (startX > endX)
            {
                bottomStartX -= height;
                bottomEndX += height;
            }
            else
            {
                bottomStartX += height;
                bottomEndX -= height;
            }

            g.DrawLine(_predictionArrowsPen, startX, topY, bottomStartX, bottomY);
            g.DrawLine(_predictionArrowsPen, bottomStartX, bottomY, bottomEndX, bottomY);
            g.DrawLine(_predictionArrowsPen, bottomEndX, bottomY, endX, topY);

            int endCapSize = 8;
            int endCapXDelta = (startX < endX) ? -endCapSize : endCapSize;
            g.FillPolygon(_predictionArrowsFillBrush, new Point[]
            {
                new Point(endX, topY),
                new Point(endX, topY + endCapSize),
                new Point(endX + endCapXDelta, topY)
            });
        }
        private int getPictureCenterXOffset(int pictureIndex)
        {
            int rv = _totalPictureSize.Width;
            rv *= pictureIndex;
            rv += _totalPictureSize.Width / 2;
            rv -= _scrollbar.Value;
            return rv;
        }
        enum PredictionArrowTakeOffType
        {
            CENTER,
            INNER_LEFT,
            INNER_RIGHT,
            OUTER_LEFT,
            OUTER_RIGHT,
        }
        private int translatePredictionArrowXTakeOffPosition(int centerX, PredictionArrowTakeOffType takeoffType)
        {
            double offset = 0.00;
            const double spacing = 1.00 / 6.00;

            switch (takeoffType)
            {
                case PredictionArrowTakeOffType.INNER_LEFT:
                    offset = -_pictureSize.Width;
                    offset *= spacing;
                    break;
                case PredictionArrowTakeOffType.INNER_RIGHT:
                    offset = _pictureSize.Width;
                    offset *= spacing;
                    break;
                case PredictionArrowTakeOffType.OUTER_LEFT:
                    offset = -_pictureSize.Width;
                    offset *= spacing * 2.0;
                    break;
                case PredictionArrowTakeOffType.OUTER_RIGHT:
                    offset = _pictureSize.Width;
                    offset *= spacing * 2.0;
                    break;
            }
            return centerX + (int)offset;
        }





        private void drawSingleGop(Graphics g, MPEG1GOP gop, int firstPictureIndex, int lastPictureIndex)
        {
            //drawPredictionArrows(g, gop);

            if (!_gopVisible) return;
            if (gop == null) return;

            String gopText = String.Format("GOP {0:d02}:{1:d02}:{2:d02}.{3:d02}", gop.Hour, gop.Minute, gop.Second, gop.Frame);

            int xoff = _totalPictureSize.Width;
            xoff *= firstPictureIndex;
            xoff += _pictureMargin.Width;
            xoff -= _scrollbar.Value;

            int width = _totalPictureSize.Width;
            width *= (lastPictureIndex - firstPictureIndex) + 1;
            width -= _pictureMargin.Width * 2;

            if (_gopFillBrush.Color != Color.Transparent)
                g.FillRectangle(_gopFillBrush, xoff, _gopYOffset, width, _gopHeight);
            //if (_gopBorderPen.Color != Color.Transparent)
            //    g.DrawRectangle(_gopBorderPen, selectedXTakeOff, _gopYOffset, width, _gopHeight);

            if (gopText.Length > 0)
            {
                var strfont = this.Font;
                var strdimens = g.MeasureString(gopText, strfont);

                float strxoff = width;
                strxoff /= 2f;
                strxoff += xoff;
                strxoff -= strdimens.Width / 2f;

                float stryoff = _gopHeight;
                stryoff /= 2f;
                stryoff += _gopYOffset;
                stryoff -= strdimens.Height / 2f;

                g.DrawString(gopText, strfont, _gopTextBrush, strxoff, stryoff);
            }
        }

        private void drawSingleSequence(Graphics g, MPEG1Sequence sequence, int firstPictureIndex, int lastPictureIndex)
        {
            if (!_sequenceVisible) return;
            if (sequence == null) return;

            String sequenceText = sequence.Name;

            int xoff = _totalPictureSize.Width;
            xoff *= firstPictureIndex;
            xoff += _pictureMargin.Width;
            xoff -= _scrollbar.Value;

            int width = _totalPictureSize.Width;
            width *= (lastPictureIndex - firstPictureIndex) + 1;
            width -= _pictureMargin.Width * 2;

            if (_sequenceFillBrush.Color != Color.Transparent)
                g.FillRectangle(_sequenceFillBrush, xoff, _sequenceYOffset, width, _sequenceHeight);
            //if (_sequenceBorderPen.Color != Color.Transparent)
            //    g.DrawRectangle(_sequenceBorderPen, selectedXTakeOff, _sequenceYOffset, width, _sequenceHeight);

            if (sequenceText.Length > 0)
            {
                var strfont = this.Font;
                var strdimens = g.MeasureString(sequenceText, strfont);

                float strxoff = width;
                strxoff /= 2f;
                strxoff += xoff;
                strxoff -= strdimens.Width / 2f;

                float stryoff = _sequenceHeight;
                stryoff /= 2f;
                stryoff += _sequenceYOffset;
                stryoff -= strdimens.Height / 2f;

                g.DrawString(sequenceText, strfont, _sequenceTextBrush, strxoff, stryoff);
            }
        }

        private void _scrollbar_Scroll(object sender, ScrollEventArgs e)
        {
            this.Invalidate();
        }

        private void PictureStream_Resize(object sender, EventArgs e)
        {
            viewConfigUpdate();
        }

        private void PictureStream_MouseClick(object sender, MouseEventArgs e)
        {
            if (PictureStreamObjectClickEvent == null) return;

            if (e.Y < _pictureYOffset) goto FireMouseOnlyEvent;
            if (e.Y > (_pictureYOffset + _pictureSize.Height)) goto FireMouseOnlyEvent;

            int absoluteClickOffset = e.X + _scrollbar.Value;
            int pictureIndex = absoluteClickOffset / _totalPictureSize.Width;
            if (pictureIndex >= PictureCount) goto FireMouseOnlyEvent;

            int absolutePictureStartOffset = pictureIndex * _totalPictureSize.Width;
            int relativeClickOffset = absoluteClickOffset - absolutePictureStartOffset;
            if (relativeClickOffset < _pictureMargin.Width) goto FireMouseOnlyEvent;
            if (relativeClickOffset >= (_totalPictureSize.Width - _pictureMargin.Width)) goto FireMouseOnlyEvent;

            PictureStreamObjectClickEvent(this, new PictureStreamObjectClickEventArgs(e, _sourceData[pictureIndex], pictureIndex));

            return;

        FireMouseOnlyEvent:
            PictureStreamObjectClickEvent(this, new PictureStreamObjectClickEventArgs(e));
            return;
        }
    }
}
