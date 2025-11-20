using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pacman.Core
{
    /// <summary>
    /// Interface for all game entities that can be updated and drawn
    /// </summary>
    public interface IGameEntity
    {
        Vector2 Position { get; set; }
        Vector2 CurrentTile { get; }

        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
    }
}
