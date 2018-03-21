using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MarbleBoardGame
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        public static sbyte SimulateGame()
        {
            Board board = new Board();
            Algorithm mpMix = new MPMix(board, 1, 0.5);
            Algorithm maxN = new MaxN(board);
            Algorithm paranoid = new Paranoid(board);
            Algorithm offensive = new Offensive(board);
            Algorithm mcts = new MCTS(board);

            int doublesCount = 0;
            sbyte lastTeam = 0;
            sbyte team = 0;
            int turns = 0;
            while (!board.IsGameOver())
            {
                Console.Title = turns.ToString();
                lastTeam = team;

                DiceRoll roll = board.GetRandomRoll();
                Move m = null;
                switch (team)
                {
                    case 0:
                        m = board.ThinkBest(mcts, roll, team, 0);
                        break;
                    case 1:
                        m = board.ThinkBest(maxN, roll, team, 0);
                        break;
                    case 2:
                        m = board.ThinkBest(mcts, roll, team, 0);
                        break;
                    case 3:
                        m = board.ThinkBest(mcts, roll, team, 0);
                        break;
                }
                if (m != null)
                {
                    board.PerformMove(m);
                }

                if (roll.IsDoubles())
                {
                    doublesCount++;
                    if (doublesCount >= 3)
                    {
                        board.RemoveFrontMarble(team);
                        doublesCount = 0;
                        team = board.NextPlayer(team);
                        turns++;
                    }
                }
                else
                {
                    doublesCount = 0;
                    team = board.NextPlayer(team);
                    turns++;
                }
            }

            Console.WriteLine("{0} won ({1} turns)", GetName(lastTeam), turns);
            return lastTeam;
        }

        public static string GetName(sbyte team)
        {
            switch (team)
            {
                case Board.YELLOW:
                    return "Yellow";
                case Board.GREEN:
                    return "Green";
                case Board.BLUE:
                    return "Blue";
                case Board.RED:
                    return "Red";
            }

            return "null";
        }

        public static int[] SimulateGames(int count)
        {
            int game = 0;
            int games = count;
            int[] wins = new int[4];
            while (game < games)
            {
                sbyte winner = SimulateGame();
                wins[winner]++;
                game++;
            }

            for (sbyte i = 0; i < 4; i++)
                Console.WriteLine("{0} won {1} times", GetName(i), wins[i]);
            Console.ReadKey();
            return wins;
        }

        public static void TestSearch()
        {
            Position position = new Position(Board.EMPTY_BOARD);
            position.Set("hy5", Board.YELLOW);
            position.Set("hr5", Board.RED);
            position.Set("hg5", Board.GREEN);
            position.Set("hb5", Board.BLUE);
            position.Set("y6", Board.BLUE);
            position.Set("r2", Board.GREEN);

            
            Board board = new Board();
            board.SetPosition(position);

            Algorithm alg = new MaxN(board);
            alg.Root = new PositionNode(position, null, new DiceRoll(1, 6));
            alg.Player = Board.YELLOW;

            

            Console.ReadKey();

            //Assert.Inconclusive("Search Time: " + time);
        }

        public static void TestEvaluation()
        {
            Position position = new Position(Board.EMPTY_BOARD);

            position.Set("g1", Board.BLUE);
            position.Set("hb2", Board.BLUE);
            position.Set("hb3", Board.BLUE);
            position.Set("hb4", Board.BLUE);
            position.Set("hb5", Board.BLUE);

            position.Set("b1", Board.YELLOW);
            position.Set("hy2", Board.YELLOW);
            position.Set("hy3", Board.YELLOW);
            position.Set("hy4", Board.YELLOW);
            position.Set("hy5", Board.YELLOW);

            Console.WriteLine(position.GetVisual());
            Console.ReadKey();

            Evaluator e = new Evaluator();
            var sc = e.Evaluate(position, Board.YELLOW);
            var scwith = e.Evaluate(position, Board.BLUE);
        }

        static void TestMethod()
        {
            Stopwatch sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < 20000)
            {
                Console.WriteLine("TIME: {0}", sw.ElapsedMilliseconds);
                Thread.Sleep(2500);
            }
        }

        static void TestThreading()
        {
            Board board = new Board();
            board.Set("g5", Board.YELLOW);

            BoardView model = new BoardView(board, 700);
            ComputerPlayer plyr = new ComputerPlayer(Board.YELLOW, Personality.Passive, new Difficulty(10000, 0.0), model);
            plyr.StartThink(board, new DiceRoll(5, 6));

            while (plyr.IsThinking)
            {
                Console.WriteLine("Thinking...");
                Thread.Sleep(150);
            }

            //Dont need to dispose boardView, Load has not been called
            Console.WriteLine("Done, best move is: {0}", plyr.GetMove().GetNotation());
            Console.ReadKey();
        }

        [STAThread]
        static void Main()
        {
            //SimulateGames(100);
            //return;

            //TestThreading();
            //TestSearch();
            //return;

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            Console.WriteLine("{0}\n{1}", ex.Message, ex.StackTrace);
        }
    }
#endif
}
