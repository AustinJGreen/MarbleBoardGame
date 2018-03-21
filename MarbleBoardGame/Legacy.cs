using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleBoardGame
{
    public static class Legacy
    {
        public static Texture2D LoadTexture(GraphicsDevice device, string assetName)
        {
            string curPath = Environment.CurrentDirectory;
            using (FileStream fs = File.OpenRead(Path.Combine(curPath, "Content", string.Concat(assetName, ".png"))))
            {
                Texture2D texture = Texture2D.FromStream(device, fs);
                return texture;
            }
        }
    }
}
