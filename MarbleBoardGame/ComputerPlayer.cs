using System;
using System.Threading;

namespace MarbleBoardGame
{
    public class ComputerPlayer : Player
    {
        private MaxN maxNAlgorithm;
        private Paranoid paranoidAlgorithm;
        private Offensive offensiveAlgorithm;
        private MPMix mixAlgorithm;
        private MCTS mctsAlgorithm;

        //Thinking thread
        private Thread thinkThread;

        /// <summary>
        /// Gets or Sets the difficulty for the computer
        /// </summary>
        public Difficulty Difficulty { get; set; }

        /// <summary>
        /// Gets or Sets the personality for the computer
        /// </summary>
        public Personality Personality { get; set; }

        /// <summary>
        /// Gets the last evaluation from the algorithm
        /// </summary>
        public Variation[] LastEvaluation;

        /// <summary>
        /// Gets the corresponding algorithm to use depending the personaility
        /// </summary>
        /// <returns></returns>
        public Algorithm GetAlgorithm()
        {
            switch (Personality)
            {
                case Personality.Active:
                    return maxNAlgorithm;
                case Personality.Aggressive:
                    return offensiveAlgorithm;
                case Personality.Balanced:
                    return mixAlgorithm;
                case Personality.Passive:
                    return paranoidAlgorithm;
                case Personality.Adaptive:
                    return mctsAlgorithm;
            }

            return null;
        }

        private Variation[] GetVariations(MoveCollection moves, Vector4[] scores)
        {
            if (scores == null)
            {
                return null;
            }

            if (moves.Count != scores.Length)
            {
                throw new ArgumentException();
            }

            Variation[] vars = new Variation[moves.Count];
            for (int i = 0; i < moves.Count; i++)
            {
                vars[i] = new Variation(moves[i], scores[i]);
            }

            return vars;
        }

        /// <summary>
        /// Starts thinking
        /// </summary>
        public void Think(DiceRoll roll, Board board)
        {
            int timeThink = Difficulty.TimeThink;
            Algorithm algorithm = GetAlgorithm();

            if (timeThink < 0)
            {
                base.SetMove(board.ThinkBest(algorithm, roll, Team, timeThink));
                return;
            }

            int blunderValue = Board.Next(100);
            if (blunderValue < Difficulty.BlunderPercent)
            {
                //Worst move
                Vector4[] scores;
                MoveCollection moves = board.ThinkAll(algorithm, roll, Team, ComputerPlayer.Difficulties[ComputerPlayer.Difficulties.Length - 1].TimeThink, out scores);
                LastEvaluation = GetVariations(moves, scores);

                if (moves != null)
                {
                    base.SetMove(moves[moves.Count - 1]);
                    return;
                }
            }
            else
            {
                //Best move
                Vector4[] scores;
                MoveCollection moves = board.ThinkAll(algorithm, roll, Team, Difficulty.TimeThink, out scores);
                LastEvaluation = GetVariations(moves, scores);

                if (moves != null)
                {
                    base.SetMove(moves[0]);
                    return;
                }
            }

            base.SetMove(null);
        }

        public override void StartThink(Board board, DiceRoll roll)
        {
            base.StartThink(board, roll);
            //Continue logic

            if (thinkThread != null)
            {
                if (thinkThread.ThreadState == ThreadState.Running)
                {
                    thinkThread.Abort();
                    thinkThread = new Thread(() => Think(roll, board));
                }
            }

            thinkThread = new Thread(() => Think(roll, board));
            thinkThread.Name = "MarbleBoard_AI";
            thinkThread.IsBackground = true;
            DateTime start = DateTime.Now;
            thinkThread.Start();
        }

        public void StopThink()
        {
            if (thinkThread != null)
            {
                if (thinkThread.ThreadState == ThreadState.Running)
                {
                    thinkThread.Abort();
                }
            }
        }

        public ComputerPlayer(sbyte team, Personality personality, Difficulty difficulty, BoardView boardView)
            : base(team, false, boardView)
        {
            Difficulty = difficulty;
            Personality = personality;
            maxNAlgorithm = new MaxN(boardView.Board);
            paranoidAlgorithm = new Paranoid(boardView.Board);
            offensiveAlgorithm = new Offensive(boardView.Board);
            mixAlgorithm = new MPMix(boardView.Board, 0.5, 0.5);
            mctsAlgorithm = new MCTS(boardView.Board);
        }


        /// <summary>
        /// Levels of progessively harder computer difficulties
        /// </summary>
        public static Difficulty[] Difficulties = new Difficulty[10]
        {
            new Difficulty(0, 100),
            new Difficulty(0, 90),
            new Difficulty(0, 80),
            new Difficulty(0, 70),
            new Difficulty(250, 60),
            new Difficulty(500, 45),
            new Difficulty(1000, 35),
            new Difficulty(1250, 20),
            new Difficulty(1500, 10),
            new Difficulty(2000, 0)
        };
    }
}
