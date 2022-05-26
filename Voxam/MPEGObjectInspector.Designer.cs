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
    partial class MPEGObjectInspector
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._dataGridView = new System.Windows.Forms.DataGridView();
            this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colField = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this._dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // _dataGridView
            // 
            this._dataGridView.AllowUserToAddRows = false;
            this._dataGridView.AllowUserToDeleteRows = false;
            this._dataGridView.AllowUserToResizeRows = false;
            this._dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dataGridView.ColumnHeadersVisible = false;
            this._dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colType,
            this.colField,
            this.colValue});
            this._dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dataGridView.Location = new System.Drawing.Point(0, 0);
            this._dataGridView.Name = "_dataGridView";
            this._dataGridView.ReadOnly = true;
            this._dataGridView.RowHeadersVisible = false;
            this._dataGridView.RowHeadersWidth = 62;
            this._dataGridView.RowTemplate.Height = 28;
            this._dataGridView.Size = new System.Drawing.Size(680, 843);
            this._dataGridView.TabIndex = 0;
            // 
            // colType
            // 
            this.colType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.colType.FillWeight = 1F;
            this.colType.HeaderText = "Type";
            this.colType.MinimumWidth = 8;
            this.colType.Name = "colType";
            this.colType.ReadOnly = true;
            this.colType.Width = 8;
            // 
            // colField
            // 
            this.colField.HeaderText = "Field";
            this.colField.MinimumWidth = 8;
            this.colField.Name = "colField";
            this.colField.ReadOnly = true;
            this.colField.Width = 150;
            // 
            // colValue
            // 
            this.colValue.HeaderText = "Value";
            this.colValue.MinimumWidth = 8;
            this.colValue.Name = "colValue";
            this.colValue.ReadOnly = true;
            this.colValue.Width = 150;
            // 
            // MPEGObjectInspector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(680, 843);
            this.Controls.Add(this._dataGridView);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MPEGObjectInspector";
            this.Text = "MPEG Object Inspector";
            ((System.ComponentModel.ISupportInitialize)(this._dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView _dataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colField;
        private System.Windows.Forms.DataGridViewTextBoxColumn colValue;
    }
}