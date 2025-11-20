using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pacman.Components
{
    /// <summary>
    /// Improved animation component with better encapsulation
    /// Handles sprite animation timing and rendering
    /// </summary>
    public class Animation
    {
        private float _timer;
        private readonly float _frameTime;
        private Rectangle[] _frames;
        private int _currentFrame;
        private readonly bool _isLooped;
        private bool _isPlaying;

        public int CurrentFrame => _currentFrame;
        public bool IsPlaying
        {
            get => _isPlaying;
            set => _isPlaying = value;
        }

        public Rectangle[] Frames => _frames;

        public Animation(float frameTime, Rectangle[] frames, bool isLooped = true, bool startPlaying = true, int startFrame = 0)
        {
            _frameTime = frameTime;
            _frames = frames;
            _isLooped = isLooped;
            _isPlaying = startPlaying;
            _currentFrame = startFrame;
            _timer = 0;
        }

        public void SetFrames(Rectangle[] newFrames)
        {
            if (newFrames.Length != _frames.Length)
                _currentFrame = 0;
            _frames = newFrames;
        }

        public void SetFrame(int frameIndex)
        {
            _currentFrame = MathHelper.Clamp(frameIndex, 0, _frames.Length - 1);
        }

        public void Start()
        {
            _isPlaying = true;
            _currentFrame = 0;
            _timer = 0;
        }

        public void Stop()
        {
            _isPlaying = false;
        }

        public void Reset()
        {
            _currentFrame = 0;
            _timer = 0;
        }

        public void Update(GameTime gameTime)
        {
            if (!_isPlaying || _frames.Length == 0)
                return;

            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_timer >= _frameTime)
            {
                _timer -= _frameTime;
                _currentFrame++;

                if (_currentFrame >= _frames.Length)
                {
                    if (_isLooped)
                    {
                        _currentFrame = 0;
                    }
                    else
                    {
                        _currentFrame = _frames.Length - 1;
                        _isPlaying = false;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D spriteSheet, Vector2 position)
        {
            if (_isPlaying && _frames.Length > 0)
            {
                spriteBatch.Draw(spriteSheet, position, _frames[_currentFrame], Color.White);
            }
        }
    }
}
