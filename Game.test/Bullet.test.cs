using BulletHell.Helpers;
using BulletHell.Interfaces;
using BulletHell.Managers;
using BulletHell.Models;
using Microsoft.Xna.Framework;

namespace Game.test;

public class BulletManagerTest
{
    [Fact]
    public void Update_CalledMultipleTimes_ShouldNotThrow()
    {
        var bulletManager = new BulletManager();
        var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1.0 / 60.0));

        for (int i = 0; i < 100; i++)
        {
            var exception = Record.Exception(() => bulletManager.Update(gameTime, 800, 600));
            Assert.Null(exception);
        }
    }
}

public class BulletTest
{
    [Theory]
    [InlineData(-5, 5, true)]
    [InlineData(5, -5, true)]
    [InlineData(105, 50, true)]
    [InlineData(50, 105, true)]
    [InlineData(5, 5, false)]
    public void IsOutOfBounds_ReturnsValidBoolean(int x, int y, bool expected)
    {
        Vector2 startPosition = new(x, y);
        Vector2 direction = new(x, y);
        ISpriteHelper bulletSprite = new SpriteHelper();

        var bullet = new Bullet(startPosition, direction, bulletSprite);

        var actual = bullet.IsOutOfBounds(100, 100);

        Assert.Equal(expected, actual);
    }
}
