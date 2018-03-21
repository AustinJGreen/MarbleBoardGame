using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarbleBoardGame
{
    public struct GameContent
    {
        public TextureLib Textures { get; set; }

        public SmartFont[] Fonts { get; set; }

        public void Unload()
        {
            for (int i = 0; i < Fonts.Length; i++)
            {
                if (Fonts[i] != null)
                {
                    Fonts[i].Unload();
                }
            }
        }

        public GameContent(TextureLib textures, SmartFont[] fonts) :this()
        {
            Textures = textures;
            Fonts = fonts;
        }
    }
}
