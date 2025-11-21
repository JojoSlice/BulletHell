using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Utilities;

/// <summary>
/// Helper methods for drawing UI elements
/// </summary>
public static class DrawingHelpers
{
    /// <summary>
    /// Draws a border around a rectangle
    /// </summary>
    /// <param name="spriteBatch">The sprite batch to draw with</param>
    /// <param name="texture">A 1x1 white texture to draw the border with</param>
    /// <param name="rect">The rectangle to draw the border around</param>
    /// <param name="color">The color of the border</param>
    /// <param name="thickness">The thickness of the border in pixels</param>
    public static void DrawBorder(
        SpriteBatch spriteBatch,
        Texture2D texture,
        Rectangle rect,
        Color color,
        int thickness
    )
    {
        // Top
        spriteBatch.Draw(texture, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
        // Bottom
        spriteBatch.Draw(
            texture,
            new Rectangle(rect.X, rect.Bottom - thickness, rect.Width, thickness),
            color
        );
        // Left
        spriteBatch.Draw(texture, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
        // Right
        spriteBatch.Draw(
            texture,
            new Rectangle(rect.Right - thickness, rect.Y, thickness, rect.Height),
            color
        );
    }
}
