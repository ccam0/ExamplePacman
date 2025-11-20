namespace Pacman.Core
{
    /// <summary>
    /// Represents movement directions in the game
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
    /// Helper methods for Direction enum
    /// </summary>
    public static class DirectionExtensions
    {
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

        public static bool IsHorizontal(this Direction direction)
        {
            return direction == Direction.Left || direction == Direction.Right;
        }

        public static bool IsVertical(this Direction direction)
        {
            return direction == Direction.Up || direction == Direction.Down;
        }
    }
}
