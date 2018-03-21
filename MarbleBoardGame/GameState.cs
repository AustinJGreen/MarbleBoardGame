using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MarbleBoardGame
{
    public abstract class GameState
    {
        private List<IDrawable> drawableObjects;
        private List<IRenderable> renderableObjects;
        private List<IUpdatable> updatableObjects;
        protected Engine engine;

        public virtual void Draw(SpriteBatch batch, GameContent content)
        {
            for (int i = 0; i < drawableObjects.Count; i++)
            {
                drawableObjects[i].Draw(batch, content);
            }
        }

        public virtual void Render(GraphicsDevice device)
        {
            for (int  i = 0; i < renderableObjects.Count; i++)
            {
                renderableObjects[i].Render(device);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            for (int i = 0; i < updatableObjects.Count; i++)
            {
                updatableObjects[i].Update(gameTime);
            }
        }

        public abstract void Load(GameContent content);

        public virtual void Dispose()
        {

        }

        protected void AddObject(IObject gameObject)
        {
            AddDrawable(gameObject);
            AddUpdatable(gameObject);
        }

        protected void AddDrawable(IDrawable drawable)
        {
            drawableObjects.Add(drawable);
        }

        protected void AddRenderable(IRenderable renderable)
        {
            renderableObjects.Add(renderable);
        }

        protected void AddUpdatable(IUpdatable updatable)
        {
            updatableObjects.Add(updatable);
        }

        protected GameState(Engine engine)
        {
            this.engine = engine;
            drawableObjects = new List<IDrawable>();
            renderableObjects = new List<IRenderable>();
            updatableObjects = new List<IUpdatable>();
        }
    }
}
