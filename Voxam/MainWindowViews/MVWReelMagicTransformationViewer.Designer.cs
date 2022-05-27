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
    partial class MVWReelMagicTransformationViewer
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
            this._dataGridView = new System.Windows.Forms.DataGridView();
            this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTemporalSequenceNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCorrectedFCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this._dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // _dataGridView
            // 
            this._dataGridView.AllowUserToAddRows = false;
            this._dataGridView.AllowUserToDeleteRows = false;
            this._dataGridView.AllowUserToOrderColumns = true;
            this._dataGridView.AllowUserToResizeRows = false;
            this._dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colType,
            this.colTemporalSequenceNumber,
            this.colFCode,
            this.colCorrectedFCode});
            this._dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dataGridView.Location = new System.Drawing.Point(0, 0);
            this._dataGridView.Name = "_dataGridView";
            this._dataGridView.ReadOnly = true;
            this._dataGridView.RowHeadersVisible = false;
            this._dataGridView.RowHeadersWidth = 62;
            this._dataGridView.RowTemplate.Height = 28;
            this._dataGridView.Size = new System.Drawing.Size(696, 362);
            this._dataGridView.TabIndex = 1;
            this._dataGridView.VirtualMode = true;
            this._dataGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this._dataGridView_CellValueNeeded);
            // 
            // colType
            // 
            this.colType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.colType.FillWeight = 1F;
            this.colType.HeaderText = "Type";
            this.colType.MinimumWidth = 8;
            this.colType.Name = "colType";
            this.colType.ReadOnly = true;
            this.colType.Width = 79;
            // 
            // colTemporalSequenceNumber
            // 
            this.colTemporalSequenceNumber.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colTemporalSequenceNumber.HeaderText = "TSN";
            this.colTemporalSequenceNumber.MinimumWidth = 8;
            this.colTemporalSequenceNumber.Name = "colTemporalSequenceNumber";
            this.colTemporalSequenceNumber.ReadOnly = true;
            // 
            // colFCode
            // 
            this.colFCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.colFCode.FillWeight = 1F;
            this.colFCode.HeaderText = "f_code";
            this.colFCode.MinimumWidth = 8;
            this.colFCode.Name = "colFCode";
            this.colFCode.ReadOnly = true;
            this.colFCode.Width = 94;
            // 
            // colCorrectedFCode
            // 
            this.colCorrectedFCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.colCorrectedFCode.FillWeight = 1F;
            this.colCorrectedFCode.HeaderText = "Corrected f_code";
            this.colCorrectedFCode.MinimumWidth = 8;
            this.colCorrectedFCode.Name = "colCorrectedFCode";
            this.colCorrectedFCode.ReadOnly = true;
            this.colCorrectedFCode.Width = 154;
            // 
            // MVWReelMagicTransformationViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._dataGridView);
            this.DoubleBuffered = true;
            this.Name = "MVWReelMagicTransformationViewer";
            this.Size = new System.Drawing.Size(696, 362);
            this.VisibleChanged += new System.EventHandler(this.MVWReelMagicTransformationViewer_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this._dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView _dataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTemporalSequenceNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCorrectedFCode;
    }
}
