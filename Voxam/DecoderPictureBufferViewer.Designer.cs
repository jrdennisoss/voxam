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
    partial class DecoderPictureBufferViewer
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
            this._pbForward = new System.Windows.Forms.PictureBox();
            this._pbCurrent = new System.Windows.Forms.PictureBox();
            this._pbBackward = new System.Windows.Forms.PictureBox();
            this._lblForward = new System.Windows.Forms.Label();
            this._lblCurrent = new System.Windows.Forms.Label();
            this._lblBackward = new System.Windows.Forms.Label();
            this._lblPointer = new System.Windows.Forms.Label();
            this._lblNoBuffers = new System.Windows.Forms.Label();
            this._lblForwardDecodeTag = new System.Windows.Forms.Label();
            this._lblCurrentDecodeTag = new System.Windows.Forms.Label();
            this._lblBackwardDecodeTag = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this._pbForward)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._pbCurrent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._pbBackward)).BeginInit();
            this.SuspendLayout();
            // 
            // _pbForward
            // 
            this._pbForward.Location = new System.Drawing.Point(54, 85);
            this._pbForward.Name = "_pbForward";
            this._pbForward.Size = new System.Drawing.Size(218, 135);
            this._pbForward.TabIndex = 0;
            this._pbForward.TabStop = false;
            // 
            // _pbCurrent
            // 
            this._pbCurrent.Location = new System.Drawing.Point(282, 91);
            this._pbCurrent.Name = "_pbCurrent";
            this._pbCurrent.Size = new System.Drawing.Size(218, 135);
            this._pbCurrent.TabIndex = 1;
            this._pbCurrent.TabStop = false;
            // 
            // _pbBackward
            // 
            this._pbBackward.Location = new System.Drawing.Point(536, 91);
            this._pbBackward.Name = "_pbBackward";
            this._pbBackward.Size = new System.Drawing.Size(218, 135);
            this._pbBackward.TabIndex = 2;
            this._pbBackward.TabStop = false;
            // 
            // _lblForward
            // 
            this._lblForward.AutoSize = true;
            this._lblForward.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblForward.ForeColor = System.Drawing.Color.Yellow;
            this._lblForward.Location = new System.Drawing.Point(121, 31);
            this._lblForward.Name = "_lblForward";
            this._lblForward.Size = new System.Drawing.Size(129, 33);
            this._lblForward.TabIndex = 3;
            this._lblForward.Text = "Forward";
            this._lblForward.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // _lblCurrent
            // 
            this._lblCurrent.AutoSize = true;
            this._lblCurrent.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblCurrent.ForeColor = System.Drawing.Color.Yellow;
            this._lblCurrent.Location = new System.Drawing.Point(362, 31);
            this._lblCurrent.Name = "_lblCurrent";
            this._lblCurrent.Size = new System.Drawing.Size(119, 33);
            this._lblCurrent.TabIndex = 4;
            this._lblCurrent.Text = "Current";
            this._lblCurrent.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // _lblBackward
            // 
            this._lblBackward.AutoSize = true;
            this._lblBackward.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblBackward.ForeColor = System.Drawing.Color.Yellow;
            this._lblBackward.Location = new System.Drawing.Point(625, 31);
            this._lblBackward.Name = "_lblBackward";
            this._lblBackward.Size = new System.Drawing.Size(151, 33);
            this._lblBackward.TabIndex = 5;
            this._lblBackward.Text = "Backward";
            this._lblBackward.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // _lblPointer
            // 
            this._lblPointer.AutoSize = true;
            this._lblPointer.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblPointer.ForeColor = System.Drawing.Color.Red;
            this._lblPointer.Location = new System.Drawing.Point(152, 242);
            this._lblPointer.Name = "_lblPointer";
            this._lblPointer.Size = new System.Drawing.Size(53, 61);
            this._lblPointer.TabIndex = 6;
            this._lblPointer.Text = "^";
            this._lblPointer.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // _lblNoBuffers
            // 
            this._lblNoBuffers.AutoSize = true;
            this._lblNoBuffers.Font = new System.Drawing.Font("Microsoft Sans Serif", 150F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblNoBuffers.ForeColor = System.Drawing.Color.Red;
            this._lblNoBuffers.Location = new System.Drawing.Point(410, 85);
            this._lblNoBuffers.Name = "_lblNoBuffers";
            this._lblNoBuffers.Size = new System.Drawing.Size(343, 340);
            this._lblNoBuffers.TabIndex = 7;
            this._lblNoBuffers.Text = "X";
            this._lblNoBuffers.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _lblForwardDecodeTag
            // 
            this._lblForwardDecodeTag.AutoSize = true;
            this._lblForwardDecodeTag.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblForwardDecodeTag.ForeColor = System.Drawing.Color.Yellow;
            this._lblForwardDecodeTag.Location = new System.Drawing.Point(227, 242);
            this._lblForwardDecodeTag.Name = "_lblForwardDecodeTag";
            this._lblForwardDecodeTag.Size = new System.Drawing.Size(59, 20);
            this._lblForwardDecodeTag.TabIndex = 8;
            this._lblForwardDecodeTag.Text = "#1234";
            this._lblForwardDecodeTag.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // _lblCurrentDecodeTag
            // 
            this._lblCurrentDecodeTag.AutoSize = true;
            this._lblCurrentDecodeTag.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblCurrentDecodeTag.ForeColor = System.Drawing.Color.Yellow;
            this._lblCurrentDecodeTag.Location = new System.Drawing.Point(262, 275);
            this._lblCurrentDecodeTag.Name = "_lblCurrentDecodeTag";
            this._lblCurrentDecodeTag.Size = new System.Drawing.Size(59, 20);
            this._lblCurrentDecodeTag.TabIndex = 9;
            this._lblCurrentDecodeTag.Text = "#1234";
            this._lblCurrentDecodeTag.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // _lblBackwardDecodeTag
            // 
            this._lblBackwardDecodeTag.AutoSize = true;
            this._lblBackwardDecodeTag.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblBackwardDecodeTag.ForeColor = System.Drawing.Color.Yellow;
            this._lblBackwardDecodeTag.Location = new System.Drawing.Point(293, 304);
            this._lblBackwardDecodeTag.Name = "_lblBackwardDecodeTag";
            this._lblBackwardDecodeTag.Size = new System.Drawing.Size(59, 20);
            this._lblBackwardDecodeTag.TabIndex = 10;
            this._lblBackwardDecodeTag.Text = "#1234";
            this._lblBackwardDecodeTag.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // DecoderPictureBufferViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(782, 343);
            this.Controls.Add(this._lblBackwardDecodeTag);
            this.Controls.Add(this._lblCurrentDecodeTag);
            this.Controls.Add(this._lblForwardDecodeTag);
            this.Controls.Add(this._lblNoBuffers);
            this.Controls.Add(this._lblPointer);
            this.Controls.Add(this._lblBackward);
            this.Controls.Add(this._lblCurrent);
            this.Controls.Add(this._lblForward);
            this.Controls.Add(this._pbBackward);
            this.Controls.Add(this._pbCurrent);
            this.Controls.Add(this._pbForward);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DecoderPictureBufferViewer";
            this.ShowInTaskbar = false;
            this.Text = "Video Decoder Picture Buffer Viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DecoderPictureBufferViewer_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this._pbForward)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._pbCurrent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._pbBackward)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox _pbForward;
        private System.Windows.Forms.PictureBox _pbCurrent;
        private System.Windows.Forms.PictureBox _pbBackward;
        private System.Windows.Forms.Label _lblForward;
        private System.Windows.Forms.Label _lblCurrent;
        private System.Windows.Forms.Label _lblBackward;
        private System.Windows.Forms.Label _lblPointer;
        private System.Windows.Forms.Label _lblNoBuffers;
        private System.Windows.Forms.Label _lblForwardDecodeTag;
        private System.Windows.Forms.Label _lblCurrentDecodeTag;
        private System.Windows.Forms.Label _lblBackwardDecodeTag;
    }
}