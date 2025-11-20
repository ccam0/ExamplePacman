using Microsoft.Xna.Framework;

namespace Pacman
{
    /// <summary>
    /// Blinky (red ghost) - Chases Pac-Man directly.
    /// </summary>
    public class Blinky : Ghost
    {
        public Blinky(int tileX, int tileY, Tile[,] tileArray) : base(tileX, tileY, tileArray)
        {
            ScatterTargetTile = new Vector2(26, 2);
            Type = GhostType.Blinky;

            RectsDown[0] = new Rectangle(1659, 195, 42, 42);
            RectsDown[1] = new Rectangle(1707, 195, 42, 42);

            RectsUp[0] = new Rectangle(1563, 195, 42, 42);
            RectsUp[1] = new Rectangle(1611, 195, 42, 42);

            RectsLeft[0] = new Rectangle(1467, 195, 42, 42);
            RectsLeft[1] = new Rectangle(1515, 195, 42, 42);

            RectsRight[0] = new Rectangle(1371, 195, 42, 42);
            RectsRight[1] = new Rectangle(1419, 195, 42, 42);

            animation = new SpriteAnimation(0.08f, RectsLeft);
        }
    }
}
