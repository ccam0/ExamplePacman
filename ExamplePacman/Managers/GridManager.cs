using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pacman.Core;
using System.Collections.Generic;

namespace Pacman.Managers
{
    /// <summary>
    /// Manages the game grid, tile system, and snacks
    /// Follows Single Responsibility Principle
    /// </summary>
    public class GridManager
    {
        private readonly int[,] _mapDesign = new int[,]{
            { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,1},
            { 1,0,1,1,1,1,0,1,1,1,1,1,0,1,1,0,1,1,1,1,1,0,1,1,1,1,0,1},
            { 1,3,1,1,1,1,0,1,1,1,1,1,0,1,1,0,1,1,1,1,1,0,1,1,1,1,3,1},
            { 1,0,1,1,1,1,0,1,1,1,1,1,0,1,1,0,1,1,1,1,1,0,1,1,1,1,0,1},
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            { 1,0,1,1,1,1,0,1,1,0,1,1,1,1,1,1,1,1,0,1,1,0,1,1,1,1,0,1},
            { 1,0,1,1,1,1,0,1,1,0,1,1,1,1,1,1,1,1,0,1,1,0,1,1,1,1,0,1},
            { 1,0,0,0,0,0,0,1,1,0,0,0,0,1,1,0,0,0,0,1,1,0,0,0,0,0,0,1},
            { 1,1,1,1,1,1,0,1,1,1,1,1,5,1,1,5,1,1,1,1,1,0,1,1,1,1,1,1},
            { 1,1,1,1,1,1,0,1,1,1,1,1,5,1,1,5,1,1,1,1,1,0,1,1,1,1,1,1},
            { 1,1,1,1,1,1,0,1,1,5,5,5,5,5,5,5,5,5,5,1,1,0,1,1,1,1,1,1},
            { 1,1,1,1,1,1,0,1,1,5,1,1,1,2,2,1,1,1,5,1,1,0,1,1,1,1,1,1},
            { 1,1,1,1,1,1,0,1,1,5,1,2,2,2,2,2,2,1,5,1,1,0,1,1,1,1,1,1},
            { 0,0,0,0,0,0,0,5,5,5,1,2,2,2,2,2,2,1,5,5,5,0,0,0,0,0,0,0},
            { 1,1,1,1,1,1,0,1,1,5,1,2,2,2,2,2,2,1,5,1,1,0,1,1,1,1,1,1},
            { 1,1,1,1,1,1,0,1,1,5,1,1,1,1,1,1,1,1,5,1,1,0,1,1,1,1,1,1},
            { 1,1,1,1,1,1,0,1,1,5,5,5,5,5,5,5,5,5,5,1,1,0,1,1,1,1,1,1},
            { 1,1,1,1,1,1,0,1,1,5,1,1,1,1,1,1,1,1,5,1,1,0,1,1,1,1,1,1},
            { 1,1,1,1,1,1,0,1,1,5,1,1,1,1,1,1,1,1,5,1,1,0,1,1,1,1,1,1},
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,1},
            { 1,0,1,1,1,1,0,1,1,1,1,1,0,1,1,0,1,1,1,1,1,0,1,1,1,1,0,1},
            { 1,0,1,1,1,1,0,1,1,1,1,1,0,1,1,0,1,1,1,1,1,0,1,1,1,1,0,1},
            { 1,3,0,0,1,1,0,0,0,0,0,0,0,5,5,0,0,0,0,0,0,0,1,1,0,0,3,1},
            { 1,1,1,0,1,1,0,1,1,0,1,1,1,1,1,1,1,1,0,1,1,0,1,1,0,1,1,1},
            { 1,1,1,0,1,1,0,1,1,0,1,1,1,1,1,1,1,1,0,1,1,0,1,1,0,1,1,1},
            { 1,0,0,0,0,0,0,1,1,0,0,0,0,1,1,0,0,0,0,1,1,0,0,0,0,0,0,1},
            { 1,0,1,1,1,1,1,1,1,1,1,1,0,1,1,0,1,1,1,1,1,1,1,1,1,1,0,1},
            { 1,0,1,1,1,1,1,1,1,1,1,1,0,1,1,0,1,1,1,1,1,1,1,1,1,1,0,1},
            { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
        };

        public Tile[,] TileArray { get; private set; }
        public List<Snack> Snacks { get; private set; }

        public GridManager()
        {
            TileArray = new Tile[GameConstants.NumberOfTilesX, GameConstants.NumberOfTilesY];
            Snacks = new List<Snack>();
        }

        public void Initialize()
        {
            CreateGrid();
        }

        private void CreateGrid()
        {
            for (int y = 0; y < GameConstants.NumberOfTilesY; y++)
            {
                for (int x = 0; x < GameConstants.NumberOfTilesX; x++)
                {
                    var position = new Vector2(
                        x * GameConstants.TileWidth,
                        y * GameConstants.TileHeight + GameConstants.ScoreOffset
                    );

                    TileArray[x, y] = _mapDesign[y, x] switch
                    {
                        0 => CreateSnackTile(position, x, y, false), // Small snack
                        1 => new Tile(position, Tile.TileType.Wall) { IsEmpty = false }, // Wall
                        2 => new Tile(position, Tile.TileType.GhostHouse) { IsEmpty = false }, // Ghost house
                        3 => CreateSnackTile(position, x, y, true), // Big snack
                        5 => new Tile(position), // Empty
                        _ => new Tile(position)
                    };
                }
            }
        }

        private Tile CreateSnackTile(Vector2 position, int x, int y, bool isBigSnack)
        {
            var snackType = isBigSnack ? Snack.SnackType.Big : Snack.SnackType.Small;
            Snacks.Add(new Snack(snackType, position, new[] { x, y }));
            return new Tile(position, Tile.TileType.Snack) { IsEmpty = false };
        }

        public void RegenerateSnacks()
        {
            Snacks.Clear();

            for (int y = 0; y < GameConstants.NumberOfTilesY; y++)
            {
                for (int x = 0; x < GameConstants.NumberOfTilesX; x++)
                {
                    var position = new Vector2(
                        x * GameConstants.TileWidth,
                        y * GameConstants.TileHeight + GameConstants.ScoreOffset
                    );

                    if (_mapDesign[y, x] == 0 || _mapDesign[y, x] == 3)
                    {
                        var isBigSnack = _mapDesign[y, x] == 3;
                        var snackType = isBigSnack ? Snack.SnackType.Big : Snack.SnackType.Small;
                        Snacks.Add(new Snack(snackType, position, new[] { x, y }));
                        TileArray[x, y] = new Tile(position, Tile.TileType.Snack) { IsEmpty = false };
                    }
                }
            }
        }

        public bool IsTileWalkable(Direction direction, Vector2 tile)
        {
            // Handle teleport tiles
            if (tile == GameConstants.LeftTeleportTile || tile == GameConstants.RightTeleportTile)
            {
                return direction.IsHorizontal();
            }

            // Get the target tile based on direction
            var targetTile = GetAdjacentTile(tile, direction);
            if (!IsValidTile(targetTile))
                return false;

            var tileType = TileArray[(int)targetTile.X, (int)targetTile.Y].tileType;
            return tileType != Tile.TileType.Wall && tileType != Tile.TileType.GhostHouse;
        }

        public bool IsTileWalkableForGhost(Direction direction, Vector2 tile)
        {
            // Handle teleport tiles
            if (tile == GameConstants.LeftTeleportTile || tile == GameConstants.RightTeleportTile)
            {
                return direction.IsHorizontal();
            }

            var targetTile = GetAdjacentTile(tile, direction);
            if (!IsValidTile(targetTile))
                return false;

            var tileType = TileArray[(int)targetTile.X, (int)targetTile.Y].tileType;
            return tileType != Tile.TileType.Wall;
        }

        private Vector2 GetAdjacentTile(Vector2 tile, Direction direction)
        {
            return direction switch
            {
                Direction.Right => new Vector2(tile.X + 1, tile.Y),
                Direction.Left => new Vector2(tile.X - 1, tile.Y),
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

        public int FindSnackAtPosition(Vector2 position)
        {
            for (int i = 0; i < Snacks.Count; i++)
            {
                if (Snacks[i].Position == position)
                    return i;
            }
            return -1;
        }

        public bool CheckTileType(Vector2 gridIndex, Tile.TileType tileType)
        {
            if (!IsValidTile(gridIndex))
                return false;

            return TileArray[(int)gridIndex.X, (int)gridIndex.Y].tileType == tileType;
        }

        public void DrawDebugGrid(SpriteBatch spriteBatch, Texture2D lineX, Texture2D lineY)
        {
            for (int x = 0; x < GameConstants.NumberOfTilesX; x++)
            {
                for (int y = 0; y < GameConstants.NumberOfTilesY; y++)
                {
                    var position = TileArray[x, y].Position;
                    spriteBatch.Draw(lineX, position, Color.White);
                    spriteBatch.Draw(lineY, position, Color.White);
                }
            }
        }
    }
}
