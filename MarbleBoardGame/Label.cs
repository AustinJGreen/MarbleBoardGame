using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarbleBoardGame
{
    public class Label : IObject
    {
        /// <summary>
        /// Label position
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Label text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Label Font
        /// </summary>
        public SmartFont Font { get; set; }

        /// <summary>
        /// Color of label
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Draws the label
        /// </summary>
        /// <param name="batch">SpriteBatch</param>
        public void Draw(SpriteBatch batch, GameContent content)
        {
            if (Font != null)
            {
                Font.Draw(batch, Text, Position, Color);
            }
        }

        /// <summary>
        /// Updates the label
        /// </summary>
        /// <param name="gameTime">Game Time</param>
        public void Update(GameTime gameTime)
        {

        }

        /// <summary>
        /// Creates a new label object
        /// </summary>
        /// <param name="position">Position of label</param>
        /// <param name="text">Text of label</param>
        /// <param name="font">Label font</param>
        /// <param name="color">Color of label</param>
        public Label(Vector2 position, string text, SmartFont font, Color color)
        {
            Position = position;
            Text = text;
            Font = font;
            Color = color;
        }
    }
}
