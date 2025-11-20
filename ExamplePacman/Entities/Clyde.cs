using Microsoft.Xna.Framework;
using Pacman.Components;
using Pacman.Core;
using Pacman.Managers;

namespace Pacman.Entities
{
    /// <summary>
    /// Clyde (Orange Ghost) - Chases player when far, retreats when close
    /// </summary>
    public class Clyde : Ghost
    {
        private const float ScareDistance = 8f;

        public Clyde(Vector2 startTile, Tile[,] tileArray)
            : base(startTile, tileArray, GameConstants.ClydeScatterTarget)
        {
            _transform.Direction = Direction.Up;
        }

        protected override Animation CreateAnimation()
        {
            return new Animation(GameConstants.GhostAnimationSpeed, SpriteData.ClydeUp);
        }

        protected override Rectangle[] GetUpFrames() => SpriteData.ClydeUp;
        protected override Rectangle[] GetDownFrames() => SpriteData.ClydeDown;
        protected override Rectangle[] GetLeftFrames() => SpriteData.ClydeLeft;
        protected override Rectangle[] GetRightFrames() => SpriteData.ClydeRight;

        public override Vector2 CalculateChaseTarget(Player player, GridManager gridManager, Vector2 blinkyPosition)
        {
            // Clyde targets player when far away, his scatter corner when close
            float distance = Vector2.Distance(_transform.CurrentTile, player.CurrentTile);

            return distance > ScareDistance ? player.CurrentTile : ScatterTarget;
        }
    }
}
