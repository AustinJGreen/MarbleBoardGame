using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MarbleBoardGame
{
    public class Line
    {
        private Vector2 p1, p2; //this will be the position in the center of the line
        private int length; //length and thickness of the line, or width and height of rectangle
        private Rectangle rect;
        private float rotation; // rotation of the line, with axis at the center of the line
        private Color color;

        /// <summary>
        /// Creates a line segment
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="thickness"></param>
        /// <param name="color"></param>
        public Line(Point p1, Point p2, int thickness, Color color)
        {
            this.p1 = new Vector2(p1.X, p1.Y);
            this.p2 = new Vector2(p2.X, p2.Y);
            this.color = color;

            Update();
        }

        /// <summary>
        /// Updates the line
        /// </summary>
        public void Update()
        {
            length = (int)Vector2.Distance(p1, p2);
            rotation = GetRotation(p1.X, p1.Y, p2.X, p2.Y);
            rect = new Rectangle((int)p1.X, (int)p1.Y, length, 20);
        }

        /// <summary>
        /// Draws the line
        /// </summary>
        /// <param name="spriteBatch">Sprite Batch</param>
        /// <param name="textures">Textures</param>
        public void Draw(SpriteBatch spriteBatch, TextureLib textures)
        {
            spriteBatch.Draw(textures["arrow"], rect, null, color, rotation, new Vector2(0), SpriteEffects.None, 0.0f);
        }

        /// <summary>
        /// Calculates the angle between two points in radians 
        /// </summary>
        /// <param name="x">First x coordinate</param>
        /// <param name="y">First y coordinate</param>
        /// <param name="x2">Second x coordinate</param>
        /// <param name="y2">Second y coordinate</param>
        private float GetRotation(float x, float y, float x2, float y2)
        {
            float adj = x - x2;
            float opp = y - y2;
            float tan = opp / adj;
            float res = MathHelper.ToDegrees((float)Math.Atan2(opp, adj));
            res = (res - 180) % 360;
            if (res < 0) { res += 360; }
            res = MathHelper.ToRadians(res);
            return res;
        }
    }
}
