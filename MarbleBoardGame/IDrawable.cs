using Microsoft.Xna.Framework.Graphics;

namespace MarbleBoardGame
{
    public interface IDrawable
    {
        void Draw(SpriteBatch batch, GameContent content);
    }
}
