using System;

namespace Pacman
{
    /// <summary>
    /// Represents the four cardinal directions plus none.
    /// </summary>
    public enum Direction
    {
        None,
        Up,
        Down,
        Left,
        Right
    }

    /// <summary>
    /// Helper methods for Direction operations.
    /// </summary>
    public static class DirectionExtensions
    {
        /// <summary>
        /// Returns the opposite direction.
        /// </summary>
        public static Direction GetOpposite(this Direction direction)
        {
            return direction switch
            {
                Direction.Up => Direction.Down,
                Direction.Down => Direction.Up,
                Direction.Left => Direction.Right,
                Direction.Right => Direction.Left,
                _ => Direction.None
            };
        }
    }
}
