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
    partial class ReelMagicVideoConverterSettings
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
            this._cboDecodeMode = new System.Windows.Forms.ComboBox();
            this.lblDecodeMode = new System.Windows.Forms.Label();
            this._gbMagicKey = new System.Windows.Forms.GroupBox();
            this._rbMagicKeyC39D7088 = new System.Windows.Forms.RadioButton();
            this._rbMagicKey40044041 = new System.Windows.Forms.RadioButton();
            this._gbFCode = new System.Windows.Forms.GroupBox();
            this._nudBPictureBackwardFCode = new System.Windows.Forms.NumericUpDown();
            this.lblBPictureBackward = new System.Windows.Forms.Label();
            this._nudBPictureForwardFCode = new System.Windows.Forms.NumericUpDown();
            this.lblBPictureForward = new System.Windows.Forms.Label();
            this._nudPPictureForwardFCode = new System.Windows.Forms.NumericUpDown();
            this.lblPPictureForward = new System.Windows.Forms.Label();
            this._gbMagicKey.SuspendLayout();
            this._gbFCode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._nudBPictureBackwardFCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._nudBPictureForwardFCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._nudPPictureForwardFCode)).BeginInit();
            this.SuspendLayout();
            // 
            // _cboDecodeMode
            // 
            this._cboDecodeMode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._cboDecodeMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cboDecodeMode.FormattingEnabled = true;
            this._cboDecodeMode.Items.AddRange(new object[] {
            "Disabled",
            "Seek Truthful f_code",
            "Apply Static f_code"});
            this._cboDecodeMode.Location = new System.Drawing.Point(12, 32);
            this._cboDecodeMode.Name = "_cboDecodeMode";
            this._cboDecodeMode.Size = new System.Drawing.Size(396, 28);
            this._cboDecodeMode.TabIndex = 0;
            this._cboDecodeMode.SelectedIndexChanged += new System.EventHandler(this._cboDecodeMode_SelectedIndexChanged);
            // 
            // lblDecodeMode
            // 
            this.lblDecodeMode.AutoSize = true;
            this.lblDecodeMode.Location = new System.Drawing.Point(12, 9);
            this.lblDecodeMode.Name = "lblDecodeMode";
            this.lblDecodeMode.Size = new System.Drawing.Size(109, 20);
            this.lblDecodeMode.TabIndex = 1;
            this.lblDecodeMode.Text = "Decode Mode";
            // 
            // _gbMagicKey
            // 
            this._gbMagicKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._gbMagicKey.Controls.Add(this._rbMagicKeyC39D7088);
            this._gbMagicKey.Controls.Add(this._rbMagicKey40044041);
            this._gbMagicKey.Location = new System.Drawing.Point(12, 66);
            this._gbMagicKey.Name = "_gbMagicKey";
            this._gbMagicKey.Size = new System.Drawing.Size(396, 177);
            this._gbMagicKey.TabIndex = 2;
            this._gbMagicKey.TabStop = false;
            this._gbMagicKey.Text = "Magic Key";
            this._gbMagicKey.Visible = false;
            // 
            // _rbMagicKeyC39D7088
            // 
            this._rbMagicKeyC39D7088.AutoSize = true;
            this._rbMagicKeyC39D7088.Location = new System.Drawing.Point(31, 103);
            this._rbMagicKeyC39D7088.Name = "_rbMagicKeyC39D7088";
            this._rbMagicKeyC39D7088.Size = new System.Drawing.Size(127, 24);
            this._rbMagicKeyC39D7088.TabIndex = 1;
            this._rbMagicKeyC39D7088.TabStop = true;
            this._rbMagicKeyC39D7088.Text = "0xC39D7088";
            this._rbMagicKeyC39D7088.UseVisualStyleBackColor = true;
            this._rbMagicKeyC39D7088.CheckedChanged += new System.EventHandler(this._rbMagicKey_CheckedChanged);
            // 
            // _rbMagicKey40044041
            // 
            this._rbMagicKey40044041.AutoSize = true;
            this._rbMagicKey40044041.Location = new System.Drawing.Point(31, 55);
            this._rbMagicKey40044041.Name = "_rbMagicKey40044041";
            this._rbMagicKey40044041.Size = new System.Drawing.Size(267, 24);
            this._rbMagicKey40044041.TabIndex = 0;
            this._rbMagicKey40044041.TabStop = true;
            this._rbMagicKey40044041.Text = "0x40044041 (ReelMagic Default)";
            this._rbMagicKey40044041.UseVisualStyleBackColor = true;
            this._rbMagicKey40044041.CheckedChanged += new System.EventHandler(this._rbMagicKey_CheckedChanged);
            // 
            // _gbFCode
            // 
            this._gbFCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._gbFCode.Controls.Add(this._nudBPictureBackwardFCode);
            this._gbFCode.Controls.Add(this.lblBPictureBackward);
            this._gbFCode.Controls.Add(this._nudBPictureForwardFCode);
            this._gbFCode.Controls.Add(this.lblBPictureForward);
            this._gbFCode.Controls.Add(this._nudPPictureForwardFCode);
            this._gbFCode.Controls.Add(this.lblPPictureForward);
            this._gbFCode.Location = new System.Drawing.Point(12, 295);
            this._gbFCode.Name = "_gbFCode";
            this._gbFCode.Size = new System.Drawing.Size(396, 177);
            this._gbFCode.TabIndex = 3;
            this._gbFCode.TabStop = false;
            this._gbFCode.Text = "f_code Values";
            this._gbFCode.Visible = false;
            // 
            // _nudBPictureBackwardFCode
            // 
            this._nudBPictureBackwardFCode.Location = new System.Drawing.Point(159, 137);
            this._nudBPictureBackwardFCode.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this._nudBPictureBackwardFCode.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._nudBPictureBackwardFCode.Name = "_nudBPictureBackwardFCode";
            this._nudBPictureBackwardFCode.Size = new System.Drawing.Size(93, 26);
            this._nudBPictureBackwardFCode.TabIndex = 5;
            this._nudBPictureBackwardFCode.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._nudBPictureBackwardFCode.ValueChanged += new System.EventHandler(this._nudFCode_ValueChanged);
            // 
            // lblBPictureBackward
            // 
            this.lblBPictureBackward.AutoSize = true;
            this.lblBPictureBackward.Location = new System.Drawing.Point(6, 137);
            this.lblBPictureBackward.Name = "lblBPictureBackward";
            this.lblBPictureBackward.Size = new System.Drawing.Size(147, 20);
            this.lblBPictureBackward.TabIndex = 4;
            this.lblBPictureBackward.Text = "B Picture Backward";
            // 
            // _nudBPictureForwardFCode
            // 
            this._nudBPictureForwardFCode.Location = new System.Drawing.Point(159, 89);
            this._nudBPictureForwardFCode.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this._nudBPictureForwardFCode.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._nudBPictureForwardFCode.Name = "_nudBPictureForwardFCode";
            this._nudBPictureForwardFCode.Size = new System.Drawing.Size(93, 26);
            this._nudBPictureForwardFCode.TabIndex = 3;
            this._nudBPictureForwardFCode.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._nudBPictureForwardFCode.ValueChanged += new System.EventHandler(this._nudFCode_ValueChanged);
            // 
            // lblBPictureForward
            // 
            this.lblBPictureForward.AutoSize = true;
            this.lblBPictureForward.Location = new System.Drawing.Point(6, 91);
            this.lblBPictureForward.Name = "lblBPictureForward";
            this.lblBPictureForward.Size = new System.Drawing.Size(135, 20);
            this.lblBPictureForward.TabIndex = 2;
            this.lblBPictureForward.Text = "B Picture Forward";
            // 
            // _nudPPictureForwardFCode
            // 
            this._nudPPictureForwardFCode.Location = new System.Drawing.Point(159, 42);
            this._nudPPictureForwardFCode.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this._nudPPictureForwardFCode.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._nudPPictureForwardFCode.Name = "_nudPPictureForwardFCode";
            this._nudPPictureForwardFCode.Size = new System.Drawing.Size(93, 26);
            this._nudPPictureForwardFCode.TabIndex = 1;
            this._nudPPictureForwardFCode.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._nudPPictureForwardFCode.ValueChanged += new System.EventHandler(this._nudFCode_ValueChanged);
            // 
            // lblPPictureForward
            // 
            this.lblPPictureForward.AutoSize = true;
            this.lblPPictureForward.Location = new System.Drawing.Point(6, 44);
            this.lblPPictureForward.Name = "lblPPictureForward";
            this.lblPPictureForward.Size = new System.Drawing.Size(134, 20);
            this.lblPPictureForward.TabIndex = 0;
            this.lblPPictureForward.Text = "P Picture Forward";
            // 
            // ReelMagicVideoConverterSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 494);
            this.Controls.Add(this._gbFCode);
            this.Controls.Add(this._gbMagicKey);
            this.Controls.Add(this.lblDecodeMode);
            this.Controls.Add(this._cboDecodeMode);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReelMagicVideoConverterSettings";
            this.ShowInTaskbar = false;
            this.Text = "Reel Magic Video Converter Settings";
            this._gbMagicKey.ResumeLayout(false);
            this._gbMagicKey.PerformLayout();
            this._gbFCode.ResumeLayout(false);
            this._gbFCode.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._nudBPictureBackwardFCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._nudBPictureForwardFCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._nudPPictureForwardFCode)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox _cboDecodeMode;
        private System.Windows.Forms.Label lblDecodeMode;
        private System.Windows.Forms.GroupBox _gbMagicKey;
        private System.Windows.Forms.RadioButton _rbMagicKeyC39D7088;
        private System.Windows.Forms.RadioButton _rbMagicKey40044041;
        private System.Windows.Forms.GroupBox _gbFCode;
        private System.Windows.Forms.NumericUpDown _nudBPictureBackwardFCode;
        private System.Windows.Forms.Label lblBPictureBackward;
        private System.Windows.Forms.NumericUpDown _nudBPictureForwardFCode;
        private System.Windows.Forms.Label lblBPictureForward;
        private System.Windows.Forms.NumericUpDown _nudPPictureForwardFCode;
        private System.Windows.Forms.Label lblPPictureForward;
    }
}