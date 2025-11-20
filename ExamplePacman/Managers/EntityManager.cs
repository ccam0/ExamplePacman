using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pacman.Core;
using Pacman.Entities;
using System.Collections.Generic;

namespace Pacman.Managers
{
    /// <summary>
    /// Manages all game entities (player and ghosts)
    /// Centralizes entity lifecycle management
    /// </summary>
    public class EntityManager
    {
        private readonly GridManager _gridManager;
        private readonly AudioManager _audioManager;

        public Player Player { get; private set; }
        public List<Ghost> Ghosts { get; private set; }

        private float _ghostInitialTimer;
        private float _ghostScatterTimer;
        private float _ghostChaseTimer;
        private GhostState _globalGhostState;
        public int GhostScoreMultiplier { get; set; }

        public EntityManager(GridManager gridManager, AudioManager audioManager)
        {
            _gridManager = gridManager;
            _audioManager = audioManager;
            Ghosts = new List<Ghost>();
            _globalGhostState = GhostState.Scatter;
            GhostScoreMultiplier = 1;
        }

        public void Initialize(Texture2D spriteSheet)
        {
            // Create player
            Player = new Player(
                GameConstants.PlayerStartTile,
                _gridManager.TileArray,
                _audioManager
            );

            // Create ghosts
            Ghosts.Clear();
            Ghosts.Add(new Blinky(GameConstants.BlinkyStartTile, _gridManager.TileArray));
            Ghosts.Add(new Pinky(GameConstants.PinkyStartTile, _gridManager.TileArray));
            Ghosts.Add(new Inky(GameConstants.InkyStartTile, _gridManager.TileArray));
            Ghosts.Add(new Clyde(GameConstants.ClydeStartTile, _gridManager.TileArray));
        }

        public void Update(GameTime gameTime)
        {
            // Update player
            Player.Update(gameTime);

            // Update ghost timers and states
            UpdateGhostBehavior(gameTime);

            // Update each ghost
            var blinkyPosition = (Ghosts[0] as Blinky)?.CurrentTile ?? Vector2.Zero;

            foreach (var ghost in Ghosts)
            {
                // Handle staggered ghost releases
                if (ShouldUpdateGhost(ghost))
                {
                    ghost.Update(gameTime, _gridManager, Player, blinkyPosition);
                }
                else
                {
                    ghost.UpdateAnimation(gameTime);
                }
            }
        }

        private bool ShouldUpdateGhost(Ghost ghost)
        {
            // Blinky and Pinky start immediately
            if (ghost is Blinky || ghost is Pinky)
                return true;

            // Inky starts after half the initial timer
            if (ghost is Inky && _ghostInitialTimer > GameConstants.GhostInitialTimerLength / 2)
                return true;

            // Clyde starts after the full initial timer
            if (ghost is Clyde && _ghostInitialTimer > GameConstants.GhostInitialTimerLength)
                return true;

            return false;
        }

        private void UpdateGhostBehavior(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update initial timer
            if (_ghostInitialTimer < GameConstants.GhostInitialTimerLength)
            {
                _ghostInitialTimer += deltaTime;
                return;
            }

            // Check if any ghost is in special state
            bool hasSpecialState = false;
            foreach (var ghost in Ghosts)
            {
                if (ghost.State == GhostState.Frightened || ghost.State == GhostState.Eaten)
                {
                    hasSpecialState = true;
                    break;
                }
            }

            if (hasSpecialState)
                return;

            // Switch between scatter and chase
            if (_globalGhostState == GhostState.Scatter)
            {
                _ghostScatterTimer += deltaTime;
                if (_ghostScatterTimer > GameConstants.GhostScatterTimerLength)
                {
                    _ghostScatterTimer = 0;
                    _globalGhostState = GhostState.Chase;
                    SetAllGhostStates(GhostState.Chase);
                }
            }
            else if (_globalGhostState == GhostState.Chase)
            {
                _ghostChaseTimer += deltaTime;
                if (_ghostChaseTimer > GameConstants.GhostChaseTimerLength)
                {
                    _ghostChaseTimer = 0;
                    _globalGhostState = GhostState.Scatter;
                    SetAllGhostStates(GhostState.Scatter);
                }
            }
        }

        public void SetAllGhostsFrightened()
        {
            foreach (var ghost in Ghosts)
            {
                if (ghost.State != GhostState.Eaten)
                {
                    ghost.SetState(GhostState.Frightened);
                }
            }
            GhostScoreMultiplier = 1;
        }

        private void SetAllGhostStates(GhostState state)
        {
            foreach (var ghost in Ghosts)
            {
                if (ghost.State != GhostState.Eaten && ghost.State != GhostState.Frightened)
                {
                    ghost.SetState(state);
                }
            }
        }

        public void ResetEntities()
        {
            // Reset player
            var playerTile = _gridManager.TileArray[
                (int)GameConstants.PlayerStartTile.X,
                (int)GameConstants.PlayerStartTile.Y
            ];
            Player.Reset(playerTile.Position + new Vector2(GameConstants.PlayerPositionOffsetX, 0));

            // Reset ghosts
            ResetGhosts();

            // Reset timers
            _ghostInitialTimer = 0;
            _ghostScatterTimer = 0;
            _ghostChaseTimer = 0;
            _globalGhostState = GhostState.Scatter;
            GhostScoreMultiplier = 1;
        }

        private void ResetGhosts()
        {
            foreach (var ghost in Ghosts)
            {
                Vector2 startTile = ghost switch
                {
                    Blinky => GameConstants.BlinkyStartTile,
                    Pinky => GameConstants.PinkyStartTile,
                    Inky => GameConstants.InkyStartTile,
                    Clyde => GameConstants.ClydeStartTile,
                    _ => Vector2.Zero
                };

                var tile = _gridManager.TileArray[(int)startTile.X, (int)startTile.Y];
                ghost.Reset(tile.Position + new Vector2(GameConstants.GhostPositionOffsetX, 0), startTile);
                ghost.SetState(GhostState.Scatter);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D spriteSheet)
        {
            // Draw player if not in death animation
            Player.Draw(spriteBatch);

            // Draw ghosts
            foreach (var ghost in Ghosts)
            {
                ghost.Draw(spriteBatch);
            }
        }

        public bool AreAllGhostsNormal()
        {
            foreach (var ghost in Ghosts)
            {
                if (ghost.State == GhostState.Frightened || ghost.State == GhostState.Eaten)
                    return false;
            }
            return true;
        }
    }
}
