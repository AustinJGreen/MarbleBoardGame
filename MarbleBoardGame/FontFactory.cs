using Microsoft.Xna.Framework.Graphics;
using System.Drawing;

namespace MarbleBoardGame
{
    /// <summary>
    /// Class for generating a smart fonts
    /// </summary>
    public class FontFactory
    {
        private GraphicsDevice device;

        /// <summary>
        /// Gets the size of a character given a font
        /// </summary>
        /// <param name="c">Chracter to get size of</param>
        /// <param name="font">Font</param>
        public SizeF GetBitmapSize(char c, Font font)
        {
            Bitmap bmp = new Bitmap(1, 1);
            Graphics gfx = Graphics.FromImage(bmp);
            SizeF s = gfx.MeasureString(c.ToString(), font);
            gfx.Dispose();
            bmp.Dispose();
            return s;
        }

        /// <summary>
        /// Sets the bitmaps alpha
        /// </summary>
        /// <param name="bitmap">Bitmap to set alpha for</param>
        public void SetAlpha(Bitmap bitmap)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(0, 0, 0, 0));
                }
            }
        }

        /// <summary>
        /// Loads a font
        /// </summary>
        /// <param name="fontName">Font Name</param>
        /// <param name="size">Size of point in pts</param>
        public SmartFont LoadFont(string fontName, float size)
        {
            Font font = new Font(fontName, size);
            Bitmap[] bitmaps = new Bitmap[255];

            for (int i = 0; i < 255; i++)
            {
                char cur = (char)i;
                SizeF bmpSize = GetBitmapSize(cur, font);
                Bitmap bitmap = new Bitmap((int)bmpSize.Width, (int)bmpSize.Height);
                SetAlpha(bitmap);
                using (Graphics gfx = Graphics.FromImage(bitmap))
                {
                    string current = cur.ToString();
                    SizeF position = gfx.MeasureString(current, font);

                    gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    gfx.DrawString(current, font, Brushes.White, new PointF(0, bitmap.Height - position.Height));
                }
                bitmaps[i] = bitmap;
            }

            return new SmartFont(device, bitmaps);
        }

        /// <summary>
        /// Creates a new font factory for creating smart fonts
        /// </summary>
        /// <param name="device">Graphics Device</param>
        public FontFactory(GraphicsDevice device)
        {
            this.device = device;
        }
    }
}
