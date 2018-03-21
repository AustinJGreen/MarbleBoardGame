using System;
using System.Runtime.CompilerServices;

namespace MarbleBoardGame
{
    public class DiceRoll
    {
        /// <summary>
        /// Value of the first die
        /// </summary>
        public int Die1;

        /// <summary>
        /// Value of the second die
        /// </summary>
        public int Die2;

        /// <summary>
        /// Evaluates whether the dice roll is doubles
        /// </summary>
        public bool IsDoubles()
        {
            return Die1 == Die2;
        }

        /// <summary>
        /// Can the roll be used to take out a marble
        /// </summary>
        public int[] CanTakeOutWith()
        {
            bool firstCan = Die1 == 1 || Die1 == 6;
            bool secondCan = Die2 == 1 || Die2 == 6;
            if (firstCan && secondCan)
            {
                return new int[] { Die1, Die2 };
            }

            //Return the opposite die
            if (firstCan)
            {
                return new int[] { Die2 };
            }
            else if (secondCan)
            {
                return new int[] { Die1 };
            }

            return new int[0];
        }

        /// <summary>
        /// Gets the combinations of all take out values for doubles
        /// </summary>
        public int[] GetDoublesTakeOutCombinations()
        {
            bool firstCan = Die1 == 1 || Die1 == 6;
            bool secondCan = Die2 == 1 || Die2 == 6;
            if (firstCan && secondCan)
            {
                return new int[] { Die1, Die2 };
            }
            else if (firstCan)
            {
                return new int[] {  Die2 };
            }
            else if (secondCan)
            {
                return new int[] { Die1 };
            }

            return new int[0];
        }

        /// <summary>
        /// Gets the list of playable values based on pieceCount
        /// </summary>
        /// <param name="pieceCount">Amount of pieces 1 or 2</param>
        /// <returns></returns>
        public int[][] GetValues(int pieceCount)
        {
            if (pieceCount == 1)
            {
                return new int[][] { new int[] { Die1 + Die2 } };
            }
            else if (pieceCount == 2)
            {
                return new int[][] { new int[] { Die1, Die2 }, new int[] { Die2, Die1 } };
            }
            else
            {
                throw new ArgumentOutOfRangeException("pieceCount");
            }
        }

        /// <summary>
        /// Subtracts a value from the total roll value
        /// </summary>
        /// <param name="dieValue">die value to subtract</param>
        public int Subtract(int dieValue)
        {
            return (Die1 + Die2) - dieValue;
        }

        /// <summary>
        /// Gets all die values including total, without duplicates
        /// </summary>
        public int[] GetValues()
        {
            if (IsDoubles())
            {
                return new int[] { Die1, Die1 + Die2 };
            }
            else
            {
                return new int[] { Die1, Die2, Die1 + Die2 };
            }
        }

        /// <summary>
        /// Gets the probability of rolling the same combined value of this dice roll
        /// </summary>
        public double GetProbability()
        {
            return GetProbabilityQuick(Die1 + Die2);  
        }

        /// <summary>
        /// Gets the rank of probability, 7 being 1
        /// </summary>
        public double GetProbabilityRank()
        {
            //7 Is the highest probability, return distance from 7
            return Math.Abs(7 - (Die1 + Die2)) + 1;
        }

        /// <summary>
        /// Parses a integer
        /// </summary>
        /// <param name="dieNotation">die roll integer</param>
        private int ParseInt(string dieNotation)
        {
            int parsed = -1;
            int.TryParse(dieNotation, out parsed);
            return parsed;
        }

        /// <summary>
        /// Parses the die rolls
        /// </summary>
        /// <param name="diceNotation">Dice notation</param>
        private void Parse(string diceNotation)
        {
            string[] plusSplit = diceNotation.Split('+');
            if (plusSplit.Length == 2)
            {
                Die1 = ParseInt(plusSplit[0]);
                Die2 = ParseInt(plusSplit[1]);
            }
        }

        /// <summary>
        /// Creates a new dice roll representation
        /// </summary>
        /// <param name="diceNotation">Dice notation</param>
        public DiceRoll(string diceNotation)
        {
            Parse(diceNotation);
        }

        /// <summary>
        /// Create a dice roll
        /// </summary>
        /// <param name="die1">Value of the first die</param>
        /// <param name="die2">Value of the second die</param>
        public DiceRoll(int die1, int die2)
        {
            if (die1 < 0 || die1 > 6  || die2 < 0 || die2 > 6)
            {
                throw new ArgumentException();
            }

            Die1 = die1;
            Die2 = die2;
        }

        /// <summary>
        /// Gets the amount of combinations that can roll a die value
        /// </summary>
        /// <param name="dieValue">Die value</param>
        public static int GetCombinations(int dieValue)
        {
            int combinations = 0;
            int cDieValue = dieValue - 1;

            while (cDieValue >= Math.Ceiling(dieValue / 2.0))
            {
                if (cDieValue <= 6)
                {
                    if (dieValue - cDieValue == cDieValue)
                    {
                        combinations++;
                    }
                    else
                    {
                        combinations += 2;
                    }
                }

                cDieValue--;
            }

            return combinations;
        }

        /// <summary>
        /// Gets the probability of rolling a number on a die
        /// </summary>
        /// <param name="number">Number to roll</param>
        /// <param name="depth">Depth of doubles rolled</param>
        public static double GetProbability(int number, int depth = 0)
        {
            if (number <= 12)
            {
                int combinations = GetCombinations(number);
                return combinations / 36.0;
            }
            else if (depth < 2 && number < 36) //Maximum roll of doubles
            {
                //Probability of rolling doubles
                double doubles = Math.Pow(6 / 36.0, depth + 1);

                double prob = 0.0;
                for (int i = 2; i <= 12; i += 2)
                {
                    prob += doubles * GetProbability(number - i, depth + 1);
                }

                return prob;
            }
            else
            {
                return 0.0f;
            }
        }

        /// <summary>
        /// Retrieves the probability from memory instead of calculating the probability
        /// </summary>
        /// <param name="number">Number to get probability for</param>
#if AGGR_INLINE 
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
#endif 
        public static double GetProbabilityQuick(int number)
        {
            return PreCalculatedProbabilities[number];
        }

        /// <summary>
        /// Gets the probability of rolling a pair of die that can take a marble out
        /// </summary>
        public static double P_GETTING_OUT = 20 / 36.0;

        /// <summary>
        /// Gets the probability of rolling doubles 3 times
        /// </summary>
        public static double P_ROLLING_DOUBLES_3X = 0.0046296296296296285;

        /// <summary>
        /// Gets the probability of rolling a single number 1-6 on one of the die
        /// </summary>
        public static double P_ANY_NUM = 1 / 6.0;

        /// <summary>
        /// Pre calculated dice roll probabilities for quick reference
        /// </summary>
        public static double[] PreCalculatedProbabilities = new double[36]
        {
            0,
            0,
            0.0277777777777778,
            0.0555555555555556,
            0.0833333333333333,
            0.111111111111111,
            0.138888888888889,
            0.166666666666667,
            0.138888888888889,
            0.111111111111111,
            0.0833333333333333,
            0.0555555555555556,
            0.0277777777777778,
            0.0833333333333333,
            0.0833333333333333,
            0.0856481481481481,
            0.0810185185185185,
            0.0787037037037037,
            0.0693158436213992,
            0.0622427983539095,
            0.0479681069958848,
            0.0360082304526749,
            0.025977366255144,
            0.018261316872428,
            0.0126028806584362,
            0.00925925925925926,
            0.00810185185185185,
            0.00694444444444444,
            0.00578703703703704,
            0.00462962962962963,
            0.00360082304526749,
            0.00257201646090535,
            0.00180041152263374,
            0.00102880658436214,
            0.000643004115226337,
            0.000257201646090535
        };

        /// <summary>
        /// All possible dice rolls for a result of seven without reversed duplicates
        /// </summary>
        public static DiceRoll[] SEVEN = new DiceRoll[] 
        {
            new DiceRoll(1, 6),
            new DiceRoll(2, 5),
            new DiceRoll(3, 4)
        };

        /// <summary>
        /// All possible dice rolls for a pair of die
        /// </summary>
        public static DiceRoll[] ALL = new DiceRoll[]
        {
            new DiceRoll(1, 1),
            new DiceRoll(1, 2),
            new DiceRoll(1, 3),
            new DiceRoll(1, 4),
            new DiceRoll(1, 5),
            new DiceRoll(1, 6),
            new DiceRoll(2, 1),
            new DiceRoll(2, 2),
            new DiceRoll(2, 3),
            new DiceRoll(2, 4),
            new DiceRoll(2, 5),
            new DiceRoll(2, 6),
            new DiceRoll(3, 1),
            new DiceRoll(3, 2),
            new DiceRoll(3, 3),
            new DiceRoll(3, 4),
            new DiceRoll(3, 5),
            new DiceRoll(3, 6),
            new DiceRoll(4, 1),
            new DiceRoll(4, 2),
            new DiceRoll(4, 3),
            new DiceRoll(4, 4),
            new DiceRoll(4, 5),
            new DiceRoll(4, 6),
            new DiceRoll(5, 1),
            new DiceRoll(5, 2),
            new DiceRoll(5, 3),
            new DiceRoll(5, 4),
            new DiceRoll(5, 5),
            new DiceRoll(5, 6),
            new DiceRoll(6, 1),
            new DiceRoll(6, 2),
            new DiceRoll(6, 3),
            new DiceRoll(6, 4),
            new DiceRoll(6, 5),
            new DiceRoll(6, 6)
        };

        /// <summary>
        /// All possible dice rolls for a pair of die without duplicates
        /// </summary>
        public static DiceRoll[] ALL_NO_DUPL = new DiceRoll[]
        {
            new DiceRoll(1, 1),//0
            new DiceRoll(1, 2),
            new DiceRoll(1, 3),
            new DiceRoll(1, 4),
            new DiceRoll(1, 5),
            new DiceRoll(1, 6),
            new DiceRoll(2, 2),
            new DiceRoll(2, 3),
            new DiceRoll(2, 4),
            new DiceRoll(2, 5),
            new DiceRoll(2, 6),
            new DiceRoll(3, 3),
            new DiceRoll(3, 4),
            new DiceRoll(3, 5),
            new DiceRoll(3, 6),
            new DiceRoll(4, 4),
            new DiceRoll(4, 5),
            new DiceRoll(4, 6),
            new DiceRoll(5, 5),
            new DiceRoll(5, 6),
            new DiceRoll(6, 6)
        };

        /// <summary>
        /// The list of rolls to get out, with no repeating nodes
        /// </summary>
        public static DiceRoll[] ALL_START_NO_REPT = new DiceRoll[]
        {
            new DiceRoll(1, 1),
            new DiceRoll(1, 2),
            new DiceRoll(1, 3),
            new DiceRoll(1, 4),
            new DiceRoll(1, 5),
            new DiceRoll(1, 6)
        };

        /// <summary>
        /// Group of rolls that result in high amount of nodes
        /// </summary>
        public static DiceRoll[] HIGH_ROLLS = new DiceRoll[]
        {
            new DiceRoll(1, 1),
            new DiceRoll(1, 2),
            new DiceRoll(1, 3),
            new DiceRoll(1, 4),
            new DiceRoll(1, 5),
            new DiceRoll(1, 6)
        };
    }
}
