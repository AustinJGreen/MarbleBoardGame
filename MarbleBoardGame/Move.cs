using System.Text;

namespace MarbleBoardGame
{
    public class Move
    {
        /// <summary>
        /// List of individual piece moves
        /// </summary>
        public PieceMove[] PieceMoves { get; set; }

        /// <summary>
        /// Gets the PieceMove at the specified index
        /// </summary>
        /// <param name="index">Index of PieceMove</param>
        public PieceMove this[int index]
        {
            get 
            {
                return PieceMoves[index];
            }
        }

        /// <summary>
        /// Gets the number of pieces involved in the move
        /// </summary>
        public int Pieces { get { return PieceMoves.Length; } }

        /// <summary>
        /// Team of the move
        /// </summary>
        public sbyte Team { get; set; }

        /// <summary>
        /// Gets the notation for the move
        /// </summary>
        public string GetNotation()
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < Pieces; i++)
            {
                builder.Append(this[i].GetNotation());
                if (i != Pieces - 1)
                {
                    builder.Append(", ");
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Checks if the move is the same as another move disgregarding order
        /// </summary>
        /// <param name="move">Move to check</param>
        public bool Equals(Move move)
        {
            if (Pieces == move.Pieces)
            {
                for (int i = 0; i < Pieces; i++)
                {
                    bool matched = false;
                    for (int j = 0; j < move.Pieces; j++)
                    {
                        if (this[i].Equals(move[j]))
                        {
                            matched = true;
                            break;
                        }
                    }

                    if (!matched)
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the notation for the move
        /// </summary>
        public override string ToString()
        {
            return GetNotation();
        }

        /// <summary>
        /// Create a move
        /// </summary>
        /// <param name="move">Piece move</param>
        public Move(PieceMove move, sbyte team)
        {
            PieceMoves = new PieceMove[] { move };
            Team = team;
        }

        /// <summary>
        /// Create a move
        /// </summary>
        /// <param name="pieceMoves">Series of piece moves</param>
        public Move(PieceMove[] pieceMoves, sbyte team)
        {
            PieceMoves = pieceMoves;
            Team = team;
        }
    }
}
