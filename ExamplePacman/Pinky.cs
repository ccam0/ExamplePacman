using Microsoft.Xna.Framework;

namespace Pacman
{
    /// <summary>
    /// Pinky (pink ghost) - Targets 4 tiles ahead of Pac-Man.
    /// </summary>
    public class Pinky : Ghost
    {
        public Pinky(int tileX, int tileY, Tile[,] tileArray) : base(tileX, tileY, tileArray)
        {
            ScatterTargetTile = new Vector2(1, 2);
            Type = GhostType.Pinky;

            RectsDown[0] = new Rectangle(1659, 243, 42, 42);
            RectsDown[1] = new Rectangle(1707, 243, 42, 42);

            RectsUp[0] = new Rectangle(1563, 243, 42, 42);
            RectsUp[1] = new Rectangle(1611, 243, 42, 42);

            RectsLeft[0] = new Rectangle(1467, 243, 42, 42);
            RectsLeft[1] = new Rectangle(1515, 243, 42, 42);

            RectsRight[0] = new Rectangle(1371, 243, 42, 42);
            RectsRight[1] = new Rectangle(1419, 243, 42, 42);

            animation = new SpriteAnimation(0.08f, RectsDown);
        }

        public override Vector2 GetChaseTarget(Vector2 playerTilePos, Direction playerDirection, Tile[,] tileArray)
        {
            // Remember last direction if player is not moving
            if (playerDirection == Direction.None)
            {
                playerDirection = playerLastDirection;
            }
            else
            {
                playerLastDirection = playerDirection;
            }

            // Target 4 tiles ahead of Pac-Man
            Vector2 targetPos = playerDirection switch
            {
                Direction.Right => new Vector2(playerTilePos.X + 4, playerTilePos.Y),
                Direction.Left => new Vector2(playerTilePos.X - 4, playerTilePos.Y),
                Direction.Down => new Vector2(playerTilePos.X, playerTilePos.Y + 4),
                Direction.Up => new Vector2(playerTilePos.X, playerTilePos.Y - 4),
                _ => playerTilePos
            };

            // Validate target is within bounds
            if (targetPos.X < 0 || targetPos.Y < 0 ||
                targetPos.X > Maze.NumberOfTilesX - 1 || targetPos.Y > Maze.NumberOfTilesY - 1)
            {
                return playerTilePos;
            }

            // Check if target is a wall
            if (tileArray[(int)targetPos.X, (int)targetPos.Y].tileType == Tile.TileType.Wall)
            {
                return playerTilePos;
            }

            return targetPos;
        }
    }
}
