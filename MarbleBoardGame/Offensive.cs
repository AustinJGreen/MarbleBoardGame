using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MarbleBoardGame
{
    public class Offensive : Algorithm
    {
        private Vector4 currentEvaluation;

        /// <summary>
        /// Target player to minimize
        /// </summary>
        public sbyte TargetPlayer { get; set; }

        /// <summary>
        /// Starts the initial call of the MaxN algorithm
        /// </summary>
        private void Run()
        {
            PlySearch search = new PlySearch(board, Root, Player, board.NextPlayer(Player), Board.TEAM_COUNT);
            OffensiveAlgorithm(search);
        }

        /// <summary>
        /// Offensive Algorithm
        /// </summary>
        /// <param name="position">Root position</param>
        /// <param name="player">Player to evaluate</param>
        /// <param name="currentPlyDepth">Depth to evaluate at</param>
        private Vector4 OffensiveAlgorithm(PlySearch s)
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

            Vector4 worst = OffensiveAlgorithm(s.Next(children[0]));
            for (int i = 1; i < children.Count; i++)
            {
                Vector4 curr = OffensiveAlgorithm(s.Next(children[i]));
                if (curr.GetMagnitude(TargetPlayer) < worst.GetMagnitude(TargetPlayer))
                {
                    Best = children[i];

                    this.currentEvaluation = curr;                    
                    worst = curr;
                }
            }
            return worst;
        }

        /// <summary>
        /// Starts calculating for a certain time
        /// </summary>
        /// <param name="ms">Milliseconds to calculate</param>
        public override Vector4 Go(int ms)
        {
            currentEvaluation = eval.Evaluate(Root.Value, Root.RolledDoubles ? Player : Board.NO_PLAYER);

            //If no target player was set, target the best player
            if (TargetPlayer == -1)
            {
                if (currentEvaluation.GetDirection() != Player)
                {
                    TargetPlayer = (sbyte)currentEvaluation.GetDirection();
                }
                else
                {
                    List<int> players = new List<int>(Enumerable.Range(0, Board.TEAM_COUNT));
                    players.Remove(Player);

                    int index = Board.Next(players.Count);
                    TargetPlayer = (sbyte)players[index];
                }
            }

            Start(Run, ms);

            if (ms != 0)
            {
                NodesPerSecond = (1000 * Nodes) / ms;
            }
            else
            {
                NodesPerSecond = (1000 * Nodes);
            }

            //Reset target player
            TargetPlayer = -1;
            return currentEvaluation;
        }

        public override Vector4 Go()
        {
            DateTime startTime = DateTime.Now;
            currentEvaluation = eval.Evaluate(Root.Value, Root.RolledDoubles ? Player : Board.NO_PLAYER);

            //If no target player was set, target the best player
            if (TargetPlayer == -1)
            {
                if (currentEvaluation.GetDirection() != Player)
                {
                    TargetPlayer = (sbyte)currentEvaluation.GetDirection();
                }
                else
                {
                    List<int> players = new List<int>(Enumerable.Range(0, Board.TEAM_COUNT));
                    players.Remove(Player);

                    int index = Board.Next(players.Count);
                    TargetPlayer = (sbyte)players[index];
                }
            }

            Run();

            TimeSpan elapsed = DateTime.Now.Subtract(startTime);
            NodesPerSecond = (elapsed.TotalSeconds * Nodes);
            return currentEvaluation;
        }

        /// <summary>
        /// Creates a new instance of the MaxN Algorithm for a board
        /// </summary>
        /// <param name="board">Board</param>
        public Offensive(Board board) : base(board)
        {
        }
    }
}
