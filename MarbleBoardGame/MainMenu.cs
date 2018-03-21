using Microsoft.Xna.Framework;

namespace MarbleBoardGame
{
    public class MainMenu : GameState
    {
        private Sprite bg;
        private Sprite titleLabel;

        private Button playBtn;
        private Button multiplayerBtn;
        private Button howtoplayBtn;
        private Button optionsBtn;
        private Button aboutBtn;

        public override void Load(GameContent content)
        {
            AddDrawable((bg = new Sprite(content.Textures["board"], engine.Window.ClientBounds)));
            AddDrawable((titleLabel = new Sprite(content.Textures["title"], new Vector2(10, 20))));

            AddObject((playBtn = new Button(engine, content.Textures["play"], new Vector2(10, 155), ButtonAnimation.None, engine.GetGameState("singleplayer"))));
            AddObject((multiplayerBtn = new Button(engine, content.Textures["multiplayer"], new Vector2(10, 225), ButtonAnimation.None)));
            AddObject((howtoplayBtn = new Button(engine, content.Textures["howtoplay"], new Vector2(10, 295), ButtonAnimation.None)));
            AddObject((optionsBtn = new Button(engine, content.Textures["options"], new Vector2(10, 365), ButtonAnimation.None)));
            AddObject((aboutBtn = new Button(engine, content.Textures["about"], new Vector2(10, 435), ButtonAnimation.None)));
        }

        public MainMenu(Engine engine) : base(engine) { }
    }
}
