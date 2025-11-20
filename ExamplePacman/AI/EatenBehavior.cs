using Microsoft.Xna.Framework;
using Pacman.Core;
using Pacman.Entities;
using Pacman.Managers;

namespace Pacman.AI
{
    /// <summary>
    /// Eaten behavior - ghost returns to ghost house after being eaten
    /// </summary>
    public class EatenBehavior : IGhostBehavior
    {
        public Vector2 GetTargetTile(Ghost ghost, Player player, GridManager gridManager, Vector2 blinkyPosition)
        {
            return GameConstants.GhostHouseTargetTile;
        }
    }
}
