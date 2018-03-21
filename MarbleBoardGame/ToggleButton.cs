using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarbleBoardGame
{
    class ToggleButton : IObject
    {
        /// <summary>
        /// Gets or Sets the sprite for the button to render
        /// </summary>
        public Texture2D[] Sprites { get; set; }

        /// <summary>
        /// Currently drawn sprite
        /// </summary>
        public int SelectionIndex { get; set; }

        /// <summary>
        /// Gets or Sets the position of the button
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Is the mouse over the button
        /// </summary>
        public bool IsMouseOver { get; set; }

        /// <summary>
        /// Is the mouse pressed on the button while over it
        /// </summary>
        public bool IsMouseDown { get; set; }

        public void Draw(SpriteBatch batch, GameContent content)
        {
            if (!IsMouseOver)
            {
                batch.Draw(Sprites[SelectionIndex], Position, Color.White);
            }
            else
            {

                batch.Draw(Sprites[SelectionIndex], Position, Sprites[SelectionIndex].Bounds, Color.White, 0, Vector2.Zero, 1.15f, SpriteEffects.None, 1);
            }
        }

        public void Update(GameTime gameTime)
        {
            MouseState state = Mouse.GetState();

            if (state.X >= Position.X && state.X <= Position.X + Sprites[SelectionIndex].Width &&
                state.Y >= Position.Y && state.Y <= Position.Y + Sprites[SelectionIndex].Height)
            {

                if (IsMouseDown && state.LeftButton == ButtonState.Released)
                {
                    SelectionIndex++;
                    SelectionIndex %= Sprites.Length;
                }

                IsMouseDown = state.LeftButton == ButtonState.Pressed;

                IsMouseOver = true;
            }
            else
            {
                IsMouseOver = false;
            }
        }

        public ToggleButton(Texture2D[] sprites, Vector2 position)
        {
            Sprites = sprites;
            Position = position;
        }
    }
}
