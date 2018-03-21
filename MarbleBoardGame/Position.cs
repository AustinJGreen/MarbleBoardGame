using System;
using System.Text;

namespace MarbleBoardGame
{
    public struct Position
    {
        private sbyte[][] quadrants;
        private static readonly int[] hashValues = { 3, 7, 11, 17 };

        /// <summary>
        /// Creates a deep-copy of an 2D array
        /// </summary>
        /// <param name="array">Array to copy</param>
        private sbyte[][] Clone(sbyte[][] array)
        {
            sbyte[][] copy = new sbyte[array.GetLength(0)][];
            for (int i = 0; i < array.Length; i++)
            {
                copy[i] = new sbyte[array[i].Length];
                for (int j = 0; j < array[i].Length; j++)
                {
                    copy[i][j] = array[i][j];
                }
            }

            return copy;
        }

        /// <summary>
        /// Creates a deep-copy of an 2D array using buffers
        /// </summary>
        /// <param name="array">Array to copy</param>
        private sbyte[][] CloneFast(sbyte[][] array)
        {
            int length = array.GetLength(0);
            sbyte[][] copy = new sbyte[length][];
            for (int i = 0; i < array.Length; i++)
            {
                copy[i] = new sbyte[array[i].Length];

                int len = Buffer.ByteLength(array[i]);
                Buffer.BlockCopy(array[i], 0, copy[i], 0, len);
            }

            return copy;
        }

        /// <summary>
        /// Gets the position data
        /// </summary>
        /// <param name="pointer">Should the data be a pointer to the position</param>
        public sbyte[][] GetData(bool pointer)
        {
            if (pointer)
            {
                return quadrants;
            }
            else
            {
                return CloneFast(quadrants);
            }
        }

        /// <summary>
        /// Clones the position to a new place in memory
        /// </summary>
        public Position Clone()
        {
            return new Position(GetData(false));
        }

        /// <summary>
        /// Gets the marble on a square
        /// </summary>
        /// <param name="squareNotation">Square notation of square</param>
        public int Get(string squareNotation)
        {
            return Get(new Square(squareNotation));
        }

        /// <summary>
        /// Gets the marble on a square
        /// </summary>
        /// <param name="square">Square to get the marble on</param>
        public int Get(Square square)
        {
            //if (!square.Valid())
            //{
            //    return -1;
            //}

            return quadrants[square.QuadrantValue][square.SquareValue];
        }

        /// <summary>
        /// Sets the square on the board with a marble
        /// </summary>
        /// <param name="square">Square notation of square to set marble on</param>
        /// <param name="marble">Marble value</param>
        public void Set(string squareNotation, sbyte marble)
        {
            Set(new Square(squareNotation), marble);
        }

        /// <summary>
        /// Sets the square on the board with a marble
        /// </summary>
        /// <param name="square">Square to set marble on</param>
        /// <param name="marble">Marble value</param>
        public void Set(Square square, sbyte marble)
        {
            //if (!square.Valid())
            //{
            //    return;
            //}

            quadrants[square.QuadrantValue][square.SquareValue] = marble;
        }

        /// <summary>
        /// Checks if the position is won and outputs the winner
        /// </summary>
        public bool IsWon(out sbyte winner)
        {
            for (sbyte b = Board.QUAD_COUNT; b < Board.QUAD_COUNT + Board.BASE_SIZE; b++)
            {
                bool missingMarble = false;
                sbyte[] tBase = quadrants[b];
                for (int m = 0; m < tBase.Length; m++)
                {
                    if (tBase[m] == -1)
                    {
                        missingMarble = true;
                        break;
                    }
                }

                if (!missingMarble)
                {
                    winner = (sbyte)(b - 4);
                    return true;
                }
            }

            winner = -1;
            return false;
        }

        /// <summary>
        /// Gets all the marble's squares for a team
        /// </summary>
        /// <param name="team">Team</param>
        public Square[] GetMarbles(sbyte team)
        {
            int marbleIndex = 0;
            Square[] marbles = new Square[Board.TEAM_SIZE];
            for (int i = 0; i < Board.QUAD_COUNT + Board.BASE_SIZE; i++)
            {
                sbyte length = i < Board.QUAD_COUNT ? Board.QUAD_LENGTH : Board.BASE_LENGTH;
                for (int j = 0; j < length; j++)
                {
                    int marbleType = quadrants[i][j];
                    if (marbleType == team)
                    {
                        marbles[marbleIndex] = new Square(i, j);
                        marbleIndex++;
                    }
                }
            }

            return marbles;
        }

        /// <summary>
        /// Removes the furthest marble of a team from the board due to rolling doubles three times
        /// </summary>
        /// <param name="team">Team to remove</param>
        public void RemoveFrontMarble(sbyte team)
        {
            Square[] marbles = GetMarbles(team);
            for (int j = marbles.Length - 1; j >= 0; j--)
            {
                if (marbles[j] != null)
                {
                    if (marbles[j].IsInPath())
                    {
                        Set(marbles[j], -1);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Returns readable marble value for a square
        /// </summary>
        /// <param name="notation">Square to get marble for</param>
        private string S(string notation)
        {
            int m = Get(notation);
            switch (m)
            {
                case Board.YELLOW:
                    return "Y";
                case Board.GREEN:
                    return "G";
                case Board.BLUE:
                    return "B";
                case Board.RED:
                    return "R";
            }

            return "+";
        }

        /// <summary>
        /// Gets a board representation from greens perspective
        /// </summary>
        public string GetVisual()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format("-----{0}{1}{2}-----", S("b11"), S("b12"), S("y1")));
            builder.AppendLine(string.Format("-----{0}{1}{2}-----", S("b10"), S("hy1"), S("y2")));
            builder.AppendLine(string.Format("-----{0}{1}{2}-----", S("b9"), S("hy2"), S("y3")));
            builder.AppendLine(string.Format("-----{0}{1}{2}-----", S("b8"), S("hy3"), S("y4")));
            builder.AppendLine(string.Format("-----{0}{1}{2}-----", S("b7"), S("hy4"), S("y5")));
            builder.AppendLine(string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}", S("b1"), S("b2"), S("b3"), S("b4"), S("b5"), S("b6"), S("hy5"), S("y6"), S("y7"), S("y8"), S("y9"), S("y10"), S("y11")));
            builder.AppendLine(string.Format("{0}{1}{2}{3}{4}{5}-{6}{7}{8}{9}{10}{11}", S("g12"), S("hb1"), S("hb2"), S("hb3"), S("hb4"), S("hb5"), S("hr5"), S("hr4"), S("hr3"), S("hr2"), S("hr1"), S("y12")));
            builder.AppendLine(string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}", S("g11"), S("g10"), S("g9"), S("g8"), S("g7"), S("g6"), S("hg5"), S("r6"), S("r5"), S("r4"), S("r3"), S("r2"), S("r1")));
            builder.AppendLine(string.Format("-----{0}{1}{2}-----", S("g5"), S("hg4"), S("r7")));
            builder.AppendLine(string.Format("-----{0}{1}{2}-----", S("g4"), S("hg3"), S("r8")));
            builder.AppendLine(string.Format("-----{0}{1}{2}-----", S("g3"), S("hg2"), S("r9")));
            builder.AppendLine(string.Format("-----{0}{1}{2}-----", S("g2"), S("hg1"), S("r10")));
            builder.AppendLine(string.Format("-----{0}{1}{2}-----", S("g1"), S("r12"), S("r11")));
            return builder.ToString();
        }

        /// <summary>
        /// Gets the hash code of the position's quadrants
        /// </summary>
        public override int GetHashCode()
        {
            int hash = 19;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < quadrants[i].Length; j++)
                {
                    if (quadrants[i][j] != -1)
                    {
                        hash *= j + hashValues[quadrants[i][j]];
                    }
                }
            }
            return hash;
        }

        /// <summary>
        /// Converts the position to the hash code of the position
        /// </summary>
        public override string ToString()
        {
            return GetHashCode().ToString();
        }

        /// <summary>
        /// Creates a new position
        /// </summary>
        /// <param name="quadrants">Quadrants data</param>
        public Position(sbyte[][] quadrants) : this()
        {
            this.quadrants = this.CloneFast(quadrants);
        }
    }
}
