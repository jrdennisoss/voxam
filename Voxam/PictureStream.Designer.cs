namespace Voxam
{
    partial class PictureStream
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
            this._scrollbar = new System.Windows.Forms.HScrollBar();
            this.SuspendLayout();
            // 
            // _scrollbar
            // 
            this._scrollbar.Location = new System.Drawing.Point(21, 38);
            this._scrollbar.Name = "_scrollbar";
            this._scrollbar.Size = new System.Drawing.Size(352, 16);
            this._scrollbar.TabIndex = 0;
            this._scrollbar.Scroll += new System.Windows.Forms.ScrollEventHandler(this._scrollbar_Scroll);
            // 
            // PictureStream
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this._scrollbar);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "PictureStream";
            this.Size = new System.Drawing.Size(620, 157);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.PictureStream_Paint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.PictureStream_MouseClick);
            this.Resize += new System.EventHandler(this.PictureStream_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.HScrollBar _scrollbar;
    }
}
