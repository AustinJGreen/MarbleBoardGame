
namespace MarbleBoardGame
{
    public class Search
    {
        protected Board board;

        /// <summary>
        /// Current position node
        /// </summary>
        public PositionNode Node { get; set; }

        /// <summary>
        /// Current position
        /// </summary>
        public Position Value {  get { return Node.Value; } }

        /// <summary>
        /// Evaluation Weight of Node
        /// </summary>
        public double EvalWeight { get { return Node.EvalWeight; } }
        
        /// <summary>
        /// Did the node roll doubles
        /// </summary>
        public bool RolledDoubles {  get { return Node.RolledDoubles; } }

        /// <summary>
        /// Next target player
        /// </summary>
        public sbyte NextPlayer { get; set; }

        /// <summary>
        /// Current player
        /// </summary>
        public sbyte CurrentPlayer { get; set; }

        /// <summary>
        /// Root player of the search
        /// </summary>
        public sbyte RootPlayer { get; set; }

        /// <summary>
        /// Current roll of doubles for the current player
        /// </summary>
        public int DoublesCount { get; set; }

        /// <summary>
        /// Gets the next target search
        /// </summary>
        /// <param name="current">Next Node</param>
        public Search Next(PositionNode current)
        {
            sbyte nextPlayer = board.NextPlayer(CurrentPlayer);

            Search next = new Search(board, current, RootPlayer, nextPlayer);

            if (current.RolledDoubles)
            {
                next.NextPlayer = next.CurrentPlayer;

                next.DoublesCount = DoublesCount + 1;
                if (next.DoublesCount >= 3)
                {
                    current.Value.RemoveFrontMarble(next.CurrentPlayer);
                    next.DoublesCount = 0;

                    next.NextPlayer = board.NextPlayer(next.CurrentPlayer);
                }
            }
            else
            {
                next.DoublesCount = 0;
            }

            return next;
        }

        /// <summary>
        /// Creates a new search
        /// </summary>
        /// <param name="board">Board</param>
        /// <param name="node">Current node</param>
        /// <param name="rootPlayer">Root player</param>
        /// <param name="currentPlayer">Current player</param>
        public Search(Board board, PositionNode node, sbyte rootPlayer, sbyte currentPlayer)
        {
            this.board = board;

            RootPlayer = rootPlayer;
            CurrentPlayer = currentPlayer;

            Node = node;

            DoublesCount = 0;
        }
    }
}
