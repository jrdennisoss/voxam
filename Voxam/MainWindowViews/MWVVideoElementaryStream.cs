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
using Voxam.MPEG1ToolKit.Streams;
using Voxam.MPEG1ToolKit.Threading;


namespace Voxam
{
    public partial class MWVVideoElementaryStream : UserControl, IMainWindowView
    {
        private MasterSourceProvider _masterSourceProvider = null;
        private const bool _scalePreviewPicture = true;

        private readonly MPEG1ToolKit.PLMPEG.AutoVESImageDecoder _decoder;


        private readonly ToolStripItem[] _menuItems;

        public MWVVideoElementaryStream(ProgramStyleScheme programStyleScheme = null, ThreadWorkerPool threadWorkerPool = null)
        {
            _decoder = new MPEG1ToolKit.PLMPEG.AutoVESImageDecoder();
            _decoder.ThreadWorkerPool = threadWorkerPool;
            this.Disposed += delegate { _decoder.Dispose(); };

            InitializeComponent();
            _menuItems = generateMenuItems();
            programStyleScheme?.StyleControlChildren(this);
            masterSourceProviderUpdated();
        }

        private ToolStripItem[] generateMenuItems()
        {
            return new ToolStripItem[]
            {
                new ToolStripMenuItem("View Decoder Picture Buffers", null, mnuViewDecoderPictureBuffers_Click),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Manual Decoder Feeding Enabled"),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Show Prediction Arrows"),
                new ToolStripMenuItem("Show Complete Prediction Arrow Dependencies"),
                new ToolStripMenuItem("Show GOPs"),
                new ToolStripMenuItem("Show Sequences"),
            };
        }



        private DecoderPictureBufferViewer _dpbViewer = null;
        private void mnuViewDecoderPictureBuffers_Click(object sender, EventArgs e)
        {
            if (_dpbViewer != null)
                if (_dpbViewer.IsDisposed) _dpbViewer = null;
            if (_dpbViewer == null)
                _dpbViewer = new DecoderPictureBufferViewer();
            _dpbViewer.Decoder = _decoder;
            try
            {
                _dpbViewer.Show(this);
            }
            catch
            {
                _dpbViewer.Focus();
            }
        }


        //IMainWindowView Stuff
        public MasterSourceProvider MasterSourceProvider 
        {
            get => _masterSourceProvider;
            set
            {
                if (_masterSourceProvider == value) return;
                _masterSourceProvider = value;
                masterSourceProviderUpdated();
            }
        }
        public Control Control => this;
        public string ViewMenuName => "Video Elementary Stream";
        public void PopulateViewMenu(ToolStripMenuItem tsmi)
        {
            tsmi.Text = "Options";
            tsmi.DropDownItems.AddRange(_menuItems);
            tsmi.Visible = true;
        }





        private void masterSourceProviderUpdated()
        {
            _pbPreview.Image = null;
            _pictureStream.SourceData = null;
            _decoder.Reset();
            UpdateSourceDataIfVisible();
        }
        private void UpdateSourceDataIfVisible()
        {
            //this is done like this because accessing
            //the 'Pictures' member in master source provider
            //could possible trigger a load...
            if (!this.Visible) return;
            if (_masterSourceProvider == null) return;
            if (_masterSourceProvider.Pictures == _pictureStream.SourceData) return;
            _pictureStream.SourceData = _masterSourceProvider.Pictures;
            _pictureStream.SelectedPictureIndex = 0;
            displaySelectedPicture();
        }

        private void MWVVideoElementaryStream_Resize(object sender, EventArgs e)
        {
            _pictureStream.Left = 0;
            _pictureStream.Width = this.ClientRectangle.Width;
            _pictureStream.Height = _pictureStream.MaximumHeight;
            _pictureStream.Top = this.ClientRectangle.Height - _pictureStream.Height;

            if (_scalePreviewPicture)
            {
                _pbPreview.SizeMode = PictureBoxSizeMode.Zoom;
                _pbPreview.Height = _pictureStream.Top;
                _pbPreview.Width = this.ClientRectangle.Width / 2;
            }
            else if (_pbPreview.Image != null)
            {
                _pbPreview.SizeMode = PictureBoxSizeMode.AutoSize;
            }
            _pbPreview.Top = 0;
            _pbPreview.Left = this.ClientRectangle.Width - _pbPreview.Width;
        }

        private void MWVVideoElementaryStream_VisibleChanged(object sender, EventArgs e)
        {
            UpdateSourceDataIfVisible();
        }



        private Size _lastDisplayedPictureSize = new Size();
        private void displaySelectedPicture()
        {
            var picture = _pictureStream.SelectedPicture;
            if (picture == null) goto failed;

            if (!_decoder.Decode(picture)) goto failed;
            var img = _decoder.DecodedPicture;
            _pbPreview.Image = img;
            if ((img.Width != _lastDisplayedPictureSize.Width) || (img.Height != _lastDisplayedPictureSize.Height))
            {
                _lastDisplayedPictureSize.Width = img.Width;
                _lastDisplayedPictureSize.Height = img.Height;
                MWVVideoElementaryStream_Resize(this, null);
            }
            return;

        failed:
            _pbPreview.Image = null;
            return;
        }



        private void _pictureStream_MPEG1ObjectClickEvent(object sender, PictureStream.MPEG1ObjectClickEventArgs args)
        {
            _pictureStream.SelectedPictureIndex = args.PictureIndex;
            displaySelectedPicture();
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch(keyData)
            {
                case Keys.Left:
                    MoveSelectedPicture(-1);
                    return true;

                case Keys.Right:
                    MoveSelectedPicture(1);
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void MoveSelectedPicture(int count)
        {
            if (_pictureStream.SelectedPictureIndex < 0) return;
            if (_pictureStream.PictureCount < 1) return;
            _pictureStream.SelectedPictureIndex += count;
            if (_pictureStream.SelectedPictureIndex < 0) _pictureStream.SelectedPictureIndex = 0;
            if (_pictureStream.SelectedPictureIndex >= _pictureStream.PictureCount) _pictureStream.SelectedPictureIndex = _pictureStream.PictureCount - 1;
            displaySelectedPicture();
            _pictureStream.SetFocusToIndex(_pictureStream.SelectedPictureIndex);
        }
    }
}
