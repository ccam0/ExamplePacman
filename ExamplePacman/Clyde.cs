using Microsoft.Xna.Framework;

namespace Pacman
{
    /// <summary>
    /// Clyde (orange ghost) - Chases Pac-Man when far, scatters when close.
    /// </summary>
    public class Clyde : Ghost
    {
        public Clyde(int tileX, int tileY, Tile[,] tileArray) : base(tileX, tileY, tileArray)
        {
            ScatterTargetTile = new Vector2(2, 29);
            Type = GhostType.Clyde;

            RectsDown[0] = new Rectangle(1659, 339, 42, 42);
            RectsDown[1] = new Rectangle(1707, 339, 42, 42);

            RectsUp[0] = new Rectangle(1563, 339, 42, 42);
            RectsUp[1] = new Rectangle(1611, 339, 42, 42);

            RectsLeft[0] = new Rectangle(1467, 339, 42, 42);
            RectsLeft[1] = new Rectangle(1515, 339, 42, 42);

            RectsRight[0] = new Rectangle(1371, 339, 42, 42);
            RectsRight[1] = new Rectangle(1419, 339, 42, 42);
        }

        public override Vector2 GetChaseTarget(Vector2 playerTilePos, Direction playerDirection, Tile[,] tileArray)
        {
            // If far from Pac-Man (>8 tiles), chase directly
            // If close, scatter to corner
            if (Tile.GetDistanceBetweenTiles(playerTilePos, CurrentTile) > 8)
            {
                return playerTilePos;
            }
            return ScatterTargetTile;
        }
    }
}
