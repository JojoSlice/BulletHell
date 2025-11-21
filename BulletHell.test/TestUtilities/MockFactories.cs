using BulletHell.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NSubstitute;

namespace BulletHell.test.TestUtilities;

/// <summary>
/// Provides factory methods for creating mock objects with common configurations
/// </summary>
public static class MockFactories
{
    public static IInputProvider CreateMockInputProvider(
        Vector2? direction = null,
        bool isShootPressed = false)
    {
        var mock = Substitute.For<IInputProvider>();
        mock.GetDirection().Returns(direction ?? Vector2.Zero);
        mock.IsShootPressed().Returns(isShootPressed);
        return mock;
    }

    public static ISpriteHelper CreateMockSpriteHelper(
        int width = 32,
        int height = 32)
    {
        var mock = Substitute.For<ISpriteHelper>();
        mock.Width.Returns(width);
        mock.Height.Returns(height);
        return mock;
    }

    /// <summary>
    /// Note: Cannot mock SpriteFont directly due to MonoGame limitations.
    /// This returns null as a placeholder for tests that check for null handling.
    /// </summary>
    public static SpriteFont? CreateMockSpriteFont()
    {
        // Cannot mock SpriteFont - it's sealed
        return null;
    }

    /// <summary>
    /// Note: Cannot mock Texture2D directly due to MonoGame limitations.
    /// Tests requiring real textures should use integration tests or skip.
    /// This returns null as a placeholder for tests that check for null handling.
    /// </summary>
    public static Texture2D? CreateMockTexture2D(
        int width = 64,
        int height = 64)
    {
        // Cannot mock Texture2D/GraphicsDevice with NSubstitute
        // Tests that need real textures should be integration tests
        return null;
    }

    /// <summary>
    /// Note: Cannot mock SpriteBatch directly due to MonoGame limitations.
    /// Tests requiring real SpriteBatch should use integration tests or skip.
    /// This returns null as a placeholder for tests that check for null handling.
    /// </summary>
    public static SpriteBatch? CreateMockSpriteBatch()
    {
        // Cannot mock SpriteBatch/GraphicsDevice with NSubstitute
        // Tests that need real SpriteBatch should be integration tests
        return null;
    }
}
