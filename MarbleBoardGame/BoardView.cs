using Jitter;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MarbleBoardGame
{
    public class BoardView : IDisposable
    {
        //Content variables
        private Prism die1Entity, die2Entity;
        private SoundEffect[] placeEffects;
        private SoundEffect rollEffect;
        private DateTime? lastRollEffectPlay;

        //Logic related variables
        private List<PieceMove> currentMove;

        //Graphics related variables
        private List<Line> guiLines;
        private Queue<MarbleView[]> animations;
        private MarbleView currentAnimation;
        private MarbleView currentDrag;
        private int currentAnimIndex = 0;
        private int currentAnimViewIndex = 0;
        private int teamPerspective;

        //Logic references   
        private Board board;
        private World world;
        private RigidBody die1Body, die2Body;
        private BoardSquareView[][] squares;
        private SquareView[][] starts;
        private string[][] layout;
        private Random random;
        private DiceRoll currentRoll;

        //Function and logic variables
        private Evaluator defaultEval = new Evaluator();

        private Player[] players;
        private List<DiceRoll> rolls;
        private TimeSpan? rollStartTime;
        private int die1 = -1, die2 = -1;
        private bool isDiceRolling = false;
        private bool rollOver = true;
        private bool animationOver = true;

        private List<Move> moveHistory = new List<Move>();
        private Player currentPlayer = null;
        private int doublesCount = 0;

        /// <summary>
        /// Gets or Sets if the game is being played
        /// </summary>
        public bool Playing { get; set; }

        /// <summary>
        /// Gets or Sets if the game is being resized
        /// </summary>
        public bool Resizing { get; set; }

        public bool PlayDiceSounds { get; set; }

        public bool PlayMarbleSounds { get; set; }

        public AnalysisPane AnalysisView { get; set; }
        public NotationPane NotationView { get; set; }

        /// <summary>
        /// Gets the current moving player
        /// </summary>
        public Player Current { get { return currentPlayer; } }

        /// <summary>
        /// Gets the board
        /// </summary>
        public Board Board { get { return board; } }

        /// <summary>
        /// View of the board
        /// </summary>
        public Rectangle View { get; private set; }

        /// <summary>
        /// Checks if the board has a created user move
        /// </summary>
        public bool HasMove { get { return currentMove.Count > 0; } }

        /// <summary>
        /// Current queued move created by the user
        /// </summary>
        public Move QueuedMove { get; set; }

        /// <summary>
        /// Takes out a marble from a teams start
        /// </summary>
        /// <param name="team">Team to take a marble from</param>
        public void TakeOutMarble(sbyte team)
        {
            for (int j = starts[team].Length - 1; j >= 0; j--)
            {
                if (starts[team][j].HasMarble)
                {
                    starts[team][j].HasMarble = false;
                    break;
                }
            }
        }

        /// <summary>
        /// Puts a marble back into a teams start
        /// </summary>
        /// <param name="team">Team</param>
        public void PutBackMarble(int team)
        {
            for (int j = 0; j < starts[team].Length; j++)
            {
                if (!starts[team][j].HasMarble)
                {
                    starts[team][j].HasMarble = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Plays a random marble placement sound
        /// </summary>
        private void PlayRandomPlaceSound()
        {
            int index = random.Next(placeEffects.Length);
            placeEffects[index].Play();
        }

        /// <summary>
        /// Callback when a animation is finished
        /// </summary>
        public void OnAnimationDone()
        {
            if (PlayMarbleSounds)
            {
                PlayRandomPlaceSound();
            }

            //If there are any moves left
            if (animations.Count > 0)
            {
                //Get the move part animations
                MarbleView[] animationList = animations.Peek();

                DoMoveInternals(QueuedMove[currentAnimIndex], QueuedMove.Team, currentAnimViewIndex, animationList.Length);

                //Increment the current part of the piece move animation we are on
                currentAnimViewIndex++;

                //If we are finished animating all parts of the piece move, move on the next piece move
                if (currentAnimViewIndex >= animationList.Length)
                {
                    currentAnimViewIndex = 0;
                    animations.Dequeue();

                    board.PerformMove(QueuedMove[currentAnimIndex], QueuedMove.Team);
                    if (animations.Count == 0)
                    {
                        currentAnimIndex = 0;
                        currentAnimation = null;
                        OnMoveAnimationDone();
                    }
                    else
                    {
                        currentAnimIndex++;
                        currentAnimation = animations.Peek()[0];
                    }
                }
                else
                {
                    currentAnimation = animationList[currentAnimViewIndex];
                }
            }
        }

        /// <summary>
        /// Event handler for when the move's entire animation is finished
        /// </summary>
        public void OnMoveAnimationDone()
        {
            animationOver = true;

            ProcessRoll();
            SetReadyForRoll();
        }

        /// <summary>
        /// Removes a moves graphical indicators
        /// </summary>
        public void RemoveMove()
        {
            currentMove.Clear();
            guiLines.Clear();
        }

        /// <summary>
        /// Gets the corner position of a square if the square exists
        /// </summary>
        /// <param name="square">Square</param>
        public Vector2? GetVector(Square square)
        {
            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    if (squares[i][j] != null)
                    {
                        Square current = squares[i][j].Square;
                        if (current.Equals(square))
                        {
                            return new Vector2(squares[i][j].Rect.X, squares[i][j].Rect.Y);
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a square from a position on the screen
        /// </summary>
        /// <param name="position">Position</param>
        public Square GetSquare(Vector2 position)
        {
            for (int i = 0; i < squares.Length; i++)
            {
                BoardSquareView[] views = squares[i];
                for (int j = 0; j < views.Length; j++)
                {
                    if (views[j] != null)
                    {
                        if (views[j].Rect.Contains((int)position.X, (int)position.Y))
                        {
                            return views[j].Square;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Queues a piecemove as a animation
        /// </summary>
        /// <param name="move">Piecemove to animate</param>
        /// <param name="team">Team of move</param>
        /// <param name="time">Elapsed time for animation</param>
        public void QueueAnimation(PieceMove move, sbyte team, float time)
        {
            if (move.IsTakingOutMarble)
            {
                Vector2 start = Vector2.Zero;

                if (!currentPlayer.IsHuman)
                {
                    for (int j = starts[team].Length - 1; j >= 0; j--)
                    {
                        if (starts[team][j].HasMarble)
                        {
                            start = new Vector2(starts[team][j].Rect.X, starts[team][j].Rect.Y);
                            starts[team][j].HasMarble = false;
                            break;
                        }
                    }
                }

                Square homeSquare = new Square(team, 0);

                Vector2? homeSquareTarget = GetVector(homeSquare);
                Vector2? target = GetVector(move.To);
                if (homeSquareTarget.HasValue && target.HasValue)
                {
                    MarbleView takeOut = new MarbleView(this, move.From, team, start);

                    if (homeSquare.Equals(move.To))
                    {
                        animations.Enqueue(new MarbleView[1] { takeOut });
                        takeOut.MoveTo(homeSquareTarget.Value, time);
                    }
                    else
                    {
                        MarbleView moveAnim = new MarbleView(this, move.From, team, homeSquareTarget.Value);
                        animations.Enqueue(new MarbleView[2] { takeOut, moveAnim });
                        takeOut.MoveTo(homeSquareTarget.Value, time / 2);
                        moveAnim.MoveTo(target.Value, time / 2);
                    }
                }
            }
            else
            {
                Vector2? start = GetVector(move.From);
                Vector2? target = GetVector(move.To);
                if (start.HasValue && target.HasValue)
                {
                    MarbleView anim = new MarbleView(this, move.From, team, start.Value);
                    animations.Enqueue(new MarbleView[1] { anim });
                    anim.MoveTo(target.Value, time);
                }
            }
        }

        /// <summary>
        /// Starts the list of queued animations
        /// </summary>
        public void StartAnimations()
        {
            currentAnimIndex = 0;
            currentAnimation = animations.Peek()[0];
        }

        /// <summary>
        /// Gets the current move created by the player for a team
        /// </summary>
        /// <param name="team">Team</param>
        public Move GetMove(sbyte team)
        {
            return new Move(currentMove.ToArray(), team);
        }

        /// <summary>
        /// Sets the current start square as clicked
        /// </summary>
        /// <param name="view">Start Square</param>
        public void SetStart(SquareView view)
        {
            if (view.Team == currentPlayer.Team)
            {
                if (currentPlayer.IsHuman && currentRoll != null && IsAllowedToMoveMarbles())
                {
                    if (currentDrag == null)
                    {
                        Vector2 pos = view.GetPosition();
                        view.HasMarble = false;

                        currentDrag = new MarbleView(this, view, currentPlayer.Team, pos);
                        currentDrag.StartDrag();
                    }
                }
            }
        }

        /// <summary>
        /// Sets the current start square as clicked
        /// </summary>
        /// <param name="view">Start Square</param>
        public void SetStart(BoardSquareView view)
        {
            if (currentPlayer.IsHuman && currentRoll != null && IsAllowedToMoveMarbles())
            {
                Square sq = view.Square;
                if (!IsPartOfMove(sq))
                {
                    if (currentDrag == null && board.Get(sq) == currentPlayer.Team)
                    {
                        Vector2? pos = GetVector(sq);

                        if (pos.HasValue)
                        {
                            currentDrag = new MarbleView(this, sq, currentPlayer.Team, pos.Value);
                            currentDrag.StartDrag();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks if a vector is approaching zero
        /// </summary>
        /// <param name="vector">Jitter Vector</param>
        private bool IsNearlyZero(JVector vector)
        {
            return Math.Abs(vector.X) < 0.15f &&
                   Math.Abs(vector.Y) < 0.15 &&
                   Math.Abs(vector.Z) < 0.15;
        }

        /// <summary>
        /// Sets the state of the board ready to be rolled
        /// </summary>
        private void SetReadyForRoll()
        {
            die1 = -1;
            die2 = -1;
            isDiceRolling = false;
        }

        /// <summary>
        /// Sets the turn of the next player
        /// </summary>
        public void NextPlayer()
        {
            sbyte team = board.NextPlayer(currentPlayer.Team);
            SetPlayer(team);
        }

        /// <summary>
        /// Sets the current player to a team
        /// </summary>
        /// <param name="team">Team</param>
        public void SetPlayer(sbyte team)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].Team == team)
                {
                    currentPlayer = players[i];
                    break;
                }
            }
        }

        /// <summary>
        /// Event for when the dice are finished rolling
        /// </summary>
        private void OnRollFinished()
        {
            rolls.Add(currentRoll);

            rollOver = true;
            isDiceRolling = false;
            rollStartTime = null;

            if (!IsInvalidDoubles())
            {
                currentPlayer.StartThink(board, currentRoll);
            }
        }

        /// <summary>
        /// Randomizes the orientation of a die
        /// </summary>
        /// <param name="die">Die body</param>
        public void RandomizeDieOrientation(RigidBody die)
        {
            float xRot = -MathHelper.Pi + ((float)random.NextDouble() * MathHelper.TwoPi);
            float yRot = -MathHelper.Pi + ((float)random.NextDouble() * MathHelper.TwoPi);
            float zRot = -MathHelper.Pi + ((float)random.NextDouble() * MathHelper.TwoPi);
            die.Orientation = JMatrix.CreateRotationX(xRot) * JMatrix.CreateRotationY(yRot) * JMatrix.CreateRotationZ(zRot);
        }

        /// <summary>
        /// Checks if the roll is invalid
        /// </summary>
        /// <returns></returns>
        public bool IsInvalidDoubles()
        {
            if (currentRoll.IsDoubles())
            {
                doublesCount++;
                if (doublesCount >= 3)
                {
                    doublesCount = 0;
                    board.RemoveFrontMarble(currentPlayer.Team);
                    PutBackMarble(currentPlayer.Team);
                    NextPlayer();
                    SetReadyForRoll();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Processes the rolls doubles
        /// </summary>
        public void ProcessRoll()
        {
            if (currentRoll != null)
            {
                if (!currentRoll.IsDoubles())
                {
                    doublesCount = 0;
                    NextPlayer();
                }
            }
        }

        /// <summary>
        /// Returns a value as to whether the current player is allowed to move his marbles
        /// </summary>
        /// <returns></returns>
        public bool IsAllowedToMoveMarbles()
        {
            return !CanRoll() || currentRoll.IsDoubles();
        }

        /// <summary>
        /// Checks if the current player can roll
        /// </summary>
        public bool CanRoll()
        {
            return this.die1 == -1 && this.die2 == -1 && this.rollOver && !isDiceRolling && animationOver;
        }

        /// <summary>
        /// Rolls the dice
        /// </summary>
        public void RollDice(GameTime gameTime)
        {
            die1 = -1;
            die2 = -1;
            isDiceRolling = true;

            die1Body.IsStatic = false;
            die1Body.LinearVelocity = new JVector(0, -9.8f, 0);
            die1Body.Position = new JVector(-20, 0.3048f, 0);
            die1Body.AngularVelocity = new JVector((float)random.NextDouble() * 2, (float)random.NextDouble() * 2, (float)random.NextDouble() * 2);
            RandomizeDieOrientation(die1Body);

            die2Body.IsStatic = false;
            die2Body.LinearVelocity = new JVector(0, -9.8f, 0);
            die2Body.Position = new JVector(20, 0.3048f, 0);
            die2Body.AngularVelocity = new JVector((float)random.NextDouble() * 2, (float)random.NextDouble() * 2, (float)random.NextDouble() * 2);
            RandomizeDieOrientation(die2Body);
            rollStartTime = gameTime.TotalGameTime;
        }

        /// <summary>
        /// Stops the dice movement
        /// </summary>
        public void StopDice()
        {
            die1Body.LinearVelocity.MakeZero();
            die1Body.AngularVelocity.MakeZero();
            die1Body.IsStatic = true;

            die2Body.LinearVelocity.MakeZero();
            die2Body.AngularVelocity.MakeZero();
            die2Body.IsStatic = true;
        }

        /// <summary>
        /// Performs the move's internals to the board
        /// </summary>
        /// <param name="move">Move</param>
        /// <param name="team">Team</param>
        public void DoMoveInternals(PieceMove move, sbyte team)
        {
            int targetMarble = board.Get(move.To);
            if (targetMarble != -1)
            {
                PutBackMarble(targetMarble);
            }

            Square homeSquare = new Square(team, 0);
            if (move.IsTakingOutMarble && !move.To.Equals(homeSquare))
            {
                int homeSquareMarble = board.Get(homeSquare);
                if (homeSquareMarble != -1)
                {
                    PutBackMarble(homeSquareMarble);
                }
            }

            board.PerformMove(move, team);
        }

        /// <summary>
        /// Performs the move's internals to the board
        /// </summary>
        /// <param name="move">Move</param>
        /// <param name="team">Team</param>
        /// <param name="animIndex">Animation Index</param>
        /// <param name="animCount">Animation Count</param>
        public void DoMoveInternals(PieceMove move, sbyte team, int animIndex, int animCount)
        {
            if (animIndex == animCount - 1)
            {
                int targetMarble = board.Get(move.To);
                if (targetMarble != -1)
                {
                    PutBackMarble(targetMarble);
                }

                Square homeSquare = new Square(team, 0);
                if (move.IsTakingOutMarble && !move.To.Equals(homeSquare))
                {
                    int homeSquareMarble = board.Get(homeSquare);
                    if (homeSquareMarble != -1)
                    {
                        PutBackMarble(homeSquareMarble);
                    }
                }

                board.PerformMove(move, team);
            }
        }

        /// <summary>
        /// Does a move visually and internally
        /// </summary>
        /// <param name="move">Move to perform</param>
        public void DoMove(Move move, bool animate, bool performInternally)
        {
            if (animate)
            {
                QueuedMove = move;

                for (int i = 0; i < move.Pieces; i++)
                {
                    QueueAnimation(move[i], move.Team, 0.75f);
                }

                StartAnimations();
            }
            else
            {
                if (performInternally)
                {
                    for (int i = 0; i < move.Pieces; i++)
                    {
                        DoMoveInternals(move[i], move.Team);
                    }
                }
                else
                {
                    board.PerformMove(move);
                }
            }

            RemoveMove();
            SetReadyForRoll();
        }

        /// <summary>
        /// Checks if the square is part of the current move
        /// </summary>
        /// <param name="sq">Square</param>
        public bool IsPartOfMove(Square sq)
        {
            for (int k = 0; k < currentMove.Count; k++)
            {
                if (currentMove[k].From != null)
                {
                    if (currentMove[k].From.Equals(sq))
                    {
                        return true;
                    }
                }

                if (currentMove[k].To.Equals(sq))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Event handler for when a move is ready for a player
        /// </summary>
        /// <param name="move">Move</param>
        public void OnMoveFound(Move move)
        {
            if (move != null)
            {
                if (!currentPlayer.IsHuman)
                {
                    if (AnalysisView != null)
                    {
                        AnalysisView.Algorithm = (currentPlayer as ComputerPlayer).GetAlgorithm();
                    }

                    Variation[] comEval = ((ComputerPlayer)currentPlayer).LastEvaluation;
                    if (comEval != null && AnalysisView != null)
                    {
                        AnalysisView.Variations = comEval;
                    }
                }

                animationOver = false;
                moveHistory.Add(move);

                if (NotationView != null)
                {
                    NotationView.AddMove(move);
                }

                //Create variable because current player changes during internals
                bool isHuman = currentPlayer.IsHuman;

                //If is a human since we skipped the animation, tell the engine we're done
                if (isHuman)
                {
                    //Perform the internals, without animations
                    for (int j = 0; j < move.Pieces; j++)
                    {
                        DoMoveInternals(move[j], move.Team);
                    }

                    OnMoveAnimationDone();
                }

                DoMove(move, !isHuman, false);

                //Rest of function is continued in OnMoveAnimationDone() since DoMove is async
                //and if we call nextplayer too early it will interrupt the current animation
            }
            else
            {
                ProcessRoll();
                SetReadyForRoll();
            }
        }

        /// <summary>
        /// Updates the board's "squares"
        /// </summary>
        private void UpdateSquares()
        {
            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    if (squares[i][j] != null)
                    {
                        squares[i][j].Update();
                    }
                }
            }

            //Update start squares
            for (int t = 0; t < 4; t++)
            {
                for (int s = 0; s < 5; s++)
                {
                    starts[t][s].Update();
                }
            }
        }

        /// <summary>
        /// Updates a die
        /// </summary>
        /// <param name="die">Die physics body</param>
        /// <param name="dieValue">Reference to die value</param>
        private void UpdateDie(RigidBody die, ref int dieValue, bool nearlyZero)
        {
            if (!die.IsStatic && dieValue == -1 && isDiceRolling && (nearlyZero || IsNearlyZero(die.AngularVelocity)))
            {
                JMatrix m = die.Orientation;
                Matrix mMatrix = new Matrix(m.M11, m.M12, m.M13, 0, m.M21, m.M22, m.M23, 0, m.M31, m.M32, m.M33, 0, 0, 0, 0, 1);

                List<DiceFace> sides = new List<DiceFace>();
                sides.Add(new DiceFace(mMatrix.Left.Y, 1));
                sides.Add(new DiceFace(mMatrix.Right.Y, 6));
                sides.Add(new DiceFace(mMatrix.Up.Y, 4));
                sides.Add(new DiceFace(mMatrix.Down.Y, 3));
                sides.Add(new DiceFace(mMatrix.Forward.Y, 5));
                sides.Add(new DiceFace(mMatrix.Backward.Y, 2));
                sides.Sort(new Comparison<DiceFace>((f, f2) => -f.Length.CompareTo(f2.Length)));

                DiceFace top = sides[0];
                dieValue = top.Value;
            }
        }

        /// <summary>
        /// Updates the drag logic
        /// </summary>
        /// <param name="mState">Current Mouse State</param>
        private void UpdateDrag(MouseState mState)
        {
            if (currentPlayer.IsHuman && currentRoll != null)
            {
                //Update drag if not null
                if (currentDrag != null)
                {
                    currentDrag.DragTo(new Vector2(mState.X, mState.Y));
                }

                //Check if drag is done
                if (mState.LeftButton == ButtonState.Released)
                {
                    if (currentDrag != null)
                    {
                        Square target = currentDrag.StopDrag();
                        if (target != null)
                        {
                            PieceMove move = new PieceMove(currentDrag.From, target);
                            if (board.IsPossibleTreeMove(currentRoll, move, currentPlayer.Team))
                            {
                                currentMove.Add(move);

                                if (PlayMarbleSounds)
                                {
                                    PlayRandomPlaceSound();
                                }

                                if (move.IsTakingOutMarble)
                                {
                                    currentDrag.FromView.HasMarble = false;
                                }
                            }
                            else
                            {
                                if (currentDrag.FromView != null)
                                {
                                    currentDrag.FromView.HasMarble = true;
                                }
                            }
                        }
                        else
                        {
                            if (currentDrag.FromView != null)
                            {
                                currentDrag.FromView.HasMarble = true;
                            }
                        }

                        currentDrag = null;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the view of the board
        /// </summary>
        /// <param name="gameTime">Current GameTime</param>
        public void Update(GameTime gameTime)
        {
            //Update logic
            KeyboardState kState = Keyboard.GetState();
            MouseState mState = MouseHandle.GetState();

            //Updates squares
            UpdateSquares();

            if (Playing)
            {
                //Update current animation
                if (currentAnimation != null)
                {
                    currentAnimation.Update(gameTime);
                }

                if (kState.IsKeyDown(Keys.Q) && rollOver)
                {
                    for (int i = 0; i < currentMove.Count; i++)
                    {
                        if (currentMove[i].IsTakingOutMarble)
                        {
                            PutBackMarble(currentPlayer.Team);
                        }
                    }

                    RemoveMove();
                }

                if (!currentPlayer.IsThinking && CanRoll())
                {
                    if (!currentPlayer.IsHuman)
                    {
                        rollOver = false;
                        RollDice(gameTime);
                    }
                    else if (!isDiceRolling && kState.IsKeyDown(Keys.R)) //Check for user starting a roll
                    {
                        rollOver = false;
                        RollDice(gameTime);
                    }
                }

                //Check if dice are done rolling
                if (die1 != -1 && die2 != -1 && !rollOver)
                {
                    currentRoll = new DiceRoll(die1, die2);
                    OnRollFinished();
                }

                //Check if dice have rolled for too long
                if (rollStartTime.HasValue)
                {
                    TimeSpan timeElapsed = gameTime.TotalGameTime.Subtract(rollStartTime.Value);
                    double timeElapsedMs = timeElapsed.TotalMilliseconds;
                    if (timeElapsedMs >= 10000)
                    {
                        StopDice();
                        UpdateDie(die1Body, ref die1, true);
                        UpdateDie(die2Body, ref die2, true);

                        if (die1 != -1 && die2 != -1)
                        {
                            currentRoll = new DiceRoll(die1, die2);
                            OnRollFinished();
                        }
                    }
                }

                //If the player is a human and hasnt rolled yet, disallow any movement of marbles
                if (!CanRoll())
                {
                    //Update drag
                    UpdateDrag(mState);

                    ////Handle user submitting move
                    if (rollOver && kState.IsKeyDown(Keys.Enter) && currentPlayer.IsHuman)
                    {
                        if (!HasMove)
                        {
                            //Pressed enter without making a move
                            MoveCollection collection = board.GetMoves(currentRoll, currentPlayer.Team);

                            if (collection.Count == 0)
                            {
                                //End the player step
                                ProcessRoll();
                                SetReadyForRoll();
                                //
                            }
                        }
                        else
                        {
                            if (currentRoll == null)
                            {
                                RemoveMove();
                            }
                            else
                            {
                                Move move = GetMove(currentPlayer.Team);
                                if (board.IsPossibleMove(currentRoll, move, out move))
                                {
                                    currentPlayer.SetMove(move);
                                }
                                else
                                {
                                    for (int i = 0; i < currentMove.Count; i++)
                                    {
                                        if (currentMove[i].IsTakingOutMarble)
                                        {
                                            PutBackMarble(currentPlayer.Team);
                                        }
                                    }

                                    RemoveMove();
                                }
                            }
                        }
                    }
                }

                if (currentPlayer.HasMove)
                {
                    OnMoveFound(currentPlayer.GetMove());
                }

                //Update dice
                UpdateDie(die1Body, ref die1, IsNearlyZero(die1Body.LinearVelocity));
                UpdateDie(die2Body, ref die2, IsNearlyZero(die2Body.LinearVelocity));

                if (!Resizing)
                {
                    world.Step(1 / 30f, false);
                    //world.Step((float)gameTime.TotalGameTime.TotalSeconds, false, (float)gameTime.ElapsedGameTime.TotalSeconds, 1);
                }
            }
        }

        /// <summary>
        /// Gets the indent asset for a team
        /// </summary>
        /// <param name="team">Team</param>
        public string GetIndentAsset(int team)
        {
            switch (team)
            {
                case Board.YELLOW:
                    return "indentYellow";
                case Board.RED:
                    return "indentRed";
                case Board.GREEN:
                    return "indentGreen";
                case Board.BLUE:
                    return "indentBlue";
            }

            throw new ArgumentException();
        }

        /// <summary>
        /// Gets asset file for marble
        /// </summary>
        /// <param name="team">Marble Team</param>
        public string GetMarbleAsset(int team)
        {
            switch (team)
            {
                case Board.YELLOW:
                    return "yellowMarble";
                case Board.RED:
                    return "redMarble";
                case Board.GREEN:
                    return "greenMarble";
                case Board.BLUE:
                    return "blueMarble";
            }

            throw new ArgumentException();
        }

        /// <summary>
        /// Draws the board's tiles
        /// </summary>
        /// <param name="batch">SpriteBatch</param>
        /// <param name="textures">Texture Library</param>
        public void DrawBoard(SpriteBatch batch, TextureLib textures)
        {
            batch.Draw(textures["board"], new Rectangle(View.X, View.Y, View.Width + 20, View.Height + 20), Color.White);

            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    if (squares[i][j] != null)
                    {
                        squares[i][j].Draw(batch, textures);
                    }
                }
            }

            for (int t = 0; t < 4; t++)
            {
                for (int s = 0; s < 5; s++)
                {
                    starts[t][s].Draw(batch, textures);
                }
            }
        }

        /// <summary>
        /// Draws the marbles on the board
        /// </summary>
        /// <param name="batch">SpriteBatch</param>
        /// <param name="textures">Texture Library</param>
        public void DrawMarbles(SpriteBatch batch, TextureLib textures)
        {
            bool artificiallyDrawMarble = false;
            bool isPartOfMove = false;
            PieceMove partOfMove = null;
            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    if (layout[i][j] != null)
                    {
                        Square current = new Square(layout[i][j]);
                        if (currentAnimation != null)
                        {
                            if (current.Equals(currentAnimation.From))
                            {
                                continue;
                            }
                        }

                        if (currentDrag != null)
                        {
                            if (current.Equals(currentDrag.From))
                            {
                                continue;
                            }
                        }

                        artificiallyDrawMarble = false;
                        isPartOfMove = false;
                        for (int k = 0; k < currentMove.Count; k++)
                        {
                            if (currentMove[k].From != null)
                            {
                                if (currentMove[k].From.Equals(current))
                                {
                                    partOfMove = currentMove[k];
                                    artificiallyDrawMarble = false;
                                    isPartOfMove = true;
                                }
                            }

                            if (currentMove[k].To.Equals(current))
                            {
                                partOfMove = currentMove[k];
                                artificiallyDrawMarble = true;
                                isPartOfMove = true;
                                break;
                            }
                        }

                        if (isPartOfMove)
                        {
                            if (artificiallyDrawMarble)
                            {
                                string asset = GetMarbleAsset(currentPlayer.Team);
                                batch.Draw(textures[asset], squares[i][j].Rect, (squares[i][j].Selected) ? Color.LightBlue : Color.White);
                            }
                        }
                        else
                        {
                            int marble = board.Get(layout[i][j]);
                            if (marble != -1)
                            {
                                string asset = GetMarbleAsset(marble);
                                batch.Draw(textures[asset], squares[i][j].Rect, (squares[i][j].Selected) ? Color.LightBlue : Color.White);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draws the board
        /// </summary>
        /// <param name="batch">SpriteBatch</param>
        /// <param name="textures">Texture Library</param>
        public void Draw(SpriteBatch batch, GameContent content)
        {
            DrawBoard(batch, content.Textures);
            DrawMarbles(batch, content.Textures);

            for (int l = 0; l < guiLines.Count; l++)
            {
                guiLines[l].Draw(batch, content.Textures);
            }

            if (currentAnimation != null)
            {
                currentAnimation.Draw(batch, content.Textures);
            }

            if (currentDrag != null)
            {
                currentDrag.Draw(batch, content.Textures);
            }
        }

        /// <summary>
        /// Renders non spritebatch content
        /// </summary>
        /// <param name="device"></param>
        public void Render(GraphicsDevice device)
        {
            if (!die1Body.IsStatic)
            {
                die1Entity.Render(device, View.Width);
            }

            if (!die2Body.IsStatic)
            {
                die2Entity.Render(device, View.Width);
            }
        }

        /// <summary>
        /// Loads the board's content-
        /// </summary>
        /// <param name="device">GraphicsDevice</param>
        /// <param name="content">ContentManager</param>
        public void LoadContent(GraphicsDevice device, ContentManager content)
        {
            //Load sounds
            rollEffect = content.Load<SoundEffect>("roll1");

            placeEffects = new SoundEffect[3];
            //placeEffects[0] = content.Load<SoundEffect>("place1");
            //placeEffects[1] = content.Load<SoundEffect>("place2");
            //placeEffects[2] = content.Load<SoundEffect>("place3");

            Texture2D[] dieTextures = new Texture2D[6]
            {
                content.Load<Texture2D>("die1"),
                content.Load<Texture2D>("die6"),
                content.Load<Texture2D>("die3"),
                content.Load<Texture2D>("die4"),
                content.Load<Texture2D>("die5"),
                content.Load<Texture2D>("die2")
            };


            CollisionSystem sap = new CollisionSystemPersistentSAP();
            world = new World(sap);
            world.SetDampingFactors(1.0f, 1.0f);
            sap.CollisionDetected += OnCollisionDetected;
            world.Gravity = new JVector(0, -9.8f * 5, 0f);

            RigidBody board = new RigidBody(new BoxShape(new JVector(1000, 25, 1000)));
            board.Position = new JVector(0, -25, 0);
            board.IsStatic = true;
            world.AddBody(board);

            die1Body = new RigidBody(new BoxShape(new JVector(1.5875f)));
            die1Body.Mass = 5.8f;
            die1Body.IsStatic = true; //Temporary
            die1Body.Position = new JVector(-15, 60, 0);
            world.AddBody(die1Body);

            die2Body = new RigidBody(new BoxShape(new JVector(1.5875f)));
            die2Body.Mass = 5.8f;
            die2Body.IsStatic = true; //Temporary
            die2Body.Position = new JVector(15, 60, 0);
            world.AddBody(die2Body);

            die1Entity = new Prism(device, die1Body, dieTextures);
            die2Entity = new Prism(device, die2Body, dieTextures);
        }

        private void OnCollisionDetected(RigidBody body1, RigidBody body2, JVector point1, JVector point2, JVector normal, float penetration)
        {
            if (penetration >= 1)
            {
                if (!lastRollEffectPlay.HasValue || DateTime.Now.Subtract(lastRollEffectPlay.Value).TotalMilliseconds > 500)
                {
                    if (PlayDiceSounds)
                    {
                        rollEffect.Play(); //Maybe do something with JVectors to determine pitch and pan
                    }
                    lastRollEffectPlay = DateTime.Now;
                }
            }
        }

        /// <summary>
        /// Disposes all the view graphical objects
        /// </summary>
        public void Dispose()
        {
            Debug.WriteLine("Dispose();");
            for (int p = 0; p < 4; p++)
            {
                Debug.WriteLine("Player {0}", p);
                if (players[p] is ComputerPlayer)
                {
                    ComputerPlayer cpu = (ComputerPlayer)players[p];
                    cpu.StopThink();
                }
            }

            die1Entity.Dispose();
            die2Entity.Dispose();
        }

        /// <summary>
        /// Sets the current players
        /// </summary>
        /// <param name="players">Players</param>
        public void SetPlayers(Player[] players)
        {
            if (players.Length != 4)
            {
                throw new ArgumentException();
            }

            this.players = players;
            SetPlayer(Board.GREEN);
        }

        /// <summary>
        /// Rotates the board
        /// </summary>
        public void Rotate()
        {
            teamPerspective++;
            teamPerspective %= Board.TEAM_COUNT;

            this.layout = BoardLayouts.LAYOUTS[teamPerspective];
            Resize(View);
        }

        /// <summary>
        /// Resizes the board
        /// </summary>
        /// <param name="size">The size of the viewport</param>
        public void Resize(int size)
        {
            const int margin = 45;
            Rectangle boardRect = new Rectangle(margin, margin, size - (margin * 2) - 20, size - (margin * 2) - 20);
            this.Resize(boardRect);
        }

        /// <summary>
        /// Resizes the board
        /// </summary>
        private void Resize(Rectangle size)
        {
            this.View = size;
            const int startOffset = 20;

            int tileWidth = (int)Math.Round(View.Width / 29.166666666666666666666666666667);
            int tileHeight = (int)Math.Round(View.Height / 29.166666666666666666666666666667);

            int multiplierX = (int)Math.Round(View.Width / 12.962962962962962962962962962963);
            int multiplierY = (int)Math.Round(View.Height / 12.962962962962962962962962962963);

            for (int i = 0; i < 13; i++)
            {
                if (squares[i] == null)
                {
                    squares[i] = new BoardSquareView[13];
                }
                for (int j = 0; j < 13; j++)
                {
                    if (layout[i][j] != null)
                    {
                        Square square = new Square(layout[i][j]);
                        squares[i][j] = new BoardSquareView(this, square, new Rectangle(View.X + startOffset + j * multiplierX, View.Y + startOffset + i * multiplierY, tileWidth, tileHeight));
                    }
                }
            }

            int offset = startOffset / 2;
            int porportionX = (int)Math.Round(View.Width / 15.555555555555555555555555555556);
            int porportionY = (int)Math.Round(View.Height / 15.555555555555555555555555555556);

            for (sbyte t = 0; t < Board.TEAM_COUNT; t++)
            {
                if (starts[t] == null)
                {
                    starts[t] = new SquareView[Board.TEAM_SIZE];
                }
                for (int m = 0; m < Board.TEAM_SIZE; m++)
                {
                    Rectangle target = new Rectangle();
                    int orientation = t + (2 - teamPerspective);
                    if (orientation < 0)
                    {
                        orientation += 4;
                    }
                    else if (orientation >= 4)
                    {
                        orientation -= 4;
                    }

                    switch (orientation)
                    {
                        case 0: //UPPER LEFT
                            target = new Rectangle(View.X + offset + m * porportionX, View.Y + 10 + m * porportionY, tileWidth, tileHeight);
                            break;
                        case 1: //UPPER RIGHT
                            target = new Rectangle(View.X + View.Width - offset - (m * porportionX), View.Y + offset + m * porportionY, tileWidth, tileHeight);
                            break;
                        case 2: //BOTTOM RIGHT
                            target = new Rectangle(View.X + View.Width - offset - (m * porportionX), View.Y + View.Height - offset - (m * porportionY), tileWidth, tileHeight);
                            break;
                        case 3: //BOTTOM LEFT
                            target = new Rectangle(View.X + offset + m * porportionX, View.Y + View.Height - offset - (m * porportionY), tileWidth, tileHeight);
                            break;
                        default:
                            break;
                    }

                    if (starts[t][m] == null)
                    {
                        starts[t][m] = new SquareView(this, target, t);
                    }
                    else
                    {
                        starts[t][m].Rect = target;
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new view for a board
        /// </summary>
        /// <param name="board">Board to view</param>
        public BoardView(Board board, int size)
        {
            this.board = board;
            this.teamPerspective = Board.GREEN;
            this.layout = BoardLayouts.LAYOUTS[teamPerspective];
            this.squares = new BoardSquareView[13][];
            this.starts = new SquareView[Board.TEAM_COUNT][];

            this.animations = new Queue<MarbleView[]>();
            this.currentMove = new List<PieceMove>();
            this.guiLines = new List<Line>();
            this.rolls = new List<DiceRoll>();
            this.random = new Random();

            Resize(size);

            Playing = false;
            PlayDiceSounds = false;
            PlayMarbleSounds = false;
        }
    }
}