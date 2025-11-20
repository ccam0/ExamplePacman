using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pacman
{
    /// <summary>
    /// Main game class that manages game state, entities, and rendering.
    /// </summary>
    public class Game1 : Game
    {
        // Window dimensions
        public const int ScoreOffset = 27;
        public const int WindowHeight = 744 + ScoreOffset;
        public const int WindowWidth = 672;

        // Game entities
        private Maze maze;
        private PacMan pacman;
        private Inky inky;
        private Blinky blinky;
        private Clyde clyde;
        private Pinky pinky;

        // Graphics
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        public static Texture2D GeneralSprites1;
        public static Texture2D GeneralSprites2;
        public static SpriteSheet spriteSheet1;
        public static SpriteSheet spriteSheet2;
        public static Text text;

        // Debug textures
        public static Texture2D debuggingDot;
        public static Texture2D debugLineX;
        public static Texture2D debugLineY;
        public static Texture2D playerDebugLineX;
        public static Texture2D playerDebugLineY;
        public static Texture2D pathfindingDebugLineX;
        public static Texture2D pathfindingDebugLineY;

        // Game state
        public static GameState CurrentGameState = GameState.Menu;
        public static int score;
        public static int GhostScoreMultiplier = 1;
        public static bool HasEatenPowerPellet = false;

        // Timing
        public static float gamePauseTimer;
        public static float gameStartSongLength;
        public static bool hasPassedInitialSong = false;
        private bool hasPauseJustEnded;

        // Ghost management
        private Ghost.GhostState ghostsState = Ghost.GhostState.Scatter;
        private float ghostInitialTimer;
        private const float GhostInitialTimerLength = 2f;
        private float ghostTimerScatter;
        private const float GhostTimerScatterLength = 15f;
        private float ghostTimerChaser;
        private const float GhostTimerChaserLength = 20f;

        // Death animation
        public static SpriteAnimation pacmanDeathAnimation;
        private bool startPacmanDeathAnim = false;
        private Vector2 pacmanDeathPosition;

        private readonly Rectangle backgroundRect = new Rectangle(684, 0, 672, 744);

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = WindowWidth;
            graphics.PreferredBackBufferHeight = WindowHeight;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load sounds
            Sounds.Credit = Content.Load<SoundEffect>("Sounds/credit");
            Sounds.Death1 = Content.Load<SoundEffect>("Sounds/death_1");
            Sounds.Death2 = Content.Load<SoundEffect>("Sounds/death_2");
            Sounds.EatFruit = Content.Load<SoundEffect>("Sounds/eat_fruit");
            Sounds.EatGhost = Content.Load<SoundEffect>("Sounds/eat_ghost");
            Sounds.Extend = Content.Load<SoundEffect>("Sounds/extend");
            Sounds.GameStart = Content.Load<SoundEffect>("Sounds/game_start");
            Sounds.Intermission = Content.Load<SoundEffect>("Sounds/intermission");

            Sounds.Munch = Content.Load<SoundEffect>("Sounds/munch");
            Sounds.MunchInstance = Sounds.Munch.CreateInstance();
            Sounds.MunchInstance.Volume = 0.35f;
            Sounds.MunchInstance.IsLooped = true;

            Sounds.PowerPellet = Content.Load<SoundEffect>("Sounds/power_pellet");
            Sounds.PowerPelletInstance = Sounds.PowerPellet.CreateInstance();
            Sounds.PowerPelletInstance.IsLooped = true;

            Sounds.Retreating = Content.Load<SoundEffect>("Sounds/retreating");
            Sounds.RetreatingInstance = Sounds.Retreating.CreateInstance();
            Sounds.RetreatingInstance.IsLooped = true;

            Sounds.Siren1 = Content.Load<SoundEffect>("Sounds/siren_1");
            Sounds.Siren1Instance = Sounds.Siren1.CreateInstance();
            Sounds.Siren1Instance.Volume = 0.8f;
            Sounds.Siren1Instance.IsLooped = true;

            Sounds.Siren2 = Content.Load<SoundEffect>("Sounds/siren_2");
            Sounds.Siren3 = Content.Load<SoundEffect>("Sounds/siren_3");
            Sounds.Siren4 = Content.Load<SoundEffect>("Sounds/siren_4");
            Sounds.Siren5 = Content.Load<SoundEffect>("Sounds/siren_5");

            // Load graphics
            GeneralSprites1 = Content.Load<Texture2D>("SpriteSheets/GeneralSprites1");
            GeneralSprites2 = Content.Load<Texture2D>("SpriteSheets/GeneralSprites2");
            debuggingDot = Content.Load<Texture2D>("SpriteSheets/debuggingDot");
            debugLineX = Content.Load<Texture2D>("SpriteSheets/debugLineX");
            debugLineY = Content.Load<Texture2D>("SpriteSheets/debugLineY");
            playerDebugLineX = Content.Load<Texture2D>("SpriteSheets/playerDebugLineX");
            playerDebugLineY = Content.Load<Texture2D>("SpriteSheets/playerDebugLineY");
            pathfindingDebugLineX = Content.Load<Texture2D>("SpriteSheets/pathfindingDebugLineX");
            pathfindingDebugLineY = Content.Load<Texture2D>("SpriteSheets/pathfindingDebugLineY");

            spriteSheet1 = new SpriteSheet(GeneralSprites1);
            spriteSheet2 = new SpriteSheet(GeneralSprites2);

            // Menu and UI
            Menu.setPacmanLogo = Content.Load<Texture2D>("SpriteSheets/pac-man-logo");
            Menu.setBasicFont = Content.Load<SpriteFont>("simpleFont");
            GameOver.setBasicFont = Content.Load<SpriteFont>("simpleFont");
            text = new Text(new SpriteSheet(Content.Load<Texture2D>("Spritesheets/TextSprites")));

            // Initialize game
            maze = new Maze(WindowWidth, WindowHeight, ScoreOffset);

            // Create ghosts
            inky = new Inky(11, 14, maze.Tiles);
            blinky = new Blinky(13, 11, maze.Tiles);
            pinky = new Pinky(13, 14, maze.Tiles);
            clyde = new Clyde(15, 14, maze.Tiles);

            // Create Pac-Man
            pacman = new PacMan(13, 23, maze.Tiles);
            pacman.Lives = 4;

            // Create death animation
            pacmanDeathAnimation = new SpriteAnimation(0.278f, PacMan.DeathAnimationRects, 0, false, false);

            // Start game with intro song
            gameStartSongLength = 4.23f;
            gamePauseTimer = gameStartSongLength;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Handle different game states
            switch (CurrentGameState)
            {
                case GameState.GameOver:
                    GameOver.Update();
                    break;

                case GameState.Menu:
                    Menu.Update(gameTime);
                    break;

                case GameState.Normal:
                    UpdateGameplay(gameTime, dt);
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Updates gameplay logic.
        /// </summary>
        private void UpdateGameplay(GameTime gameTime, float dt)
        {
            // Check for game over
            if (pacman.Lives < 0 && !pacmanDeathAnimation.IsPlaying)
            {
                HandleGameOver();
                return;
            }

            // Handle pause timer
            if (gamePauseTimer > 0)
            {
                gamePauseTimer -= dt;
                hasPassedInitialSong = true;
                pacmanDeathAnimation.Update(gameTime);
                Sounds.Siren1Instance.Stop();
                hasPauseJustEnded = true;
                return;
            }

            // Resume after pause
            if (hasPauseJustEnded)
            {
                Sounds.Siren1Instance.Play();
                hasPauseJustEnded = false;
            }

            // Update Pac-Man
            pacman.UpdateTilePosition(maze);
            pacman.Update(gameTime, maze);

            // Update ghosts
            UpdateGhosts(gameTime);

            // Start death animation if triggered
            if (startPacmanDeathAnim)
            {
                startPacmanDeathAnim = false;
                pacmanDeathAnimation.start();
            }

            // Check for level completion
            if (maze.Pellets.Count == 0)
            {
                HandleLevelComplete();
            }
        }

        /// <summary>
        /// Updates all ghosts and handles their behavior.
        /// </summary>
        private void UpdateGhosts(GameTime gameTime)
        {
            // Handle power pellet eaten
            if (HasEatenPowerPellet)
            {
                HasEatenPowerPellet = false;
                SetGhostStates(Ghost.GhostState.Frightened);
                Sounds.PowerPelletInstance.Play();
            }

            // Stop power pellet sound when all ghosts are no longer frightened
            if (inky.State != Ghost.GhostState.Frightened && blinky.State != Ghost.GhostState.Frightened &&
                pinky.State != Ghost.GhostState.Frightened && clyde.State != Ghost.GhostState.Frightened)
            {
                Sounds.PowerPelletInstance.Stop();
                GhostScoreMultiplier = 1;
            }

            // Stop retreating sound when no ghosts are eaten
            if (inky.State != Ghost.GhostState.Eaten && blinky.State != Ghost.GhostState.Eaten &&
                pinky.State != Ghost.GhostState.Eaten && clyde.State != Ghost.GhostState.Eaten)
            {
                Sounds.RetreatingInstance.Stop();
            }

            // Handle ghost initial timer
            if (ghostInitialTimer < GhostInitialTimerLength)
            {
                ghostInitialTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                clyde.Animation.Update(gameTime);
                inky.Animation.Update(gameTime);
            }

            // Update Inky after half initial timer
            if (ghostInitialTimer > GhostInitialTimerLength / 2 && ghostInitialTimer < GhostInitialTimerLength)
            {
                inky.Update(gameTime, maze, pacman.CurrentTile, pacman.CurrentDirection, blinky.CurrentTile);
            }
            // After initial timer, update Clyde and Inky normally and switch states
            else if (ghostInitialTimer > GhostInitialTimerLength)
            {
                clyde.Update(gameTime, maze, pacman.CurrentTile, pacman.CurrentDirection, blinky.CurrentTile);
                inky.Update(gameTime, maze, pacman.CurrentTile, pacman.CurrentDirection, blinky.CurrentTile);
                SwitchGhostStates(gameTime);
            }

            // Always update Pinky and Blinky
            pinky.Update(gameTime, maze, pacman.CurrentTile, pacman.CurrentDirection, blinky.CurrentTile);
            blinky.Update(gameTime, maze, pacman.CurrentTile, pacman.CurrentDirection, blinky.CurrentTile);

            // Check for collisions
            if (inky.IsColliding || blinky.IsColliding || pinky.IsColliding || clyde.IsColliding)
            {
                KillPacman();
                inky.IsColliding = false;
                blinky.IsColliding = false;
                pinky.IsColliding = false;
                clyde.IsColliding = false;
            }
        }

        /// <summary>
        /// Switches ghost states between scatter and chase.
        /// </summary>
        private void SwitchGhostStates(GameTime gameTime)
        {
            // Don't switch if any ghost is frightened or eaten
            if (inky.State == Ghost.GhostState.Frightened || inky.State == Ghost.GhostState.Eaten ||
                blinky.State == Ghost.GhostState.Frightened || blinky.State == Ghost.GhostState.Eaten ||
                clyde.State == Ghost.GhostState.Frightened || clyde.State == Ghost.GhostState.Eaten ||
                pinky.State == Ghost.GhostState.Frightened || pinky.State == Ghost.GhostState.Eaten)
            {
                return;
            }

            if (ghostsState == Ghost.GhostState.Scatter)
            {
                ghostTimerScatter += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (ghostTimerScatter > GhostTimerScatterLength)
                {
                    ghostTimerScatter = 0;
                    ghostsState = Ghost.GhostState.Chase;
                    SetGhostStates(Ghost.GhostState.Chase);
                }
            }
            else if (ghostsState == Ghost.GhostState.Chase)
            {
                ghostTimerChaser += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (ghostTimerChaser > GhostTimerChaserLength)
                {
                    ghostTimerChaser = 0;
                    ghostsState = Ghost.GhostState.Scatter;
                    SetGhostStates(Ghost.GhostState.Scatter);
                }
            }
        }

        /// <summary>
        /// Sets the state for all ghosts.
        /// </summary>
        private void SetGhostStates(Ghost.GhostState state)
        {
            // Set speeds
            if (state == Ghost.GhostState.Chase || state == Ghost.GhostState.Scatter || state == Ghost.GhostState.Eaten)
            {
                inky.Speed = Ghost.NormalSpeed;
                blinky.Speed = Ghost.NormalSpeed;
                pinky.Speed = Ghost.NormalSpeed;
                clyde.Speed = Ghost.NormalSpeed;
            }
            else
            {
                if (inky.State != Ghost.GhostState.Eaten)
                    inky.Speed = Ghost.FrightenedSpeed;
                if (blinky.State != Ghost.GhostState.Eaten)
                    blinky.Speed = Ghost.FrightenedSpeed;
                if (pinky.State != Ghost.GhostState.Eaten)
                    pinky.Speed = Ghost.FrightenedSpeed;
                if (clyde.State != Ghost.GhostState.Eaten)
                    clyde.Speed = Ghost.FrightenedSpeed;
            }

            // Set states (don't override eaten state)
            if (inky.State != Ghost.GhostState.Eaten)
                inky.State = state;
            if (blinky.State != Ghost.GhostState.Eaten)
                blinky.State = state;
            if (pinky.State != Ghost.GhostState.Eaten)
                pinky.State = state;
            if (clyde.State != Ghost.GhostState.Eaten)
                clyde.State = state;
        }

        /// <summary>
        /// Kills Pac-Man and resets positions.
        /// </summary>
        private void KillPacman()
        {
            pacman.Lives--;
            startPacmanDeathAnim = true;
            pacmanDeathPosition = pacman.GetDeathAnimationPosition();

            Sounds.Death1.Play();
            gamePauseTimer = 4f;

            ResetGhosts();
            pacman.Reset(maze);

            ghostTimerChaser = 0;
            ghostTimerScatter = 0;
            ghostInitialTimer = 0;
            HasEatenPowerPellet = false;

            Sounds.StopAllSounds();
        }

        /// <summary>
        /// Handles level completion.
        /// </summary>
        private void HandleLevelComplete()
        {
            maze.ResetPellets(ScoreOffset);
            ResetGhosts();

            ghostTimerChaser = 0;
            ghostTimerScatter = 0;
            ghostInitialTimer = 0;
            HasEatenPowerPellet = false;

            pacman.Reset(maze);
            Sounds.StopAllSounds();
            gamePauseTimer = 3f;
        }

        /// <summary>
        /// Handles game over.
        /// </summary>
        private void HandleGameOver()
        {
            CurrentGameState = GameState.GameOver;

            hasPassedInitialSong = false;
            score = 0;
            pacmanDeathAnimation.IsPlaying = false;
            gamePauseTimer = gameStartSongLength;
            pacman.Lives = 4;

            maze.ResetPellets(ScoreOffset);
            ResetGhosts();

            ghostTimerChaser = 0;
            ghostTimerScatter = 0;
            ghostInitialTimer = 0;
            HasEatenPowerPellet = false;

            pacman.Reset(maze);
            Sounds.StopAllSounds();
        }

        /// <summary>
        /// Resets all ghosts to starting positions.
        /// </summary>
        private void ResetGhosts()
        {
            ghostInitialTimer = 0;
            SetGhostStates(Ghost.GhostState.Scatter);

            inky.Animation.setSourceRects(inky.RectsUp);
            blinky.Animation.setSourceRects(blinky.RectsLeft);
            pinky.Animation.setSourceRects(pinky.RectsDown);
            clyde.Animation.setSourceRects(clyde.RectsUp);

            inky.FrightenedTimer = 0;
            blinky.FrightenedTimer = 0;
            pinky.FrightenedTimer = 0;
            clyde.FrightenedTimer = 0;

            inky.PathToPacMan = new System.Collections.Generic.List<Vector2>();
            blinky.PathToPacMan = new System.Collections.Generic.List<Vector2>();
            pinky.PathToPacMan = new System.Collections.Generic.List<Vector2>();
            clyde.PathToPacMan = new System.Collections.Generic.List<Vector2>();

            inky.Position = maze.Tiles[11, 14].Position + new Vector2(12, 0);
            blinky.Position = maze.Tiles[13, 11].Position + new Vector2(12, 0);
            pinky.Position = maze.Tiles[13, 14].Position + new Vector2(12, 0);
            clyde.Position = maze.Tiles[15, 14].Position + new Vector2(12, 0);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            switch (CurrentGameState)
            {
                case GameState.Normal:
                    DrawGameplay();
                    break;

                case GameState.GameOver:
                    GameOver.Draw(spriteBatch, text);
                    break;

                case GameState.Menu:
                    Menu.Draw(spriteBatch);
                    break;
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        private void DrawGameplay()
        {
            // Draw background
            spriteSheet1.drawSprite(spriteBatch, backgroundRect, new Vector2(0, ScoreOffset));

            // Draw score and lives
            text.draw(spriteBatch, "score - " + score, new Vector2(3, 3), 24, Text.Color.White);
            string livesText = pacman.Lives >= 0 ? $"lives {pacman.Lives}" : "lives 0";
            text.draw(spriteBatch, livesText, new Vector2(500, 3), 24, Text.Color.White);

            // Draw pellets
            maze.DrawPellets(spriteBatch);

            // Draw Pac-Man
            if (!pacmanDeathAnimation.IsPlaying)
            {
                pacman.Draw(spriteBatch, spriteSheet1);
            }

            // Draw ghosts
            if (hasPassedInitialSong || score == 0)
            {
                if (!pacmanDeathAnimation.IsPlaying)
                {
                    inky.Draw(spriteBatch, spriteSheet1);
                    blinky.Draw(spriteBatch, spriteSheet1);
                    pinky.Draw(spriteBatch, spriteSheet1);
                    clyde.Draw(spriteBatch, spriteSheet1);
                }
            }

            // Draw death animation
            pacmanDeathAnimation.Draw(spriteBatch, spriteSheet1, pacmanDeathPosition);
        }
    }
}
