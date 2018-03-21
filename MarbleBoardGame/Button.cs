using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MarbleBoardGame
{
    public class Button : IObject
    {
        /// <summary>
        /// Gets or Sets the sprite for the button to render
        /// </summary>
        public Texture2D Sprite { get; set;  }

        /// <summary>
        /// Gets or Sets the game interface
        /// </summary>
        public IInterface Interface { get; set; }

        /// <summary>
        /// Target game state when clicked
        /// </summary>
        public GameState Target { get; set; }

        /// <summary>
        /// Gets the target scope of the target game state
        /// </summary>
        public TargetScope TargetScope { get; set; }

        /// <summary>
        /// Gets or Sets the type of button animation
        /// </summary>
        public ButtonAnimation AnimationType { get; set; }

        /// <summary>
        /// Gets or Sets the position of the button
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// The initial position of the button
        /// </summary>
        public Vector2 InitialPosition { get; set; }

        /// <summary>
        /// Is the mouse over the button
        /// </summary>
        public bool IsMouseOver { get; set; }

        /// <summary>
        /// Is the mouse pressed on the button while over it
        /// </summary>
        public bool IsMouseDown { get; set; }

        /// <summary>
        /// Is the animation currently activating
        /// </summary>
        public bool AnimatingForward { get; set; }

        /// <summary>
        /// Is the animation currently deactivating
        /// </summary>
        public bool AnimatingBackward { get; set; }

        /// <summary>
        /// Gets whether the button is animating
        /// </summary>
        public bool Animating {  get { return AnimatingForward && AnimatingBackward; } }

        /// <summary>
        /// Target animation time per part
        /// </summary>
        public double AnimationTime { get; set; }

        /// <summary>
        /// Current elapsed time of the animation elapsed
        /// </summary>
        public double ElapsedAnimationTime { get; set; }

        /// <summary>
        /// Checks if the position is roughly at a position
        /// </summary>
        public bool IsAtPosition(Vector2 pos)
        {
            return Vector2.Distance(pos, Position) <= 0.1f;
        }

        public void Draw(SpriteBatch batch, GameContent content)
        {
            if (!IsMouseOver)
            {
                batch.Draw(Sprite, Position, Color.White);
            }
            else
            {

                batch.Draw(Sprite, Position, Sprite.Bounds, Color.White, 0, Vector2.Zero, 1.15f, SpriteEffects.None, 1);
            }
        }

        public void Update(GameTime gameTime)
        {
            MouseState state = Mouse.GetState();

            if (state.X >= Position.X && state.X <= Position.X + Sprite.Width &&
                state.Y >= Position.Y && state.Y <= Position.Y + Sprite.Height)
            {
                if (!Animating && IsAtPosition(InitialPosition))
                {
                    AnimatingForward = true;
                }

                if (IsMouseDown && state.LeftButton == ButtonState.Released)
                {
                    if (Target != null)
                    {
                        Interface.SwitchTo(Target);
                    }
                    else if (TargetScope != null)
                    {
                        GameState target = TargetScope.Invoke();
                        if (target != null)
                        {
                            Interface.SwitchTo(target);
                        }
                    }
                }

                IsMouseDown = state.LeftButton == ButtonState.Pressed;

                IsMouseOver = true;
            }
            else
            {
                if (!Animating && !IsAtPosition(InitialPosition))
                {
                    AnimatingBackward = true;
                }

                IsMouseOver = false;
            }

            if (AnimatingForward)
            {
                Vector2 target = Position;
                switch (AnimationType)
                {
                    case ButtonAnimation.GoRight:
                        target = new Vector2(InitialPosition.X + 75, InitialPosition.Y);
                        Position = Vector2.Lerp(InitialPosition, target, (float)(ElapsedAnimationTime / AnimationTime));
                        break;
                }

                ElapsedAnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
                if (IsAtPosition(target))
                {
                    ElapsedAnimationTime = 0;
                    AnimatingForward = false;
                }
            }
            else if (AnimatingBackward)
            {
                Vector2 target = Position; 
                switch (AnimationType)
                {
                    case ButtonAnimation.GoRight:
                        target = InitialPosition;
                        Position = Vector2.Lerp(Position, target, (float)(ElapsedAnimationTime / AnimationTime));
                        break;
                }

                ElapsedAnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
                if (IsAtPosition(target))
                {
                    ElapsedAnimationTime = 0;
                    AnimatingBackward = false;
                }
            }
        }

        public Button(IInterface _interface, Texture2D sprite, Vector2 position, ButtonAnimation animationType)
        {
            Interface = _interface;
            Sprite = sprite;
            Position = position;
            InitialPosition = position;
            AnimationType = animationType;
            AnimationTime = 0.25;
        }

        public Button(IInterface _interface, Texture2D sprite, Vector2 position, ButtonAnimation animationType, GameState target)
            : this(_interface, sprite, position, animationType)
        {
            Target = target;
        }

        public Button(IInterface _interface, Texture2D sprite, Vector2 position, ButtonAnimation animationType, TargetScope targetScope)
            : this(_interface, sprite, position, animationType)
        {
            TargetScope = targetScope;
        }
    }
}
