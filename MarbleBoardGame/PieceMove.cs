using System.Collections.Generic;
using System.Text;

namespace MarbleBoardGame
{
    public class PieceMove
    {
        /// <summary>
        /// Evaluates whether the move involves taking a marble from their base
        /// </summary>
        public bool IsTakingOutMarble { get { return From == null; } }
        
        /// <summary>
        /// Evaluates whether the move is a valid move in terms of syntax
        /// </summary>
        public bool ValidSyntax
        {
            get
            {
                bool validFromSquare = true;
                if (From != null)
                {
                    validFromSquare = From.Valid();
                }

                if (To != null)
                {
                    return validFromSquare && To.Valid();
                }

                return false;
            }
        }

        /// <summary>
        /// The square the marble is moving from, or null if coming from the start
        /// </summary>
        public Square From;

        /// <summary>
        /// The square the marble is moving to
        /// </summary>
        public Square To;

        /// <summary>
        /// Gets the move notation for this move
        /// </summary>
        public string GetNotation()
        {
            if (!ValidSyntax)
            {
                return null;
            }

            StringBuilder notation = new StringBuilder();
            if (From != null) //Check if from is null (If IsTakingOutMarble)
            {
                notation.Append(From.GetNotation());
            }

            //To should never be null
            notation.Append(To.GetNotation());
            return notation.ToString();
        }

        /// <summary>
        /// Gets a chronological path from the start quadrant to the target quadrant
        /// </summary>
        /// <param name="team">Team</param>
        public List<int> GetQuadrantPath(sbyte team)
        {
            List<int> quadrants = new List<int>();

            bool adding = false;
            sbyte[] order = Board.QUAD_ORDER[team];
            for (int i = 0; i < order.Length; i++)
            {
                if (order[i] == From.QuadrantValue)
                {                 
                    adding = true;
                }
                else if (order[i] == To.QuadrantValue)
                {                  
                    adding = false;
                    quadrants.Add(order[i]);
                }

                if (adding)
                {
                    quadrants.Add(order[i]);
                }
            }

            return quadrants;
        }

        /// <summary>
        /// Gets the chronological path of squares from the start to the target squares
        /// </summary>
        /// <param name="team">Team</param>
        public Square[] GetPath(sbyte team)
        {
            Square from = (From == null) ? new Square(To.QuadrantValue, 0) : From;

            int pathLength = GetPathLength(team);
            Square[] path = new Square[pathLength];

            for (int i = 0; i < path.Length; i++)
            {
                int squareIndex = from.GetBoardIndex(team) + i;

                int targetQuadIndexUnordered = (squareIndex / Board.QUAD_LENGTH);
                int targetQuadIndex = (targetQuadIndexUnordered < Board.QUAD_COUNT) ? Board.QUAD_ORDER[team][targetQuadIndexUnordered] : team + 4;
                int quad = (targetQuadIndex < Board.QUAD_COUNT) ? Board.GetQuadIndex(team, targetQuadIndex) : team + 4;
                int square = squareIndex % Board.QUAD_LENGTH;
                path[i] = new Square((quad < Board.QUAD_COUNT) ? Board.QUAD_ORDER[team][quad] : quad, square);
            }

            return path;
        }

        /// <summary>
        /// Gets the path length from the start to the target
        /// </summary>
        /// <param name="team">Team</param>
        public int GetPathLength(sbyte team)
        {
            if (From == null)
            {
                return To.GetBoardIndex(team) - new Square(To.QuadrantValue, 0).GetBoardIndex(team);
            }

            return To.GetBoardIndex(team) - From.GetBoardIndex(team);
        }

        /// <summary>
        /// Gets the move notation for this move
        /// </summary>
        public override string ToString()
        {
            return ValidSyntax ? GetNotation() : "INVALID";
        }

        /// <summary>
        /// Evaluates whether this move is the same as another move
        /// </summary>
        /// <param name="square">Move to evaluate</param>
        public bool Equals(PieceMove move)
        {
            if (From == null)
            {
                return move.From == null && To.Equals(move.To);
            }

            return From.Equals(move.From) && To.Equals(move.To);
        }

        /// <summary>
        /// Finds the index of the split in the move notation that describes the moves squares
        /// </summary>
        /// <param name="moveNotation">Move notation</param>
        private int ParseSplit(string moveNotation)
        {
            bool inNumber = false;

            char[] moveNotationArray = moveNotation.ToCharArray();
            for (int i = 0; i < moveNotationArray.Length; i++)
            {
                if (!inNumber)
                {
                    if (char.IsNumber(moveNotationArray[i]))
                    {
                        inNumber = true;
                    }
                }
                else
                {
                    if (!char.IsNumber(moveNotationArray[i]))
                    {
                        return i;                       
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Parses move notation into a move
        /// </summary>
        /// <param name="moveNotation">Move notation</param>
        private void Parse(string moveNotation, Board board)
        {
            if (moveNotation.Contains("+"))
            {
                int index = moveNotation.IndexOf('+');
                From = new Square(moveNotation.Substring(0, index));

                sbyte team = (sbyte)board.Get(From);
                if (team == -1)
                {
                    return;
                }

                int addValue = int.Parse(moveNotation.Substring(index + 1));
                To = From.Add(addValue, team);
            }
            else
            {
                int split = ParseSplit(moveNotation);
                if (split == -1)
                {
                    //Move is taking out a piece
                    From = null;
                    To = new Square(moveNotation);
                }
                else
                {
                    From = new Square(moveNotation.Substring(0, split));
                    To = new Square(moveNotation.Substring(split));
                }
            }
        }

        /// <summary>
        /// Create a move
        /// </summary>
        /// <param name="moveNotation">Move notation</param>
        public PieceMove(string moveNotation, Board board)
        {
            Parse(moveNotation, board);
        }

        /// <summary>
        /// Creates a move using two squares
        /// </summary>
        /// <param name="from">From square</param>
        /// <param name="to">To square</param>
        public PieceMove(Square from, Square to)
        {
            From = from;
            To = to;
        }
    }
}
