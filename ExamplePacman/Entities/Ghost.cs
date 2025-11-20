using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pacman.AI;
using Pacman.Components;
using Pacman.Core;
using Pacman.Managers;
using System.Collections.Generic;

namespace Pacman.Entities
{
    /// <summary>
    /// Base Ghost class using Template Method and Strategy patterns
    /// Each ghost type overrides CalculateChaseTarget for unique behavior
    /// </summary>
    public abstract class Ghost : MovableEntity
    {
        protected readonly Dictionary<GhostState, IGhostBehavior> _behaviors;
        protected List<Vector2> _pathToTarget;
        protected Vector2 _lastPathTile;

        public GhostState State { get; protected set; }
        public Vector2 ScatterTarget { get; protected set; }
        public float FrightenedTimer { get; protected set; }
        public bool HasCollided { get; set; }

        protected Ghost(Vector2 startTile, Tile[,] tileArray, Vector2 scatterTarget)
            : base(startTile, tileArray, null, GameConstants.GhostNormalSpeed)
        {
            ScatterTarget = scatterTarget;
            State = GhostState.Scatter;
            _pathToTarget = new List<Vector2>();
            _lastPathTile = new Vector2(-1, -1);

            // Initialize behaviors (Strategy pattern)
            _behaviors = new Dictionary<GhostState, IGhostBehavior>
            {
                { GhostState.Scatter, new ScatterBehavior() },
                { GhostState.Chase, new ChaseBehavior() },
                { GhostState.Frightened, new FrightenedBehavior() },
                { GhostState.Eaten, new EatenBehavior() }
            };

            // Adjust position
            var offset = new Vector2(GameConstants.GhostPositionOffsetX, 0);
            _transform.Position += offset;
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update frightened timer
            if (State == GhostState.Frightened)
            {
                FrightenedTimer += deltaTime;
                if (FrightenedTimer >= GameConstants.GhostFrightenedTimerLength)
                {
                    SetState(GhostState.Chase);
                }
            }

            // Update animation
            _animation.Update(gameTime);
            UpdateTilePosition();
        }

        public void Update(GameTime gameTime, GridManager gridManager, Player player, Vector2 blinkyPosition)
        {
            // Check if reached ghost house (for eaten ghosts)
            if (State == GhostState.Eaten && _transform.CurrentTile == GameConstants.GhostHouseTargetTile)
            {
                SetState(GhostState.Chase);
            }

            // Check collision with player
            if (player.CurrentTile == _transform.CurrentTile)
            {
                if (State == GhostState.Frightened)
                {
                    GetEaten();
                }
                else if (State != GhostState.Eaten)
                {
                    HasCollided = true;
                }
            }

            // Update pathfinding
            if (_lastPathTile != _transform.CurrentTile)
            {
                var behavior = _behaviors[State];
                var targetTile = behavior.GetTargetTile(this, player, gridManager, blinkyPosition);
                _pathToTarget = Pathfinding.findPath(_transform.CurrentTile, targetTile, gridManager.TileArray, ConvertToOldDirection(_transform.Direction));
                _lastPathTile = _transform.CurrentTile;
            }

            // Determine direction from path
            if (_pathToTarget.Count > 0)
            {
                DetermineDirectionFromPath(gridManager);
            }

            // Move
            Move(_transform.Direction, (float)gameTime.ElapsedGameTime.TotalSeconds);
            UpdateAnimationDirection();

            base.Update(gameTime);
        }

        private void DetermineDirectionFromPath(GridManager gridManager)
        {
            if (_pathToTarget.Count == 0) return;

            var nextTile = _pathToTarget[0];
            var currentTile = _transform.CurrentTile;

            if (nextTile.X > currentTile.X)
            {
                _transform.Direction = Direction.Right;
                AlignToTile(Direction.Right, gridManager);
            }
            else if (nextTile.X < currentTile.X)
            {
                _transform.Direction = Direction.Left;
                AlignToTile(Direction.Left, gridManager);
            }
            else if (nextTile.Y > currentTile.Y)
            {
                _transform.Direction = Direction.Down;
                AlignToTile(Direction.Down, gridManager);
            }
            else if (nextTile.Y < currentTile.Y)
            {
                _transform.Direction = Direction.Up;
                AlignToTile(Direction.Up, gridManager);
            }
        }

        private void AlignToTile(Direction direction, GridManager gridManager)
        {
            var tilePos = gridManager.TileArray[(int)_transform.CurrentTile.X, (int)_transform.CurrentTile.Y].Position;

            if (direction.IsHorizontal())
                _transform.Position = new Vector2(_transform.Position.X, tilePos.Y);
            else
                _transform.Position = new Vector2(tilePos.X, _transform.Position.Y);
        }

        protected abstract Rectangle[] GetUpFrames();
        protected abstract Rectangle[] GetDownFrames();
        protected abstract Rectangle[] GetLeftFrames();
        protected abstract Rectangle[] GetRightFrames();

        private void UpdateAnimationDirection()
        {
            if (State == GhostState.Frightened)
            {
                var frames = FrightenedTimer > GameConstants.GhostFrightenedWarningTime
                    ? SpriteData.GhostFrightenedEnd
                    : SpriteData.GhostFrightened;
                _animation.SetFrames(frames);
            }
            else if (State != GhostState.Eaten)
            {
                _animation.SetFrames(_transform.Direction switch
                {
                    Direction.Right => GetRightFrames(),
                    Direction.Left => GetLeftFrames(),
                    Direction.Up => GetUpFrames(),
                    Direction.Down => GetDownFrames(),
                    _ => GetUpFrames()
                });
            }
        }

        protected override void DrawEntity(SpriteBatch spriteBatch)
        {
            var drawPos = _transform.Position + new Vector2(GameConstants.GhostDrawOffsetX, GameConstants.GhostDrawOffsetY);

            if (State == GhostState.Eaten)
            {
                var eyesRect = _transform.Direction switch
                {
                    Direction.Up => SpriteData.GhostEatenUp,
                    Direction.Down => SpriteData.GhostEatenDown,
                    Direction.Left => SpriteData.GhostEatenLeft,
                    Direction.Right => SpriteData.GhostEatenRight,
                    _ => SpriteData.GhostEatenRight
                };
                spriteBatch.Draw(Player.ResourceManagerInstance.GeneralSprites1, drawPos, eyesRect, Color.White);
            }
            else
            {
                _animation.Draw(spriteBatch, Player.ResourceManagerInstance.GeneralSprites1, drawPos);
            }
        }

        public virtual void SetState(GhostState newState)
        {
            State = newState;
            FrightenedTimer = 0;

            _speed = newState switch
            {
                GhostState.Frightened => GameConstants.GhostFrightenedSpeed,
                GhostState.Eaten => GameConstants.GhostEatenSpeed,
                _ => GameConstants.GhostNormalSpeed
            };

            _pathToTarget.Clear();
        }

        protected virtual void GetEaten()
        {
            State = GhostState.Eaten;
            _speed = GameConstants.GhostEatenSpeed;
            FrightenedTimer = 0;
        }

        public abstract Vector2 CalculateChaseTarget(Player player, GridManager gridManager, Vector2 blinkyPosition);

        protected override float GetPositionOffsetX() => 10;
        protected override float GetPositionOffsetY() => 10;

        public void UpdateAnimation(GameTime gameTime)
        {
            _animation.Update(gameTime);
        }

        public override void Reset(Vector2 position, Vector2 tile)
        {
            base.Reset(position, tile);
            State = GhostState.Scatter;
            FrightenedTimer = 0;
            _speed = GameConstants.GhostNormalSpeed;
            _pathToTarget.Clear();
            _lastPathTile = new Vector2(-1, -1);
            HasCollided = false;
        }

        // Helper to convert to old Dir enum for pathfinding
        private static Dir ConvertToOldDirection(Direction direction)
        {
            return direction switch
            {
                Direction.Up => Dir.Up,
                Direction.Down => Dir.Down,
                Direction.Left => Dir.Left,
                Direction.Right => Dir.Right,
                _ => Dir.None
            };
        }
    }
}
