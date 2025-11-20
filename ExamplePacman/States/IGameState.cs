using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pacman.States
{
    /// <summary>
    /// Interface for implementing the State pattern
    /// Each game state (Menu, Playing, GameOver) implements this
    /// </summary>
    public interface IGameState
    {
        void Enter();
        void Exit();
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
    }
}
