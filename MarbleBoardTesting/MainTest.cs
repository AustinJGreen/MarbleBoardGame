using System;
using MarbleBoardGame;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace MarbleBoardTesting
{
    [TestClass]
    public class MainTest
    {
        [TestMethod]
        public void TestMarbleOverlapping()
        {
            Position test = new Position(Board.EMPTY_BOARD);
            test.Set("b11", Board.YELLOW);
            test.Set("b12", Board.YELLOW);

            Board board = new Board();
            board.SetPosition(test);

            MoveCollection moves = board.GetMoves(new DiceRoll(2, 5), Board.YELLOW);
            Assert.IsTrue(moves.Count == 1);
        }

        [TestMethod]
        public void TestPosition1()
        {
            Position position = new Position(Board.EMPTY_BOARD);
            position.Set("hy2", Board.YELLOW);
            position.Set("hy3", Board.YELLOW);
            position.Set("hy4", Board.YELLOW);
            position.Set("hy5", Board.YELLOW);

            position.Set("hg3", Board.GREEN);
            position.Set("hg4", Board.GREEN);
            position.Set("hg5", Board.GREEN);

            position.Set("r9", Board.GREEN);
            position.Set("g7", Board.YELLOW);

            Board board = new Board();
            board.SetPosition(position);

            Algorithm maxN = new MaxN(board);
            Algorithm para = new Paranoid(board);

            Move maxNMove = board.ThinkBest(maxN, new DiceRoll(1, 6), Board.GREEN, 2000);
            Move paraMove = board.ThinkBest(para, new DiceRoll(1, 6), Board.GREEN, 2000);
            
            Assert.IsTrue(maxNMove.GetNotation() == "g7" && paraMove.GetNotation() == "g7");
        }

        [TestMethod]
        public void TestPosition2()
        {
            Position position = new Position(Board.EMPTY_BOARD);
            position.Set("g6", Board.GREEN);
            position.Set("b6", Board.BLUE);

            Board board = new Board();
            board.SetPosition(position);

            Algorithm maxN = new MaxN(board);
            Algorithm mpMix = new MPMix(board, 0.5, 0.5);

            Move maxNMove = board.ThinkBest(maxN, new DiceRoll(1, 6), Board.GREEN, 2000);
            Assert.IsFalse(maxNMove.GetNotation() == "b1");

            Move mixMove = board.ThinkBest(mpMix, new DiceRoll(1, 6), Board.GREEN, 2000);
            Assert.IsFalse(maxNMove.GetNotation() == "b1");
        }

        [TestMethod]
        public void TestPosition3()
        {
            

        }
    }
}
