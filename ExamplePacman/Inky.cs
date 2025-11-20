using Microsoft.Xna.Framework;

namespace Pacman
{
    /// <summary>
    /// Inky (cyan ghost) - Uses complex targeting based on Blinky's position.
    /// </summary>
    public class Inky : Ghost
    {
        public Inky(int tileX, int tileY, Tile[,] tileArray) : base(tileX, tileY, tileArray)
        {
            ScatterTargetTile = new Vector2(25, 29);
            Type = GhostType.Inky;

            RectsDown[0] = new Rectangle(1659, 291, 42, 42);
            RectsDown[1] = new Rectangle(1707, 291, 42, 42);

            RectsUp[0] = new Rectangle(1563, 291, 42, 42);
            RectsUp[1] = new Rectangle(1611, 291, 42, 42);

            RectsLeft[0] = new Rectangle(1467, 291, 42, 42);
            RectsLeft[1] = new Rectangle(1515, 291, 42, 42);

            RectsRight[0] = new Rectangle(1371, 291, 42, 42);
            RectsRight[1] = new Rectangle(1419, 291, 42, 42);
        }

        public override Vector2 GetChaseTarget(Vector2 playerTilePos, Direction playerDirection, Tile[,] tileArray, Vector2 blinkyPos)
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

            // Calculate 2 tiles ahead of Pac-Man
            Vector2 ahead = playerDirection switch
            {
                Direction.Down => new Vector2(0, 2),
                Direction.Up => new Vector2(0, -2),
                Direction.Left => new Vector2(-2, 0),
                Direction.Right => new Vector2(2, 0),
                _ => Vector2.Zero
            };

            // Calculate vector from Blinky to ahead position
            Vector2 distance = new Vector2(
                Math.Abs(playerTilePos.X - blinkyPos.X),
                Math.Abs(playerTilePos.Y - blinkyPos.Y)
            );

            // Double the vector and add to Inky's position
            Vector2 finalTarget = CurrentTile + (distance * 2) + ahead;

            // Validate bounds
            if (finalTarget.X < 0 || finalTarget.Y < 0 ||
                finalTarget.X > Maze.NumberOfTilesX - 1 || finalTarget.Y > Maze.NumberOfTilesY - 1)
            {
                return playerTilePos;
            }

            // Check if target is a wall
            if (tileArray[(int)finalTarget.X, (int)finalTarget.Y].tileType == Tile.TileType.Wall)
            {
                return playerTilePos;
            }

            return finalTarget;
        }
    }
}
