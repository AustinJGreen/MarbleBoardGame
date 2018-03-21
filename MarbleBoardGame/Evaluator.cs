using System;

namespace MarbleBoardGame
{
    public class Evaluator
    {
        /// <summary>
        /// Gets the squares of the marbles of the specified team
        /// </summary>
        /// <param name="position">Postion to evaluate</param>
        /// <param name="team">Team to find</param>
        private Square[] GetMarbles(Position position, sbyte team)
        {
            int marbleIndex = 0;
            Square[] marbles = new Square[Board.TEAM_SIZE];
            for (int i = 0; i < Board.QUAD_COUNT + Board.BASE_SIZE; i++)
            {
                sbyte length = i < Board.QUAD_COUNT ? Board.QUAD_LENGTH : Board.BASE_LENGTH;
                for (int j = 0; j < length; j++)
                {
                    Square current = new Square(i, j);
                    int marbleType = position.Get(current);
                    if (marbleType == team)
                    {
                        marbles[marbleIndex] = current;
                        marbleIndex++;
                    }
                }
            }

            return marbles;
        }

        /// <summary>
        /// Gets the squares of enemy marbles of the specified team
        /// </summary>
        /// <param name="position">Position to evaluate</param>
        /// <param name="team">Team to get enemies of</param>
        /// <param name="activeMarbleCount">Reference for active marbles for each team, NOTE: The count of active marbles is not calculated for calling team</param>
        private Square[] GetEnemyMarbles(Position position, sbyte team, out int[] activeMarbleCount)
        {
            int marbleIndex = 0;
            activeMarbleCount = new int[Board.TEAM_COUNT];
            Square[] marbles = new Square[Board.TEAM_SIZE * 3]; //3 Enemy teams
            for (int i = 0; i < Board.QUAD_COUNT + Board.BASE_SIZE; i++)
            {
                sbyte length = i < Board.QUAD_COUNT ? Board.QUAD_LENGTH : Board.BASE_LENGTH;
                for (int j = 0; j < length; j++)
                {
                    Square current = new Square(i, j);
                    int marbleType = position.Get(current);
                    if (marbleType != -1 && marbleType != team)
                    {
                        marbles[marbleIndex] = current;
                        marbleIndex++;

                        if (current.IsInPath())
                        {
                            activeMarbleCount[marbleType]++;
                        }
                    }
                }
            }

            return marbles;
        }

        /// <summary>
        /// Counts the number of active marbles
        /// </summary>
        /// <param name="marbles">Marble squares</param>
        private int ActiveMarbles(Square[] marbles)
        {
            int active = 0;
            for (int i = 0; i < marbles.Length; i++)
            {
                if (marbles[i] != null)
                {
                    active++;
                }
            }

            return active;
        }

        /// <summary>
        /// Counts the number of active path marbles
        /// </summary>
        /// <param name="marbles">Marble squares</param>
        private int ActivePathMarbles(Square[] marbles)
        {
            int active = 0;
            for (int i = 0; i < marbles.Length; i++)
            {
                if (marbles[i] != null)
                {
                    if (marbles[i].IsInPath())
                    {
                        active++;
                    }
                }
            }

            return active;
        }

        /// <summary>
        /// Stable insertion sort that sorts the marbles by board index from a perspective of a team
        /// </summary>
        /// <param name="marbles">Marbles list</param>
        private Square[] GetSortedByIndex(Square[] marbles, sbyte team)
        {
            for (int i = 0; i < marbles.Length - 1; i++)
            {
                int j = i + 1;

                while (j > 0)
                {
                    if (marbles[j] != null)
                    {
                        if (marbles[j - 1] == null || marbles[j - 1].GetBoardIndex(team) > marbles[j].GetBoardIndex(team))
                        {
                            Square temp = marbles[j - 1];
                            marbles[j - 1] = marbles[j];
                            marbles[j] = temp;

                        }
                    }
                    j--;
                }
            }

            return marbles;
        }

        /// <summary>
        /// Gets all the home squares for a team
        /// </summary>
        /// <param name="team">Team to get home squares for</param>
        /// <returns></returns>
        private Square[] GetHomeSquares(sbyte team)
        {
            Square[] homeSquares = new Square[Board.BASE_LENGTH];
            for (sbyte i = 0; i < homeSquares.Length; i++)
            {
                homeSquares[i] = new Square(team + 4, i);
            }

            return homeSquares;
        }

        /// <summary>
        /// Gets the current end goal
        /// </summary>
        /// <param name="team">Team</param>
        /// <returns></returns>
        private Square GetEndGoal(Position pos, sbyte team)
        {
            for (sbyte i = Board.BASE_LENGTH - 1; i >= 0; i--)
            {
                Square square =  new Square(team + 4, i);
                if (pos.Get(square) != team)
                {
                    return square;
                }
            }

            return null;
        }

        /// <summary>
        /// Checks if the position has no moves and that the player must roll doubles three times, precondition is that all marbles are active
        /// </summary>
        private bool MustRollDoubles3x(Position position, sbyte team)
        {
            Square[] marbles = GetMarbles(position, team);
            for (int i = 1; i < marbles.Length; i++)
            {
                if (marbles[i] == null || !marbles[i].IsInBase() || marbles[i].SquareValue != i)
                {
                    return false;
                }
            }

            if (marbles[marbles.Length - 1] != null)
            {
                return marbles[0].QuadrantValue == Board.QUAD_ORDER[team][Board.TEAM_COUNT - 1] && marbles[0].SquareValue == Board.QUAD_LENGTH - 1;
            }

            return false;
        }

        /// <summary>
        /// Evaluates a teams overall score in units of scored marbles (5 = won)
        /// </summary>
        /// <param name="position">Position to evaluate for a team</param>
        /// <param name="team"></param>
        /// <returns></returns>
        public double EvaluateTeam(Position position, sbyte team, bool rolledDoubles)
        {
            //Score in units of scores (5 = won)
            double score = 0.0;

            const double marbleValue = 0.1;

            //Sorted list of marbles
            Square[] marbles = GetMarbles(position, team);
            int activeMarbles = ActiveMarbles(marbles);
            int activePathMarbles = ActivePathMarbles(marbles);

            int[] activeEnemyMarbleCount = null;
            Square[] enemies = GetEnemyMarbles(position, team, out activeEnemyMarbleCount);

            //Evaluation scorelist
            //O-Check for impossible position with roll for 1 to win
            //O-Evaluate distance to goal)
            //O-Evaluate danger (probability of being taken) (Add special case for being on enemy spawn squares?)
            //O-Evaluate scored marbles (Add bonus for being aligned back in a row)

            //-Check for impossible position with roll for 1 to win
            if (activeMarbles == Board.TEAM_SIZE)
            {
                if (MustRollDoubles3x(position, team))
                {
                    return DiceRoll.P_ROLLING_DOUBLES_3X;
                }
            }

            //-Evaluate distance to goal
            //-Pentalize marbles on own goal (for taking out)
            Square endGoal = GetEndGoal(position, team);
            double endIndex = endGoal.GetBoardIndex(team);

            double distanceScore = 0.0;
            for (int i = 0; i < marbles.Length; i++)
            {
                if (marbles[i] != null)
                {
                    if (marbles[i].IsInPath())
                    {
                        if (marbles[i].GetBoardIndex(team) == 1)
                        {
                            score -= marbleValue;
                        }

                        double dis = (endIndex - marbles[i].DistanceTo(endGoal, team));
                        distanceScore += dis;
                    }
                }
            }
            
            //Add score for activity
            score += activeMarbles * marbleValue;

            //Add the distance traveled 
            if (activeMarbles > 0)
            {
                score += (distanceScore - 1) / (double)(endIndex * activeMarbles);
            }

            //-Evaluate danger (probability of being taken) (Add special case for being on enemy spawn squares?) 
            double dangerScore = 0.0f;
            if (!rolledDoubles)
            {
                //Add special case for being on enemy spawn squares
                for (int m = 0; m < marbles.Length; m++)
                {
                    if (marbles[m] != null)
                    {
                        double weight = 2 * ((endIndex - marbles[m].DistanceTo(endGoal, team)) / endIndex);
                        for (sbyte eT = 0; eT < Board.TEAM_COUNT; eT++)
                        {
                            if (eT != team)
                            {
                                int marbleCount = activeEnemyMarbleCount[eT];
                                if (marbleCount > 0)
                                {
                                    Square enemySpawn = new Square(eT, 0);
                                    if (marbles[m].Equals(enemySpawn))
                                    {
                                        dangerScore += weight * DiceRoll.P_GETTING_OUT;
                                    }
                                }
                            }
                        }
                    }
                }

                for (int x = 0; x < enemies.Length; x++)
                {
                    if (enemies[x] != null)
                    {
                        if (enemies[x].IsInPath())
                        {
                            for (int t = 0; t < marbles.Length; t++)
                            {
                                if (marbles[t] != null)
                                {
                                    if (marbles[t].IsInPath())
                                    {
                                        double weight = (endIndex - marbles[t].DistanceTo(endGoal, team)) / endIndex;

                                        int distance = marbles[t].DistanceTo(enemies[x], team);
                                        if (distance > 0)
                                        {
                                            if (distance <= 6)
                                            {
                                                int marbleTeam = position.Get(enemies[x]);
                                                int activeEnemyMarbles = activeEnemyMarbleCount[marbleTeam]; //Active in path
                                                if (activeEnemyMarbles >= 2)
                                                {
                                                    dangerScore += weight * DiceRoll.P_ANY_NUM;
                                                }
                                                else if (activeEnemyMarbles >= 1)
                                                {
                                                    dangerScore += weight * (DiceRoll.P_ANY_NUM * DiceRoll.P_GETTING_OUT);
                                                }
                                                else
                                                {
                                                    double probability = DiceRoll.GetProbabilityQuick(distance);
                                                    dangerScore += weight * probability;
                                                }
                                            }
                                            else if (distance < 36) //If greater than, the probability is 0, so no danger
                                            {
                                                double probability = DiceRoll.GetProbabilityQuick(distance);
                                                dangerScore += weight * probability;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                //Subtract danger score
                score -= dangerScore;
            }
                      
            double eDistanceScore = 0.0;
            double eDividendTotal = 0.0;
            int eCount = 0;

            //O-Evaluate other marbles
            for (sbyte sEM = 0; sEM < enemies.Length; sEM++)
            {               
                if (enemies[sEM] != null)
                {
                    if (enemies[sEM].IsInPath())
                    {
                        sbyte eTeam = (sbyte)position.Get(enemies[sEM]);
                        Square eEndGoal = GetEndGoal(position, eTeam);

                        double eDividend = eEndGoal.GetBoardIndex(eTeam);                 
                        eDistanceScore += (eDividend - enemies[sEM].DistanceTo(eEndGoal, eTeam));

                        eDividendTotal += eDividend;
                        eCount++;
                    }
                }
            }

            if (eCount > 0)
            {
                //We don't want a full negative impact from enemy moves (between 1/2-3/4 scale factor)
                score -= 0.55 * (eDistanceScore / eDividendTotal);
            }
   
            //-Evaluate scored marbles (Add bonus for being aligned back in a row)
            bool inARow = true;

            int marblesScored = 0;
            double marblesScoredScore = 0; //Marbles as far back as possible = 1 score

            Square[] homeSquares = GetHomeSquares(team);
            for (int i = homeSquares.Length - 1; i >= 0; i--)
            {
                int marble = position.Get(homeSquares[i]);
                if (marble == team)
                {
                    if (inARow)
                    {
                        marblesScoredScore++;
                    }
                    else
                    {
                        marblesScoredScore += 0.5 + (0.5 / (double)(homeSquares.Length - i));
                    }

                    marblesScored++;
                }
                else
                {
                    //Gap, stop giving points
                    inARow = false;
                }
            }

            
            score += marblesScoredScore;

            return score;
        }

        /// <summary>
        /// Returns a heuristic evaluation in units of scores for each team
        /// </summary>
        /// <param name="position">Position to evaluate</param>
        /// <param name="rolledDoubles">The team that has roleld</param>
        /// <returns></returns>
        public Vector4 Evaluate(Position position, sbyte rolledDoubles)
        {
            Vector4 evaluation = new Vector4();
            sbyte winner = -1;
            if (position.IsWon(out winner))
            {
                evaluation[winner] = double.MaxValue;
                return evaluation;
            }

            for (sbyte i = 0; i < Board.TEAM_COUNT; i++)
            {
                evaluation.Add(EvaluateTeam(position, i, rolledDoubles == i), i);
            }

            return evaluation;
        }
    }
}
