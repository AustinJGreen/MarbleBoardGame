using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Content;
using WinFormsGraphicsDevice;
using System.Windows.Forms;

namespace MarbleBoardGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MarbleBoardControl : GraphicsDeviceControl
    {
        private FontFactory fontFactory;
        private GameServiceContainer services;
        private ContentManager contentManager;
        private SpriteBatch spriteBatch;    
        private GameContent content;

        private Board board;
        private BoardView boardView;

        public AnalysisPane AnalysisView { get; set; }
        public NotationPane NotationPane { get; set; }

        public BoardView GetBoard()
        {
            return boardView;
        }

        public int Size { get; set; }

        public ContentManager Content { get { return contentManager; } }

        public GraphicsDevice Device { get { return GraphicsDevice; } }

        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            fontFactory = new FontFactory(GraphicsDevice);

            contentManager = new ContentManager(Services);
            contentManager.RootDirectory = "Content";

            Size = ClientSize.Width;
            LoadContent();
        }

        public void Resize()
        {
            if (boardView != null)
            {
                boardView.Resize(Size);
            }
        }

        public void ResizeBegin()
        {
            if (boardView != null)
            {
                boardView.Resizing = true;
            }
        }

        public void ResizeEnd()
        {
            if (boardView != null)
            {
                boardView.Resizing = false;
            }
        }

        protected void LoadContent()
        {    
            //Load fonts
            SmartFont[] fonts = new SmartFont[100];
            fonts[16] = fontFactory.LoadFont("Consolas", 16f).Load();
            //fonts[32] = fontFactory.LoadFont("Consolas", 32f).Load();
            //fonts[72] = fontFactory.LoadFont("LemonMilk", 72f).Load();

            TextureLib textures = new TextureLib(contentManager);

            //Load textures
            textures.LoadTexture("pixel");
            textures.LoadTexture("board");
            textures.LoadTexture("boardBevel");
            textures.LoadTexture("marbleTable");

            textures.LoadTexture("title");
            textures.LoadTexture("play");
            textures.LoadTexture("singleplayer");
            textures.LoadTexture("singleplayerBig");
            textures.LoadTexture("multiplayer");
            textures.LoadTexture("howtoplay");
            textures.LoadTexture("options");
            textures.LoadTexture("about");
            textures.LoadTexture("goback");
            textures.LoadTexture("gamesetup");
            textures.LoadTexture("human");
            textures.LoadTexture("computer");

            textures.LoadTexture("indentBlue");
            textures.LoadTexture("indentBrown");
            textures.LoadTexture("indentGreen");
            textures.LoadTexture("indentRed");
            textures.LoadTexture("indentYellow");

            textures.LoadTexture("blueMarble");
            textures.LoadTexture("greenMarble");
            textures.LoadTexture("redMarble");
            textures.LoadTexture("yellowMarble");

            content = new GameContent(textures, fonts);

            board = new Board();
            boardView = new BoardView(board, Size);
            boardView.AnalysisView = this.AnalysisView;
            boardView.NotationView = this.NotationPane;

            boardView.LoadContent(Device, Content);


            Player[] players = new Player[4];
            players[0] = new ComputerPlayer(0, Personality.Adaptive, ComputerPlayer.Difficulties[9], boardView);
            players[1] = new ComputerPlayer(1, Personality.Aggressive, ComputerPlayer.Difficulties[9], boardView);
            players[2] = new ComputerPlayer(2, Personality.Active, ComputerPlayer.Difficulties[9], boardView);
            players[3] = new ComputerPlayer(3, Personality.Passive, ComputerPlayer.Difficulties[9], boardView);

            boardView.SetPlayers(players);
            boardView.Playing = true;    
        }

        protected override void Dispose(bool disposing)
        {
            content.Unload();
            boardView.Dispose();

            base.Dispose(disposing);      
        }

        protected override void Update(GameTime gameTime)
        {
            boardView.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.Draw(content.Textures["pixel"], new Rectangle(0, 0, Width, Height), Color.LightSlateGray);
            spriteBatch.Draw(content.Textures["marbleTable"], new Rectangle(0, 0, Size, Size), Color.White);
            boardView.Draw(spriteBatch, content);
            spriteBatch.End();

            if (!gameTime.IsRunningSlowly) //only do 3d rendering if affordable
            {
                boardView.Render(GraphicsDevice);
            }
        }
    }
}
