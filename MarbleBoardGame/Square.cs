using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace MarbleBoardGame
{
    public class Square
    {
        /// <summary>
        /// Evaluates whether the square is a valid square
        /// </summary>
#if AGGR_INLINE 
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
#endif 
        public bool Valid()
        {
            return QuadrantValue >= 0 && QuadrantValue < Board.QUAD_COUNT + Board.BASE_SIZE && SquareValue >= 0 && SquareValue < GetQuadrantLength();
        }

        /// <summary>
        /// The quadrant value
        /// </summary>
        public int QuadrantValue = -1;

        /// <summary>
        /// The index of the square in the quadrant
        /// </summary>
        public int SquareValue = -1;

        /// <summary>
        /// Adds and returns the resultant square of a value
        /// </summary>
        /// <param name="value">Value to add</param>
        /// <param name="baseQuadrant">Base quadrant</param>
        public Square Add(int value, sbyte baseQuadrant)
        {
            if (value < 0)
            {
                throw new InvalidOperationException("Cannot add a negative value.");
            }

            if (IsInPath())
            {
                int index = Board.GetQuadIndex(baseQuadrant, baseQuadrant);

                //Squares to the end of safety
                int squaresToGoal = (52 - (12 * index)) - (SquareValue + 1);

                //Check if value is greater than end of path
                if (value > squaresToGoal)
                {
                    //Square is not existant
                    return null;
                }

                //Sum of square overlap
                int squareOverlap = SquareValue + value;

                //Get the overlap value into the next quadrant
                int squareOverlapQuadrant = squareOverlap % 12;

                //Get the amount of quadrants the squares skip
                int quadrantsOverlap = (squareOverlap / 12);

                //The target index in order of the teams quadrant
                int targetIndex = Board.GetQuadIndex(baseQuadrant, QuadrantValue) + quadrantsOverlap;

                //The target quadrant
                int targetQuadrant = 0;

                if (targetIndex < Board.QUAD_COUNT)
                {
                    targetQuadrant = Board.QUAD_ORDER[baseQuadrant][targetIndex];
                }
                else
                {
                    targetQuadrant = baseQuadrant + 4;
                }

                return new Square(targetQuadrant, squareOverlapQuadrant);
            }
            else
            {
                return new Square(QuadrantValue, SquareValue + value);
            }
        }

        /// <summary>
        /// Gets the index of the square in the board from a team's perspective
        /// </summary>
        /// <param name="team">Team's perspective</param>
        public int GetBoardIndex(sbyte team)
        {
            if (QuadrantValue >= Board.QUAD_COUNT)
            {
                //All home bases should return 48 + Square index
                int teamQuad = QuadrantValue - 4;

                int lastQuad = Board.QUAD_ORDER[teamQuad][Board.QUAD_COUNT - 1];
                int index = Board.GetQuadIndex((sbyte)teamQuad, lastQuad);
                return (12 * index) + 12 + (SquareValue + 1);        
            }

            return (12 * Board.GetQuadIndex(team, QuadrantValue)) + (SquareValue + 1);
        }

        /// <summary>
        /// Gets the distance to a square
        /// </summary>
        /// <param name="square">Square</param>
        /// <param name="team">Team to use as the path perspective</param>
        public int DistanceTo(Square square, sbyte team)
        {
            int index1 = square.GetBoardIndex(team);
            int index2 = GetBoardIndex(team);
            return index1 - index2; 
        }

        /// <summary>
        /// Gets the current length of the square the quadrant is in
        /// </summary>
        /// <returns></returns>
        public int GetQuadrantLength()
        {
            if (IsInPath())
            {
                return Board.QUAD_LENGTH;
            }
            else if (IsInBase())
            {
                return Board.BASE_LENGTH;
            }

            return Board.INVALID;
        }

        /// <summary>
        /// Gets whether the square is in a path
        /// </summary>
#if AGGR_INLINE 
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
#endif 
        public bool IsInPath()
        {
            return QuadrantValue >= 0 && QuadrantValue < Board.QUAD_COUNT;
        }

        /// <summary>
        /// Gets whether the square is in a base
        /// </summary>
#if AGGR_INLINE 
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
#endif 
        public bool IsInBase()
        {
            return QuadrantValue >= Board.QUAD_COUNT && QuadrantValue < Board.QUAD_COUNT + Board.BASE_SIZE;
        }

        /// <summary>
        /// Converts the square back into square notation
        /// </summary>
        public string GetNotation()
        {
            if (!Valid())
            {
                return null;
            }

            StringBuilder notation = new StringBuilder();
            if (IsInBase())
            {
                notation.Append('h');
            }

            switch (QuadrantValue % Board.BASE_SIZE)
            {
                case Board.BLUE: notation.Append('b'); break;
                case Board.GREEN: notation.Append('g'); break;
                case Board.RED: notation.Append('r'); break;
                case Board.YELLOW: notation.Append('y'); break;
            }

            int square = SquareValue + 1;
            notation.Append(square);

            return notation.ToString();
        }

        /// <summary>
        /// Evaluates whether this square is the same as another square
        /// </summary>
        /// <param name="square">Square to evaluate</param>
        public bool Equals(Square square)
        {
            if (square == null)
            {
                return false;
            }

            return QuadrantValue == square.QuadrantValue && SquareValue == square.SquareValue;
        }

        /// <summary>
        /// Converts the square back into square notation
        /// </summary>
        public override string ToString()
        {
            return Valid() ? GetNotation() : "INVALID";
        }

        /// <summary>
        /// Parses the value of a square in square notation
        /// </summary>
        /// <param name="str">Square notation value</param>
        /// <param name="index">Index of square value in the notation</param>
        private int ParseSquare(string str, int index)
        {
            int parsed = Board.INVALID;
            int.TryParse(str.Substring(index), out parsed);
            return parsed - 1;
        }

        /// <summary>
        /// Parses BGRY Quadrant from a string
        /// </summary>
        /// <param name="str">Notation string</param>
        /// <param name="index">Index of quadrant</param>
        private int ParseQuadrant(string str, int index)
        {
            switch (str[index])
            {
                case 'b': return Board.BLUE;
                case 'g': return Board.GREEN;
                case 'r': return Board.RED;
                case 'y': return Board.YELLOW;
                case 'h': return Board.QUAD_COUNT + ParseQuadrant(str, index + 1);
            }

            return Board.INVALID;
        }

        /// <summary>
        /// Parses square notation
        /// </summary>
        private void Parse(string squareNotation)
        {
            if (squareNotation[0] == 'h')
            {
                if (squareNotation.Length < 3)
                {
                    return;
                }

                QuadrantValue = ParseQuadrant(squareNotation, 0);
                SquareValue = ParseSquare(squareNotation, 2);
            }
            else
            {
                if (squareNotation.Length < 2)
                {
                    return;
                }

                QuadrantValue = ParseQuadrant(squareNotation, 0);
                SquareValue = ParseSquare(squareNotation, 1);
            }
        }

        /// <summary>
        /// Create a reference of a square
        /// </summary>
        /// <param name="quadrant">Square quadrant value</param>
        /// <param name="square">Square value indexed at 0</param>
        public Square(int quadrant, int square)
        {
            QuadrantValue = quadrant;
            SquareValue = square;
        }

        /// <summary>
        /// Create a reference of a square
        /// </summary>
        /// <param name="squareNotation">Square represented by square notation</param>
        public Square(string squareNotation)
        {
            this.Parse(squareNotation);
        }
    }
}
