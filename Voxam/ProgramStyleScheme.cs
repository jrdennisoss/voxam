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
using System.Drawing;
using System.Windows.Forms;

namespace Voxam
{
    public class ProgramStyleScheme
    {
        internal interface IStyleProvider
        {
            Color FormBackColor { get; }
            Color DefaultTextColor { get; }
            Color MenuStripBackColor { get; }
            Color ToolStripSeparatorColor { get; }
            Color ButtonBackColor { get; }
        }

        internal readonly IStyleProvider StyleProvider = new DefaultStyle();
        //internal readonly IStyleProvider StyleProvider = new DarkStyle();

        internal void StyleForm(Form f) => StyleControl(f, true);

        internal void StyleControl(Control c, bool recursive = true)
        {
            c.BackColor = this.StyleProvider.FormBackColor;
            c.ForeColor = this.StyleProvider.DefaultTextColor;
            if (recursive) StyleControlChildren(c, recursive);
        }

        internal void StyleControlChildren(Control parent, bool recursive = true)
        {
            foreach (Control c in parent.Controls)
            {
                if (c == null) continue;
                if (recursive && c.HasChildren) StyleControlChildren(c, recursive);

                if (c is MenuStrip) StyleMenuStrip((MenuStrip)c, recursive);
                else if (c is Button) StyleButton((Button)c);
            }
        }



        //This bullshit is used to fix the broken ToolStripSeparator BackColor and ForeColor attributes not working
        //solution modified from: https://stackoverflow.com/questions/15926377/change-the-backcolor-of-the-toolstripseparator-control
        private class FixedToolStripSeparatorRenderer : ToolStripProfessionalRenderer
        {
            protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
            {
                var tss = e.Item as ToolStripSeparator;
                if (tss == null)
                {
                    base.OnRenderSeparator(e);
                    return;
                }
                if ((tss.BackColor == SystemColors.Control) && (tss.ForeColor == SystemColors.ControlDark))
                {
                    base.OnRenderSeparator(e);
                    return;
                }

                int width = tss.Width;
                int height = tss.Height;

                if (tss.BackColor != SystemColors.Control)
                {
                    using (var backColorBrush = new SolidBrush(tss.BackColor))
                        e.Graphics.FillRectangle(backColorBrush, 0, 0, width, height);
                }
                using (Pen foreColorPen = new Pen(tss.ForeColor))
                    e.Graphics.DrawLine(foreColorPen, 4, height / 2, width - 4, height / 2);
            }
        }
        private ToolStripRenderer _toolStripSeparatorRendererFix = new FixedToolStripSeparatorRenderer();

        internal void StyleMenuStrip(MenuStrip ms, bool recursive = true)
        {
            ms.BackColor = this.StyleProvider.MenuStripBackColor;
            ms.ForeColor = this.StyleProvider.DefaultTextColor;
            ms.Renderer = _toolStripSeparatorRendererFix;
            if (recursive) StyleToolStripItemCollection(ms.Items);
        }
        internal void StyleToolStripMenuItem(ToolStripMenuItem tsmi, bool recursive = true)
        {
            tsmi.BackColor = this.StyleProvider.MenuStripBackColor;
            tsmi.ForeColor = this.StyleProvider.DefaultTextColor;
            if (recursive) StyleToolStripItemCollection(tsmi.DropDownItems);
        }

        internal void StyleToolStripSeparator(ToolStripSeparator tss)
        {
            tss.BackColor = this.StyleProvider.MenuStripBackColor;
            tss.ForeColor = this.StyleProvider.ToolStripSeparatorColor;
        }

        private void StyleToolStripItemCollection(ToolStripItemCollection tsic)
        {
            foreach (ToolStripItem tsi in tsic)
            {
                if (tsi is ToolStripMenuItem) StyleToolStripMenuItem((ToolStripMenuItem)tsi);
                else if (tsi is ToolStripSeparator) StyleToolStripSeparator((ToolStripSeparator)tsi);
            }
        }


        internal void StyleButton(Button b)
        {
            b.BackColor = this.StyleProvider.ButtonBackColor;
            b.ForeColor = this.StyleProvider.DefaultTextColor;
        }







        internal class DefaultStyle : IStyleProvider
        {
            public Color FormBackColor => SystemColors.Control;

            public Color DefaultTextColor => SystemColors.ControlText;
            public Color MenuStripBackColor => FormBackColor;
            public Color ToolStripSeparatorColor => SystemColors.ControlDark;
            public Color ButtonBackColor => FormBackColor;
        }

        internal class DarkStyle : IStyleProvider
        {
            public Color FormBackColor => SystemColors.ControlDarkDark;

            public Color DefaultTextColor => Color.Blue;
            public Color MenuStripBackColor => FormBackColor;
            public Color ToolStripSeparatorColor => Color.Blue;
            public Color ButtonBackColor => FormBackColor;
        }
    }
}
