using Microsoft.Xna.Framework;
using Pacman.Components;
using Pacman.Core;
using Pacman.Managers;

namespace Pacman.Entities
{
    /// <summary>
    /// Inky (Cyan Ghost) - Uses Blinky's position for complex targeting
    /// </summary>
    public class Inky : Ghost
    {
        public Inky(Vector2 startTile, Tile[,] tileArray)
            : base(startTile, tileArray, GameConstants.InkyScatterTarget)
        {
            _transform.Direction = Direction.Up;
        }

        protected override Animation CreateAnimation()
        {
            return new Animation(GameConstants.GhostAnimationSpeed, SpriteData.InkyUp);
        }

        protected override Rectangle[] GetUpFrames() => SpriteData.InkyUp;
        protected override Rectangle[] GetDownFrames() => SpriteData.InkyDown;
        protected override Rectangle[] GetLeftFrames() => SpriteData.InkyLeft;
        protected override Rectangle[] GetRightFrames() => SpriteData.InkyRight;

        public override Vector2 CalculateChaseTarget(Player player, GridManager gridManager, Vector2 blinkyPosition)
        {
            // Inky's targeting: 2 tiles ahead of player, then vector from Blinky doubled
            var offset = player.Direction switch
            {
                Direction.Up => new Vector2(0, -2),
                Direction.Down => new Vector2(0, 2),
                Direction.Left => new Vector2(-2, 0),
                Direction.Right => new Vector2(2, 0),
                _ => Vector2.Zero
            };

            var pivot = player.CurrentTile + offset;
            var vectorFromBlinky = pivot - blinkyPosition;
            var target = blinkyPosition + (vectorFromBlinky * 2);

            // Clamp to valid tiles
            target.X = MathHelper.Clamp(target.X, 0, GameConstants.NumberOfTilesX - 1);
            target.Y = MathHelper.Clamp(target.Y, 0, GameConstants.NumberOfTilesY - 1);

            return target;
        }
    }
}
