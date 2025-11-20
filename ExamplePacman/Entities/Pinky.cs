using Microsoft.Xna.Framework;
using Pacman.Components;
using Pacman.Core;
using Pacman.Managers;

namespace Pacman.Entities
{
    /// <summary>
    /// Pinky (Pink Ghost) - Targets 4 tiles ahead of the player
    /// </summary>
    public class Pinky : Ghost
    {
        public Pinky(Vector2 startTile, Tile[,] tileArray)
            : base(startTile, tileArray, GameConstants.PinkyScatterTarget)
        {
            _transform.Direction = Direction.Down;
        }

        protected override Animation CreateAnimation()
        {
            return new Animation(GameConstants.GhostAnimationSpeed, SpriteData.PinkyDown);
        }

        protected override Rectangle[] GetUpFrames() => SpriteData.PinkyUp;
        protected override Rectangle[] GetDownFrames() => SpriteData.PinkyDown;
        protected override Rectangle[] GetLeftFrames() => SpriteData.PinkyLeft;
        protected override Rectangle[] GetRightFrames() => SpriteData.PinkyRight;

        public override Vector2 CalculateChaseTarget(Player player, GridManager gridManager, Vector2 blinkyPosition)
        {
            // Pinky targets 4 tiles ahead of the player
            var offset = player.Direction switch
            {
                Direction.Up => new Vector2(0, -4),
                Direction.Down => new Vector2(0, 4),
                Direction.Left => new Vector2(-4, 0),
                Direction.Right => new Vector2(4, 0),
                _ => Vector2.Zero
            };

            var target = player.CurrentTile + offset;

            // Clamp to valid tiles
            target.X = MathHelper.Clamp(target.X, 0, GameConstants.NumberOfTilesX - 1);
            target.Y = MathHelper.Clamp(target.Y, 0, GameConstants.NumberOfTilesY - 1);

            return target;
        }
    }
}
