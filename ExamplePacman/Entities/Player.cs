using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pacman.Components;
using Pacman.Core;
using Pacman.Managers;

namespace Pacman.Entities
{
    /// <summary>
    /// Player entity with improved input handling and movement
    /// Inherits from MovableEntity to eliminate code duplication
    /// </summary>
    public class Player : MovableEntity
    {
        private readonly AudioManager _audioManager;
        private readonly GridManager _gridManager;

        private Direction _nextDirection;
        private float _moveTimer;
        private bool _canMove;

        public int Lives { get; set; }
        public Animation DeathAnimation { get; private set; }

        public Player(Vector2 startTile, Tile[,] tileArray, AudioManager audioManager)
            : base(startTile, tileArray, null, GameConstants.PlayerSpeed)
        {
            _audioManager = audioManager;
            Lives = GameConstants.InitialLives;
            _canMove = true;
            _nextDirection = Direction.None;

            DeathAnimation = new Animation(
                GameConstants.DeathAnimationDuration,
                SpriteData.PlayerDeath,
                isLooped: false,
                startPlaying: false
            );

            // Adjust position
            var offset = new Vector2(GameConstants.PlayerPositionOffsetX, 0);
            _transform.Position += offset;
            _transform.Direction = Direction.Right;
        }

        protected override Animation CreateAnimation()
        {
            return new Animation(
                GameConstants.PlayerAnimationSpeed,
                SpriteData.PlayerRight,
                startFrame: 2
            );
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update death animation if playing
            if (DeathAnimation.IsPlaying)
            {
                DeathAnimation.Update(gameTime);
                return;
            }

            // Handle movement cooldown
            if (!_canMove)
            {
                _moveTimer += deltaTime;
                if (_moveTimer >= GameConstants.PlayerMoveThreshold)
                {
                    _canMove = true;
                    _moveTimer = 0;
                }
            }

            // Handle input
            HandleInput();

            // Try to change direction if requested
            if (_canMove && _nextDirection != Direction.None)
            {
                var gridManager = new GridManager { TileArray = _tileArray };
                if (gridManager.IsTileWalkable(_nextDirection, _transform.CurrentTile))
                {
                    _transform.Direction = _nextDirection;
                    AlignToGrid(_nextDirection);
                    _nextDirection = Direction.None;
                    _canMove = false;
                }
            }

            // Check if current direction is still walkable
            if (_transform.Direction != Direction.None)
            {
                var gridManager = new GridManager { TileArray = _tileArray };
                if (!gridManager.IsTileWalkable(_transform.Direction, _transform.CurrentTile))
                {
                    _transform.Direction = Direction.None;
                    _audioManager.MunchInstance.Stop();
                }
            }

            // Move in current direction
            if (_transform.Direction != Direction.None)
            {
                Move(_transform.Direction, deltaTime);
                UpdateAnimationDirection();
            }
            else
            {
                // Stop at tile center when not moving
                var tilePos = _tileArray[(int)_transform.CurrentTile.X, (int)_transform.CurrentTile.Y].Position;
                _transform.Position = tilePos + new Vector2(2, 1);
                _audioManager.MunchInstance.Stop();
            }

            base.Update(gameTime);
        }

        private void HandleInput()
        {
            var keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.D) || keyState.IsKeyDown(Keys.Right))
                _nextDirection = Direction.Right;
            else if (keyState.IsKeyDown(Keys.A) || keyState.IsKeyDown(Keys.Left))
                _nextDirection = Direction.Left;
            else if (keyState.IsKeyDown(Keys.W) || keyState.IsKeyDown(Keys.Up))
                _nextDirection = Direction.Up;
            else if (keyState.IsKeyDown(Keys.S) || keyState.IsKeyDown(Keys.Down))
                _nextDirection = Direction.Down;
        }

        private void AlignToGrid(Direction direction)
        {
            var tilePos = _tileArray[(int)_transform.CurrentTile.X, (int)_transform.CurrentTile.Y].Position;

            if (direction.IsHorizontal())
                _transform.Position = new Vector2(_transform.Position.X, tilePos.Y + 1);
            else
                _transform.Position = new Vector2(tilePos.X + 2, _transform.Position.Y);
        }

        private void UpdateAnimationDirection()
        {
            _animation.SetFrames(_transform.Direction switch
            {
                Direction.Right => SpriteData.PlayerRight,
                Direction.Left => SpriteData.PlayerLeft,
                Direction.Up => SpriteData.PlayerUp,
                Direction.Down => SpriteData.PlayerDown,
                _ => SpriteData.PlayerRight
            });
        }

        protected override void DrawEntity(SpriteBatch spriteBatch)
        {
            if (!DeathAnimation.IsPlaying)
            {
                var drawPos = _transform.Position - new Vector2(GameConstants.PlayerRadiusOffset / 2f);
                _animation.Draw(spriteBatch, ResourceManagerInstance.GeneralSprites1, drawPos);
            }
        }

        public void DrawDeathAnimation(SpriteBatch spriteBatch, Vector2 position)
        {
            if (DeathAnimation.IsPlaying)
            {
                DeathAnimation.Draw(spriteBatch, ResourceManagerInstance.GeneralSprites1, position);
            }
        }

        public void Die(Vector2 deathPosition)
        {
            Lives--;
            DeathAnimation.Start();
        }

        protected override float GetPositionOffsetX() => GameConstants.PlayerRadiusOffset / 2f;
        protected override float GetPositionOffsetY() => GameConstants.PlayerRadiusOffset / 2f;

        public override void Reset(Vector2 position)
        {
            base.Reset(position);
            _transform.Direction = Direction.Right;
            _nextDirection = Direction.None;
            _canMove = true;
            _moveTimer = 0;
            _animation.SetFrames(SpriteData.PlayerRight);
            _animation.SetFrame(2);
            DeathAnimation.Stop();
        }

        // Temporary workaround for resource access
        private static ResourceManager _resourceManagerInstance;
        public static ResourceManager ResourceManagerInstance
        {
            get => _resourceManagerInstance;
            set => _resourceManagerInstance = value;
        }
    }
}
