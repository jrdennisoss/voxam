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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Voxam
{
    public partial class MWVSystem : UserControl, IMainWindowView
    {
        private MasterSourceProvider _masterSourceProvider = null;

        public MWVSystem(ProgramStyleScheme programStyleScheme = null)
        {
            InitializeComponent();
            programStyleScheme?.StyleControlChildren(this);
        }

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
        public string ViewMenuName => "System";
        public void PopulateViewMenu(ToolStripMenuItem tsmi)
        {
            //
        }


        private void masterSourceProviderUpdated()
        {
            _objectStream.SourceData = null;
            _dgv.RowCount = 0;
            UpdateSourceDataIfVisible();
        }
        private void UpdateSourceDataIfVisible()
        {
            //this is done like this because accessing
            //the 'Objects' member in master source provider
            //could possible trigger a load...
            if (!this.Visible) return;
            if (_masterSourceProvider == null) return;
            if (_masterSourceProvider.Objects == _objectStream.SourceData) return;
            _objectStream.SourceData = _masterSourceProvider.Objects;
            _objectStream.SelectedObjectIndex = -1;

            _dgv.RowCount = _masterSourceProvider.Objects.Count;
            _dgv.Refresh();
        }
        private void MWVSystem_Resize(object sender, EventArgs e)
        {
            _objectStream.Left = 0;
            _objectStream.Width = this.ClientRectangle.Width;
            _objectStream.Height = _objectStream.MaximumHeight;
            _objectStream.Top = this.ClientRectangle.Height - _objectStream.Height;

            _dgv.Left = 0;
            _dgv.Width = this.ClientRectangle.Width;
            _dgv.Top = 0;
            _dgv.Height = _objectStream.Top;
        }
        private void MWVSystem_VisibleChanged(object sender, EventArgs e) => UpdateSourceDataIfVisible();




        private void _dgv_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e) => e.Value = GetCellValue(e.RowIndex, e.ColumnIndex);
        private string GetCellValue(int row, int col)
        {
            if (_masterSourceProvider == null) return "";
            if (row >= _masterSourceProvider.Objects.Count) return "";
            var obj = _masterSourceProvider.Objects[row];
            switch (col)
            {
                case 0: //stream id type
                    return String.Format("{0:X02}", obj.StreamIdType);
                case 1: //name
                    return obj.Name;
                case 2: //offset
                    if (obj.Source == null) return "";
                    return obj.Source.IteratorSourceStreamPosition.ToString();
                case 3: //length
                    //
                    // TODO: this is horrible... should fix this...
                    //
                    if ((row + 1) >= _masterSourceProvider.Objects.Count) return ""; //
                    var nextObj = _masterSourceProvider.Objects[row + 1];
                    if (nextObj.Source == null) return "";
                    return (nextObj.Source.IteratorSourceStreamPosition - obj.Source.IteratorSourceStreamPosition).ToString();
            }
            return "";
        }
    }
}
