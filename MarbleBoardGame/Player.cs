namespace MarbleBoardGame
{
    public abstract class Player
    {
        private Move move;
        protected BoardView boardView;

        /// <summary>
        /// Is the player human
        /// </summary>
        public bool IsHuman { get; protected set; }

        /// <summary>
        /// Gets the team of the player
        /// </summary>
        public sbyte Team { get; protected set; }

        /// <summary>
        /// Gets whether the player is thinking
        /// </summary>
        public bool IsThinking { get; private set; }

        /// <summary>
        /// Has a move to be played
        /// </summary>
        public bool HasMove { get; set; }

        /// <summary>
        /// Gets the players move
        /// </summary>
        public virtual void StartThink(Board board, DiceRoll roll)
        {
            IsThinking = true;
        }

        /// <summary>
        /// Sets the current move
        /// </summary>
        /// <param name="move">Move</param>
        public void SetMove(Move move)
        {
            this.move = move;
            HasMove = true;
            IsThinking = false;
        }

        /// <summary>
        /// Gets the move if any calculated and sets it back to null
        /// </summary>
        /// <returns></returns>
        public Move GetMove()
        {
            if (HasMove)
            {
                HasMove = false;
                return move;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Creates a base player
        /// </summary>
        /// <param name="team">Team</param>
        /// <param name="isHuman">Is the player human</param>
        /// <param name="callback">Move found callback</param>
        protected Player(sbyte team, bool isHuman, BoardView boardView)
        {
            this.boardView = boardView;

            Team = team;
            IsHuman = isHuman;
        }
    }
}
