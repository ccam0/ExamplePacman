using Microsoft.Xna.Framework;
using Pacman.Entities;
using Pacman.Managers;

namespace Pacman.AI
{
    /// <summary>
    /// Scatter behavior - ghosts retreat to their assigned corners
    /// </summary>
    public class ScatterBehavior : IGhostBehavior
    {
        public Vector2 GetTargetTile(Ghost ghost, Player player, GridManager gridManager, Vector2 blinkyPosition)
        {
            return ghost.ScatterTarget;
        }
    }
}
