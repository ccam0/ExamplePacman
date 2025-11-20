using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pacman.Components;
using Pacman.Core;

namespace Pacman.Entities
{
    /// <summary>
    /// Base class for all movable entities (Player and Ghosts)
    /// Implements Template Method pattern for common movement behavior
    /// Eliminates code duplication between Player and Enemy
    /// </summary>
    public abstract class MovableEntity : IGameEntity
    {
        protected readonly Transform _transform;
        protected readonly Animation _animation;
        protected readonly Texture2D _spriteSheet;

        protected int _speed;
        protected Tile[,] _tileArray;

        public Vector2 Position
        {
            get => _transform.Position;
            set => _transform.Position = value;
        }

        public Vector2 CurrentTile => _transform.CurrentTile;
        public Vector2 PreviousTile => _transform.PreviousTile;
        public Direction Direction => _transform.Direction;

        protected MovableEntity(Vector2 startTile, Tile[,] tileArray, Texture2D spriteSheet, int speed)
        {
            _tileArray = tileArray;
            _spriteSheet = spriteSheet;
            _speed = speed;

            var tilePosition = tileArray[(int)startTile.X, (int)startTile.Y].Position;
            _transform = new Transform(tilePosition, startTile);
            _animation = CreateAnimation();
        }

        protected abstract Animation CreateAnimation();

        public virtual void Update(GameTime gameTime)
        {
            _animation.Update(gameTime);
            UpdateTilePosition();
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            DrawEntity(spriteBatch);
        }

        protected abstract void DrawEntity(SpriteBatch spriteBatch);

        protected void UpdateTilePosition()
        {
            // Handle teleportation
            var teleportType = CheckForTeleport();
            if (teleportType > 0)
            {
                HandleTeleport(teleportType);
                return;
            }

            // Update tile position based on current position
            for (int x = 0; x < GameConstants.NumberOfTilesX; x++)
            {
                for (int y = 0; y < GameConstants.NumberOfTilesY; y++)
                {
                    if (CheckIfInTile(x, y))
                    {
                        _transform.UpdateTile(new Vector2(x, y));
                        return;
                    }
                }
            }
        }

        protected virtual bool CheckIfInTile(int x, int y)
        {
            var tilePosX = _tileArray[x, y].Position.X;
            var tilePosY = _tileArray[x, y].Position.Y;

            var nextTilePosX = tilePosX + GameConstants.TileWidth;
            var nextTilePosY = tilePosY + GameConstants.TileHeight;

            float posOffsetX = GetPositionOffsetX();
            float posOffsetY = GetPositionOffsetY();

            switch (_transform.Direction)
            {
                case Direction.Right:
                    nextTilePosX = tilePosX + GameConstants.TileWidth;
                    break;
                case Direction.Left:
                    nextTilePosX = tilePosX - GameConstants.TileWidth;
                    posOffsetX *= -1;
                    break;
                case Direction.Down:
                    nextTilePosY = tilePosY + GameConstants.TileHeight;
                    break;
                case Direction.Up:
                    nextTilePosY = tilePosY - GameConstants.TileHeight;
                    posOffsetY *= -1;
                    break;
            }

            float entityPosX = _transform.Position.X + posOffsetX;
            float entityPosY = _transform.Position.Y + posOffsetY;

            // Check if entity is within tile bounds based on direction
            if (_transform.Direction == Direction.Right || _transform.Direction == Direction.Down)
            {
                return entityPosX >= tilePosX && entityPosX < nextTilePosX &&
                       entityPosY >= tilePosY && entityPosY < nextTilePosY;
            }
            else if (_transform.Direction == Direction.Left)
            {
                return entityPosX <= tilePosX && entityPosX > nextTilePosX &&
                       entityPosY >= tilePosY && entityPosY < nextTilePosY;
            }
            else if (_transform.Direction == Direction.Up)
            {
                return entityPosX >= tilePosX && entityPosX < nextTilePosX &&
                       entityPosY <= tilePosY && entityPosY > nextTilePosY;
            }

            return false;
        }

        protected abstract float GetPositionOffsetX();
        protected abstract float GetPositionOffsetY();

        protected int CheckForTeleport()
        {
            // Left teleport
            if (_transform.CurrentTile == GameConstants.LeftTeleportTile)
            {
                if (_transform.Position.X < -GameConstants.TeleportThreshold)
                    return 1;
            }
            // Right teleport
            else if (_transform.CurrentTile == GameConstants.RightTeleportTile)
            {
                var rightEdge = _tileArray[(int)_transform.CurrentTile.X, (int)_transform.CurrentTile.Y].Position.X;
                if (_transform.Position.X > rightEdge + GameConstants.TeleportThreshold)
                    return 2;
            }

            return 0;
        }

        protected void HandleTeleport(int teleportType)
        {
            if (teleportType == 1 && _transform.Direction == Direction.Left)
            {
                var newPos = new Vector2(
                    GameConstants.WindowWidth + GameConstants.TeleportThreshold,
                    _transform.Position.Y
                );
                _transform.Teleport(newPos, GameConstants.RightTeleportTile);
            }
            else if (teleportType == 2 && _transform.Direction == Direction.Right)
            {
                var newPos = new Vector2(
                    -GameConstants.TeleportThreshold,
                    _transform.Position.Y
                );
                _transform.Teleport(newPos, GameConstants.LeftTeleportTile);
            }
        }

        protected void Move(Direction direction, float deltaTime)
        {
            switch (direction)
            {
                case Direction.Right:
                    _transform.Position += new Vector2(_speed * deltaTime, 0);
                    break;
                case Direction.Left:
                    _transform.Position -= new Vector2(_speed * deltaTime, 0);
                    break;
                case Direction.Down:
                    _transform.Position += new Vector2(0, _speed * deltaTime);
                    break;
                case Direction.Up:
                    _transform.Position -= new Vector2(0, _speed * deltaTime);
                    break;
            }
        }

        public virtual void Reset(Vector2 position, Vector2 tile)
        {
            _transform.Position = position;
            _transform.CurrentTile = tile;
            _transform.PreviousTile = tile;
            _transform.Direction = Direction.None;
        }

        public virtual void Reset(Vector2 position)
        {
            _transform.Position = position;
        }
    }
}
