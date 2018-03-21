using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarbleBoardGame
{
    public interface IInterface
    {
        /// <summary>
        /// Gets a game state
        /// </summary>
        GameState GetGameState(string name);

        /// <summary>
        /// Gets the previous game state
        /// </summary>
        GameState GetPrevious();

        /// <summary>
        /// Sets the current game state
        /// </summary>
        void SwitchTo(GameState gameState);
    }
}
