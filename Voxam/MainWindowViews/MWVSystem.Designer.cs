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



namespace Voxam
{
    partial class MWVSystem
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._dgv = new System.Windows.Forms.DataGridView();
            this._objectStream = new Voxam.ObjectStream();
            this.colTypeId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTypeName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colByteOffset = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLength = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this._dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // _dgv
            // 
            this._dgv.AllowUserToAddRows = false;
            this._dgv.AllowUserToDeleteRows = false;
            this._dgv.AllowUserToResizeRows = false;
            this._dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTypeId,
            this.colTypeName,
            this.colByteOffset,
            this.colLength});
            this._dgv.Location = new System.Drawing.Point(74, 61);
            this._dgv.Name = "_dgv";
            this._dgv.RowHeadersVisible = false;
            this._dgv.RowHeadersWidth = 62;
            this._dgv.RowTemplate.Height = 28;
            this._dgv.Size = new System.Drawing.Size(614, 136);
            this._dgv.TabIndex = 1;
            this._dgv.VirtualMode = true;
            this._dgv.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this._dgv_CellValueNeeded);
            // 
            // _objectStream
            // 
            this._objectStream.AudioSolidFillColor = System.Drawing.Color.DarkBlue;
            this._objectStream.AudioTextColor = System.Drawing.Color.White;
            this._objectStream.BackColor = System.Drawing.Color.Black;
            this._objectStream.Location = new System.Drawing.Point(105, 311);
            this._objectStream.Name = "_objectStream";
            this._objectStream.ObjectBorderColor = System.Drawing.Color.White;
            this._objectStream.ObjectMargin = new System.Drawing.Size(2, 4);
            this._objectStream.ObjectSize = new System.Drawing.Size(80, 80);
            this._objectStream.ObjectSolidFillColor = System.Drawing.Color.DarkRed;
            this._objectStream.ObjectTextColor = System.Drawing.Color.White;
            this._objectStream.PaddingSolidFillColor = System.Drawing.Color.DarkGray;
            this._objectStream.PaddingTextColor = System.Drawing.Color.White;
            this._objectStream.SelectedObjectIndex = -1;
            this._objectStream.Size = new System.Drawing.Size(519, 94);
            this._objectStream.SourceData = null;
            this._objectStream.TabIndex = 0;
            this._objectStream.VideoSolidFillColor = System.Drawing.Color.DarkGreen;
            this._objectStream.VideoTextColor = System.Drawing.Color.White;
            // 
            // colTypeId
            // 
            this.colTypeId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.colTypeId.FillWeight = 1F;
            this.colTypeId.HeaderText = "ID";
            this.colTypeId.MinimumWidth = 8;
            this.colTypeId.Name = "colTypeId";
            this.colTypeId.ReadOnly = true;
            this.colTypeId.Width = 62;
            // 
            // colTypeName
            // 
            this.colTypeName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colTypeName.HeaderText = "Object Type";
            this.colTypeName.MinimumWidth = 8;
            this.colTypeName.Name = "colTypeName";
            this.colTypeName.ReadOnly = true;
            // 
            // colByteOffset
            // 
            this.colByteOffset.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.colByteOffset.FillWeight = 1F;
            this.colByteOffset.HeaderText = "Byte Offset";
            this.colByteOffset.MinimumWidth = 8;
            this.colByteOffset.Name = "colByteOffset";
            this.colByteOffset.ReadOnly = true;
            this.colByteOffset.Width = 125;
            // 
            // colLength
            // 
            this.colLength.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.colLength.FillWeight = 1F;
            this.colLength.HeaderText = "Length";
            this.colLength.MinimumWidth = 8;
            this.colLength.Name = "colLength";
            this.colLength.ReadOnly = true;
            this.colLength.Width = 95;
            // 
            // MWVSystem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._dgv);
            this.Controls.Add(this._objectStream);
            this.DoubleBuffered = true;
            this.Name = "MWVSystem";
            this.Size = new System.Drawing.Size(926, 549);
            this.VisibleChanged += new System.EventHandler(this.MWVSystem_VisibleChanged);
            this.Resize += new System.EventHandler(this.MWVSystem_Resize);
            ((System.ComponentModel.ISupportInitialize)(this._dgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ObjectStream _objectStream;
        private System.Windows.Forms.DataGridView _dgv;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTypeId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTypeName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colByteOffset;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLength;
    }
}
