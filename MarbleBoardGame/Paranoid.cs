using System;
using System.Collections.Generic;
using System.Threading;

namespace MarbleBoardGame
{
    public class Paranoid : Algorithm
    {
        private Vector4 currentEvaluation;

        /// <summary>
        /// Starts the initial call of the Paranoid algorithm
        /// </summary>
        private void Run()
        {
            PlySearch start = new PlySearch(board, Root, Player, board.NextPlayer(Player), Board.TEAM_COUNT * 2);
            ParanoidAlgorithm(start, double.MinValue, double.MaxValue);
        }

        /// <summary>
        /// Paranoid Algorithm
        /// </summary>
        /// <param name="position">Root position</param>
        /// <param name="player">Player to evaluate</param>
        /// <param name="currentPlyDepth">Depth to evaluate at</param>
        public Vector4 ParanoidAlgorithm(PlySearch s, double alpha, double beta)
        {
            if (ShouldStop())
            {
                return Vector4.Zero;
            }

            List<PositionNode> children = board.GetNodes(s.Value, s.CurrentPlayer);
            if (children.Count == 0 || s.CurrentPlyDepth == 0)
            {
                Vector4 baseEval = eval.Evaluate(s.Value, s.RolledDoubles ? s.CurrentPlayer : Board.NO_PLAYER);
                return Vector4.Multiply(baseEval, s.EvalWeight);
            }

            Nodes += children.Count;

            if (s.Maximizing)
            {
                Vector4 bestValue = ParanoidAlgorithm(s.NextMiniMax(children[0]), alpha, beta);
                for (int i = 1; i < children.Count; i++)
                {
                    Vector4 curr = ParanoidAlgorithm(s.NextMiniMax(children[i]), alpha, beta);
                    alpha = Math.Max(alpha, curr.GetMagnitude(s.RootPlayer));

                    if (curr.GetMagnitude(s.RootPlayer) > bestValue.GetMagnitude(s.RootPlayer))
                    {
                        bestValue = curr;
                    }

                    if (beta < alpha)
                    {
                        break;
                    }
                }

                return bestValue;
            }
            else
            {
                Vector4 bestValue = ParanoidAlgorithm(s.NextMiniMax(children[0]), alpha, beta);
                for (int i = 1; i < children.Count; i++)
                {
                    Vector4 curr = ParanoidAlgorithm(s.NextMiniMax(children[i]), alpha, beta);

                    double min = curr.GetLength(s.RootPlayer);
                    double max = curr.GetMagnitude(s.RootPlayer);

                    double dif = max - min;
                    beta = Math.Min(beta, dif);

                    if (dif < bestValue.GetMagnitude(s.RootPlayer))
                    {
                        bestValue = curr;
                    }

                    if (beta < alpha)
                    {
                        break;
                    }
                }

                return bestValue;
            }
        }

        public Vector4 ParanoidIterative()
        {
            Vector4 bestValue = new Vector4();

            int depth = 0;

            double alpha = double.MinValue;
            double beta = double.MaxValue;

            NodeStack<Search> tree = new NodeStack<Search>(100);
            tree.Enqueue(new Search(board, Root, Player, board.NextPlayer(Player)), 0);

            while (tree.Count > 0)
            {
                Search n = tree.Dequeue(ref depth);

                this.Depth = depth;

                Console.Clear();
                Console.WriteLine("Depth = {0}", Depth);
                Console.WriteLine("Eval = {0}", bestValue.GetMagnitude(Player));
                Console.WriteLine("Nodes = {0}", tree.Count);

                //Thread.Sleep(50);

                Vector4 val = eval.Evaluate(n.Value, n.RolledDoubles ? n.CurrentPlayer : Board.NO_PLAYER);
                double min = val.GetLength(n.RootPlayer);
                double max = val.GetMagnitude(n.RootPlayer);

                //Evaluate new node
                if (n.CurrentPlayer == n.RootPlayer) //If player is us, maximize
                {
                    alpha = Math.Max(alpha, max);

                    if (max > bestValue.GetMagnitude(n.RootPlayer))
                    {
                        bestValue = val;
                    }

                    if (beta < alpha)
                    {
                        //Dont expand node.
                        //continue;
                    }
                }
                else //Minimize
                {
                    double dif = max - min;
                    beta = Math.Min(beta, dif);

                    if (dif < bestValue.GetMagnitude(n.RootPlayer))
                    {
                        bestValue = val;
                    }

                    if (beta < alpha)
                    {
                        //continue;
                    }
                }

                //Start expanding node to the next branch
                List<PositionNode> children = board.GetNodes(n.Value, n.CurrentPlayer);
                for (int c = 0; c < children.Count; c++)
                {
                    tree.Enqueue(n.Next(children[c]), depth + 1);
                }
            }

            return bestValue;
        }

        /// <summary>
        /// Starts calculating for a certain time
        /// </summary>
        /// <param name="ms">Milliseconds to calculate</param>
        public override Vector4 Go(int ms)
        {
            currentEvaluation = eval.Evaluate(Root.Value, Root.RolledDoubles ? Player : Board.NO_PLAYER);

            Start(Run, ms);

            if (ms != 0)
            {
                NodesPerSecond = (1000 * Nodes) / ms;
            }
            else
            {
                NodesPerSecond = (1000 * Nodes);
            }

            return currentEvaluation;
        }

        public override Vector4 Go()
        {
            currentEvaluation = eval.Evaluate(Root.Value, Root.RolledDoubles ? Player : Board.NO_PLAYER);

            DateTime startTime = DateTime.Now;
            Run();

            TimeSpan elapsed = DateTime.Now.Subtract(startTime);
            NodesPerSecond = (elapsed.TotalSeconds * Nodes);
            return currentEvaluation;
        }

        /// <summary>
        /// Creates a new instance of the Paranoid Algorithm for a board
        /// </summary>
        /// <param name="board">Board</param>
        public Paranoid(Board board) : base(board)
        {
        }
    }
}
