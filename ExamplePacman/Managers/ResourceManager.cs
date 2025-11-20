using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Pacman.Managers
{
    /// <summary>
    /// Centralized resource management following the Service Locator pattern
    /// Eliminates static texture references and improves testability
    /// </summary>
    public class ResourceManager
    {
        private readonly Dictionary<string, Texture2D> _textures;
        private readonly Dictionary<string, SpriteFont> _fonts;

        public Texture2D GeneralSprites1 { get; private set; }
        public Texture2D GeneralSprites2 { get; private set; }
        public Texture2D TextSprites { get; private set; }
        public Texture2D PacmanLogo { get; private set; }
        public SpriteFont BasicFont { get; private set; }

        // Debug textures
        public Texture2D DebuggingDot { get; private set; }
        public Texture2D DebugLineX { get; private set; }
        public Texture2D DebugLineY { get; private set; }
        public Texture2D PlayerDebugLineX { get; private set; }
        public Texture2D PlayerDebugLineY { get; private set; }
        public Texture2D PathfindingDebugLineX { get; private set; }
        public Texture2D PathfindingDebugLineY { get; private set; }

        public ResourceManager()
        {
            _textures = new Dictionary<string, Texture2D>();
            _fonts = new Dictionary<string, SpriteFont>();
        }

        public void LoadContent(ContentManager content)
        {
            // Load sprite sheets
            GeneralSprites1 = LoadTexture(content, "SpriteSheets/GeneralSprites1");
            GeneralSprites2 = LoadTexture(content, "SpriteSheets/GeneralSprites2");
            TextSprites = LoadTexture(content, "Spritesheets/TextSprites");
            PacmanLogo = LoadTexture(content, "SpriteSheets/pac-man-logo");

            // Load debug textures
            DebuggingDot = LoadTexture(content, "SpriteSheets/debuggingDot");
            DebugLineX = LoadTexture(content, "SpriteSheets/debugLineX");
            DebugLineY = LoadTexture(content, "SpriteSheets/debugLineY");
            PlayerDebugLineX = LoadTexture(content, "SpriteSheets/playerDebugLineX");
            PlayerDebugLineY = LoadTexture(content, "SpriteSheets/playerDebugLineY");
            PathfindingDebugLineX = LoadTexture(content, "SpriteSheets/pathfindingDebugLineX");
            PathfindingDebugLineY = LoadTexture(content, "SpriteSheets/pathfindingDebugLineY");

            // Load fonts
            BasicFont = LoadFont(content, "simpleFont");
        }

        private Texture2D LoadTexture(ContentManager content, string assetName)
        {
            var texture = content.Load<Texture2D>(assetName);
            _textures[assetName] = texture;
            return texture;
        }

        private SpriteFont LoadFont(ContentManager content, string assetName)
        {
            var font = content.Load<SpriteFont>(assetName);
            _fonts[assetName] = font;
            return font;
        }

        public Texture2D GetTexture(string name)
        {
            return _textures.TryGetValue(name, out var texture) ? texture : null;
        }

        public SpriteFont GetFont(string name)
        {
            return _fonts.TryGetValue(name, out var font) ? font : null;
        }
    }
}
