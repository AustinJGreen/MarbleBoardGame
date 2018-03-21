using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MarbleBoardGame
{
    public class MCTS : Algorithm
    {
        private sbyte NextTeam(sbyte cur)
        {
            return (sbyte)((cur + 1) % 4);
        }

        private double Iterate(Position pos, sbyte us, sbyte evaluating_team, int time)
        {
            Stopwatch sw = Stopwatch.StartNew();

            double wins = 0;
            double losses = 0;

            Board board = new Board();

            sbyte team = NextTeam(us);

            int doublesCount = 0;
            sbyte lastTeam = 0;
            
            int turns = 0;
            do
            {
                if (sw.ElapsedMilliseconds >= time)
                {
                    return 0;
                }
                //Reset board
                board.SetPosition(pos);
                if (sw.ElapsedMilliseconds >= time)
                {
                    return 0;
                }

                //Pick random nodes and play out game
                while (!board.IsGameOver())
                {
                    lastTeam = team;

                    DiceRoll roll = board.GetRandomRoll();
                    if (sw.ElapsedMilliseconds >= time)
                    {
                        return 0;
                    }

                    MoveCollection moves = board.GetMoves(roll, team);
                    if (sw.ElapsedMilliseconds >= time)
                    {
                        return 0;
                    }

                    if (moves.Count != 0)
                    {
                        Move m = moves[Board.Next(moves.Count)];
                        if (m != null)
                        {
                            board.PerformMove(m);
                        }
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

                    if (sw.ElapsedMilliseconds >= time)
                    {
                        return 0;
                    }
                }

                if (lastTeam == evaluating_team)
                {
                    wins++;
                }
                else
                {
                    losses++;
                }
            }
            while (sw.ElapsedMilliseconds < time);

            if (wins == 0 || losses == 0)
            {
                return 0;
            }

            return wins / losses;
        }

        public override Vector4 Go(int ms)
        {
            Position pos = board.GetPosition();

            Vector4 eval = new Vector4();

            for (sbyte p = 0; p < 4; p++)
            {
                double s = Iterate(pos, Player, p, ms / 4);
                eval[p] = s;
            }

            return eval;
        }

        public override Vector4 Go()
        {
            return Go(60000);
        }

        /// <summary>
        /// Creates a new instance of the MaxN Algorithm for a board
        /// </summary>
        /// <param name="board">Board</param>
        public MCTS(Board board) : base(board)
        {
        }
    }
}
