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
    public partial class ObjectStream : UserControl
    {
        public class ObjectStreamObjectClickEventArgs : EventArgs
        {
            public readonly MouseEventArgs MouseEventArgs;
            public readonly IMPEG1Object MPEG1Object;
            public readonly int ObjectIndex;
            public ObjectStreamObjectClickEventArgs(MouseEventArgs mouseEventArgs, IMPEG1Object mpeg1Object = null, int objectIndex = -1)
            {
                MouseEventArgs = mouseEventArgs;
                MPEG1Object = mpeg1Object;
                ObjectIndex = objectIndex;
            }
        }
        public delegate void ObjectStreamObjectClickEventHandler(object sender, ObjectStreamObjectClickEventArgs args);
        public event ObjectStreamObjectClickEventHandler ObjectStreamObjectClickEvent;


        //
        // variables for styling objects...
        //
        private Size _objectMargin = new Size(2, 4);
        private Size _objectSize = new Size(80, 80);
        private Pen _objectBorderPen = new Pen(Color.White);
        private SolidBrush _objectFillBrush = new SolidBrush(Color.DarkRed);
        private SolidBrush _objectTextBrush = new SolidBrush(Color.White);
        private SolidBrush _videoFillBrush = new SolidBrush(Color.DarkGreen);
        private SolidBrush _videoTextBrush = new SolidBrush(Color.White);
        private SolidBrush _audioFillBrush = new SolidBrush(Color.DarkBlue);
        private SolidBrush _audioTextBrush = new SolidBrush(Color.White);
        private SolidBrush _paddingFillBrush = new SolidBrush(Color.DarkGray);
        private SolidBrush _paddingTextBrush = new SolidBrush(Color.White);
        private Size _totalObjectSize;
        private int _objectYOffset;



        //
        // accessors for styling objects...
        //
        public Size ObjectMargin
        {
            get { return _objectMargin; }
            set { _objectMargin = value; viewConfigUpdate(); }
        }
        public Size ObjectSize
        {
            get { return _objectSize; }
            set { _objectSize = value; viewConfigUpdate(); }
        }
        public Color ObjectBorderColor
        {
            get { return _objectBorderPen.Color; }
            set { _objectBorderPen.Color = value; viewConfigUpdate(); }
        }
        public Color ObjectSolidFillColor
        {
            get { return _objectFillBrush.Color; }
            set { _objectFillBrush.Color = value; viewConfigUpdate(); }
        }
        public Color ObjectTextColor
        {
            get { return _objectTextBrush.Color; }
            set { _objectTextBrush.Color = value; viewConfigUpdate(); }
        }
        public Color VideoSolidFillColor
        {
            get { return _videoFillBrush.Color; }
            set { _videoFillBrush.Color = value; viewConfigUpdate(); }
        }
        public Color VideoTextColor
        {
            get { return _videoTextBrush.Color; }
            set { _videoTextBrush.Color = value; viewConfigUpdate(); }
        }
        public Color AudioSolidFillColor
        {
            get { return _audioFillBrush.Color; }
            set { _audioFillBrush.Color = value; viewConfigUpdate(); }
        }
        public Color AudioTextColor
        {
            get { return _audioTextBrush.Color; }
            set { _audioTextBrush.Color = value; viewConfigUpdate(); }
        }
        public Color PaddingSolidFillColor
        {
            get { return _paddingFillBrush.Color; }
            set { _paddingFillBrush.Color = value; viewConfigUpdate(); }
        }
        public Color PaddingTextColor
        {
            get { return _paddingTextBrush.Color; }
            set { _paddingTextBrush.Color = value; viewConfigUpdate(); }
        }




        //
        // accessors for the source data...
        //
        private IMPEG1ObjectCollection _sourceData = null;
        private int _selectedObjectIndex;
        public IMPEG1ObjectCollection SourceData
        {
            get { return _sourceData; }
            set
            {
                _sourceData = value;
                _selectedObjectIndex = -1;
                viewConfigUpdate();
            }
        }
        public int ObjectCount
        {
            get
            {
                if (_sourceData == null) return 0;
                return _sourceData.Count;
            }
        }

        //
        // accessors for selecting an object
        //
        public int SelectedObjectIndex
        {
            get { return _selectedObjectIndex; }
            set { _selectedObjectIndex = value; Invalidate(); }
        }
        public IMPEG1Object SelectedObject
        {
            get
            {
                if (SourceData == null) return null;
                if (SelectedObjectIndex == -1) return null;
                if (SelectedObjectIndex >= SourceData.Count) return null;
                return SourceData[SelectedObjectIndex];
            }
        }


        //
        // General visual / style accessors...
        //
        public int MaximumHeight { get => this.ClientRectangle.Height - _objectYOffset; }



        public ObjectStream()
        {
            InitializeComponent();
            viewConfigUpdate();
        }
        public void SetFocusToIndex(int objectIndex)
        {
            if (objectIndex < 0) return;
            if (objectIndex >= ObjectCount) return;
            int scrollOffset = objectIndex * _totalObjectSize.Width;
            scrollOffset -= this.ClientRectangle.Width / 2;
            scrollOffset += _totalObjectSize.Width / 2;
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


            //objects
            _totalObjectSize = new Size(
                _objectSize.Width + (_objectMargin.Width * 2),
                _objectSize.Height + (_objectMargin.Height * 2)
                );
            yBottom -= _totalObjectSize.Height;
            _objectYOffset = yBottom + _objectMargin.Height;


            //
            //set the scroll parameters after all visual sizes have been computed...
            //
            //
            int scrollMax = (_totalObjectSize.Width * ObjectCount) - this.ClientRectangle.Width + 1;
            if (scrollMax < 0) scrollMax = 0;
            _scrollbar.Maximum = scrollMax;

            this.Invalidate();
        }

        private void ObjectStream_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(this.BackColor);

            if (ObjectCount == 0) return;

            int i = computeFirstDrawableObjectIndex();
            int maxi = computeMaxDrawableObjectIndex();
            for (; i < maxi; ++i)
            {
                var obj = _sourceData[i];
                drawSingleObject(g, obj, i);
            }
        }

        private int computeFirstDrawableObjectIndex()
        {
            int rv = _scrollbar.Value;
            rv /= _totalObjectSize.Width;
            return rv;
        }

        private int computeMaxDrawableObjectIndex()
        {
            int rv = _scrollbar.Value + this.ClientRectangle.Width;
            rv /= _totalObjectSize.Width;
            rv += 1;
            if (rv > ObjectCount) rv = ObjectCount;
            return rv;
        }



        //
        //object drawing...
        //
        private void drawSingleObject(Graphics g, IMPEG1Object obj, int objectIndex)
        {
            String objText = obj.Name;
            if (objText.StartsWith("MPEG-1 ")) objText = objText.Substring(7);
            if (objText.EndsWith(" Header")) objText = objText.Substring(0, objText.Length - 7);

            SolidBrush fillBrush = _objectFillBrush;
            SolidBrush textBrush = _objectTextBrush;

            if (obj is MPEG1PESPacket)
            {
                var pesPacket = obj as MPEG1PESPacket;
                if (pesPacket.IsVideoType)
                {
                    objText = String.Format("Video\r\n{0:X02}", pesPacket.StreamIdType);
                    fillBrush = _videoFillBrush;
                    textBrush = _videoTextBrush;
                }
                else  if (pesPacket.IsAudioType)
                {
                    objText = String.Format("Audio\r\n{0:X02}", pesPacket.StreamIdType);
                    fillBrush = _audioFillBrush;
                    textBrush = _audioTextBrush;
                }
                else if (pesPacket.IsPaddingType)
                {
                    objText = "Padding";
                    fillBrush = _paddingFillBrush;
                    textBrush = _paddingTextBrush;
                }
                else
                {
                    objText = String.Format("PES\r\n{0:X02}", pesPacket.StreamIdType);
                }
            }
            else if (obj.StreamIdType <= 0xB8)
            {
                //color Video elementary stream types as video
                fillBrush = _videoFillBrush;
                textBrush = _videoTextBrush;
            }

            int xoff = _totalObjectSize.Width;
            xoff *= objectIndex;
            xoff += _objectMargin.Width;
            xoff -= _scrollbar.Value;

            if (fillBrush.Color != Color.Transparent)
                g.FillRectangle(fillBrush, xoff, _objectYOffset, _objectSize.Width, _objectSize.Height);
            if ((objectIndex == _selectedObjectIndex) && (_objectBorderPen.Color != Color.Transparent))
                g.DrawRectangle(_objectBorderPen, xoff, _objectYOffset, _objectSize.Width, _objectSize.Height);

            if (objText.Length > 0)
            {
                var strfont = this.Font;
                var strdimens = g.MeasureString(objText, strfont);

                float strxoff = _totalObjectSize.Width;
                strxoff /= 2f;
                strxoff += xoff;
                strxoff -= strdimens.Width / 2f;

                float stryoff = _totalObjectSize.Height;
                stryoff /= 2f;
                stryoff += _objectYOffset;
                stryoff -= strdimens.Height / 2f;

                g.DrawString(objText, strfont, textBrush, strxoff, stryoff);
            }
        }

        private void _scrollbar_Scroll(object sender, ScrollEventArgs e)
        {
            this.Invalidate();
        }

        private void ObjectStream_Resize(object sender, EventArgs e)
        {
            viewConfigUpdate();
        }

        private void ObjectStream_MouseClick(object sender, MouseEventArgs e)
        {
            if (ObjectStreamObjectClickEvent == null) return;

            if (e.Y < _objectYOffset) goto FireMouseOnlyEvent;
            if (e.Y > (_objectYOffset + _objectSize.Height)) goto FireMouseOnlyEvent;

            int absoluteClickOffset = e.X + _scrollbar.Value;
            int pictureIndex = absoluteClickOffset / _totalObjectSize.Width;
            if (pictureIndex >= ObjectCount) goto FireMouseOnlyEvent;

            int absolutePictureStartOffset = pictureIndex * _totalObjectSize.Width;
            int relativeClickOffset = absoluteClickOffset - absolutePictureStartOffset;
            if (relativeClickOffset < _objectMargin.Width) goto FireMouseOnlyEvent;
            if (relativeClickOffset >= (_totalObjectSize.Width - _objectMargin.Width)) goto FireMouseOnlyEvent;

            ObjectStreamObjectClickEvent(this, new ObjectStreamObjectClickEventArgs(e, _sourceData[pictureIndex], pictureIndex));

            return;

        FireMouseOnlyEvent:
            ObjectStreamObjectClickEvent(this, new ObjectStreamObjectClickEventArgs(e));
            return;
        }
    }
}
