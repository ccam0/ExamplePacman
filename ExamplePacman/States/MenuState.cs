using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pacman.Managers;

namespace Pacman.States
{
    /// <summary>
    /// Menu state implementation using State pattern
    /// </summary>
    public class MenuState : IGameState
    {
        private readonly Game _game;
        private readonly GameStateManager _stateManager;
        private readonly ResourceManager _resourceManager;
        private readonly AudioManager _audioManager;

        private readonly Rectangle _logoRect = new Rectangle(13, 40, 4530 / 7, 1184 / 7);
        private readonly Vector2 _textPosition = new Vector2(150, 400);

        private KeyboardState _previousKeyState;

        public MenuState(Game game, GameStateManager stateManager, ResourceManager resourceManager, AudioManager audioManager)
        {
            _game = game;
            _stateManager = stateManager;
            _resourceManager = resourceManager;
            _audioManager = audioManager;
        }

        public void Enter()
        {
            _previousKeyState = Keyboard.GetState();
        }

        public void Exit()
        {
        }

        public void Update(GameTime gameTime)
        {
            var currentKeyState = Keyboard.GetState();

            // Check for Enter key press (only on key down, not held)
            if (currentKeyState.IsKeyDown(Keys.Enter) && _previousKeyState.IsKeyUp(Keys.Enter))
            {
                _audioManager.PlaySound(_audioManager.GameStart);
                _stateManager.StartGame();
            }

            _previousKeyState = currentKeyState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(_resourceManager.BasicFont, "PRESS ENTER TO PLAY", _textPosition, Color.Red);
            spriteBatch.Draw(_resourceManager.PacmanLogo, _logoRect, Color.White);
        }
    }
}
