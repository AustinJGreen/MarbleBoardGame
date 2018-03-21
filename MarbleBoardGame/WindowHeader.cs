using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarbleBoardGame
{
    public delegate T TransformDelegate<T>(T item);

    public class WindowHeader
    {
        private int lastWidth = -1;
        private TransformDelegate<Point> transform;
        private ContextMenuStrip menuStrip;
        private MouseEventArgs mouseState;

        public event EventHandler OnCloseClicked;

        public string Text { get; set; }

        public void Update(MouseEventArgs mouseState)
        {
            this.mouseState = mouseState;

            if (mouseState.Button == MouseButtons.Left && lastWidth != -1)
            {
                Rectangle triangleBox = new Rectangle(lastWidth - 17 - 11, 2, 12, 12);
                if (triangleBox.Contains(mouseState.Location))
                {
                    menuStrip.Show(transform(mouseState.Location));
                }

                Rectangle closeBox = new Rectangle(lastWidth - 14, 2, 12, 12);
                if (closeBox.Contains(mouseState.Location))
                {
                    if (OnCloseClicked != null)
                    {
                        OnCloseClicked(this, EventArgs.Empty);
                    }
                }
            }
        }

        public void MouseLeave()
        {
            if (mouseState != null)
            {
                this.mouseState = new MouseEventArgs(MouseButtons.None, mouseState.Clicks, mouseState.X, mouseState.Y, mouseState.Delta);
            }
        }

        public void Draw(Graphics gfx, int x, int y, int width)
        {
            this.lastWidth = width;
            Rectangle c = new Rectangle(0, 0, width, 16);

            Rectangle closeButton = new Rectangle(width - 14, x + 2, y + 12, 12);
            Color borderColor = Color.FromArgb(255, 190 + 55, 245);
            if (mouseState != null && closeButton.Contains(mouseState.Location))
            {
                Color bC = Color.FromArgb(194 + 25, 190 + 25, 209 + 25);
                SolidBrush bB = new SolidBrush(bC);
                Pen border = new Pen(bB, 1.0f);
                gfx.DrawRectangle(border, c);
            }
            else
            {
                //SolidBrush borderBrush = new SolidBrush(borderColor);
                //Pen border = new Pen(borderBrush, 1.0f);
                //gfx.DrawRectangle(border, c);
            }


            Color fillColor = Color.Plum;// Color.FromArgb(180 - 25, 176 - 25, 185 - 25);
            Brush gradient = new LinearGradientBrush(c, borderColor, fillColor, LinearGradientMode.ForwardDiagonal);
            gfx.FillRectangle(gradient, c);

            Font font = SystemFonts.CaptionFont;

            string fitted_text = Text;
            SizeF textSize = gfx.MeasureString(fitted_text, font);

            int margin = 30;

            if (textSize.Width > width - margin)
            {
                SizeF ellipsis = gfx.MeasureString("...", font);

                while (textSize.Width > width - ellipsis.Width - margin && fitted_text.Length > 0)
                {
                    fitted_text = fitted_text.Substring(0, fitted_text.Length - 1);
                    textSize = gfx.MeasureString(fitted_text, font);
                }

                fitted_text += "...";
            }

            gfx.DrawString(fitted_text, SystemFonts.CaptionFont, Brushes.Black, PointF.Empty);

            DrawPaneOrderControl(gfx, width);
            DrawCloseControl(gfx, width);
        }

        private void DrawPaneOrderControl(Graphics gfx, int width)
        {
            Rectangle triangleBox = new Rectangle(width - 17 - 11, 2, 12, 12);
            Point[] triangle = new Point[]
            {
                new Point(width - 17 - 6, 9 + 2), //Top Vertex
                new Point(width - 17 - 11, 3 + 2), //Left Vertex
                new Point(width - 17 - 1, 3 + 2) //Right Vertex
            };

            if (mouseState != null && triangleBox.Contains(mouseState.Location))
            {
                gfx.SmoothingMode = SmoothingMode.AntiAlias;
                gfx.FillPolygon(new SolidBrush(Color.FromArgb(94, 96, 105)), triangle);
            }
            else
            {
                gfx.SmoothingMode = SmoothingMode.AntiAlias;
                gfx.FillPolygon(new SolidBrush(Color.FromArgb(94 - 25, 96 - 25, 105 - 15)), triangle);
            }
        }

        private void DrawCloseControl(Graphics gfx, int width)
        {
            Rectangle closeButton = new Rectangle(width - 14, 2, 12, 12);
            if (mouseState != null && closeButton.Contains(mouseState.Location))
            {
                Color red1 = Color.FromArgb(254, 142, 148);
                Color red2 = Color.FromArgb(252, 55, 45);
                Brush gradient = new LinearGradientBrush(closeButton, red1, red2, LinearGradientMode.ForwardDiagonal);
                gfx.FillRectangle(gradient, closeButton);

                Pen whiteLine = new Pen(new SolidBrush(Color.Bisque), 1f);

                gfx.SmoothingMode = SmoothingMode.AntiAlias;
                gfx.DrawLine(whiteLine, new Point(width - 12, 4), new Point(width - 5, 11));
                gfx.DrawLine(whiteLine, new Point(width - 5, 4), new Point(width - 12, 11));
            }
            else
            {
                Color red1 = Color.FromArgb(214, 112, 118);
                Color red2 = Color.FromArgb(232, 45, 32);
                Brush gradient = new LinearGradientBrush(closeButton, red1, red2, LinearGradientMode.ForwardDiagonal);
                gfx.FillRectangle(gradient, closeButton);

                Pen whiteLine = new Pen(new SolidBrush(Color.White), 1f);

                gfx.SmoothingMode = SmoothingMode.AntiAlias;
                gfx.DrawLine(whiteLine, new Point(width - 12, 4), new Point(width - 5, 11));
                gfx.DrawLine(whiteLine, new Point(width - 5, 4), new Point(width - 12, 11));
            }
        }

        public WindowHeader(ContextMenuStrip menuStrip, TransformDelegate<Point> transform)
        {
            this.menuStrip = menuStrip;
            this.transform = transform;
        }
    }
}
