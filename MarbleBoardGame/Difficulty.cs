namespace MarbleBoardGame
{
    public class Difficulty
    {
        /// <summary>
        /// Time of think
        /// </summary>
        public int TimeThink { get; set; }

        /// <summary>
        /// Blunder percentage
        /// </summary>
        public double BlunderPercent { get; set; }

        /// <summary>
        /// Creates a new difficulty setting
        /// </summary>
        /// <param name="timeThink">Time to think</param>
        /// <param name="blunderPercent">Blunder percentage</param>
        public Difficulty(int timeThink, double blunderPercent)
        {
            TimeThink = timeThink;
            BlunderPercent = blunderPercent;
        }
    }
}
