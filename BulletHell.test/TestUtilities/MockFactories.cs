using BulletHell.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moq;

namespace BulletHell.test.TestUtilities;

/// <summary>
/// Provides factory methods for creating Moq-based mock objects
/// with common configurations
/// </summary>
public static class MockFactories
{
    public static IInputProvider CreateMockInputProvider(
        Vector2? direction = null,
        bool isShootPressed = false)
    {
        var mock = new Mock<IInputProvider>();

        mock.Setup(i => i.GetDirection())
            .Returns(direction ?? Vector2.Zero);

        mock.Setup(i => i.IsShootPressed())
            .Returns(isShootPressed);

        return mock.Object;
    }

    public static ISpriteHelper CreateMockSpriteHelper(
        int width = 32,
        int height = 32)
    {
        var mock = new Mock<ISpriteHelper>();

        mock.SetupGet(s => s.Width).Returns(width);
        mock.SetupGet(s => s.Height).Returns(height);

        return mock.Object;
    }

    /// <summary>
    /// Cannot mock SpriteFont directly with Moq (sealed type).
    /// Tests requiring it should use real instances or integration tests.
    /// </summary>
    public static SpriteFont? CreateMockSpriteFont() => null;

    /// <summary>
    /// Cannot mock Texture2D (requires GraphicsDevice).
    /// </summary>
    public static Texture2D? CreateMockTexture2D(int width = 64, int height = 64) => null;

    /// <summary>
    /// Cannot mock SpriteBatch (requires GraphicsDevice).
    /// </summary>
    public static SpriteBatch? CreateMockSpriteBatch() => null;
}