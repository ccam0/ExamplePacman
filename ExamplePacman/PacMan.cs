using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pacman
{
    /// <summary>
    /// Represents the player-controlled Pac-Man character.
    /// </summary>
    public class PacMan
    {
        private const int Speed = 150;
        private const int RadiusOffset = 19;
        private const float MoveCooldown = 0.2f;

        // Animation rectangles
        public static readonly Rectangle[] DeathAnimationRects = new Rectangle[11]
        {
            new Rectangle(1515, 3, 39, 39),
            new Rectangle(1563, 3, 39, 39),
            new Rectangle(1611, 3, 39, 39),
            new Rectangle(1659, 3, 39, 39),
            new Rectangle(1707, 6, 39, 39),
            new Rectangle(1755, 9, 39, 39),
            new Rectangle(1803, 12, 39, 39),
            new Rectangle(1851, 12, 39, 39),
            new Rectangle(1899, 12, 39, 39),
            new Rectangle(1947, 9, 39, 39),
            new Rectangle(1995, 15, 39, 39)
        };

        private static readonly Rectangle LastRect = new Rectangle(1467, 3, 39, 39);
        public static readonly Rectangle[] RectsDown = new Rectangle[3]
        {
            new Rectangle(1371, 147, 39, 39),
            new Rectangle(1419, 147, 39, 39),
            LastRect
        };

        public static readonly Rectangle[] RectsUp = new Rectangle[3]
        {
            new Rectangle(1371, 99, 39, 39),
            new Rectangle(1419, 99, 39, 39),
            LastRect
        };

        public static readonly Rectangle[] RectsLeft = new Rectangle[3]
        {
            new Rectangle(1371, 51, 39, 39),
            new Rectangle(1419, 51, 39, 39),
            LastRect
        };

        public static readonly Rectangle[] RectsRight = new Rectangle[3]
        {
            new Rectangle(1371, 3, 39, 39),
            new Rectangle(1419, 3, 39, 39),
            LastRect
        };

        // Properties
        public Vector2 Position { get; set; }
        public Direction CurrentDirection { get; set; } = Direction.Right;
        public Vector2 CurrentTile { get; set; }
        public Vector2 PreviousTile { get; private set; }
        public int Lives { get; set; } = 0;
        public SpriteAnimation Animation { get; private set; }

        // Movement control
        private Direction nextDirection = Direction.None;
        private bool canMove = true;
        private float moveCooldownTimer = 0;

        public PacMan(int tileX, int tileY, Tile[,] tileArray)
        {
            Position = tileArray[tileX, tileY].Position + new Vector2(14, 0);
            CurrentTile = new Vector2(tileX, tileY);
            PreviousTile = new Vector2(tileX - 1, tileY);
            Animation = new SpriteAnimation(0.08f, RectsRight, 2);
        }

        /// <summary>
        /// Updates Pac-Man's position and handles input.
        /// </summary>
        public void Update(GameTime gameTime, Maze maze)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update move cooldown
            if (!canMove)
            {
                moveCooldownTimer += dt;
                if (moveCooldownTimer >= MoveCooldown)
                {
                    canMove = true;
                    moveCooldownTimer = 0;
                }
            }

            Animation.Update(gameTime);
            HandleInput();
            ProcessMovement(dt, maze);
        }

        /// <summary>
        /// Handles keyboard input for movement.
        /// </summary>
        private void HandleInput()
        {
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.D) || keyState.IsKeyDown(Keys.Right))
                nextDirection = Direction.Right;
            else if (keyState.IsKeyDown(Keys.A) || keyState.IsKeyDown(Keys.Left))
                nextDirection = Direction.Left;
            else if (keyState.IsKeyDown(Keys.W) || keyState.IsKeyDown(Keys.Up))
                nextDirection = Direction.Up;
            else if (keyState.IsKeyDown(Keys.S) || keyState.IsKeyDown(Keys.Down))
                nextDirection = Direction.Down;
        }

        /// <summary>
        /// Processes movement and collision.
        /// </summary>
        private void ProcessMovement(float dt, Maze maze)
        {
            // Try to change direction if cooldown is over
            if (canMove && nextDirection != Direction.None)
            {
                if (maze.IsNextTileAvailable(nextDirection, CurrentTile))
                {
                    canMove = false;
                    CurrentDirection = nextDirection;
                    AlignToGrid(maze.Tiles);
                    nextDirection = Direction.None;
                }
            }

            // Stop if hitting a wall
            if (!maze.IsNextTileAvailable(CurrentDirection, CurrentTile))
            {
                CurrentDirection = Direction.None;
            }

            // Move in current direction
            switch (CurrentDirection)
            {
                case Direction.Right:
                    if (maze.IsNextTileAvailable(Direction.Right, CurrentTile))
                    {
                        Position += new Vector2(Speed * dt, 0);
                        Animation.setSourceRects(RectsRight);
                    }
                    break;

                case Direction.Left:
                    if (maze.IsNextTileAvailable(Direction.Left, CurrentTile))
                    {
                        Position -= new Vector2(Speed * dt, 0);
                        Animation.setSourceRects(RectsLeft);
                    }
                    break;

                case Direction.Down:
                    if (maze.IsNextTileAvailable(Direction.Down, CurrentTile))
                    {
                        Position += new Vector2(0, Speed * dt);
                        Animation.setSourceRects(RectsDown);
                    }
                    break;

                case Direction.Up:
                    if (maze.IsNextTileAvailable(Direction.Up, CurrentTile))
                    {
                        Position -= new Vector2(0, Speed * dt);
                        Animation.setSourceRects(RectsUp);
                    }
                    break;

                case Direction.None:
                    Vector2 tilePos = maze.Tiles[(int)CurrentTile.X, (int)CurrentTile.Y].Position;
                    Position = new Vector2(tilePos.X + 2, tilePos.Y + 1);
                    Sounds.MunchInstance.Stop();
                    break;
            }
        }

        /// <summary>
        /// Aligns Pac-Man's position to the grid based on direction.
        /// </summary>
        private void AlignToGrid(Tile[,] tileArray)
        {
            Vector2 tilePos = tileArray[(int)CurrentTile.X, (int)CurrentTile.Y].Position;

            if (CurrentDirection == Direction.Right || CurrentDirection == Direction.Left)
            {
                Position = new Vector2(Position.X, tilePos.Y + 1);
            }
            else if (CurrentDirection == Direction.Down || CurrentDirection == Direction.Up)
            {
                Position = new Vector2(tilePos.X + 2, Position.Y);
            }
        }

        /// <summary>
        /// Eats a pellet and updates the score.
        /// </summary>
        public void EatPellet(Pellet pellet, Maze maze)
        {
            Game1.score += pellet.ScoreValue;

            if (pellet.Type == Pellet.PelletType.Power)
            {
                Game1.HasEatenPowerPellet = true;
                Sounds.EatFruit.Play();
            }

            Sounds.MunchInstance.Play();
        }

        /// <summary>
        /// Draws Pac-Man on screen.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, SpriteSheet spriteSheet)
        {
            Vector2 drawPosition = Position - new Vector2(RadiusOffset / 2, RadiusOffset / 2 - 1);
            Animation.Draw(spriteBatch, spriteSheet, drawPosition);
        }

        /// <summary>
        /// Updates Pac-Man's tile position based on pixel position.
        /// </summary>
        public void UpdateTilePosition(Maze maze)
        {
            maze.Tiles[(int)PreviousTile.X, (int)PreviousTile.Y].tileType = Tile.TileType.None;
            maze.Tiles[(int)CurrentTile.X, (int)CurrentTile.Y].tileType = Tile.TileType.Player;

            // Check for tunnel teleportation
            CheckTunnelTeleport(maze.Tiles);

            // Find pellets at current tile
            int pelletIndex = maze.FindPelletAtPosition(maze.Tiles[(int)CurrentTile.X, (int)CurrentTile.Y].Position);
            if (pelletIndex != -1)
            {
                EatPellet(maze.Pellets[pelletIndex], maze);
                maze.Pellets.RemoveAt(pelletIndex);
            }

            // Update tile based on position
            UpdateTileFromPosition(maze.Tiles);
        }

        /// <summary>
        /// Checks and handles tunnel teleportation.
        /// </summary>
        private void CheckTunnelTeleport(Tile[,] tileArray)
        {
            // Left tunnel exit
            if (CurrentTile.Equals(new Vector2(0, 14)) && Position.X < -30 && CurrentDirection == Direction.Left)
            {
                Position = new Vector2(Game1.windowWidth + 30, Position.Y);
                PreviousTile = CurrentTile;
                CurrentTile = new Vector2(Maze.NumberOfTilesX - 1, 14);
            }
            // Right tunnel exit
            else if (CurrentTile.Equals(new Vector2(Maze.NumberOfTilesX - 1, 14)) &&
                     Position.X > tileArray[(int)CurrentTile.X, (int)CurrentTile.Y].Position.X + 30 &&
                     CurrentDirection == Direction.Right)
            {
                Position = new Vector2(-30, Position.Y);
                PreviousTile = CurrentTile;
                CurrentTile = new Vector2(0, 14);
            }
        }

        /// <summary>
        /// Updates the current tile based on Pac-Man's pixel position.
        /// </summary>
        private void UpdateTileFromPosition(Tile[,] tileArray)
        {
            Vector2 checkPosition = Position + new Vector2(RadiusOffset / 2, RadiusOffset / 2);

            for (int x = 0; x < tileArray.GetLength(0); x++)
            {
                for (int y = 0; y < tileArray.GetLength(1); y++)
                {
                    Vector2 tilePos = tileArray[x, y].Position;
                    Vector2 nextTilePos = tilePos + new Vector2(Maze.TileWidth, Maze.TileHeight);

                    bool inTileX = checkPosition.X >= tilePos.X && checkPosition.X < nextTilePos.X;
                    bool inTileY = checkPosition.Y >= tilePos.Y && checkPosition.Y < nextTilePos.Y;

                    if (inTileX && inTileY && !CurrentTile.Equals(new Vector2(x, y)))
                    {
                        PreviousTile = CurrentTile;
                        CurrentTile = new Vector2(x, y);

                        if (maze.CheckTileType(CurrentTile, Tile.TileType.None))
                        {
                            Sounds.MunchInstance.Stop();
                        }
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Resets Pac-Man to the starting position.
        /// </summary>
        public void Reset(Maze maze)
        {
            Position = maze.Tiles[13, 23].Position + new Vector2(14, 0);
            CurrentTile = new Vector2(13, 23);
            CurrentDirection = Direction.Right;
            Animation.setSourceRects(RectsRight);
            Animation.setAnimIndex(2);
        }

        /// <summary>
        /// Gets Pac-Man's death animation position.
        /// </summary>
        public Vector2 GetDeathAnimationPosition()
        {
            return Position - new Vector2(RadiusOffset / 2, RadiusOffset / 2 - 1);
        }
    }
}
