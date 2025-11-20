using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pacman.Core;
using Pacman.Managers;

namespace Pacman.States
{
    /// <summary>
    /// Main gameplay state implementation
    /// Coordinates all game systems during active play
    /// </summary>
    public class PlayState : IGameState
    {
        private readonly Game _game;
        private readonly GameStateManager _stateManager;
        private readonly ResourceManager _resourceManager;
        private readonly AudioManager _audioManager;
        private readonly GridManager _gridManager;
        private readonly EntityManager _entityManager;
        private readonly CollisionManager _collisionManager;

        private float _pauseTimer;
        private bool _hasPauseJustEnded;
        private bool _hasPassedInitialSong;
        private Vector2 _deathAnimationPosition;
        private Text _textRenderer;
        private int _score;

        public PlayState(
            Game game,
            GameStateManager stateManager,
            ResourceManager resourceManager,
            AudioManager audioManager,
            GridManager gridManager,
            EntityManager entityManager,
            CollisionManager collisionManager)
        {
            _game = game;
            _stateManager = stateManager;
            _resourceManager = resourceManager;
            _audioManager = audioManager;
            _gridManager = gridManager;
            _entityManager = entityManager;
            _collisionManager = collisionManager;
        }

        public void Enter()
        {
            _score = 0;
            _pauseTimer = GameConstants.GameStartSongLength;
            _hasPassedInitialSong = false;
            _hasPauseJustEnded = false;

            _textRenderer = new Text(new SpriteSheet(_resourceManager.TextSprites));

            // Initialize game systems
            _gridManager.Initialize();
            _entityManager.Initialize(_resourceManager.GeneralSprites1);

            // Play start sound
            _audioManager.PlaySound(_audioManager.GameStart);
        }

        public void Exit()
        {
            _audioManager.StopAll();
        }

        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Handle game pause
            if (_pauseTimer > 0)
            {
                _pauseTimer -= deltaTime;
                _hasPassedInitialSong = true;
                _entityManager.Player.DeathAnimation.Update(gameTime);
                _audioManager.Siren1Instance.Stop();
                _hasPauseJustEnded = true;
                return;
            }

            // Resume after pause
            if (_hasPauseJustEnded)
            {
                _audioManager.Siren1Instance.Play();
                _hasPauseJustEnded = false;
            }

            // Update entities
            _entityManager.Update(gameTime);

            // Check for snack collection
            var snackIndex = _collisionManager.CheckPlayerSnackCollision(_entityManager.Player.CurrentTile);
            if (snackIndex != -1)
            {
                HandleSnackCollection(snackIndex);
            }

            // Check for ghost collisions
            if (_collisionManager.CheckPlayerGhostCollision(_entityManager.Player, _entityManager.Ghosts, out var collidingGhost))
            {
                if (collidingGhost.State == GhostState.Frightened)
                {
                    HandleGhostEaten(collidingGhost);
                }
                else if (collidingGhost.State != GhostState.Eaten)
                {
                    HandlePlayerDeath();
                }
            }

            // Check for level completion
            if (_gridManager.Snacks.Count == 0)
            {
                HandleLevelComplete();
            }

            // Check for game over
            if (_entityManager.Player.Lives < 0 && !_entityManager.Player.DeathAnimation.IsPlaying)
            {
                HandleGameOver();
            }
        }

        private void HandleSnackCollection(int snackIndex)
        {
            var snack = _gridManager.Snacks[snackIndex];
            _score += snack.ScoreGain;

            if (snack.SnackType == Snack.SnackType.Big)
            {
                _entityManager.SetAllGhostsFrightened();
                _audioManager.PlaySound(_audioManager.EatFruit);
                _audioManager.PowerPelletInstance.Play();
            }

            _gridManager.Snacks.RemoveAt(snackIndex);
            _audioManager.MunchInstance.Play();
        }

        private void HandleGhostEaten(Entities.Ghost ghost)
        {
            // Calculate score based on multiplier
            int ghostScore = _entityManager.GhostScoreMultiplier switch
            {
                1 => GameConstants.GhostScore1,
                2 => GameConstants.GhostScore2,
                3 => GameConstants.GhostScore3,
                _ => GameConstants.GhostScore4
            };

            _score += ghostScore;
            _entityManager.GhostScoreMultiplier++;

            ghost.SetState(GhostState.Eaten);
            _audioManager.PlaySound(_audioManager.EatGhost);
            _audioManager.RetreatingInstance.Play();
            _audioManager.PowerPelletInstance.Stop();
        }

        private void HandlePlayerDeath()
        {
            _deathAnimationPosition = _entityManager.Player.Position - new Vector2(GameConstants.PlayerRadiusOffset / 2f);
            _entityManager.Player.Die(_deathAnimationPosition);

            _audioManager.PlaySound(_audioManager.Death1);
            _audioManager.MunchInstance.Stop();
            _audioManager.PowerPelletInstance.Stop();
            _audioManager.RetreatingInstance.Stop();

            _pauseTimer = GameConstants.RespawnDelay;
            _entityManager.ResetEntities();
        }

        private void HandleLevelComplete()
        {
            _gridManager.RegenerateSnacks();
            _entityManager.ResetEntities();
            _pauseTimer = GameConstants.WinDelay;

            _audioManager.MunchInstance.Stop();
            _audioManager.PowerPelletInstance.Stop();
            _audioManager.RetreatingInstance.Stop();
        }

        private void HandleGameOver()
        {
            _stateManager.ShowGameOver();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw background
            spriteBatch.Draw(_resourceManager.GeneralSprites1, new Vector2(0, GameConstants.ScoreOffset), SpriteData.Background, Color.White);

            // Draw UI
            _textRenderer.draw(spriteBatch, $"score - {_score}", new Vector2(3, 3), 24, Text.Color.White);

            int displayLives = MathHelper.Max(_entityManager.Player.Lives, 0);
            _textRenderer.draw(spriteBatch, $"lives {displayLives}", new Vector2(500, 3), 24, Text.Color.White);

            // Draw snacks
            foreach (var snack in _gridManager.Snacks)
            {
                snack.Draw(spriteBatch);
            }

            // Draw player (if not in death animation)
            if (!_entityManager.Player.DeathAnimation.IsPlaying)
            {
                _entityManager.Player.Draw(spriteBatch);
            }

            // Draw ghosts (if game has started)
            if (_hasPassedInitialSong || _score == 0)
            {
                if (!_entityManager.Player.DeathAnimation.IsPlaying)
                {
                    _entityManager.Draw(spriteBatch, _resourceManager.GeneralSprites1);
                }
            }

            // Draw death animation
            _entityManager.Player.DrawDeathAnimation(spriteBatch, _deathAnimationPosition);
        }
    }
}
