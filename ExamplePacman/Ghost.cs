using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pacman
{
    /// <summary>
    /// Base class for all ghost AI characters in the game.
    /// </summary>
    public class Ghost
    {
        public enum GhostType { Inky, Blinky, Pinky, Clyde }
        public enum GhostState { Scatter, Chase, Frightened, Eaten }

        // Ghost properties
        public GhostType Type { get; protected set; }
        public GhostState State { get; set; } = GhostState.Scatter;
        public Vector2 ScatterTargetTile { get; protected set; }
        public Vector2 CurrentTile { get; protected set; }
        public Vector2 Position { get; set; }
        public bool IsColliding { get; set; } = false;
        public List<Vector2> PathToPacMan { get; set; }

        // Movement
        protected Direction direction;
        protected Direction playerLastDirection;
        protected Vector2 previousTile;
        protected Tile.TileType previousTileType;
        protected Vector2 foundPathTile;

        private readonly Vector2 eatenTargetTile = new Vector2(13, 14);

        // Speed constants
        public int Speed { get; set; } = 140;
        public const int NormalSpeed = 140;
        public const int FrightenedSpeed = 90;
        public const int EatenSpeed = 240;

        // Animation rectangles
        public Rectangle[] RectsDown = new Rectangle[2];
        public Rectangle[] RectsUp = new Rectangle[2];
        public Rectangle[] RectsLeft = new Rectangle[2];
        public Rectangle[] RectsRight = new Rectangle[2];

        private static readonly Rectangle RectDownEaten = new Rectangle(1899, 243, 42, 42);
        private static readonly Rectangle RectUpEaten = new Rectangle(1851, 243, 42, 42);
        private static readonly Rectangle RectLeftEaten = new Rectangle(1803, 243, 42, 42);
        private static readonly Rectangle RectRightEaten = new Rectangle(1755, 243, 42, 42);

        private static readonly Rectangle[] FrightenedRects = new Rectangle[2]
        {
            new Rectangle(1755, 195, 42, 42),
            new Rectangle(1803, 195, 42, 42)
        };

        private static readonly Rectangle[] FrightenedRectsEnd = new Rectangle[8]
        {
            new Rectangle(1755, 195, 42, 42), new Rectangle(1803, 195, 42, 42),
            new Rectangle(1755, 195, 42, 42), new Rectangle(1803, 195, 42, 42),
            new Rectangle(1851, 195, 42, 42), new Rectangle(1899, 195, 42, 42),
            new Rectangle(1851, 195, 42, 42), new Rectangle(1899, 195, 42, 42)
        };

        protected const int DrawOffsetX = -9;
        protected const int DrawOffsetY = -9;

        protected SpriteAnimation animation;
        public SpriteAnimation Animation => animation;

        // Frightened timer
        public float FrightenedTimer { get; set; }
        public const float FrightenedDuration = 8f;

        public Ghost(int tileX, int tileY, Tile[,] tileArray)
        {
            Position = tileArray[tileX, tileY].Position;
            CurrentTile = new Vector2(tileX, tileY);
            previousTile = new Vector2(-1, -1);
            direction = Direction.None;

            animation = new SpriteAnimation(0.08f, RectsUp);
            Position += new Vector2(12, 0); // Center offset
        }

        /// <summary>
        /// Draws the ghost on screen.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, SpriteSheet spriteSheet)
        {
            Vector2 drawPosition = Position + new Vector2(DrawOffsetX, DrawOffsetY);

            if (State != GhostState.Eaten)
            {
                animation.Draw(spriteBatch, spriteSheet, drawPosition);
            }
            else
            {
                Rectangle eyesRect = direction switch
                {
                    Direction.Up => RectUpEaten,
                    Direction.Down => RectDownEaten,
                    Direction.Left => RectLeftEaten,
                    Direction.Right => RectRightEaten,
                    _ => RectRightEaten
                };
                spriteSheet.drawSprite(spriteBatch, eyesRect, drawPosition);
            }
        }

        /// <summary>
        /// Updates the ghost's state, position, and AI.
        /// </summary>
        public void Update(GameTime gameTime, Maze maze, Vector2 playerTilePos, Direction playerDirection, Vector2 blinkyPos)
        {
            // Update frightened timer
            if (State == GhostState.Frightened)
            {
                FrightenedTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (FrightenedTimer > FrightenedDuration)
                {
                    State = GhostState.Chase;
                    Speed = NormalSpeed;
                    FrightenedTimer = 0;
                }
            }

            UpdateTilePosition(maze.Tiles);
            animation.Update(gameTime);
            DecideDirection(playerTilePos, playerDirection, maze, blinkyPos);
            Move(gameTime, maze.Tiles);
        }

        /// <summary>
        /// Gets the target tile for scatter mode.
        /// </summary>
        public virtual Vector2 GetScatterTarget()
        {
            return ScatterTargetTile;
        }

        /// <summary>
        /// Gets the target tile for chase mode.
        /// </summary>
        public virtual Vector2 GetChaseTarget(Vector2 playerTilePos, Direction playerDirection, Tile[,] tileArray)
        {
            return playerTilePos;
        }

        /// <summary>
        /// Gets the target tile for chase mode (overload for Inky).
        /// </summary>
        public virtual Vector2 GetChaseTarget(Vector2 playerTilePos, Direction playerDirection, Tile[,] tileArray, Vector2 blinkyPos)
        {
            return playerTilePos;
        }

        /// <summary>
        /// Gets a random target tile for frightened mode.
        /// </summary>
        public virtual Vector2 GetFrightenedTarget(Maze maze)
        {
            // Handle tunnel tiles
            if (CurrentTile.Equals(new Vector2(0, 14)) || CurrentTile.Equals(new Vector2(Maze.NumberOfTilesX - 1, 14)))
            {
                return CurrentTile;
            }

            // Check if in ghost house
            if (maze.Tiles[(int)CurrentTile.X, (int)CurrentTile.Y].tileType == Tile.TileType.GhostHouse)
            {
                return new Vector2(13, 11);
            }

            // Get available directions (excluding opposite direction and ghost house)
            List<Direction> availableDirections = new List<Direction>();
            Direction opposite = direction.GetOpposite();

            if (opposite != Direction.Left && maze.IsNextTileAvailableForGhosts(Direction.Left, CurrentTile))
            {
                if (maze.Tiles[(int)CurrentTile.X - 1, (int)CurrentTile.Y].tileType != Tile.TileType.GhostHouse)
                    availableDirections.Add(Direction.Left);
            }
            if (opposite != Direction.Right && maze.IsNextTileAvailableForGhosts(Direction.Right, CurrentTile))
            {
                if (maze.Tiles[(int)CurrentTile.X + 1, (int)CurrentTile.Y].tileType != Tile.TileType.GhostHouse)
                    availableDirections.Add(Direction.Right);
            }
            if (opposite != Direction.Down && maze.IsNextTileAvailableForGhosts(Direction.Down, CurrentTile))
            {
                if (maze.Tiles[(int)CurrentTile.X, (int)CurrentTile.Y + 1].tileType != Tile.TileType.GhostHouse)
                    availableDirections.Add(Direction.Down);
            }
            if (opposite != Direction.Up && maze.IsNextTileAvailableForGhosts(Direction.Up, CurrentTile))
            {
                if (maze.Tiles[(int)CurrentTile.X, (int)CurrentTile.Y - 1].tileType != Tile.TileType.GhostHouse)
                    availableDirections.Add(Direction.Up);
            }

            if (availableDirections.Count > 0)
            {
                Random random = new Random();
                Direction randomDirection = availableDirections[random.Next(availableDirections.Count)];

                Vector2 targetPos = CurrentTile;
                switch (randomDirection)
                {
                    case Direction.Left: targetPos.X--; break;
                    case Direction.Right: targetPos.X++; break;
                    case Direction.Down: targetPos.Y++; break;
                    case Direction.Up: targetPos.Y--; break;
                }
                return targetPos;
            }

            return CurrentTile;
        }

        /// <summary>
        /// Gets eaten when in frightened state.
        /// </summary>
        public virtual void GetEaten()
        {
            // Award points based on multiplier
            int points = Game1.GhostScoreMultiplier switch
            {
                1 => 200,
                2 => 400,
                3 => 800,
                4 => 1600,
                _ => 200
            };

            Game1.score += points;
            Game1.GhostScoreMultiplier++;

            State = GhostState.Eaten;
            Speed = EatenSpeed;
            FrightenedTimer = 0;

            Sounds.EatGhost.Play();
            Sounds.RetreatingInstance.Play();
            Sounds.PowerPelletInstance.Stop();
        }

        /// <summary>
        /// Determines the direction the ghost should move based on its current state.
        /// </summary>
        protected void DecideDirection(Vector2 playerTilePos, Direction playerDirection, Maze maze, Vector2 blinkyPos)
        {
            // Recalculate path if needed
            if (!foundPathTile.Equals(CurrentTile))
            {
                PathToPacMan = State switch
                {
                    GhostState.Scatter => Pathfinding.findPath(CurrentTile, GetScatterTarget(), maze.Tiles, direction),
                    GhostState.Chase when Type == GhostType.Inky => Pathfinding.findPath(CurrentTile, GetChaseTarget(playerTilePos, playerDirection, maze.Tiles, blinkyPos), maze.Tiles, direction),
                    GhostState.Chase => Pathfinding.findPath(CurrentTile, GetChaseTarget(playerTilePos, playerDirection, maze.Tiles), maze.Tiles, direction),
                    GhostState.Frightened => Pathfinding.findPath(CurrentTile, GetFrightenedTarget(maze), maze.Tiles, direction),
                    GhostState.Eaten => Pathfinding.findPath(CurrentTile, eatenTargetTile, maze.Tiles, direction),
                    _ => new List<Vector2>()
                };
                foundPathTile = CurrentTile;
            }

            // Check if reached eaten target
            if (CurrentTile.Equals(eatenTargetTile) && State == GhostState.Eaten)
            {
                State = GhostState.Chase;
                Speed = NormalSpeed;
                Sounds.PowerPelletInstance.Play();
            }

            // Check collision with player
            if (playerTilePos.Equals(CurrentTile))
            {
                if (State == GhostState.Frightened)
                {
                    GetEaten();
                }
                else if (State != GhostState.Eaten)
                {
                    IsColliding = true;
                    return;
                }
            }

            if (PathToPacMan.Count == 0)
                return;

            // Determine direction from path
            if (PathToPacMan[0].X > CurrentTile.X)
            {
                direction = Direction.Right;
                Position = new Vector2(Position.X, maze.Tiles[(int)CurrentTile.X, (int)CurrentTile.Y].Position.Y);
            }
            else if (PathToPacMan[0].X < CurrentTile.X)
            {
                direction = Direction.Left;
                Position = new Vector2(Position.X, maze.Tiles[(int)CurrentTile.X, (int)CurrentTile.Y].Position.Y);
            }
            else if (PathToPacMan[0].Y > CurrentTile.Y)
            {
                direction = Direction.Down;
                Position = new Vector2(maze.Tiles[(int)CurrentTile.X, (int)CurrentTile.Y].Position.X, Position.Y);
            }
            else if (PathToPacMan[0].Y < CurrentTile.Y)
            {
                direction = Direction.Up;
                Position = new Vector2(maze.Tiles[(int)CurrentTile.X, (int)CurrentTile.Y].Position.X, Position.Y);
            }
        }

        /// <summary>
        /// Moves the ghost based on its current direction.
        /// </summary>
        protected void Move(GameTime gameTime, Tile[,] tileArray)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float moveAmount = Speed * dt;

            switch (direction)
            {
                case Direction.Right:
                    Position += new Vector2(moveAmount, 0);
                    UpdateAnimation(Direction.Right);
                    break;

                case Direction.Left:
                    Position -= new Vector2(moveAmount, 0);
                    UpdateAnimation(Direction.Left);
                    break;

                case Direction.Down:
                    Position += new Vector2(0, moveAmount);
                    UpdateAnimation(Direction.Down);
                    break;

                case Direction.Up:
                    Position -= new Vector2(0, moveAmount);
                    UpdateAnimation(Direction.Up);
                    break;

                case Direction.None:
                    Position = tileArray[(int)CurrentTile.X, (int)CurrentTile.Y].Position;
                    break;
            }
        }

        /// <summary>
        /// Updates animation based on direction and state.
        /// </summary>
        private void UpdateAnimation(Direction moveDirection)
        {
            if (State == GhostState.Frightened)
            {
                animation.setSourceRects(FrightenedTimer > 5 ? FrightenedRectsEnd : FrightenedRects);
            }
            else
            {
                animation.setSourceRects(moveDirection switch
                {
                    Direction.Right => RectsRight,
                    Direction.Left => RectsLeft,
                    Direction.Down => RectsDown,
                    Direction.Up => RectsUp,
                    _ => RectsUp
                });
            }
        }

        /// <summary>
        /// Checks if the ghost is at a tunnel teleport position.
        /// </summary>
        private int CheckForTeleportPosition(Tile[,] tileArray)
        {
            if (CurrentTile.Equals(new Vector2(0, 14)) && Position.X < -30)
                return 1;
            if (CurrentTile.Equals(new Vector2(Maze.NumberOfTilesX - 1, 14)) && Position.X > tileArray[(int)CurrentTile.X, (int)CurrentTile.Y].Position.X + 30)
                return 2;
            return 0;
        }

        /// <summary>
        /// Teleports the ghost to the opposite side of the tunnel.
        /// </summary>
        private void Teleport(Vector2 newPosition, Vector2 newTile)
        {
            Position = newPosition;
            previousTile = CurrentTile;
            CurrentTile = newTile;
        }

        /// <summary>
        /// Updates the ghost's current tile based on its position.
        /// </summary>
        protected void UpdateTilePosition(Tile[,] tileArray)
        {
            // Handle tunnel teleportation
            int teleportCheck = CheckForTeleportPosition(tileArray);
            if (teleportCheck == 1 && direction == Direction.Left)
            {
                Teleport(new Vector2(Game1.windowWidth + 30, Position.Y), new Vector2(Maze.NumberOfTilesX - 1, 14));
            }
            else if (teleportCheck == 2 && direction == Direction.Right)
            {
                Teleport(new Vector2(-30, Position.Y), new Vector2(0, 14));
            }

            // Update tile position based on pixel position
            Vector2 checkPosition = Position + new Vector2(10, 10); // Center offset for checking

            for (int x = 0; x < tileArray.GetLength(0); x++)
            {
                for (int y = 0; y < tileArray.GetLength(1); y++)
                {
                    Vector2 tilePos = tileArray[x, y].Position;
                    Vector2 nextTilePos = tilePos + new Vector2(Maze.TileWidth, Maze.TileHeight);

                    bool inTileX = checkPosition.X >= tilePos.X && checkPosition.X < nextTilePos.X;
                    bool inTileY = checkPosition.Y >= tilePos.Y && checkPosition.Y < nextTilePos.Y;

                    if (inTileX && inTileY)
                    {
                        if (!CurrentTile.Equals(new Vector2(x, y)))
                        {
                            previousTile = CurrentTile;
                            tileArray[(int)previousTile.X, (int)previousTile.Y].tileType = previousTileType;
                            CurrentTile = new Vector2(x, y);
                            previousTileType = tileArray[x, y].tileType;
                            tileArray[x, y].tileType = Tile.TileType.Ghost;
                        }
                        return;
                    }
                }
            }
        }
    }
}
