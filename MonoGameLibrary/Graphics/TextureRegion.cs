using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics;

/// <summary>
/// Rectangular region within a texture (individual sprite in a sprite sheet)
/// </summary>
public class TextureRegion
{
    /// <summary>
    /// Source texture this region belongs to
    /// </summary>
    public Texture2D Texture { get; set; }

    /// <summary>
    /// Boundary of this region within the source texture
    /// </summary>
    public Rectangle SourceRectangle { get; set; }

    /// <summary>
    /// Width in pixels
    /// </summary>
    public int Width => SourceRectangle.Width;

    /// <summary>
    /// Height in pixels
    /// </summary>
    public int Height => SourceRectangle.Height;

    /// <summary>
    /// Creates an empty texture region
    /// </summary>
    public TextureRegion() { }

    /// <summary>
    /// Creates a texture region from specified coordinates
    /// </summary>
    /// <param name="texture">Source texture</param>
    /// <param name="x">X position within source texture from top left</param>
    /// <param name="y">Y position within source texture from top left</param>
    /// <param name="width">Width in pixels</param>
    /// <param name="height">Height in pixels</param>
    public TextureRegion(Texture2D texture, int x, int y, int width, int height)
    {
        Texture = texture;
        SourceRectangle = new Rectangle(x, y, width, height);
    }

    /// <summary>
    /// Draws this region with basic parameters
    /// </summary>
    public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color)
    {
        Draw(
            spriteBatch, 
            position, 
            color, 
            0.0f, 
            Vector2.Zero, 
            Vector2.One, 
            SpriteEffects.None, 
            0.0f
            );
    }

    /// <summary>
    /// Draws this region with uniform scale and rotation around specified origin 
    /// </summary>
    public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
    {
        Draw(
            spriteBatch,
            position,
            color,
            rotation,
            origin,
            new Vector2(scale, scale),
            effects,
            layerDepth
            );
    }

    /// <summary>
    /// Draws this region with all rendering parameters
    /// </summary>
    public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, Vector2 origin,
        Vector2 scale, SpriteEffects effects, float layerDepth)
    {
        spriteBatch.Draw(
            Texture,
            position,
            SourceRectangle,
            color,
            rotation,
            origin,
            scale,
            effects,
            layerDepth
        );
    }
}