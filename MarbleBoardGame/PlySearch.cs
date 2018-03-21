using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarbleBoardGame
{
    public class PlySearch : Search
    {
        /// <summary>
        /// Current ply depth
        /// </summary>
        public int CurrentPlyDepth { get; set; }
        
        /// <summary>
        /// Next target depth
        /// </summary>
        public int NextDepth { get; set; }

        /// <summary>
        /// Is the search maximizing
        /// </summary>
        public bool Maximizing { get; set; }

        /// <summary>
        /// Gets the next target search for a minimax algorithm
        /// </summary>
        /// <param name="current">Next node</param>
        public PlySearch NextMiniMax(PositionNode current)
        {
            //Flipped maximizing because this is normally after we invert the bool
            sbyte nextPlayer = (!Maximizing) ? board.NextPlayer(CurrentPlayer) : CurrentPlayer;

            PlySearch next = new PlySearch(board, current, RootPlayer, nextPlayer, CurrentPlyDepth - 1);
            next.Maximizing = !Maximizing;
            next.NextDepth = next.CurrentPlyDepth - 1;

            if (current.RolledDoubles)
            {
                next.NextPlayer = next.CurrentPlayer;
                next.NextDepth = next.CurrentPlyDepth;

                next.DoublesCount = DoublesCount + 1;
                if (next.DoublesCount >= 3)
                {
                    current.Value.RemoveFrontMarble(next.CurrentPlayer);
                    next.DoublesCount = 0;

                    next.NextPlayer = board.NextPlayer(next.CurrentPlayer);
                    next.NextDepth = next.CurrentPlyDepth - 1;
                }
            }
            else
            {
                next.DoublesCount = 0;
            }

            return next;
        }

        /// <summary>
        /// Gets the next target search
        /// </summary>
        /// <param name="current">Next Node</param>
        public PlySearch Next(PositionNode current)
        {
            sbyte nextPlayer = board.NextPlayer(CurrentPlayer);

            PlySearch next = new PlySearch(board, current, RootPlayer, nextPlayer, CurrentPlyDepth - 1);
            next.NextDepth = next.CurrentPlyDepth - 1;

            if (current.RolledDoubles)
            {
                next.NextPlayer = next.CurrentPlayer;
                next.NextDepth = next.CurrentPlyDepth;

                next.DoublesCount = DoublesCount + 1;
                if (next.DoublesCount >= 3)
                {
                    current.Value.RemoveFrontMarble(next.CurrentPlayer);
                    next.DoublesCount = 0;

                    next.NextPlayer = board.NextPlayer(next.CurrentPlayer);
                    next.NextDepth = next.CurrentPlyDepth - 1;
                }
            }
            else
            {
                next.DoublesCount = 0;
            }

            return next;
        }

        /// <summary>
        /// Creates a new ply search
        /// </summary>
        /// <param name="board">Board</param>
        /// <param name="node">Current node</param>
        /// <param name="rootPlayer">Root player</param>
        /// <param name="currentPlayer">Current player</param>
        /// <param name="startPlyDepth">Current depth</param>
        public PlySearch(Board board, PositionNode node, sbyte rootPlayer, sbyte currentPlayer, int startPlyDepth) : base(board, node, rootPlayer, currentPlayer)
        {
            CurrentPlyDepth = startPlyDepth;
        }
    }
}
