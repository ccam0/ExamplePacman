using Microsoft.Xna.Framework;
using Pacman.Core;
using Pacman.Entities;
using Pacman.Managers;

namespace Pacman.AI
{
    /// <summary>
    /// Strategy pattern interface for ghost AI behaviors
    /// Allows different ghost states to have different targeting strategies
    /// </summary>
    public interface IGhostBehavior
    {
        Vector2 GetTargetTile(Ghost ghost, Player player, GridManager gridManager, Vector2 blinkyPosition);
    }
}
