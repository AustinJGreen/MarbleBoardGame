using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MarbleBoardGame
{
    public class SquareView
    {
        private bool mdown;
        private BoardView view;

        /// <summary>
        /// Representing marble team
        /// </summary>
        public sbyte Team { get; set; }

        /// <summary>
        /// Bounding Box
        /// </summary>
        public Rectangle Rect { get; set; }

        /// <summary>
        /// Color to highlight
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Is the view selected
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// Is the view clicked
        /// </summary>
        public bool Clicked { get; set; }

        /// <summary>
        /// Has a marble?
        /// </summary>
        public bool HasMarble { get; set; }

        /// <summary>
        /// Gets the position of the square
        /// </summary>
        /// <returns></returns>
        public Vector2 GetPosition()
        {
            return new Vector2(Rect.X, Rect.Y);
        }

        /// <summary>
        /// Updates the view
        /// </summary>
        public void Update()
        {
            MouseState state = MouseHandle.GetState();

            if (Rect.Contains(state.X, state.Y))
            {          
                Selected = true;

                mdown = state.LeftButton == ButtonState.Pressed;
                if (mdown && HasMarble)
                {
                    view.SetStart(this);
                }
            }
            else
            {
                Selected = false;
                mdown = false;
            }

            Color = (Selected || Clicked) ? Color.Gray : Color.White;

            
        }

        /// <summary>
        /// Draws the view
        /// </summary>
        /// <param name="batch">Sprite Batch</param>
        /// <param name="textures">Textures</param>
        public void Draw(SpriteBatch batch, TextureLib textures)
        {
            batch.Draw(textures[view.GetIndentAsset(Team)], Rect, Color);
            if (HasMarble)
            {
                batch.Draw(textures[view.GetMarbleAsset(Team)], Rect, Color);
            }
        }

        /// <summary>
        /// Creates a view of a square
        /// </summary>
        /// <param name="view">View of the board</param>
        /// <param name="rect">Bounding Box</param>
        /// <param name="team">Marble team that the square is representing</param>
        public SquareView(BoardView view, Rectangle rect, sbyte team)
        {
            this.view = view;
            Rect = rect;
            Team = team;
            HasMarble = true;
        }
    }
}
