using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;

namespace MarbleBoardGame
{
    /// <summary>
    /// Fix for monogame fonts
    /// </summary>
    public class SmartFont
    {
        private Bitmap[] bitmaps;
        private Texture2D[] characters;
        private GraphicsDevice device;

        private Texture2D ToTexture(int i)
        {
            Texture2D texture = new Texture2D(device, bitmaps[i].Width, bitmaps[i].Height);
            Microsoft.Xna.Framework.Color[] data = new Microsoft.Xna.Framework.Color[texture.Width * texture.Height];
            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    int index = (y * texture.Width) + x;

                    System.Drawing.Color pix = bitmaps[i].GetPixel(x, y);
                    Microsoft.Xna.Framework.Color xna = new Microsoft.Xna.Framework.Color(pix.R, pix.G, pix.B, pix.A);
                    data[index] = xna;
                }
            }

            texture.SetData<Microsoft.Xna.Framework.Color>(data);
            return texture;
        }

        /// <summary>
        /// Draws text
        /// </summary>
        /// <param name="batch">Sprite Batch</param>
        /// <param name="text">Text to draw</param>
        /// <param name="pos">Position to draw text</param>
        /// <param name="color">Color of text</param>
        public void Draw(SpriteBatch batch, string text, Vector2 pos, Microsoft.Xna.Framework.Color color)
        {
            Vector2 current = pos;

            int x = 0;
            char[] arr = text.ToCharArray();
            for (int i = 0; i < arr.Length; i++)
            {
                int index = (int)arr[i];
                Texture2D t = characters[index];

                batch.Draw(t, new Vector2(pos.X + x, pos.Y), color);
                x += (int)(t.Width * 0.65);
                if (index == 32)
                {
                    x += (int)(characters[65].Width * 0.5f);
                }
            }
        }

        /// <summary>
        /// Loads the font
        /// </summary>
        public SmartFont Load()
        {
            for (int i = 0; i < characters.Length; i++)
            {
                characters[i] = ToTexture(i);
            }

            return this;
        }

        /// <summary>
        /// Unloads all resources used by the font
        /// </summary>
        public void Unload()
        {
            for (int i = 0; i < characters.Length; i++)
            {
                bitmaps[i].Dispose();
                characters[i].Dispose();
            }
        }

        /// <summary>
        /// Creates a smart font
        /// </summary>
        /// <param name="device">Graphics Device</param>
        /// <param name="bitmaps">List of bitmaps</param>
        public SmartFont(GraphicsDevice device, Bitmap[] bitmaps)
        {
            this.device = device;
            this.bitmaps = bitmaps;
            characters = new Texture2D[bitmaps.Length];
        }
    }
}
