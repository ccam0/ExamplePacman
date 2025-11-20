using Microsoft.Xna.Framework;
using Pacman.Components;
using Pacman.Core;
using Pacman.Managers;

namespace Pacman.Entities
{
    /// <summary>
    /// Blinky (Red Ghost) - Directly chases the player
    /// </summary>
    public class Blinky : Ghost
    {
        public Blinky(Vector2 startTile, Tile[,] tileArray)
            : base(startTile, tileArray, GameConstants.BlinkyScatterTarget)
        {
            _transform.Direction = Direction.Left;
        }

        protected override Animation CreateAnimation()
        {
            return new Animation(GameConstants.GhostAnimationSpeed, SpriteData.BlinkyLeft);
        }

        protected override Rectangle[] GetUpFrames() => SpriteData.BlinkyUp;
        protected override Rectangle[] GetDownFrames() => SpriteData.BlinkyDown;
        protected override Rectangle[] GetLeftFrames() => SpriteData.BlinkyLeft;
        protected override Rectangle[] GetRightFrames() => SpriteData.BlinkyRight;

        public override Vector2 CalculateChaseTarget(Player player, GridManager gridManager, Vector2 blinkyPosition)
        {
            // Blinky directly targets the player
            return player.CurrentTile;
        }
    }
}
