using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pacman.Core;
using Pacman.Entities;
using Pacman.Managers;

namespace Pacman
{
    /// <summary>
    /// Main game class - Refactored with proper OOP architecture
    /// Uses Dependency Injection, State Pattern, and Manager pattern
    /// All static fields eliminated for better testability
    /// </summary>
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Managers - Following Single Responsibility Principle
        private ResourceManager _resourceManager;
        private AudioManager _audioManager;
        private GridManager _gridManager;
        private EntityManager _entityManager;
        private CollisionManager _collisionManager;
        private GameStateManager _stateManager;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Set window size
            _graphics.PreferredBackBufferWidth = GameConstants.WindowWidth;
            _graphics.PreferredBackBufferHeight = GameConstants.WindowHeight;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Initialize managers
            _resourceManager = new ResourceManager();
            _audioManager = new AudioManager();
            _gridManager = new GridManager();
            _entityManager = new EntityManager(_gridManager, _audioManager);
            _collisionManager = new CollisionManager(_gridManager);
            _stateManager = new GameStateManager(this);

            // Load content through managers
            _resourceManager.LoadContent(Content);
            _audioManager.LoadContent(Content);

            // Set up resource access for entities (temporary workaround)
            Player.ResourceManagerInstance = _resourceManager;

            // Initialize state manager with all dependencies
            _stateManager.Initialize(
                _resourceManager,
                _audioManager,
                _gridManager,
                _entityManager,
                _collisionManager
            );
        }

        protected override void Update(GameTime gameTime)
        {
            // Handle exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // Update current game state
            _stateManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            // Draw current game state
            _stateManager.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
            _audioManager?.StopAll();
            base.UnloadContent();
        }
    }
}
