using Microsoft.Xna.Framework;
using Pacman.Core;
using Pacman.Entities;
using Pacman.Managers;
using System;
using System.Collections.Generic;

namespace Pacman.AI
{
    /// <summary>
    /// Frightened behavior - ghosts move randomly when player eats power pellet
    /// </summary>
    public class FrightenedBehavior : IGhostBehavior
    {
        private readonly Random _random = new Random();

        public Vector2 GetTargetTile(Ghost ghost, Player player, GridManager gridManager, Vector2 blinkyPosition)
        {
            // Handle teleport tiles
            if (ghost.CurrentTile == GameConstants.LeftTeleportTile ||
                ghost.CurrentTile == GameConstants.RightTeleportTile)
            {
                return ghost.CurrentTile;
            }

            // If in ghost house, exit first
            if (gridManager.CheckTileType(ghost.CurrentTile, Tile.TileType.GhostHouse))
            {
                return GameConstants.GhostHouseExitTile;
            }

            // Get available directions
            var availableDirections = GetAvailableDirections(ghost, gridManager);

            if (availableDirections.Count > 0)
            {
                // Pick random direction
                var randomDirection = availableDirections[_random.Next(availableDirections.Count)];
                return GetAdjacentTile(ghost.CurrentTile, randomDirection);
            }

            return ghost.CurrentTile;
        }

        private List<Direction> GetAvailableDirections(Ghost ghost, GridManager gridManager)
        {
            var directions = new List<Direction>();
            var oppositeDir = ghost.Direction.GetOpposite();

            // Try each direction except the opposite of current direction
            if (oppositeDir != Direction.Left && IsDirectionValid(ghost, Direction.Left, gridManager))
                directions.Add(Direction.Left);

            if (oppositeDir != Direction.Right && IsDirectionValid(ghost, Direction.Right, gridManager))
                directions.Add(Direction.Right);

            if (oppositeDir != Direction.Down && IsDirectionValid(ghost, Direction.Down, gridManager))
                directions.Add(Direction.Down);

            if (oppositeDir != Direction.Up && IsDirectionValid(ghost, Direction.Up, gridManager))
                directions.Add(Direction.Up);

            return directions;
        }

        private bool IsDirectionValid(Ghost ghost, Direction direction, GridManager gridManager)
        {
            var adjacentTile = GetAdjacentTile(ghost.CurrentTile, direction);

            if (!IsValidTile(adjacentTile))
                return false;

            return gridManager.IsTileWalkableForGhost(direction, ghost.CurrentTile) &&
                   !gridManager.CheckTileType(adjacentTile, Tile.TileType.GhostHouse);
        }

        private Vector2 GetAdjacentTile(Vector2 tile, Direction direction)
        {
            return direction switch
            {
                Direction.Left => new Vector2(tile.X - 1, tile.Y),
                Direction.Right => new Vector2(tile.X + 1, tile.Y),
                Direction.Down => new Vector2(tile.X, tile.Y + 1),
                Direction.Up => new Vector2(tile.X, tile.Y - 1),
                _ => tile
            };
        }

        private bool IsValidTile(Vector2 tile)
        {
            return tile.X >= 0 && tile.X < GameConstants.NumberOfTilesX &&
                   tile.Y >= 0 && tile.Y < GameConstants.NumberOfTilesY;
        }
    }
}
