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

        private ToolStripMenuItem _mnuManualDecoderFeedingEnabled;

        private ToolStripMenuItem _mnuShowPredictionArrows;
        private ToolStripMenuItem _mnuShowPredictionArrowsCompleteDeps = new ToolStripMenuItem("Show Complete Prediction Arrow Dependencies");
        private ToolStripMenuItem _mnuShowGOPs = new ToolStripMenuItem("Show GOPs");
        private ToolStripMenuItem _mnuShowSequences = new ToolStripMenuItem("Show Sequences");

        private ToolStripItem[] generateMenuItems()
        {
            _mnuManualDecoderFeedingEnabled = new ToolStripMenuItem("Manual Decoder Feeding Enabled");
            _mnuManualDecoderFeedingEnabled.Checked = true;

            _mnuShowPredictionArrows = new ToolStripMenuItem("Show Prediction Arrows");
            _mnuShowPredictionArrows.Checked = _pictureStream.PredictionArrowsVisible;
            _mnuShowPredictionArrows.Click += _mnuShowPicstureStreamItem_Clicked;

            _mnuShowPredictionArrowsCompleteDeps = new ToolStripMenuItem("Show Complete Prediction Arrow Dependencies");
            _mnuShowPredictionArrowsCompleteDeps.Checked = _pictureStream.PredictionArrowsVisible;
            _mnuShowPredictionArrowsCompleteDeps.Click += _mnuShowPicstureStreamItem_Clicked;

            _mnuShowGOPs = new ToolStripMenuItem("Show GOPs");
            _mnuShowGOPs.Checked = _pictureStream.PredictionArrowsVisible;
            _mnuShowGOPs.Click += _mnuShowPicstureStreamItem_Clicked;

            _mnuShowSequences = new ToolStripMenuItem("Show Sequences");
            _mnuShowSequences.Checked = _pictureStream.PredictionArrowsVisible;
            _mnuShowSequences.Click += _mnuShowPicstureStreamItem_Clicked;

            return new ToolStripItem[]
            {
                new ToolStripMenuItem("ReelMagic Video Converter Settings", null, mnuReelMagicVideoConverterSettings_Click),
                new ToolStripMenuItem("View Decoder Picture Buffers", null, mnuViewDecoderPictureBuffers_Click),
                new ToolStripSeparator(),
                _mnuManualDecoderFeedingEnabled,
                new ToolStripSeparator(),
                _mnuShowPredictionArrows,
                _mnuShowPredictionArrowsCompleteDeps,
                _mnuShowGOPs,
                _mnuShowSequences,
            };
        }

        private void _mnuShowPicstureStreamItem_Clicked(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item == null) return;

            item.Checked = !item.Checked;

            if (item == _mnuShowPredictionArrows) _pictureStream.PredictionArrowsVisible = item.Checked;
            else if (item == _mnuShowPredictionArrowsCompleteDeps) _pictureStream.PredictionArrowsCompleteDependencyChain = item.Checked;
            else if (item == _mnuShowGOPs) _pictureStream.GOPVisible = item.Checked;
            else if (item == _mnuShowSequences) _pictureStream.SequenceVisible = item.Checked;

            MWVVideoElementaryStream_Resize(this, null);
        }

        private ReelMagicVideoConverterSettings _reelMagicVideoConverterSettings = null;
        private void mnuReelMagicVideoConverterSettings_Click(object sender, EventArgs e)
        {
            if (_reelMagicVideoConverterSettings != null)
                if (_reelMagicVideoConverterSettings.IsDisposed) _reelMagicVideoConverterSettings = null;
            if (_reelMagicVideoConverterSettings == null)
                _reelMagicVideoConverterSettings = new ReelMagicVideoConverterSettings(_masterSourceProvider.VideoConverterSettings);
            try
            {
                _reelMagicVideoConverterSettings.Show(this);
            }
            catch
            {
                _reelMagicVideoConverterSettings.Focus();
            }
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
            if (_reelMagicVideoConverterSettings != null) _reelMagicVideoConverterSettings.Settings = (_masterSourceProvider != null) ? _masterSourceProvider.VideoConverterSettings : null;
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
            _decoder.VideoConverterPictureCollection = _masterSourceProvider.VideoConverterPictureCollection;
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

            _btnPlayPause.Top = _pictureStream.Top - _btnPlayPause.Height - 5;
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



        private void _pictureStream_MPEG1ObjectClickEvent(object sender, PictureStream.PictureStreamObjectClickEventArgs args)
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


        //
        //play/pause stuff...
        //
        private void _btnPlayPause_Click(object sender, EventArgs e)
        {
            _playTimer.Enabled = !_playTimer.Enabled;
            _btnPlayPause.Text = _playTimer.Enabled ? "Pause" : "Play";
            _playTimer.Interval = 32; //unhardcode me!!
        }

        private void _playTimer_Tick(object sender, EventArgs e)
        {
            MoveSelectedPicture(1);
            if (_pictureStream.SelectedPictureIndex < 0) _btnPlayPause_Click(sender, null);
            if (_pictureStream.SelectedPictureIndex >= (_pictureStream.PictureCount-1)) _btnPlayPause_Click(sender, null);
        }
    }
}
