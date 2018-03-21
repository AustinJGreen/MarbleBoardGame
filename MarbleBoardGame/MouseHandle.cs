using Microsoft.Xna.Framework.Input;

namespace MarbleBoardGame
{
    public class MouseHandle
    {
        private static MouseState mousestate;

        public static MouseState GetState()
        {
            return mousestate;
        }

        public static void SetState(MouseState state)
        {
            mousestate = state;
        }
    }
}
