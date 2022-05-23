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
        }
        private void MWVSystem_Resize(object sender, EventArgs e)
        {
            _objectStream.Left = 0;
            _objectStream.Width = this.ClientRectangle.Width;
            _objectStream.Height = _objectStream.MaximumHeight;
            _objectStream.Top = this.ClientRectangle.Height - _objectStream.Height;
        }

        private void MWVSystem_VisibleChanged(object sender, EventArgs e)
        {
            UpdateSourceDataIfVisible();
        }
    }
}
