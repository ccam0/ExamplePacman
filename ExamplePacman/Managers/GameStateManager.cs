using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pacman.States;

namespace Pacman.Managers
{
    /// <summary>
    /// Manages game state transitions using the State pattern
    /// Provides clean separation of state logic
    /// </summary>
    public class GameStateManager
    {
        private IGameState _currentState;
        private readonly Game _game;

        // State instances
        public MenuState MenuState { get; private set; }
        public PlayState PlayState { get; private set; }
        public GameOverState GameOverState { get; private set; }

        public GameStateManager(Game game)
        {
            _game = game;
        }

        public void Initialize(
            ResourceManager resourceManager,
            AudioManager audioManager,
            GridManager gridManager,
            EntityManager entityManager,
            CollisionManager collisionManager)
        {
            // Create state instances
            MenuState = new MenuState(_game, this, resourceManager, audioManager);
            PlayState = new PlayState(
                _game,
                this,
                resourceManager,
                audioManager,
                gridManager,
                entityManager,
                collisionManager
            );
            GameOverState = new GameOverState(_game, this, resourceManager, audioManager);

            // Start with menu state
            ChangeState(MenuState);
        }

        public void ChangeState(IGameState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState?.Enter();
        }

        public void Update(GameTime gameTime)
        {
            _currentState?.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _currentState?.Draw(spriteBatch);
        }

        public void StartGame()
        {
            ChangeState(PlayState);
        }

        public void ShowGameOver()
        {
            ChangeState(GameOverState);
        }

        public void ReturnToMenu()
        {
            ChangeState(MenuState);
        }
    }
}
