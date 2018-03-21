using System;
using System.Collections.Generic;
using System.Threading;

namespace MarbleBoardGame
{
    public class MaxN : Algorithm
    {
        private Vector4 currentEvaluation;

        /// <summary>
        /// Starts the initial call of the MaxN algorithm
        /// </summary>
        private void Run()
        {
            PlySearch search = new PlySearch(board, Root, Player, board.NextPlayer(Player), Board.TEAM_COUNT * 2); //8 Ply ahead
            MaxNAlgorithm(search);
        }

        /// <summary>
        /// MaxN Algorithm
        /// </summary>
        /// <param name="position">Root position</param>
        /// <param name="player">Player to evaluate</param>
        /// <param name="currentPlyDepth">Depth to evaluate at</param>
        private Vector4 MaxNAlgorithm(PlySearch s)
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

            Vector4 best = MaxNAlgorithm(s.Next(children[0]));
            for (int i = 1; i < children.Count; i++)
            {
                Vector4 curr = MaxNAlgorithm(s.Next(children[i]));
                if (curr.GetMagnitude(s.RootPlayer) > best.GetMagnitude(s.RootPlayer))
                {
                    Best = children[i];

                    this.currentEvaluation = curr;                    
                    best = curr;
                }
            }
            return best;
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
        /// Creates a new instance of the MaxN Algorithm for a board
        /// </summary>
        /// <param name="board">Board</param>
        public MaxN(Board board) : base(board)
        {
            
        }
    }
}
