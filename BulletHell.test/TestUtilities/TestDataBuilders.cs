using BulletHell.Interfaces;
using BulletHell.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NSubstitute;

namespace BulletHell.test.TestUtilities;

/// <summary>
/// Provides builder methods for creating test objects with default values
/// </summary>
public static class TestDataBuilders
{
    public static Player CreateTestPlayer(
        Vector2? position = null,
        IInputProvider? input = null,
        ISpriteHelper? sprite = null)
    {
        return new Player(
            position ?? Vector2.Zero,
            input ?? Substitute.For<IInputProvider>(),
            sprite ?? Substitute.For<ISpriteHelper>()
        );
    }

    public static Bullet<Player> CreateTestBullet(
        Vector2? position = null,
        Vector2? direction = null,
        ISpriteHelper? sprite = null)
    {
        return new Bullet<Player>(
            position ?? Vector2.Zero,
            direction ?? Vector2.UnitY,
            sprite ?? Substitute.For<ISpriteHelper>()
        );
    }

    public static GameTime CreateGameTime(double elapsedSeconds = 1.0 / 60.0)
    {
        return new GameTime(
            TimeSpan.Zero,
            TimeSpan.FromSeconds(elapsedSeconds)
        );
    }

    public static GameTime OneFrame => CreateGameTime(1.0 / 60.0);
}
