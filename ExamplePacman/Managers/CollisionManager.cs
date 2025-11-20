using Microsoft.Xna.Framework;
using Pacman.Core;
using Pacman.Entities;
using System.Collections.Generic;

namespace Pacman.Managers
{
    /// <summary>
    /// Handles collision detection between game entities
    /// Separates collision logic from entity logic (SRP)
    /// </summary>
    public class CollisionManager
    {
        private readonly GridManager _gridManager;

        public CollisionManager(GridManager gridManager)
        {
            _gridManager = gridManager;
        }

        public bool CheckPlayerGhostCollision(Player player, List<Ghost> ghosts, out Ghost collidingGhost)
        {
            collidingGhost = null;

            foreach (var ghost in ghosts)
            {
                if (player.CurrentTile == ghost.CurrentTile)
                {
                    collidingGhost = ghost;
                    return true;
                }
            }

            return false;
        }

        public int CheckPlayerSnackCollision(Vector2 playerTile)
        {
            var tilePosition = _gridManager.TileArray[(int)playerTile.X, (int)playerTile.Y].Position;
            return _gridManager.FindSnackAtPosition(tilePosition);
        }

        public int CheckTeleportPosition(Vector2 currentTile, Vector2 position, Direction direction)
        {
            // Left teleport
            if (currentTile == GameConstants.LeftTeleportTile)
            {
                if (position.X < -GameConstants.TeleportThreshold)
                    return 1; // Teleport to right
            }
            // Right teleport
            else if (currentTile == GameConstants.RightTeleportTile)
            {
                var rightEdge = _gridManager.TileArray[(int)currentTile.X, (int)currentTile.Y].Position.X;
                if (position.X > rightEdge + GameConstants.TeleportThreshold)
                    return 2; // Teleport to left
            }

            return 0; // No teleport
        }

        public Vector2 CalculateTeleportDestination(int teleportType)
        {
            return teleportType switch
            {
                1 => new Vector2(GameConstants.WindowWidth + GameConstants.TeleportThreshold,
                                 GameConstants.RightTeleportTile.Y * GameConstants.TileHeight + GameConstants.ScoreOffset),
                2 => new Vector2(-GameConstants.TeleportThreshold,
                                 GameConstants.LeftTeleportTile.Y * GameConstants.TileHeight + GameConstants.ScoreOffset),
                _ => Vector2.Zero
            };
        }

        public Vector2 CalculateTeleportTile(int teleportType)
        {
            return teleportType switch
            {
                1 => GameConstants.RightTeleportTile,
                2 => GameConstants.LeftTeleportTile,
                _ => Vector2.Zero
            };
        }
    }
}
