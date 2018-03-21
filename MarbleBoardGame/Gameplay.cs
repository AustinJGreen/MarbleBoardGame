using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarbleBoardGame
{
    public class Gameplay : GameState
    {
        private Sprite bg;
        private Sprite bgMarble;

        private Board board;
        private BoardView boardView;

        public override void Load(GameContent content)
        {
            const int margin = 45;
            Rectangle boardRect = new Rectangle(margin, margin, engine.Window.ClientBounds.Width - (margin * 2) - 20, engine.Window.ClientBounds.Height - (margin * 2) - 20);

            AddDrawable((bgMarble = new Sprite(content.Textures["marbleTable"], engine.Window.ClientBounds)));
            AddDrawable((bg = new Sprite(content.Textures["board"], boardRect)));

            board = new Board();
            boardView = new BoardView(board, boardRect);
            boardView.LoadContent(engine.GraphicsDevice, engine.Content);

            AddObject(boardView);
            AddRenderable(boardView);
            
        }

        public override void Dispose()
        {
            boardView.Dispose();
        }

        public void SetPlayers(Player[] players)
        {
            boardView.SetPlayers(players);
        }

        public BoardView GetBoard()
        {
            return boardView;
        }

        public Gameplay(Engine engine) : base(engine) { }
    }
}
