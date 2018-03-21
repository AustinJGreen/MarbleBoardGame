using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarbleBoardGame
{
    /// <summary>
    /// MPMix Algorithm http://u.cs.biu.ac.il/~sarit/data/articles/Zuckermanetal-ijcai09.pdf
    /// </summary>
    public class MPMix : Algorithm
    {
        private MaxN maxNAlgorithm;
        private Paranoid paranoidAlgorithm;
        private Offensive offensiveAlgorithm;

        private double defensiveThreshold;
        private double offensiveThreshold;

        /// <summary>
        /// Sets the current algorithm values
        /// </summary>
        /// <param name="algorithm">Algorithm</param>
        private void SetValues(Algorithm algorithm)
        {
            Best = algorithm.Best;
            Depth = algorithm.Depth;
            Nodes = algorithm.Nodes;
            NodesPerSecond = algorithm.NodesPerSecond;
        }

        /// <summary>
        /// Starts calculating for a certain time
        /// </summary>
        /// <param name="ms">Milliseconds to calculate</param>
        public override Vector4 Go(int ms)
        {
            Vector4 h = eval.Evaluate(Root.Value, Root.RolledDoubles ? Player : Board.NO_PLAYER);
            double[] hArray = h.GetArray();

            board.SortDecreasing(hArray);
            double leadingEdge = hArray[0] - hArray[1];
            int leader = h.GetDirection();

            if (leader == Player)
            {
                if (leadingEdge >= defensiveThreshold)
                {
                    //Console.WriteLine("Using paranoid.");
                    paranoidAlgorithm.Player = Player;
                    paranoidAlgorithm.Root = Root;
                    Vector4 pEval = paranoidAlgorithm.Go(ms);
                    SetValues(paranoidAlgorithm);
                    return pEval;
                }
            }
            else
            {
                if (leadingEdge >= offensiveThreshold)
                {
                    //Console.WriteLine("Using offensive.");
                    offensiveAlgorithm.Player = Player;
                    offensiveAlgorithm.Root = Root;
                    offensiveAlgorithm.TargetPlayer = (sbyte)leader;
                    Vector4 oEval = offensiveAlgorithm.Go(ms);
                    SetValues(offensiveAlgorithm);
                    return oEval;
                }
            }

            //Console.WriteLine("Using maxN.");
            maxNAlgorithm.Player = Player;
            maxNAlgorithm.Root = Root;
            Vector4 maxEval = maxNAlgorithm.Go(ms);
            SetValues(maxNAlgorithm);
            return maxEval;
        }

        public override Vector4 Go()
        {
            Vector4 h = eval.Evaluate(Root.Value, Root.RolledDoubles ? Player : Board.NO_PLAYER);
            double[] hArray = h.GetArray();

            board.SortDecreasing(hArray);
            double leadingEdge = hArray[0] - hArray[1];
            int leader = h.GetDirection();

            if (leader == Player || leader == -1)
            {
                if (leadingEdge >= defensiveThreshold)
                {
                    paranoidAlgorithm.Player = Player;
                    paranoidAlgorithm.Root = Root;
                    Vector4 pEval = paranoidAlgorithm.Go();
                    SetValues(paranoidAlgorithm);
                    return pEval;
                }
            }
            else
            {
                if (leadingEdge >= offensiveThreshold)
                {
                    offensiveAlgorithm.Player = Player;
                    offensiveAlgorithm.Root = Root;
                    offensiveAlgorithm.TargetPlayer = (sbyte)leader;
                    Vector4 oEval = offensiveAlgorithm.Go();
                    SetValues(offensiveAlgorithm);
                    return oEval;
                }
            }

            //Console.WriteLine("Using maxN.");
            maxNAlgorithm.Player = Player;
            maxNAlgorithm.Root = Root;
            Vector4 maxEval = maxNAlgorithm.Go();
            SetValues(maxNAlgorithm);
            return maxEval;
        }

        /// <summary>
        /// Creates a new instance of the MaxN Algorithm for a board
        /// </summary>
        /// <param name="board">Board</param>
        public MPMix(Board board, double offensiveThreshold, double defensiveThreshold) : base(board)
        {
            maxNAlgorithm = new MaxN(board);
            paranoidAlgorithm = new Paranoid(board);
            offensiveAlgorithm = new Offensive(board);

            this.offensiveThreshold = offensiveThreshold;
            this.defensiveThreshold = defensiveThreshold;
        }
    }
}
