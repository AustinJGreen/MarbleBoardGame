
namespace MarbleBoardGame
{
    public class HumanPlayer : Player
    {
        public override void StartThink(Board board, DiceRoll roll)
        {
            base.StartThink(board, roll);
            //Continue logic 

            MoveCollection collection = board.GetMoves(roll, Team);
            if (collection.Count == 0)
            {
                base.SetMove(null);
            }
        }

        public HumanPlayer(sbyte team, BoardView boardView) : base(team, true, boardView) { }
    }
}
