using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pacman.Managers;

namespace Pacman.States
{
    /// <summary>
    /// Game Over state implementation using State pattern
    /// </summary>
    public class GameOverState : IGameState
    {
        private readonly Game _game;
        private readonly GameStateManager _stateManager;
        private readonly ResourceManager _resourceManager;
        private readonly AudioManager _audioManager;

        private readonly Vector2 _textPosition = new Vector2(93, 400);
        private readonly Vector2 _gameOverPosition = new Vector2(100, 321);

        private KeyboardState _previousKeyState;
        private Text _textRenderer;

        public GameOverState(Game game, GameStateManager stateManager, ResourceManager resourceManager, AudioManager audioManager)
        {
            _game = game;
            _stateManager = stateManager;
            _resourceManager = resourceManager;
            _audioManager = audioManager;
        }

        public void Enter()
        {
            _previousKeyState = Keyboard.GetState();
            _textRenderer = new Text(new SpriteSheet(_resourceManager.TextSprites));
        }

        public void Exit()
        {
        }

        public void Update(GameTime gameTime)
        {
            var currentKeyState = Keyboard.GetState();

            // Check for Space key press (only on key down, not held)
            if (currentKeyState.IsKeyDown(Keys.Space) && _previousKeyState.IsKeyUp(Keys.Space))
            {
                _stateManager.ReturnToMenu();
            }

            _previousKeyState = currentKeyState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _textRenderer.draw(spriteBatch, "game over!", _gameOverPosition, 48, Text.Color.Red, 2f);
            spriteBatch.DrawString(_resourceManager.BasicFont, "PRESS SPACE TO GO TO MENU", _textPosition, Color.Red);
        }
    }
}
