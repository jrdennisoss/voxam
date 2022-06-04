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
    partial class MWVExporter
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
            this._chkVESOnly = new System.Windows.Forms.CheckBox();
            this._btnSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _chkVESOnly
            // 
            this._chkVESOnly.AutoSize = true;
            this._chkVESOnly.Location = new System.Drawing.Point(3, 14);
            this._chkVESOnly.Name = "_chkVESOnly";
            this._chkVESOnly.Size = new System.Drawing.Size(301, 24);
            this._chkVESOnly.TabIndex = 0;
            this._chkVESOnly.Text = "Export Video Elementary Stream Only";
            this._chkVESOnly.UseVisualStyleBackColor = true;
            // 
            // _btnSave
            // 
            this._btnSave.Location = new System.Drawing.Point(287, 383);
            this._btnSave.Name = "_btnSave";
            this._btnSave.Size = new System.Drawing.Size(186, 69);
            this._btnSave.TabIndex = 1;
            this._btnSave.Text = "Convert and Save";
            this._btnSave.UseVisualStyleBackColor = true;
            this._btnSave.Click += new System.EventHandler(this._btnSave_Click);
            // 
            // MWVExporter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._btnSave);
            this.Controls.Add(this._chkVESOnly);
            this.Name = "MWVExporter";
            this.Size = new System.Drawing.Size(717, 455);
            this.Resize += new System.EventHandler(this.MWVExporter_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox _chkVESOnly;
        private System.Windows.Forms.Button _btnSave;
    }
}
