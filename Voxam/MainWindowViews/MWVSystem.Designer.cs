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
            this._objectStream = new Voxam.ObjectStream();
            this.SuspendLayout();
            // 
            // _objectStream
            // 
            this._objectStream.BackColor = System.Drawing.Color.Black;
            this._objectStream.Location = new System.Drawing.Point(197, 239);
            this._objectStream.Name = "_objectStream";
            this._objectStream.SelectedObjectIndex = -1;
            this._objectStream.Size = new System.Drawing.Size(519, 94);
            this._objectStream.SourceData = null;
            this._objectStream.TabIndex = 0;
            // 
            // MWVSystem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._objectStream);
            this.DoubleBuffered = true;
            this.Name = "MWVSystem";
            this.Size = new System.Drawing.Size(808, 356);
            this.VisibleChanged += new System.EventHandler(this.MWVSystem_VisibleChanged);
            this.Resize += new System.EventHandler(this.MWVSystem_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private ObjectStream _objectStream;
    }
}
