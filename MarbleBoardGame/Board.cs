using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MarbleBoardGame
{
    public class Board
    {
        private sbyte[][] quadrants;
        private static Random random = new Random();

        public static int Next(int max)
        {
            return random.Next(max);
        }

        public static readonly sbyte[][] EMPTY_BOARD = new sbyte[QUAD_COUNT + BASE_SIZE][];

        public static readonly sbyte[][] QUAD_ORDER = new sbyte[4][] 
        { 
            new sbyte[] { YELLOW, RED, GREEN, BLUE }, // Yellow
            new sbyte[] { RED, GREEN, BLUE, YELLOW }, // Red
            new sbyte[] { GREEN, BLUE, YELLOW, RED }, // Green
            new sbyte[] { BLUE, YELLOW, RED, GREEN }  // Blue
        };

        /// <summary>
        /// Initializes a 2-D array to the correct quadrant sizes
        /// </summary>
        /// <param name="quadrants">Quadrants array</param>
        public static void InitializeQuads(sbyte[][] quadrants)
        {
            for (int i = 0; i < QUAD_COUNT; i++)
            {
                quadrants[i] = new sbyte[QUAD_LENGTH];
                for (int j = 0; j < quadrants[i].Length; j++)
                {
                    quadrants[i][j] = -1;
                }
            }

            for (int i = QUAD_COUNT; i < QUAD_COUNT + BASE_SIZE; i++)
            {
                quadrants[i] = new sbyte[BASE_LENGTH];
                for (int j = 0; j < quadrants[i].Length; j++)
                {
                    quadrants[i][j] = -1;
                }
            }
        }

        /// <summary>
        /// Gets the index of the quadrant from a team's perspective
        /// </summary>
        /// <param name="team">Team</param>
        /// <param name="quadrant">Quadrant</param>
        public static int GetQuadIndex(sbyte team, int quadrant)
        {
            if (team < QUAD_COUNT)
            {
                for (int i = 0; i < TEAM_COUNT; i++)
                {
                    if (QUAD_ORDER[team][i] == quadrant)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        static Board()
        {
            InitializeQuads(EMPTY_BOARD);
        }

        public const sbyte BASE_LENGTH = 5;
        public const sbyte QUAD_LENGTH = 12;
        public const sbyte TOTAL_PATH_LENGTH = 52; //Indexed at 0

        public const sbyte TEAM_COUNT = 4;
        public const sbyte TEAM_SIZE = 5;
        public const sbyte QUAD_COUNT = TEAM_COUNT;
        public const sbyte BASE_SIZE = 4;

        public const sbyte BLUE = 3;
        public const sbyte BLUE_BASE = BLUE + 4;

        public const sbyte GREEN = 2;
        public const sbyte GREEN_BASE = GREEN + 4;

        public const sbyte RED = 1;
        public const sbyte RED_BASE = RED + 4;

        public const sbyte YELLOW = 0;
        public const sbyte YELLOW_BASE = YELLOW + 4;

        public const sbyte NO_PLAYER = -1;

        public const sbyte INVALID = -128;

        /// <summary>
        /// Checks if the square is not null and is valid
        /// </summary>
        /// <param name="square">Square to check</param>
        public bool IsValid(Square square)
        {
            if (square != null)
            {
                return square.Valid();
            }

            return false;
        }

        /// <summary>
        /// Validates a piece move
        /// </summary>
        /// <param name="pieceMove">Piece move</param>
        /// <param name="team">Team of the piece</param>
        public bool IsValid(Position position, Move move, PieceMove pieceMove, sbyte team)
        {
            if (pieceMove != null)
            {
                if (pieceMove.ValidSyntax)
                {
                    Square[] path = pieceMove.GetPath(team);
                    for (int i = 0; i < path.Length; i++)
                    {
                        if (position.Get(path[i]) == team)
                        {
                            //Check if marble is going to move
                            bool ignoreIntersection = false;
                            for (int m = 0; m < move.Pieces; m++)
                            {
                                if (!move[m].IsTakingOutMarble)
                                {
                                    if (move[m].From.Equals(path[i]) && move[m].To.GetBoardIndex(team) > pieceMove.To.GetBoardIndex(team))
                                    {
                                        ignoreIntersection = true;
                                        break;
                                    }
                                }
                            }

                            if (!ignoreIntersection)
                            {
                                return false;
                            }
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Validates a move
        /// </summary>
        /// <param name="move">Move</param>
        public bool IsValid(Position position, Move move)
        {
            for (int i = 0; i < move.Pieces; i++)
            {
                if (!IsValid(position, move, move[i], move.Team))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if the move is a possible move for a human (called by Engine)
        /// </summary>
        /// <param name="move">Move move</param>
        public bool IsPossibleMove(DiceRoll roll, Move move, out Move orderCorrect)
        {
            MoveCollection collection = GetMoves(roll, move.Team);
            for (int j = 0; j < collection.Count; j++)
            {
                if (collection[j].Equals(move))
                {
                    orderCorrect = collection[j];
                    return true;
                }
            }

            orderCorrect = null;
            return false;
        }

        /// <summary>
        /// Checks if the piece move is part of a valid move
        /// </summary>
        /// <param name="roll">Current roll</param>
        /// <param name="move">Move to check</param>
        /// <param name="team">Team</param>
        public bool IsPossibleTreeMove(DiceRoll roll, PieceMove move, sbyte team)
        {
            MoveCollection collection = GetMoves(roll, team);
            for (int j = 0; j < collection.Count; j++)
            {
                PieceMove[] moves = collection[j].PieceMoves;
                for (int i = 0; i < moves.Length; i++)
                {
                    if (moves[i].Equals(move))
                    {
                        return true;
                    }
                }
            }

            return false;
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
        /// Gets the marble on a square
        /// </summary>
        /// <param name="position">The position to change</param>
        /// <param name="squareNotation">Square notation of square</param>
        public int Get(Position position, string squareNotation)
        {
            return position.Get(new Square(squareNotation));
        }

        /// <summary>
        /// Gets the marble on a square
        /// </summary>
        /// <param name="position">The position to change</param>
        /// <param name="square">Square to get the marble on</param>
        public int Get(Position position, Square square)
        {
            return position.Get(square);
        }

        /// <summary>
        /// Checks if the game is over
        /// </summary>
        public bool IsGameOver()
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
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the current position of the board
        /// </summary>
        public Position GetPosition()
        {
            return new Position(this.quadrants);
        }

        /// <summary>
        /// Gets a random dice roll
        /// </summary>
        public DiceRoll GetRandomRoll()
        {
            return new DiceRoll(random.Next(1, 7), random.Next(1, 7));
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
        /// <param name="position">The position to change</param>
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
        /// Sets the square on the board with a marble
        /// </summary>
        /// <param name="position">The position to change</param>
        /// <param name="square">Square notation of square to set marble on</param>
        /// <param name="marble">Marble value</param>
        public void Set(Position position, string squareNotation, sbyte marble)
        {
            position.Set(new Square(squareNotation), marble);
        }

        /// <summary>
        /// Sets the square on the board with a marble
        /// </summary>
        /// <param name="position">The position to change</param>
        /// <param name="square">Square to set marble on</param>
        /// <param name="marble">Marble value</param>
        public void Set(Position position, Square square, sbyte marble)
        {
            position.Set(square, marble);
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
        /// Changes the position of the board to the position
        /// </summary>
        /// <param name="position">Position to change the board to</param>
        public void SetPosition(Position position)
        {
            this.quadrants = position.GetData(false);
        }

        /// <summary>
        /// Gets all the marble's squares for a team
        /// </summary>
        /// <param name="team">Team</param>
        public Square[] GetMarbles(sbyte team)
        {
            int marbleIndex = 0;
            Square[] marbles = new Square[TEAM_SIZE];
            for (int i = 0; i < QUAD_COUNT + BASE_SIZE; i++)
            {
                sbyte length = i < QUAD_COUNT ? QUAD_LENGTH : BASE_LENGTH;
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
        /// Gets all the marble's squares for a team
        /// </summary>
        /// <param name="team">Team</param>
        public Square[] GetMarbles(Position position, sbyte team)
        {
            int marbleIndex = 0;
            Square[] marbles = new Square[TEAM_SIZE];
            for (int i = 0; i < QUAD_COUNT + BASE_SIZE; i++)
            {
                sbyte length = i < QUAD_COUNT ? QUAD_LENGTH : BASE_LENGTH;
                for (int j = 0; j < length; j++)
                {
                    int marbleType = position.Get(new Square(i, j));
                    if (marbleType == team)
                    {
                        marbles[marbleIndex] = new Square(i, j);
                        marbleIndex++;

                        if (marbleIndex > TEAM_SIZE)
                        {
                            return marbles;
                        }
                    }
                }
            }

            return marbles;
        }

        /// <summary>
        /// Counts the amount of active marbles on the specified squares
        /// </summary>
        /// <param name="squares">List of squares</param>
        public int GetActivePiecesCount(Square[] squares)
        {
            int active = 0;
            for (int i = 0; i < squares.Length; i++)
            {
                if (squares[i] != null)
                {
                    active++;
                }
            }

            return active;
        }

        /// <summary>
        /// Gets all the possible moves for a team given a dice roll
        /// </summary>
        /// <param name="roll">Dice roll</param>
        /// <param name="team">Marble team</param>
        public MoveCollection GetMoves(DiceRoll roll, sbyte team)
        {
            return GetMoves(GetPosition(), roll, team);
        }

        /// <summary>
        /// Gets all the possible moves for a team given a dice roll
        /// </summary>
        /// <param name="roll">Dice roll</param>
        /// <param name="team">Marble team</param>
        public MoveCollection GetMoves(Position position, DiceRoll roll, sbyte team)
        {
            if (team < 0 || team > TEAM_COUNT)
            {
                return null;
            }

            sbyte winner = -1;
            if (position.IsWon(out winner))
            {
                return null;
            }

            MoveCollection moves = new MoveCollection(this, position);
            Square[] marbleSquares = GetMarbles(position, team);

            bool hasMarbleOnSpawn = position.Get(new Square(team, 0)) == team;
            int activePieces = GetActivePiecesCount(marbleSquares);

            //Loop until a marble in our base is found
            for (int i = 0; i < marbleSquares.Length; i++)
            {
                //If marble is at start square
                if (marbleSquares[i] == null)
                {
                    //If no team marble is on spawn square
                    if (!hasMarbleOnSpawn)
                    {
                        int[] takeOut = roll.CanTakeOutWith();
                        if (takeOut.Length > 0)
                        {
                            for (int j = 0; j < takeOut.Length; j++)
                            {
                                Square target = new Square(team, takeOut[j]);
                                if (position.Get(target) != team)
                                {
                                    moves.Add(new Move(new PieceMove(null, target), team));
                                }
                            }
                        }
                    }

                    //Add a special case for taking out a piece and using the other die value on another marble
                    if (activePieces > 0)
                    {
                        int[] combinations = roll.GetDoublesTakeOutCombinations();
                        PieceMove takeOutCombo = new PieceMove(null, new Square(team, 0));
                        for (int p = 0; p < activePieces; p++)
                        {
                            for (int pc = 0; pc < combinations.Length; pc++)
                            {
                                PieceMove correspondant = new PieceMove(marbleSquares[p], marbleSquares[p].Add(combinations[pc], team));

                                if (hasMarbleOnSpawn && !correspondant.From.Equals(takeOutCombo.To))
                                {
                                    continue;
                                }

                                moves.Add(new Move(new PieceMove[] { correspondant, takeOutCombo }, team));
                            }
                        }
                    }

                    break;
                }
            }

            List<int[]> pieceCombinations = GetPieceCombinations(activePieces);
            for (int c = 0; c < pieceCombinations.Count; c++)
            {
                int pieces = pieceCombinations[c].Length;
                int[][] pieceValues = roll.GetValues(pieces);
                for (int k = 0; k < pieceValues.Length; k++)
                {
                    PieceMove[] pieceMoves = new PieceMove[pieces];

                    for (int j = 0; j < pieces; j++)
                    {
                        Square marblePiece = marbleSquares[pieceCombinations[c][j]];
                        pieceMoves[j] = new PieceMove(marblePiece, marblePiece.Add(pieceValues[k][j], team));

                        if (j > 0)
                        {
                            if (pieceMoves[j].From.Equals(pieceMoves[j - 1].To))
                            {
                                //Swap the move order
                                PieceMove current = pieceMoves[j];
                                pieceMoves[j] = pieceMoves[j - 1];
                                pieceMoves[j - 1] = current;
                            }
                        }
                    }

                    Move move = new Move(pieceMoves, team);
                    moves.Add(move);
                }
            }

            return moves;
        }

        /// <summary>
        /// Performs a piece move on the board
        /// </summary>
        /// <param name="pieceMove">Piece move</param>
        /// <param name="team">Team of the piece move</param>
        public void PerformMove(PieceMove pieceMove, sbyte team)
        {
            if (pieceMove.IsTakingOutMarble)
            {
                //Since the move is really two moves, "taking out", then moving
                //remove any marble on the home square
                Square homeSquare = new Square(team, 0);
                Set(homeSquare, -1);

                Set(pieceMove.To, team);
            }
            else
            {
                Set(pieceMove.From, -1);
                Set(pieceMove.To, team);
            }
        }
        
        /// <summary>
        /// Performs the move to the board
        /// </summary>
        /// <param name="move">Move</param>
        public void PerformMove(Move move)
        {
            for (int i = 0; i < move.Pieces; i++)
            {
                PerformMove(move[i], move.Team);
            }
        }

        /// <summary>
        /// Performs a piece move on the board
        /// </summary>
        /// <param name="position">The position to change</param>
        /// <param name="pieceMove">Piece move</param>
        /// <param name="team">Team of the piece move</param>
        public void PerformMove(Position position, PieceMove pieceMove, sbyte team)
        {
            if (pieceMove.IsTakingOutMarble)
            {
                position.Set(pieceMove.To, team);
            }
            else
            {
                position.Set(pieceMove.From, -1);
                position.Set(pieceMove.To, team);
            }
        }

        /// <summary>
        /// Performs the move to the board
        /// </summary>
        /// <param name="position">The position to change</param>
        /// <param name="move">Move</param>
        public void PerformMove(Position position, Move move)
        {
            for (int i = 0; i < move.Pieces; i++)
            {
                PerformMove(position, move[i], move.Team);
            }
        }

        /// <summary>
        /// Returns the next player after the current player
        /// </summary>
        /// <param name="player">Current player</param>
        public sbyte NextPlayer(sbyte player)
        {
            player++;
            return (sbyte)(player % TEAM_COUNT);
        }

        /// <summary>
        /// Gets a list of rolls that miminimizes node clones but without losing moves
        /// </summary>
        /// <param name="position">The position</param>
        /// <param name="team">The team</param>
        public DiceRoll[] GetRollsQueue(Position position, sbyte team)
        {
            Square[] marbles = position.GetMarbles(team);

            for (int i = 0; i < marbles.Length; i++)
            {
                if (marbles[i] != null)
                {
                    return DiceRoll.ALL_NO_DUPL;
                }
            }

            return DiceRoll.ALL_START_NO_REPT;
        }

        /// <summary>
        /// Gets all the possible nodes for a given position
        /// </summary>
        /// <param name="position">Positional node</param>
        public List<PositionNode>GetNodes(Position position, sbyte team)
        {
            List<PositionNode> nodes = new List<PositionNode>();

            sbyte winner = -1;
            if (position.IsWon(out winner))
            {
                return nodes;
            }

            DiceRoll[] rolls = GetRollsQueue(position, team);
            for (int r = 0; r < rolls.Length; r++)
            {
                MoveCollection collection = GetMoves(position, rolls[r], team);
                if (collection != null)
                {
                    PositionNode[] pnodes = new PositionNode[collection.Count];
                    for (int i = 0; i < collection.Count; i++)
                    {
                        Position node = position.Clone();
                        PerformMove(node, collection[i]);

                        pnodes[i] = new PositionNode(node, collection[i], rolls[r]);
                    }

                    nodes.AddRange(pnodes);
                }
            }

            return nodes;
        }

        /// <summary>
        /// Thinks about the current position given the dice roll and calculates the best move
        /// </summary>
        /// <param name="algorithm">Evaluation algorithm to use</param>
        /// <param name="roll">Roll</param>
        /// <param name="team">Team to think for</param>
        /// <param name="depth">Depth to think</param>
        public Move ThinkBest(Algorithm algorithm, DiceRoll roll, sbyte team, int ms)
        {
            MoveCollection collection = GetMoves(roll, team);
            if (collection.Count == 0)
            {
                return null;
            }
            else if (collection.Count == 1)
            {
                return collection[0];
            }
            else if (ms < 0)
            {
                return collection[random.Next(collection.Count)];
            }

            PositionNode[] nodes = new PositionNode[collection.Count];
            for (int i = 0; i < collection.Count; i++)
            {
                Position node = new Position(this.quadrants);
                PerformMove(node, collection[i]);

                nodes[i] = new PositionNode(node, collection[i], roll);
            }

            algorithm.Player = team;
            Vector4[] evaluations = new Vector4[nodes.Length];
            for (int j = 0; j < nodes.Length; j++)
            {
                PositionNode currentRootMoveNode = nodes[j];
                algorithm.Root = currentRootMoveNode;

                Vector4 moveEval = algorithm.Go(ms / collection.Count);
                evaluations[j] = moveEval;
            }

            int highIndex = 0;
            double high = evaluations[0].GetMagnitude(team);
            for (int i = 1; i < evaluations.Length; i++)
            {
                double curMag = evaluations[i].GetMagnitude(team);
                if (curMag > high)
                {
                    high = curMag;
                    highIndex = i;
                }
            }

            return collection[highIndex];
        }

        /// <summary>
        /// Thinks about the current position given the dice roll and sorts all the moves by evaluation
        /// </summary>
        /// <param name="algorithm">Evaluation algorithm to use</param>
        /// <param name="roll">Roll</param>
        /// <param name="team">Team to think for</param>
        /// <param name="depth">Depth to think</param>
        public MoveCollection ThinkAll(Algorithm algorithm, DiceRoll roll, sbyte team, int ms)
        {
            MoveCollection collection = GetMoves(roll, team);
            if (collection.Count == 0)
            {
                return null;
            }
            else if (collection.Count == 1)
            {
                return collection;
            }
            if (ms < 0)
            {
                throw new ArgumentException();
            }

            PositionNode[] nodes = new PositionNode[collection.Count];
            for (int i = 0; i < collection.Count; i++)
            {
                Position node = new Position(this.quadrants);
                PerformMove(node, collection[i]);

                nodes[i] = new PositionNode(node, collection[i], roll);
            }

            algorithm.Player = team;
            Vector4[] evaluations = new Vector4[nodes.Length];
            for (int j = 0; j < nodes.Length; j++)
            {
                PositionNode currentRootMoveNode = nodes[j];
                algorithm.Root = currentRootMoveNode;

                Vector4 moveEval = algorithm.Go(ms / collection.Count);
                evaluations[j] = moveEval;
            }

            SortDecreasing(evaluations, collection, team);
            return collection;
        }

        public MoveCollection ThinkAll(Algorithm algorithm, DiceRoll roll, sbyte team, int ms, out Vector4[] scores)
        {
            MoveCollection collection = GetMoves(roll, team);
            if (collection.Count == 0)
            {
                scores = null;
                return null;
            }
            else if (collection.Count == 1)
            {
                scores = new Vector4[1] { algorithm.Eval(this.GetPosition(), roll.IsDoubles() ? team : Board.NO_PLAYER) };
                return collection;
            }
            if (ms < 0)
            {
                throw new ArgumentException();
            }

            PositionNode[] nodes = new PositionNode[collection.Count];
            for (int i = 0; i < collection.Count; i++)
            {
                Position node = new Position(this.quadrants);
                PerformMove(node, collection[i]);

                nodes[i] = new PositionNode(node, collection[i], roll);
            }

            algorithm.Player = team;
            Vector4[] evaluations = new Vector4[nodes.Length];
            for (int j = 0; j < nodes.Length; j++)
            {
                PositionNode currentRootMoveNode = nodes[j];
                algorithm.Root = currentRootMoveNode;

                Vector4 moveEval = algorithm.Go(ms / collection.Count);
                evaluations[j] = moveEval;
            }

            SortDecreasing(evaluations, collection, team);
            scores = evaluations;
            return collection;
        }

        /// <summary>
        /// Stable insertion sort
        /// </summary>
        /// <param name="input">Input of move evaluations</param>
        /// <param name="collection">Move values</param>
        /// <param name="team">Team to sort by</param>
        private void SortDecreasing(Vector4[] input, MoveCollection collection, sbyte team)
        {
            for (int i = 1; i < input.Length; i++)
            {
                double key = input[i].GetMagnitude(team);
                Vector4 keyValue = input[i];
                Move moveValue = collection[i];

                int j = i - 1;
                while (j >= 0 && input[j].GetMagnitude(team) < key)
                {
                    input[j + 1] = input[j];
                    collection[j + 1] = collection[j];
                    j--;
                }

                input[j + 1] = keyValue;
                collection[j + 1] = moveValue;
            }
        }

        /// <summary>
        /// Sorts an array in decreasing order
        /// </summary>
        /// <param name="input">Double array</param>
        public void SortDecreasing(double[] input)
        {
            for (int i = 1; i < input.Length; i++)
            {
                double key = input[i];

                int j = i - 1;
                while (j >= 0 && input[j] < key)
                {
                    input[j + 1] = input[j];
                    j--;
                }

                input[j + 1] = key;
            }
        }

        /// <summary>
        /// Gets a list of all possible piece combinations uses for a pair of die numbers
        /// </summary>
        /// <param name="pieceCount">Number of active pieces</param>
        private List<int[]> GetPieceCombinations(int pieceCount)
        {
            List<int[]> combinations = new List<int[]>();
            for (int i = 0; i < pieceCount; i++)
            {
                combinations.Add(new int[] { i });
                for (int j = 0; j < pieceCount; j++)
                {
                    if (i != j)
                    {
                        combinations.Add(new int[] { i, j });
                    }
                }
            }
            return combinations;
        }

        /// <summary>
        /// Creates a new empty board
        /// </summary>
        public Board()
        {
            quadrants = new sbyte[QUAD_COUNT + BASE_SIZE][];
            Board.InitializeQuads(quadrants);

            random = new Random();
        }
    }
}