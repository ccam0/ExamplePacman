using Microsoft.Xna.Framework;

namespace Pacman.Core
{
    /// <summary>
    /// Centralized sprite rectangle data
    /// Eliminates magic rectangles scattered throughout code
    /// </summary>
    public static class SpriteData
    {
        // Player sprite rectangles
        public static readonly Rectangle[] PlayerDown = new[]
        {
            new Rectangle(1371, 147, 39, 39),
            new Rectangle(1419, 147, 39, 39),
            new Rectangle(1467, 3, 39, 39)
        };

        public static readonly Rectangle[] PlayerUp = new[]
        {
            new Rectangle(1371, 99, 39, 39),
            new Rectangle(1419, 99, 39, 39),
            new Rectangle(1467, 3, 39, 39)
        };

        public static readonly Rectangle[] PlayerLeft = new[]
        {
            new Rectangle(1371, 51, 39, 39),
            new Rectangle(1419, 51, 39, 39),
            new Rectangle(1467, 3, 39, 39)
        };

        public static readonly Rectangle[] PlayerRight = new[]
        {
            new Rectangle(1371, 3, 39, 39),
            new Rectangle(1419, 3, 39, 39),
            new Rectangle(1467, 3, 39, 39)
        };

        public static readonly Rectangle[] PlayerDeath = new[]
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

        // Blinky (Red Ghost) sprites
        public static readonly Rectangle[] BlinkyRight = new[]
        {
            new Rectangle(1371, 195, 42, 42),
            new Rectangle(1419, 195, 42, 42)
        };

        public static readonly Rectangle[] BlinkyLeft = new[]
        {
            new Rectangle(1467, 195, 42, 42),
            new Rectangle(1515, 195, 42, 42)
        };

        public static readonly Rectangle[] BlinkyUp = new[]
        {
            new Rectangle(1563, 195, 42, 42),
            new Rectangle(1611, 195, 42, 42)
        };

        public static readonly Rectangle[] BlinkyDown = new[]
        {
            new Rectangle(1659, 195, 42, 42),
            new Rectangle(1707, 195, 42, 42)
        };

        // Pinky (Pink Ghost) sprites
        public static readonly Rectangle[] PinkyRight = new[]
        {
            new Rectangle(1371, 243, 42, 42),
            new Rectangle(1419, 243, 42, 42)
        };

        public static readonly Rectangle[] PinkyLeft = new[]
        {
            new Rectangle(1467, 243, 42, 42),
            new Rectangle(1515, 243, 42, 42)
        };

        public static readonly Rectangle[] PinkyUp = new[]
        {
            new Rectangle(1563, 243, 42, 42),
            new Rectangle(1611, 243, 42, 42)
        };

        public static readonly Rectangle[] PinkyDown = new[]
        {
            new Rectangle(1659, 243, 42, 42),
            new Rectangle(1707, 243, 42, 42)
        };

        // Inky (Cyan Ghost) sprites
        public static readonly Rectangle[] InkyRight = new[]
        {
            new Rectangle(1371, 291, 42, 42),
            new Rectangle(1419, 291, 42, 42)
        };

        public static readonly Rectangle[] InkyLeft = new[]
        {
            new Rectangle(1467, 291, 42, 42),
            new Rectangle(1515, 291, 42, 42)
        };

        public static readonly Rectangle[] InkyUp = new[]
        {
            new Rectangle(1563, 291, 42, 42),
            new Rectangle(1611, 291, 42, 42)
        };

        public static readonly Rectangle[] InkyDown = new[]
        {
            new Rectangle(1659, 291, 42, 42),
            new Rectangle(1707, 291, 42, 42)
        };

        // Clyde (Orange Ghost) sprites
        public static readonly Rectangle[] ClydeRight = new[]
        {
            new Rectangle(1371, 339, 42, 42),
            new Rectangle(1419, 339, 42, 42)
        };

        public static readonly Rectangle[] ClydeLeft = new[]
        {
            new Rectangle(1467, 339, 42, 42),
            new Rectangle(1515, 339, 42, 42)
        };

        public static readonly Rectangle[] ClydeUp = new[]
        {
            new Rectangle(1563, 339, 42, 42),
            new Rectangle(1611, 339, 42, 42)
        };

        public static readonly Rectangle[] ClydeDown = new[]
        {
            new Rectangle(1659, 339, 42, 42),
            new Rectangle(1707, 339, 42, 42)
        };

        // Frightened ghost sprites
        public static readonly Rectangle[] GhostFrightened = new[]
        {
            new Rectangle(1755, 195, 42, 42),
            new Rectangle(1803, 195, 42, 42)
        };

        public static readonly Rectangle[] GhostFrightenedEnd = new[]
        {
            new Rectangle(1755, 195, 42, 42),
            new Rectangle(1803, 195, 42, 42),
            new Rectangle(1851, 195, 42, 42),
            new Rectangle(1899, 195, 42, 42),
            new Rectangle(1851, 195, 42, 42),
            new Rectangle(1899, 195, 42, 42)
        };

        // Eaten ghost sprites (eyes only)
        public static readonly Rectangle GhostEatenRight = new Rectangle(1755, 243, 42, 42);
        public static readonly Rectangle GhostEatenLeft = new Rectangle(1803, 243, 42, 42);
        public static readonly Rectangle GhostEatenUp = new Rectangle(1851, 243, 42, 42);
        public static readonly Rectangle GhostEatenDown = new Rectangle(1899, 243, 42, 42);

        // Snack sprites
        public static readonly Rectangle SmallSnack = new Rectangle(33, 33, 6, 6);
        public static readonly Rectangle BigSnack = new Rectangle(24, 72, 24, 24);

        // Background
        public static readonly Rectangle Background = new Rectangle(684, 0, 672, 744);
    }
}
