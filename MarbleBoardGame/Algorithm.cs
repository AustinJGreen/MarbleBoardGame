using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace MarbleBoardGame
{
    public abstract class Algorithm
    {
        protected int targetMs;
        protected Stopwatch timer;
        protected Board board;
        protected Evaluator eval;

        /// <summary>
        /// Root node position
        /// </summary>
        public PositionNode Root { get; set; }

        /// <summary>
        /// Peak of evaluation
        /// </summary>
        public PositionNode Best { get; set; }

        /// <summary>
        /// Current player to maximize
        /// </summary>
        public sbyte Player { get; set; }

        /// <summary>
        /// Gets the amount of nodes processed
        /// </summary>
        public int Nodes { get; set; }

        /// <summary>
        /// Gets the rate of nodes per second
        /// </summary>
        public double NodesPerSecond { get; set; }

        /// <summary>
        /// Gets the depth reached by the search
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Searchs for a specified amount of time
        /// </summary>
        /// <param name="ms">Time to search in milliseconds</param>
        /// <returns></returns>
        public abstract Vector4 Go(int ms);

        /// <summary>
        /// Searchs without a time constraint
        /// </summary>
        public abstract Vector4 Go();

        /// <summary>
        /// Evaluates the position
        /// </summary>
        public Vector4 Eval(Position pos, sbyte rolledDoubles)
        {
            return eval.Evaluate(pos, rolledDoubles);
        }

        /// <summary>
        /// Checks if the algorithm should stop
        /// </summary>
        protected bool ShouldStop()
        {
            if (timer.ElapsedMilliseconds >= targetMs)
            {
                if (timer.IsRunning)
                {
                    timer.Stop();
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Starts a method, synchronized with a timer
        /// </summary>
        /// <param name="init">init Method</param>
        /// <param name="targetMs">The target time in milliseconds</param>
        protected void Start(Action init, int targetMs)
        {
            this.targetMs = targetMs;
            timer.Restart();
            init.Invoke();
        }

        /// <summary>
        /// Creates a new algorithm base
        /// </summary>
        /// <param name="board">Board</param>
        protected Algorithm(Board board)
        {
            this.board = board;
            this.timer = new Stopwatch();
            this.eval = new Evaluator();
        }
    }
}
