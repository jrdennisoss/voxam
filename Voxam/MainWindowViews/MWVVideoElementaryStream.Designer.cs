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
    partial class MWVVideoElementaryStream
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
            this._pbPreview = new System.Windows.Forms.PictureBox();
            this._pictureStream = new Voxam.PictureStream();
            ((System.ComponentModel.ISupportInitialize)(this._pbPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // _pbPreview
            // 
            this._pbPreview.BackColor = System.Drawing.Color.Blue;
            this._pbPreview.Location = new System.Drawing.Point(371, 38);
            this._pbPreview.Name = "_pbPreview";
            this._pbPreview.Size = new System.Drawing.Size(237, 185);
            this._pbPreview.TabIndex = 1;
            this._pbPreview.TabStop = false;
            // 
            // _pictureStream
            // 
            this._pictureStream.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._pictureStream.BackColor = System.Drawing.Color.Black;
            this._pictureStream.GOPBorderColor = System.Drawing.Color.Red;
            this._pictureStream.GOPHeight = 20;
            this._pictureStream.GOPHeightMargin = 4;
            this._pictureStream.GOPSolidFillColor = System.Drawing.Color.DarkBlue;
            this._pictureStream.GOPTextColor = System.Drawing.Color.Gray;
            this._pictureStream.GOPVisible = true;
            this._pictureStream.Location = new System.Drawing.Point(106, 340);
            this._pictureStream.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._pictureStream.Name = "_pictureStream";
            this._pictureStream.PictureBorderColor = System.Drawing.Color.Red;
            this._pictureStream.PictureMargin = new System.Drawing.Size(2, 4);
            this._pictureStream.PictureSize = new System.Drawing.Size(80, 60);
            this._pictureStream.PictureSolidFillColor = System.Drawing.Color.DarkBlue;
            this._pictureStream.PictureTextColor = System.Drawing.Color.YellowGreen;
            this._pictureStream.PredictionArrowsColor = System.Drawing.Color.Yellow;
            this._pictureStream.PredictionArrowsCompleteDependencyChain = true;
            this._pictureStream.PredictionArrowsHeight = 30;
            this._pictureStream.PredictionArrowsVisible = true;
            this._pictureStream.SelectedPictureIndex = -1;
            this._pictureStream.SequenceBorderColor = System.Drawing.Color.Red;
            this._pictureStream.SequenceHeight = 20;
            this._pictureStream.SequenceHeightMargin = 4;
            this._pictureStream.SequenceSolidFillColor = System.Drawing.Color.DarkBlue;
            this._pictureStream.SequenceTextColor = System.Drawing.Color.Gray;
            this._pictureStream.SequenceVisible = true;
            this._pictureStream.Size = new System.Drawing.Size(274, 90);
            this._pictureStream.SourceData = null;
            this._pictureStream.TabIndex = 0;
            this._pictureStream.PictureStreamObjectClickEvent += new Voxam.PictureStream.PictureStreamObjectClickEventHandler(this._pictureStream_MPEG1ObjectClickEvent);
            // 
            // MWVVideoElementaryStream
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._pbPreview);
            this.Controls.Add(this._pictureStream);
            this.DoubleBuffered = true;
            this.Name = "MWVVideoElementaryStream";
            this.Size = new System.Drawing.Size(800, 458);
            this.VisibleChanged += new System.EventHandler(this.MWVVideoElementaryStream_VisibleChanged);
            this.Resize += new System.EventHandler(this.MWVVideoElementaryStream_Resize);
            ((System.ComponentModel.ISupportInitialize)(this._pbPreview)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private PictureStream _pictureStream;
        private System.Windows.Forms.PictureBox _pbPreview;
    }
}
