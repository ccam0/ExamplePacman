using Microsoft.Xna.Framework;

namespace Pacman.Core
{
    /// <summary>
    /// Centralized game constants to eliminate magic numbers
    /// </summary>
    public static class GameConstants
    {
        // Window dimensions
        public const int ScoreOffset = 27;
        public const int WindowHeight = 744 + ScoreOffset;
        public const int WindowWidth = 672;

        // Grid dimensions
        public const int NumberOfTilesX = 28;
        public const int NumberOfTilesY = 31;
        public const int TileWidth = WindowWidth / NumberOfTilesX;
        public const int TileHeight = (WindowHeight - ScoreOffset) / NumberOfTilesY;

        // Movement speeds
        public const int PlayerSpeed = 150;
        public const int GhostNormalSpeed = 140;
        public const int GhostFrightenedSpeed = 90;
        public const int GhostEatenSpeed = 240;

        // Timing constants
        public const float GameStartSongLength = 4.23f;
        public const float PlayerMoveThreshold = 0.2f;
        public const float GhostInitialTimerLength = 2f;
        public const float GhostScatterTimerLength = 15f;
        public const float GhostChaseTimerLength = 20f;
        public const float GhostFrightenedTimerLength = 8f;
        public const float GhostFrightenedWarningTime = 5f;
        public const float DeathAnimationDuration = 0.278f;
        public const float PlayerAnimationSpeed = 0.08f;
        public const float GhostAnimationSpeed = 0.08f;
        public const float RespawnDelay = 4f;
        public const float WinDelay = 3f;

        // Score values
        public const int SmallSnackScore = 10;
        public const int BigSnackScore = 50;
        public const int GhostScore1 = 200;
        public const int GhostScore2 = 400;
        public const int GhostScore3 = 800;
        public const int GhostScore4 = 1600;

        // Offsets and sizes
        public const int PlayerRadiusOffset = 19;
        public const int GhostDrawOffsetX = -9;
        public const int GhostDrawOffsetY = -9;
        public const int SmallSnackRadiusOffset = 3;
        public const int BigSnackRadiusOffset = 12;
        public const int TeleportThreshold = 30;

        // Initial positions
        public static readonly Vector2 PlayerStartTile = new Vector2(13, 23);
        public static readonly Vector2 BlinkyStartTile = new Vector2(13, 11);
        public static readonly Vector2 PinkyStartTile = new Vector2(13, 14);
        public static readonly Vector2 InkyStartTile = new Vector2(11, 14);
        public static readonly Vector2 ClydeStartTile = new Vector2(15, 14);
        public static readonly Vector2 GhostHouseExitTile = new Vector2(13, 11);
        public static readonly Vector2 GhostHouseTargetTile = new Vector2(13, 14);

        // Teleport positions
        public static readonly Vector2 LeftTeleportTile = new Vector2(0, 14);
        public static readonly Vector2 RightTeleportTile = new Vector2(NumberOfTilesX - 1, 14);

        // Scatter targets (corners)
        public static readonly Vector2 BlinkyScatterTarget = new Vector2(25, 0);
        public static readonly Vector2 PinkyScatterTarget = new Vector2(2, 0);
        public static readonly Vector2 InkyScatterTarget = new Vector2(27, 30);
        public static readonly Vector2 ClydeScatterTarget = new Vector2(0, 30);

        // Gameplay
        public const int InitialLives = 4;
        public const int PlayerPositionOffsetX = 14;
        public const int GhostPositionOffsetX = 12;
    }
}
