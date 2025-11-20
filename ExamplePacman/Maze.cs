using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pacman
{
    /// <summary>
    /// Represents a single tile in the maze grid.
    /// </summary>
    public class Tile
    {
        public enum TileType { None, Wall, Ghost, GhostHouse, Player, Snack }

        public TileType tileType = TileType.None;
        public bool isEmpty = true;
        public Vector2 Position { get; }

        public Tile(Vector2 position)
        {
            Position = position;
        }

        public Tile(Vector2 position, TileType type)
        {
            Position = position;
            tileType = type;
        }

        /// <summary>
        /// Calculates Manhattan distance between two tile positions.
        /// </summary>
        public static int GetDistanceBetweenTiles(Vector2 pos1, Vector2 pos2)
        {
            return (int)Math.Sqrt(Math.Pow(pos1.X - pos2.X, 2) + Math.Pow(pos1.Y - pos2.Y, 2));
        }
    }

    /// <summary>
    /// Manages the game maze including tiles, pellets, and grid operations.
    /// </summary>
    public class Maze
    {
        private static readonly int[,] MapDesign = new int[,]
        {
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

        public const int NumberOfTilesX = 28;
        public const int NumberOfTilesY = 31;
        public static int TileWidth { get; private set; }
        public static int TileHeight { get; private set; }

        public Tile[,] Tiles { get; private set; }
        public List<Pellet> Pellets { get; private set; }

        public Maze(int screenWidth, int screenHeight, int scoreOffset)
        {
            TileWidth = screenWidth / NumberOfTilesX;
            TileHeight = (screenHeight - scoreOffset) / NumberOfTilesY;
            Tiles = new Tile[NumberOfTilesX, NumberOfTilesY];
            Pellets = new List<Pellet>();

            InitializeGrid(scoreOffset);
        }

        /// <summary>
        /// Initializes the maze grid with tiles and pellets.
        /// </summary>
        private void InitializeGrid(int scoreOffset)
        {
            for (int y = 0; y < NumberOfTilesY; y++)
            {
                for (int x = 0; x < NumberOfTilesX; x++)
                {
                    Vector2 position = new Vector2(x * TileWidth, y * TileHeight + scoreOffset);

                    switch (MapDesign[y, x])
                    {
                        case 0: // Small pellet
                            Tiles[x, y] = new Tile(position, Tile.TileType.Snack);
                            Tiles[x, y].isEmpty = false;
                            Pellets.Add(new Pellet(Pellet.PelletType.Small, position));
                            break;

                        case 1: // Wall
                            Tiles[x, y] = new Tile(position, Tile.TileType.Wall);
                            Tiles[x, y].isEmpty = false;
                            break;

                        case 2: // Ghost house
                            Tiles[x, y] = new Tile(position, Tile.TileType.GhostHouse);
                            Tiles[x, y].isEmpty = false;
                            break;

                        case 3: // Power pellet
                            Tiles[x, y] = new Tile(position, Tile.TileType.Snack);
                            Tiles[x, y].isEmpty = false;
                            Pellets.Add(new Pellet(Pellet.PelletType.Power, position));
                            break;

                        case 5: // Empty tile
                            Tiles[x, y] = new Tile(position);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Recreates all pellets (for level reset).
        /// </summary>
        public void ResetPellets(int scoreOffset)
        {
            Pellets.Clear();

            for (int y = 0; y < NumberOfTilesY; y++)
            {
                for (int x = 0; x < NumberOfTilesX; x++)
                {
                    Vector2 position = new Vector2(x * TileWidth, y * TileHeight + scoreOffset);

                    if (MapDesign[y, x] == 0) // Small pellet
                    {
                        Tiles[x, y] = new Tile(position, Tile.TileType.Snack);
                        Tiles[x, y].isEmpty = false;
                        Pellets.Add(new Pellet(Pellet.PelletType.Small, position));
                    }
                    else if (MapDesign[y, x] == 3) // Power pellet
                    {
                        Tiles[x, y] = new Tile(position, Tile.TileType.Snack);
                        Tiles[x, y].isEmpty = false;
                        Pellets.Add(new Pellet(Pellet.PelletType.Power, position));
                    }
                }
            }
        }

        /// <summary>
        /// Checks if movement in the given direction is possible for Pac-Man.
        /// </summary>
        public bool IsNextTileAvailable(Direction direction, Vector2 tile)
        {
            // Handle tunnel tiles
            if (tile.Equals(new Vector2(0, 14)) || tile.Equals(new Vector2(NumberOfTilesX - 1, 14)))
            {
                return direction == Direction.Right || direction == Direction.Left;
            }

            int nextX = (int)tile.X;
            int nextY = (int)tile.Y;

            switch (direction)
            {
                case Direction.Right:
                    nextX++;
                    break;
                case Direction.Left:
                    nextX--;
                    break;
                case Direction.Down:
                    nextY++;
                    break;
                case Direction.Up:
                    nextY--;
                    break;
                default:
                    return false;
            }

            if (nextX < 0 || nextX >= NumberOfTilesX || nextY < 0 || nextY >= NumberOfTilesY)
                return false;

            var tileType = Tiles[nextX, nextY].tileType;
            return tileType != Tile.TileType.Wall && tileType != Tile.TileType.GhostHouse;
        }

        /// <summary>
        /// Checks if movement in the given direction is possible for ghosts.
        /// </summary>
        public bool IsNextTileAvailableForGhosts(Direction direction, Vector2 tile)
        {
            // Handle tunnel tiles
            if (tile.Equals(new Vector2(0, 14)) || tile.Equals(new Vector2(NumberOfTilesX - 1, 14)))
            {
                return direction == Direction.Right || direction == Direction.Left;
            }

            int nextX = (int)tile.X;
            int nextY = (int)tile.Y;

            switch (direction)
            {
                case Direction.Right:
                    nextX++;
                    break;
                case Direction.Left:
                    nextX--;
                    break;
                case Direction.Down:
                    nextY++;
                    break;
                case Direction.Up:
                    nextY--;
                    break;
                default:
                    return false;
            }

            if (nextX < 0 || nextX >= NumberOfTilesX || nextY < 0 || nextY >= NumberOfTilesY)
                return false;

            return Tiles[nextX, nextY].tileType != Tile.TileType.Wall;
        }

        /// <summary>
        /// Finds a pellet at the given position.
        /// </summary>
        public int FindPelletAtPosition(Vector2 position)
        {
            for (int i = 0; i < Pellets.Count; i++)
            {
                if (Pellets[i].Position == position)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Checks if a tile has the specified type.
        /// </summary>
        public bool CheckTileType(Vector2 gridIndex, Tile.TileType tileType)
        {
            return Tiles[(int)gridIndex.X, (int)gridIndex.Y].tileType == tileType;
        }

        /// <summary>
        /// Draws all pellets in the maze.
        /// </summary>
        public void DrawPellets(SpriteBatch spriteBatch)
        {
            foreach (var pellet in Pellets)
            {
                pellet.Draw(spriteBatch);
            }
        }
    }
}
