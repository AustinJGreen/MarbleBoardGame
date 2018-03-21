using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace MarbleBoardGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Engine : Game, IInterface
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private FontFactory fontFactory;
        
        private GameContent content;

        private GameState lastGameState;
        private GameState currentGameState;
        private Dictionary<string, GameState> gameStates;

        public GameState GetGameState(string name)
        {
            if (gameStates.ContainsKey(name))
            {
                return gameStates[name];
            }

            return null;
        }

        public GameState GetPrevious()
        {
            return lastGameState;
        }

        public void SwitchTo(GameState gameState)
        {
            this.lastGameState = currentGameState;
            this.currentGameState = gameState;
        }

        public Engine() : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            gameStates = new Dictionary<string, GameState>();

            graphics.PreferredBackBufferWidth = 720;
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferMultiSampling = true;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            fontFactory = new FontFactory(GraphicsDevice);

            //Load fonts
            SmartFont[] fonts = new SmartFont[100];
            fonts[16] = fontFactory.LoadFont("Consolas", 16f).Load();
            //fonts[32] = fontFactory.LoadFont("Consolas", 32f).Load();
            //fonts[72] = fontFactory.LoadFont("LemonMilk", 72f).Load();

            TextureLib textures = new TextureLib(Content);

            //Load textures
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

            MainMenu mainMenu = new MainMenu(this);          
            gameStates.Add("mainmenu", mainMenu);

            Singleplayer singleplayer = new Singleplayer(this);          
            gameStates.Add("singleplayer", singleplayer);

            Gameplay gameplay = new Gameplay(this);
            gameStates.Add("gameplay", gameplay);

            mainMenu.Load(content);
            singleplayer.Load(content);
            gameplay.Load(content);

            currentGameState = mainMenu;          
        }

        protected override void UnloadContent()
        {
            content.Unload();

            using (IEnumerator<KeyValuePair<string, GameState>> list = gameStates.GetEnumerator())
            {
                while (list.MoveNext())
                {
                    list.Current.Value.Dispose();
                }
            }

            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            currentGameState.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            currentGameState.Draw(spriteBatch, content);         
            spriteBatch.End();

            currentGameState.Render(GraphicsDevice);
        }
    }
}
