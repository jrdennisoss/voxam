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
using System.IO;
using System.Windows.Forms;

using Voxam.MPEG1ToolKit;
using Voxam.MPEG1ToolKit.Objects;
using Voxam.MPEG1ToolKit.Streams;

namespace Voxam
{
    public partial class MWVExporter : UserControl, IMainWindowView
    {
        private MasterSourceProvider _masterSourceProvider = null;
        private readonly ToolStripItem[] _menuItems;

        public MWVExporter(ProgramStyleScheme programStyleScheme = null)
        {
            InitializeComponent();
            _menuItems = generateMenuItems();
            programStyleScheme?.StyleControl(this);
        }

        private ToolStripItem[] generateMenuItems()
        {
            return new ToolStripItem[]
            {
                new ToolStripMenuItem("ReelMagic Video Converter Settings", null, mnuReelMagicVideoConverterSettings_Click),
            };
        }
        private void mnuReelMagicVideoConverterSettings_Click(object sender, EventArgs e)
        {
            if (_masterSourceProvider == null) return;
            using (var frm = new ReelMagicVideoConverterSettings(_masterSourceProvider.VideoConverterSettings))
                frm.ShowDialog(this);
        }




        //
        // IMainWindowView implementation
        //
        public MasterSourceProvider MasterSourceProvider { get => _masterSourceProvider; set => _masterSourceProvider = value; }
        public Control Control { get => this; }
        public string ViewMenuName { get => "Exporter"; }
        public void PopulateViewMenu(ToolStripMenuItem tsmi)
        {
            tsmi.Text = "Options";
            tsmi.DropDownItems.AddRange(_menuItems);
            tsmi.Visible = true;
        }
        private void MWVExporter_Resize(object sender, EventArgs e)
        {
            _btnSave.Top = ClientRectangle.Height - _btnSave.Height - 10;
            _btnSave.Left = (ClientRectangle.Width / 2) - (_btnSave.Width / 2);
        }

        private void _btnSave_Click(object sender, EventArgs e)
        {
            if (_masterSourceProvider == null) return;
            using (var sfd = new SaveFileDialog())
            {
                sfd.FileName = _masterSourceProvider.Filename;

                var lastSeperatorIdx = sfd.FileName.LastIndexOf(Path.DirectorySeparatorChar);
                if (lastSeperatorIdx > -1) sfd.FileName = sfd.FileName.Substring(lastSeperatorIdx + 1);

                var extensionIdx = sfd.FileName.LastIndexOf('.');
                string extension = ".mpg";
                if (extensionIdx > 0)
                {
                    extension = sfd.FileName.Substring(extensionIdx);
                    sfd.FileName = sfd.FileName.Substring(0, extensionIdx);
                }
                sfd.FileName += "_converted" + extension;
                sfd.Title = "Convert and Save MPEG-1 File";
                sfd.Filter = "MPEG-1 Video Files (*.mpg;*.mpeg)|*.mpg;*.mpeg|All files (*.*)|*.*";
                if (sfd.ShowDialog(this) != DialogResult.OK) return;

                using (var fs = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write))
                {
                    try
                    {
                        PerformPictureBasedConversion(fs);
                        MessageBox.Show("OK", "Conversion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Conversion Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }


        private void PerformPictureBasedConversion(FileStream output)
        {
            var iter = _masterSourceProvider.VideoIterator;

            if (!_chkVESOnly.Checked)
            {
                for (var iterSource = iter.Source as MPEG1SubStreamObjectSource;
                     iterSource != null; iterSource = iterSource.Parent.Source as MPEG1SubStreamObjectSource)
                    iter = iterSource.Parent;
            }

            using (var writer = new IterWriter(iter, output))
            {
                MPEG1Sequence currentSequence = null;
                MPEG1ToolKit.ReelMagic.VideoConverter converter = null;

                foreach (var picture in _masterSourceProvider.Pictures)
                {
                    converter = _masterSourceProvider.VideoConverterPictureCollection[picture];
                    break;
                }

                iter = _masterSourceProvider.VideoIterator;
                for (iter.SeekSourceTo(0); !iter.EndOfStream; iter.Next())
                {
                    if (!iter.MPEGObjectValid) continue;
                    switch (iter.MPEGObjectType)
                    {
                        case MPEG1Sequence.STREAM_ID_TYPE:
                            var seq = MagicalSequence.Marshal(iter);
                            if (seq != null)
                            {
                                var seqheadbuf = new MPEG1ObjectBufferBuilder(seq);
                                converter?.PatchSequence(seq, seqheadbuf.Buffer, 0, seqheadbuf.Length);
                                writer.Overwrite(seq, seqheadbuf.Buffer, 0, seqheadbuf.Length);
                            }
                            break;
                        case MPEG1Picture.STREAM_ID_TYPE:
                            var picture = MPEG1Picture.Marshal(iter);
                            if (picture != null)
                            {
                                var picheadbuf = new MPEG1ObjectBufferBuilder(picture);
                                converter?.PatchPicture(picture, picheadbuf.Buffer, 0, picheadbuf.Length);
                                writer.Overwrite(picture, picheadbuf.Buffer, 0, picheadbuf.Length);
                            }
                            break;
                    }
                }

                writer.WriteRemaining();
            }
        }

        private static MPEG1Sequence getSequence(IMPEG1Object obj)
        {
            for (; (obj != null) && !(obj is MPEG1Sequence); obj = obj.Parent) ;
            return obj as MPEG1Sequence;
        }
    }
}
