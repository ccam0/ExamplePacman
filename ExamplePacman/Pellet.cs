using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pacman
{
    /// <summary>
    /// Represents a pellet (food) that Pac-Man can eat.
    /// </summary>
    public class Pellet
    {
        public enum PelletType { Small, Power }

        private static readonly Rectangle SmallPelletRect = new Rectangle(33, 33, 6, 6);
        private static readonly Rectangle PowerPelletRect = new Rectangle(24, 72, 24, 24);

        private const int SmallPelletOffset = 3;
        private const int PowerPelletOffset = 12;
        private const int PowerPelletAnimationCycle = 20;

        public PelletType Type { get; }
        public Vector2 Position { get; }
        public int ScoreValue { get; }

        private readonly int radiusOffset;
        private int animationTimer = PowerPelletAnimationCycle;

        public Pellet(PelletType type, Vector2 position)
        {
            Type = type;
            Position = position;

            if (type == PelletType.Power)
            {
                ScoreValue = 50;
                radiusOffset = PowerPelletOffset;
            }
            else
            {
                ScoreValue = 10;
                radiusOffset = SmallPelletOffset;
            }
        }

        /// <summary>
        /// Draws the pellet on screen.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 centerOffset = new Vector2(
                Maze.TileWidth / 2 - radiusOffset,
                Maze.TileHeight / 2 - radiusOffset
            );

            if (Type == PelletType.Small)
            {
                Game1.spriteSheet1.drawSprite(spriteBatch, SmallPelletRect, Position + centerOffset);
            }
            else
            {
                // Power pellet blinks
                if (animationTimer >= 10 || Game1.gamePauseTimer > 0)
                {
                    Game1.spriteSheet1.drawSprite(spriteBatch, PowerPelletRect, Position + centerOffset);
                }

                animationTimer--;
                if (animationTimer < 0)
                {
                    animationTimer = PowerPelletAnimationCycle;
                }
            }
        }
    }
}
