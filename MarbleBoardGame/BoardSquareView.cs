using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MarbleBoardGame
{
    public class BoardSquareView
    {
        private bool mdown;
        private BoardView boardView;

        /// <summary>
        /// Square of the board
        /// </summary>
        public Square Square { get; set; }

        /// <summary>
        /// Highlight color
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Is selected
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// Is clicked-
        /// </summary>
        public bool Clicked { get; set; }

        /// <summary>
        /// Bounding rectangle
        /// </summary>
        public Rectangle Rect { get; set; }

        /// <summary>
        /// Updates the view
        /// </summary>
        public void Update()
        {
            MouseState state = MouseHandle.GetState();

            if (Rect.Contains(state.X, state.Y))
            {
                Selected = true;

                //if (mdown && state.LeftButton == ButtonState.Released)
                //{
                //    boardView.SetSelected(this);
                //}

                mdown = state.LeftButton == ButtonState.Pressed;
                if (mdown)
                {
                    boardView.SetStart(this);
                }
            }
            else
            {
                mdown = false;
                Selected = false;
            }

            Color = (Selected || Clicked) ? Color.Gray : Color.White;

            
        }

        /// <summary>
        /// Draws the view of the square
        /// </summary>
        /// <param name="batch">SpriteBatch</param>
        /// <param name="textures">Textures</param>
        public void Draw(SpriteBatch batch, TextureLib textures)
        {
            if (Square.IsInBase())
            {
                batch.Draw(textures[boardView.GetIndentAsset(Square.QuadrantValue - 4)], Rect, Color);
            }
            else if (Square.SquareValue == 0)
            {
                batch.Draw(textures[boardView.GetIndentAsset(Square.QuadrantValue)], Rect, Color);
            }
            else
            {
                batch.Draw(textures["indentBrown"], Rect, Color);
            }
        }

        /// <summary>
        /// Creates a view of a square
        /// </summary>
        /// <param name="boardView">BoardView</param>
        /// <param name="square">Square</param>
        /// <param name="rect">Rectangle</param>
        public BoardSquareView(BoardView boardView, Square square, Rectangle rect)
        {
            this.boardView = boardView;

            Square = square;
            Color = Color.White;
            Rect = rect;
        }
    }
}
