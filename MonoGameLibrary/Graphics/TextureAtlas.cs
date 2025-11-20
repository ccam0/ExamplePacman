using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGameLibrary.Graphics;

/// <summary>
/// Collection of named sprite regions from a single texture
/// </summary>
public class TextureAtlas
{
    private readonly Dictionary<string, TextureRegion> regions; // Stores named texture regions
    // Stores animations added to this atlas.
    private readonly Dictionary<string, Animation> animations;


    /// <summary>
    /// Source texture for this atlas
    /// </summary>
    public Texture2D Texture { get; set; }

    /// <summary>
    /// Creates an empty texture atlas
    /// </summary>
    public TextureAtlas()
    {
        regions = new Dictionary<string, TextureRegion>();
        animations = new Dictionary<string, Animation>();
    }

    /// <summary>
    /// Creates a texture atlas with the specified texture
    /// </summary>
    public TextureAtlas(Texture2D texture)
    {
        Texture = texture;
        regions = new Dictionary<string, TextureRegion>();
        animations = new Dictionary<string, Animation>();
    }

    /// <summary>
    /// Adds a new region to this atlas
    /// </summary>
    /// <param name="name">Region identifier</param>
    /// <param name="x">X coordinate in source texture</param>
    /// <param name="y">Y coordinate in source texture</param>
    /// <param name="width">Width in pixels</param>
    /// <param name="height">Height in pixels</param>
    public void AddRegion(string name, int x, int y, int width, int height)
    {
        TextureRegion region = new TextureRegion(Texture, x, y, width, height);
        regions.Add(name, region);
    }

    /// <summary>
    /// Gets a region by name
    /// </summary>
    public TextureRegion GetRegion(string name)
    {
        return regions[name];
    }

    /// <summary>
    /// Removes a region by name
    /// </summary>
    public bool RemoveRegion(string name)
    {
        return regions.Remove(name);
    }

    /// <summary>
    /// Removes all regions
    /// </summary>
    public void Clear()
    {
        regions.Clear();
    }

    /// <summary>
    /// Creates a texture atlas from XML configuration
    /// </summary>
    /// <param name="content">Content manager to load textures</param>
    /// <param name="fileName">Path to XML file (relative to content root)</param>
    public static TextureAtlas FromFile(ContentManager content, string fileName)
    {
        TextureAtlas atlas = new TextureAtlas();
        string filePath = Path.Combine(content.RootDirectory, fileName);

        using (Stream stream = TitleContainer.OpenStream(filePath))
        using (XmlReader reader = XmlReader.Create(stream))
        {
            XDocument doc = XDocument.Load(reader);
            XElement root = doc.Root;

            string texturePath = root.Element("Texture").Value;
            atlas.Texture = content.Load<Texture2D>(texturePath);

            // Process <Region> elements with format:
            // <Region name="spriteName" x="0" y="0" width="32" height="32" />
            var regions = root.Element("Regions")?.Elements("Region");

            if (regions != null)
            {
                foreach (var region in regions)
                {
                    string name = region.Attribute("name")?.Value;
                    int x = int.Parse(region.Attribute("x")?.Value ?? "0");
                    int y = int.Parse(region.Attribute("y")?.Value ?? "0");
                    int width = int.Parse(region.Attribute("width")?.Value ?? "0");
                    int height = int.Parse(region.Attribute("height")?.Value ?? "0");

                    if (!string.IsNullOrEmpty(name))
                    {
                        atlas.AddRegion(name, x, y, width, height);
                    }
                }
            }
            // The <Animations> element contains individual <Animation> elements, each one describing
            // a different animation within the atlas.
            //
            // Example:
            // <Animations>
            //      <Animation name="animation" delay="100">
            //          <Frame region="spriteOne" />
            //          <Frame region="spriteTwo" />
            //      </Animation>
            // </Animations>
            //
            // So we retrieve all of the <Animation> elements then loop through each one
            // and generate a new Animation instance from it and add it to this atlas.
            var animationElements = root.Element("Animations").Elements("Animation");

            if (animationElements != null)
            {
                foreach (var animationElement in animationElements)
                {
                    string name = animationElement.Attribute("name")?.Value;
                    float delayInMilliseconds = float.Parse(animationElement.Attribute("delay")?.Value ?? "0");
                    TimeSpan delay = TimeSpan.FromMilliseconds(delayInMilliseconds);

                    List<TextureRegion> frames = new List<TextureRegion>();

                    var frameElements = animationElement.Elements("Frame");

                    if (frameElements != null)
                    {
                        foreach (var frameElement in frameElements)
                        {
                            string regionName = frameElement.Attribute("region").Value;
                            TextureRegion region = atlas.GetRegion(regionName);
                            frames.Add(region);
                        }
                    }

                    Animation animation = new Animation(frames, delay);
                    atlas.AddAnimation(name, animation);
                }
            }

            return atlas;
        }
    }

    /// <summary>
    /// Creates a sprite from a named region
    /// </summary>
    public Sprite CreateSprite(string regionName)
    {
        TextureRegion region = GetRegion(regionName);
        return new Sprite(region);
    }
    
    /// <summary>
    /// Adds the given animation to this texture atlas with the specified name.
    /// </summary>
    /// <param name="animationName">The name of the animation to add.</param>
    /// <param name="animation">The animation to add.</param>
    public void AddAnimation(string animationName, Animation animation)
    {
        animations.Add(animationName, animation);
    }

    /// <summary>
    /// Gets the animation from this texture atlas with the specified name.
    /// </summary>
    /// <param name="animationName">The name of the animation to retrieve.</param>
    /// <returns>The animation with the specified name.</returns>
    public Animation GetAnimation(string animationName)
    {
        return animations[animationName];
    }

    /// <summary>
    /// Removes the animation with the specified name from this texture atlas.
    /// </summary>
    /// <param name="animationName">The name of the animation to remove.</param>
    /// <returns>true if the animation is removed successfully; otherwise, false.</returns>
    public bool RemoveAnimation(string animationName)
    {
        return animations.Remove(animationName);
    }
    /// <summary>
    /// Creates a new animated sprite using the animation from this texture atlas with the specified name.
    /// </summary>
    /// <param name="animationName">The name of the animation to use.</param>
    /// <returns>A new AnimatedSprite using the animation with the specified name.</returns>
    public AnimatedSprite CreateAnimatedSprite(string animationName)
    {
        Animation animation = GetAnimation(animationName);
        return new AnimatedSprite(animation);
    }

    
}