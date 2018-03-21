using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MarbleBoardGame
{
    public class TextureLib
    {
        private ContentManager manager;
        private Dictionary<string, Texture2D> library;

        /// <summary>
        /// Gets a Texture asset by name
        /// </summary>
        /// <param name="asset">Asset name</param>
        public Texture2D this[string asset]
        {
            get { return library[asset]; }
        }

        /// <summary>
        /// Loads a texture into the library
        /// </summary>
        /// <param name="assetName">Asset name of texture</param>
        public void LoadTexture(string assetName)
        {
            Texture2D texture = manager.Load<Texture2D>(assetName);
            library.Add(assetName, texture);
        }

        /// <summary>
        /// Loads a texture into the library
        /// </summary>
        /// <param name="assetName">Asset name of texture</param>
        /// <param name="texture">Texture to add</param>
        public void LoadTexture(string assetName, Texture2D texture)
        {
            library.Add(assetName, texture);
        }

        /// <summary>
        /// Creates a texture library
        /// </summary>
        /// <param name="manager">Content Loader</param>
        public TextureLib(ContentManager manager)
        {
            this.manager = manager;
            this.library = new Dictionary<string, Texture2D>();
        }
    }
}
