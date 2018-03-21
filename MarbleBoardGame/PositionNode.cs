using System;
namespace MarbleBoardGame
{
    public struct PositionNode
    {
        /// <summary>
        /// The position value
        /// </summary>
        public Position Value { get; set; }

        /// <summary>
        /// The roll that determined the position
        /// </summary>
        public DiceRoll Roll { get; set; }

        /// <summary>
        /// Move assosiated to get the position
        /// </summary>
        public Move Move { get; set; }

        /// <summary>
        /// If the position was determined by rolling doubles
        /// </summary>
        public bool RolledDoubles { get { return Roll.IsDoubles(); } }

        /// <summary>
        /// Gets the eval weight of the roll
        /// </summary>
        public double EvalWeight { get { return (Roll == null) ? 1 : 1 / Roll.GetProbabilityRank(); } }

        /// <summary>
        /// Gets the position node hash
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int posHash = Value.GetHashCode();

            if (RolledDoubles)
            {
                int bits = (int)Math.Ceiling(Math.Log10(posHash));
                return posHash ^ (1 << bits);
            }

            return posHash;
        }

        /// <summary>
        /// Converts the node into a readable notation
        /// </summary>
        public override string ToString()
        {
            return base.ToString();
        }

        /// <summary>
        /// Creates a new position node
        /// </summary>
        /// <param name="value">Position value</param>
        /// <param name="roll">Dice roll</param>
        public PositionNode(Position value, Move move, DiceRoll roll) : this()
        {
            Value = value;
            Move = move;
            Roll = roll;
        }
    }
}
