using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MarbleBoardGame
{
    public class Sprite : IDrawable
    {
        public Texture2D Texture { get; set; }

        public Rectangle BoundingBox { get; set;  }

        public void Draw(SpriteBatch batch, GameContent content)
        {
            batch.Draw(Texture, BoundingBox, Color.White);
        }

        public Sprite(Texture2D texture, Vector2 position)
        {
            Texture = texture;
            BoundingBox = new Rectangle((int)Math.Round(position.X), (int)Math.Round(position.Y), texture.Width, texture.Height);
        }

        public Sprite(Texture2D texture, Rectangle boundingBox)
        {
            Texture = texture;
            BoundingBox = boundingBox;
        }
    }
}
