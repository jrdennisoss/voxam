﻿/*
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
    public partial class MVWReelMagicTransformationViewer : UserControl, IMainWindowView
    {
        private MasterSourceProvider _masterSourceProvider = null;
        private readonly ToolStripItem[] _menuItems;


        public MVWReelMagicTransformationViewer(ProgramStyleScheme programStyleScheme = null)
        {
            InitializeComponent();
            _menuItems = generateMenuItems();
            programStyleScheme?.StyleControlChildren(this);
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
            _dataGridView.Refresh();
        }


        //
        // IMainWindowView implementation
        //
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
        public string ViewMenuName => "ReelMagic Transformation Viewer";
        public void PopulateViewMenu(ToolStripMenuItem tsmi)
        {
            tsmi.Text = "Options";
            tsmi.DropDownItems.AddRange(_menuItems);
            tsmi.Visible = true;
        }




        private void masterSourceProviderUpdated()
        {
            _dataGridView.RowCount = 0;
            if (_masterSourceProvider == null) return;

            //
            // TODO: should thie be deferred to if visible ?
            //
            _dataGridView.RowCount = _masterSourceProvider.Pictures.Count;
            if (_dataGridView.RowCount > 0) _dataGridView.FirstDisplayedScrollingRowIndex = 0;
        }

        private void _dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e) => e.Value = GetCellValue(e.RowIndex, e.ColumnIndex);
        private string GetCellValue(int row, int col)
        {
            if (_masterSourceProvider == null) return "";
            if (row >= _masterSourceProvider.Pictures.Count) return "";
            var picture = _masterSourceProvider.Pictures[row];
            switch (col)
            {
                case 0: //picture type
                    switch (picture.Type)
                    {
                        case MPEG1Picture.PictureType.IntraCoded:   return "I";
                        case MPEG1Picture.PictureType.Predictive:   return "P";
                        case MPEG1Picture.PictureType.Bipredictive: return "B";
                        case MPEG1Picture.PictureType.DirectCoded:  return "D";
                        default:                                    return "UNKNOWN";
                    }

                case 1: //Temporal Sequence Number
                    return picture.TemporalSequenceNumber.ToString();

                case 2: //f_code
                    if ((picture.Type != MPEG1Picture.PictureType.Predictive) && (picture.Type != MPEG1Picture.PictureType.Bipredictive)) return "N/A";
                    return picture.ForwardFCode.ToString();

                case 3: //Transformed F_Code
                    if ((picture.Type != MPEG1Picture.PictureType.Predictive) && (picture.Type != MPEG1Picture.PictureType.Bipredictive)) return "N/A";

                    MPEG1Picture convertedPicture = picture;
                    var converter = _masterSourceProvider.VideoConverterPictureCollection[picture];
                    if (converter != null) convertedPicture = converter.PatchPicture(picture);
                    return convertedPicture.ForwardFCode.ToString();
            }
            return "";
        }

        private void MVWReelMagicTransformationViewer_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible) _dataGridView.Refresh();
        }
    }
}
