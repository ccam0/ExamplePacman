using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics;

public class Sprite
{
    public TextureRegion Region { get; set; }

    /// <summary>
    /// Color mask for sprite rendering. Default: Color.White
    /// </summary>
    public Color Color { get; set; } = Color.White;

    /// <summary>
    /// Rotation in radians. Default: 0.0f
    /// </summary>
    public float Rotation { get; set; } = 0.0f;

    /// <summary>
    /// Scale factor for x and y axes. Default: Vector2.One
    /// </summary>
    public Vector2 Scale { get; set; } = Vector2.One;

    /// <summary>
    /// Origin point relative to top-left. Default: Vector2.Zero
    /// </summary>
    public Vector2 Origin { get; set; } = Vector2.Zero;

    /// <summary>
    /// Sprite effects for rendering. Default: SpriteEffects.None
    /// </summary>
    public SpriteEffects Effects { get; set; } = SpriteEffects.None;

    /// <summary>
    /// Layer depth for rendering. Default: 0.0f
    /// </summary>
    public float LayerDepth { get; set; } = 0.0f;

    /// <summary>
    /// Width in pixels (Region.Width * Scale.X)
    /// </summary>
    public float Width => Region.Width * Scale.X;

    /// <summary>
    /// Height in pixels (Region.Height * Scale.Y)
    /// </summary>
    public float Height => Region.Height * Scale.Y;


    public Sprite() { }
    
    public Sprite(TextureRegion region)
    {
        Region = region;
    }

    public void CenterOrigin()
    {
        Origin = new Vector2(Region.Width, Region.Height) * 0.5f;
    }
    /// <summary>
    /// Submit this sprite for drawing to the current batch.
    /// </summary>
    /// <param name="spriteBatch">The SpriteBatch instance used for batching draw calls.</param>
    /// <param name="position">The xy-coordinate position to render this sprite at.</param>
    public void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        Region.Draw(spriteBatch, position, Color, Rotation, Origin, Scale, Effects, LayerDepth);
    }
    
    public void Draw(SpriteBatch spriteBatch, Vector2 position, float layerDepth, float scale)
    {
        Region.Draw(spriteBatch, position, Color, Rotation, Origin, scale, Effects, layerDepth);
    }
}