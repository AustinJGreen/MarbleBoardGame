using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarbleBoardGame
{
    public class MarbleView
    {
        /// <summary>
        /// Marble team
        /// </summary>
        public sbyte Team { get; set; }

        /// <summary>
        /// From square
        /// </summary>
        public Square From { get; set; }

        /// <summary>
        /// From view
        /// </summary>
        public SquareView FromView { get; set; }

        private BoardView boardView;
        private Vector2 position;
        private Vector2 dragPosition;
        private Vector2 start, target;
        private float elapsed;
        private float targetTime;

        private bool animating = false;
        private bool dragging = false;


        public void StartDrag()
        {
            dragging = true;
        }

        public void DragTo(Vector2 pos)
        {
            if (dragging)
            {
                dragPosition = pos;
            }
        }

        public Square StopDrag()
        {
            //Check if over a square
            dragging = false;
            return boardView.GetSquare(dragPosition);
        }

        /// <summary>
        /// Moves the marble to a position
        /// </summary>
        /// <param name="pos">Target position</param>
        /// <param name="time">Target duration in ms</param>
        public void MoveTo(Vector2 pos, float time)
        {
            start = position;
            target = pos;
            targetTime = time;
            animating = true;
        }

        /// <summary>
        /// Draws the view of the marble
        /// </summary>
        /// <param name="batch">Sprite Batch</param>
        /// <param name="textures">Textures</param>
        public void Draw(SpriteBatch batch, TextureLib textures)
        {
            int marbleWidth = (int)(boardView.View.Width / 29.166666666666666666666666666667);
            int marbleHeight = (int)(boardView.View.Height / 29.166666666666666666666666666667);

            string asset = boardView.GetMarbleAsset(Team);
            if (dragging)
            {
                batch.Draw(textures[asset], new Rectangle((int)dragPosition.X - 14, (int)dragPosition.Y - 14, marbleWidth, marbleHeight), Color.White);
            }
            else
            {
                batch.Draw(textures[asset], new Rectangle((int)position.X, (int)position.Y, marbleWidth, marbleHeight), Color.White);
            }
        }

        /// <summary>
        /// Updates the view
        /// </summary>
        /// <param name="gameTime">Elapsed game time</param>
        public void Update(GameTime gameTime)
        {
            if (animating)
            {
                elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
                position = Vector2.Lerp(start, target, elapsed / targetTime);
                if (elapsed >= targetTime)
                {
                    position = target;
                    animating = false;
                    elapsed = 0;

                    boardView.OnAnimationDone();
                }
            }
        }
        
        /// <summary>
        /// Creates a view of a marble
        /// </summary>
        /// <param name="boardView">View of board</param>
        /// <param name="from">Square of marble</param>
        /// <param name="team">Team of marble</param>
        /// <param name="position">Position of square</param>
        public MarbleView(BoardView boardView, Square from, sbyte team, Vector2 position)
        {
            this.boardView = boardView;
            this.position = position;
            From = from;
            Team = team;      
        }

        /// <summary>
        /// Creates a view of a marble
        /// </summary>
        /// <param name="boardView">View of board</param>
        /// <param name="fromView">Square of marble</param>
        /// <param name="team">Team of marble</param>
        /// <param name="position">Position of square</param>
        public MarbleView(BoardView boardView, SquareView fromView, sbyte team, Vector2 position)
        {
            this.boardView = boardView;
            this.position = position;
            FromView = fromView;
            Team = team;
        }
    }
}
