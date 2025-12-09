using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Interfaces;

/// <summary>
/// Interface for sprite helper that manages sprite sheets and animations
/// </summary>
public interface ISpriteHelper : IDisposable
{
    /// <summary>
    /// Gets the width of a single frame
    /// </summary>
    int Width { get; }

    /// <summary>
    /// Gets the height of a single frame
    /// </summary>
    int Height { get; }

    /// <summary>
    /// Loads a sprite sheet texture
    /// </summary>
    void LoadSpriteSheet(Texture2D texture, int frameWidth, int frameHeight, float animationSpeed);

    /// <summary>
    /// Updates the animation
    /// </summary>
    void Update(GameTime gameTime);

    /// <summary>
    /// Draws the current sprite frame with optional color
    /// </summary>
    void Draw(
        SpriteBatch spriteBatch,
        Vector2 position,
        Color? color = null,
        float rotation = 0f,
        float scale = 1f
    );

    /// <summary>
    /// Draws the current sprite frame
    /// </summary>
    void Draw(SpriteBatch spriteBatch, Vector2 position, float rotation = 0f, float scale = 1f);

    bool IsAnimationFinished { get; }
}