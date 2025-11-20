using Microsoft.Xna.Framework;
using Pacman.Core;

namespace Pacman.Components
{
    /// <summary>
    /// Component handling position, tile position, and direction
    /// Follows the Component pattern for better modularity
    /// </summary>
    public class Transform
    {
        public Vector2 Position { get; set; }
        public Vector2 CurrentTile { get; set; }
        public Vector2 PreviousTile { get; set; }
        public Direction Direction { get; set; }

        public Transform(Vector2 position, Vector2 currentTile)
        {
            Position = position;
            CurrentTile = currentTile;
            PreviousTile = currentTile;
            Direction = Direction.None;
        }

        public Transform(Vector2 position, Vector2 currentTile, Direction initialDirection)
            : this(position, currentTile)
        {
            Direction = initialDirection;
        }

        public void Teleport(Vector2 newPosition, Vector2 newTile)
        {
            PreviousTile = CurrentTile;
            Position = newPosition;
            CurrentTile = newTile;
        }

        public void UpdateTile(Vector2 newTile)
        {
            PreviousTile = CurrentTile;
            CurrentTile = newTile;
        }
    }
}
