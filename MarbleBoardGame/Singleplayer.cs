using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarbleBoardGame
{
    public class Singleplayer : GameState
    {
        private Sprite bg;
        private Sprite title;

        private Sprite yellowMarble;
        private Sprite redMarble;
        private Sprite greenMarble;
        private Sprite blueMarble;

        private Button play;
        private Button back;
        private ToggleButton toggleButtonP1;
        private ToggleButton toggleButtonP2;
        private ToggleButton toggleButtonP3;
        private ToggleButton toggleButtonP4;

        public GameState TargetScopeStartGame()
        {
            Gameplay gameplay = (Gameplay)engine.GetGameState("gameplay");
            Player[] players = new Player[4];
            BoardView view = gameplay.GetBoard();
            for (int  i = 0; i < players.Length; i++)
            {
                players[i] = GetPlayer(view, i);
            }
            
            gameplay.SetPlayers(players);
            view.Playing = true;
            return gameplay;
        }

        public Player GetPlayer(BoardView boardView, int playerNum)
        {
            switch (playerNum)
            {
                case 0:
                    return (toggleButtonP1.SelectionIndex == 0) ? (Player)new HumanPlayer(0, boardView) : (Player)new ComputerPlayer(0, Personality.Active, ComputerPlayer.Difficulties[9], boardView);
                case 1:
                    return (toggleButtonP2.SelectionIndex == 0) ? (Player)new HumanPlayer(1, boardView) : (Player)new ComputerPlayer(1, Personality.Aggressive, ComputerPlayer.Difficulties[0], boardView);
                case 2:
                    return (toggleButtonP3.SelectionIndex == 0) ? (Player)new HumanPlayer(2, boardView) : (Player)new ComputerPlayer(2, Personality.Balanced, ComputerPlayer.Difficulties[0], boardView);
                case 3:
                    return (toggleButtonP4.SelectionIndex == 0) ? (Player)new HumanPlayer(3, boardView) : (Player)new ComputerPlayer(3, Personality.Adaptive, ComputerPlayer.Difficulties[0], boardView);
            }

            return null;
        }

        public override void Load(GameContent content)
        {
            AddDrawable((bg = new Sprite(content.Textures["board"], engine.Window.ClientBounds)));
            AddDrawable((title = new Sprite(content.Textures["gamesetup"], new Vector2(10, 10))));

            AddDrawable((yellowMarble = new Sprite(content.Textures["yellowMarble"], new Rectangle(10, 100, 36, 36))));
            AddDrawable((redMarble = new Sprite(content.Textures["redMarble"], new Rectangle(10, 200, 36, 36))));
            AddDrawable((greenMarble = new Sprite(content.Textures["greenMarble"], new Rectangle(10, 300, 36, 36))));
            AddDrawable((blueMarble = new Sprite(content.Textures["blueMarble"], new Rectangle(10, 400, 36, 36))));

            AddObject((toggleButtonP1 = new ToggleButton(new Texture2D[] { content.Textures["human"], content.Textures["computer"] }, new Vector2(50, 100))));
            AddObject((toggleButtonP2 = new ToggleButton(new Texture2D[] { content.Textures["human"], content.Textures["computer"] }, new Vector2(50, 200))));
            AddObject((toggleButtonP3 = new ToggleButton(new Texture2D[] { content.Textures["human"], content.Textures["computer"] }, new Vector2(50, 300))));
            AddObject((toggleButtonP4 = new ToggleButton(new Texture2D[] { content.Textures["human"], content.Textures["computer"] }, new Vector2(50, 400))));

            AddObject((play = new Button(engine, content.Textures["play"], new Vector2(575, 665), ButtonAnimation.None, TargetScopeStartGame)));
            AddObject((back = new Button(engine, content.Textures["goback"], new Vector2(10, 665), ButtonAnimation.None, engine.GetPrevious)));
        }

        public Singleplayer(Engine engine) : base(engine) { }
    }
}
