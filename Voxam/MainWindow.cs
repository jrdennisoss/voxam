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

using Voxam.MPEG1ToolKit.Threading;

namespace Voxam
{
    public partial class MainWindow : Form
    {
        //these variables are used to allocate resources one and stick with the life of the program... 
        private readonly ProgramStyleScheme _styleScheme = new ProgramStyleScheme();
        private readonly ThreadWorkerPool _threadWorkerPool = new ThreadWorkerPool(Environment.ProcessorCount);



        private MasterSourceProvider _masterSourceProvider = null;
        public MasterSourceProvider MasterSourceProvider
        {
            get => _masterSourceProvider;
            set
            {
                foreach (var v in this.Controls)
                {
                    var mwv = v as IMainWindowView;
                    if (mwv != null) mwv.MasterSourceProvider = value;
                }
                _masterSourceProvider?.Dispose();
                _masterSourceProvider = value;
                setViewMode(_masterSourceProvider != null);
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            _styleScheme.StyleForm(this);

            //construct and attach all MainWindowViews here...
            this.SuspendLayout();
            putView(new MWVStart(_styleScheme));
            putView(new MWVTopLevel(_styleScheme));
            putView(new MWVVideoElementaryStream(_styleScheme, _threadWorkerPool));
            putView(new MVWReelMagicTransformationViewer(_styleScheme));
            putView(new MWVExporter(_styleScheme));
            this.ResumeLayout(true);
            setViewMode(_masterSourceProvider != null);
        }
        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            _threadWorkerPool.Dispose();
        }

        private IMainWindowView _activeView = null;
        private IMainWindowView putView(IMainWindowView mwv)
        {
            var tsmi = new ToolStripMenuItem();
            tsmi.Text = mwv.ViewMenuName;
            tsmi.Click += delegate { activateView(mwv); };
            this.viewToolStripMenuItem.DropDownItems.Add(tsmi);

            mwv.Control.Location = new System.Drawing.Point(0, 0);
            mwv.Control.Size = new System.Drawing.Size(50, 50);
            mwv.Control.Visible = false;
            mwv.Control.TabStop = false;
            mwv.MasterSourceProvider = _masterSourceProvider;
            this.Controls.Add(mwv.Control);

            return mwv;
        }
        private void activateView(IMainWindowView mwv)
        {
            if (_activeView != null) _activeView.Control.Visible = false;
            _mwvToolStripMenuItem.DropDownItems.Clear();
            _mwvToolStripMenuItem.Visible = false;

            _activeView = mwv;
            if (_activeView == null) return;

            _activeView.PopulateViewMenu(_mwvToolStripMenuItem);
            _styleScheme.StyleToolStripMenuItem(_mwvToolStripMenuItem);
            _activeView.Control.Visible = true;
            MainWindow_Resize(this, null);
            _activeView.Control.Focus();
        }

        private void setViewMode(bool haveMasterSourceProvider)
        {
            if (!haveMasterSourceProvider)
            {
                foreach (var c in Controls)
                {
                    if (c is MWVStart)
                    {
                        activateView((IMainWindowView)c);
                        return;
                    }
                }
            }
            else
            {
                if (!(_activeView is MWVStart)) return;
                foreach (var c in Controls)
                {
                    if (c is MWVVideoElementaryStream)
                    {
                        activateView((IMainWindowView)c);
                        return;
                    }
                }
            }
        }

        private void aboutVoxamToolStripMenuItem_Click(object sender, EventArgs e) => AboutBox.DoDialog(this, _styleScheme);

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) => this.Close();

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            if (_activeView != null)
            {
                _activeView.Control.Left = 0;
                _activeView.Control.Top = _menubar.Bottom;
                _activeView.Control.Width = this.ClientSize.Width;
                _activeView.Control.Height = this.ClientSize.Height - _activeView.Control.Top;
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Open MPEG-1 File";
                ofd.Filter = "MPEG-1 Video Files (*.mpg;*.mpeg)|*.mpg;*.mpeg|All files (*.*)|*.*";
                if (ofd.ShowDialog(this) == DialogResult.OK)
                    loadMasterSourceFromFile(ofd.FileName);
            }
        }

        private void loadMasterSourceFromFile(string filename)
        {
            this.Text = "Voxam";
            if (filename != null)
            {
                try
                {
                    this.MasterSourceProvider = new MasterSourceProvider(filename);
                    int lastSlash = filename.LastIndexOf('\\');
                    if (lastSlash >= 0) filename = filename.Substring(lastSlash + 1);
                    this.Text += " - " + filename;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Failed to open file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.MasterSourceProvider = null;
                }
            }

        }
    }
}
