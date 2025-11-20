using System;
using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Graphics;

public class AnimatedSprite : Sprite 
{
    private int currentFrame;
    private TimeSpan elapsed;
    private Animation animation;

    /// <summary>
    /// Gets or Sets the animation for this animated sprite.
    /// </summary>
    public Animation Animation
    {
        get => animation;
        set
        {
            animation = value;
            Region = animation.Frames[0];
        }
    }
    /// <summary>
    /// Creates a new animated sprite.
    /// </summary>
    public AnimatedSprite() { }

    /// <summary>
    /// Creates a new animated sprite with the specified frames and delay.
    /// </summary>
    /// <param name="animation">The animation for this animated sprite.</param>
    public AnimatedSprite(Animation animation)
    {
        Animation = animation;
    }
    /// <summary>
    /// Updates this animated sprite.
    /// </summary>
    /// <param name="gameTime">A snapshot of the game timing values provided by the framework.</param>
    public void Update(GameTime gameTime)
    {
        elapsed += gameTime.ElapsedGameTime;

        if (elapsed >= animation.Delay)
        {
            elapsed -= animation.Delay;
            currentFrame++;

            if (currentFrame >= animation.Frames.Count)
            {
                currentFrame = 0;
            }

            Region = animation.Frames[currentFrame];
        }
    }



}