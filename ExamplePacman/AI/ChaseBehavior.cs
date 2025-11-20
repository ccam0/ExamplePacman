using Microsoft.Xna.Framework;
using Pacman.Core;
using Pacman.Entities;
using Pacman.Managers;

namespace Pacman.AI
{
    /// <summary>
    /// Chase behavior - ghosts actively pursue the player
    /// Each ghost type has a unique chase strategy
    /// </summary>
    public class ChaseBehavior : IGhostBehavior
    {
        public Vector2 GetTargetTile(Ghost ghost, Player player, GridManager gridManager, Vector2 blinkyPosition)
        {
            return ghost.CalculateChaseTarget(player, gridManager, blinkyPosition);
        }
    }
}
