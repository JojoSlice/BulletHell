using BulletHell.Managers;
using BulletHell.Models;
using Microsoft.Xna.Framework;

namespace BulletHell.test.Managers;

public class EnemyBulletManagerTest(ITestOutputHelper output)
{
    [Fact]
    public void EnemyBulletManager_ShouldCreateBulletAndAddToList()
    {
        // Arrange
        using var bulletManager = new BulletManager<Enemy>();
        var startPosition = new Vector2(10, 10);
        var velocity = new Vector2(10, 10);

        // Act
        bulletManager.CreateBullet(startPosition, velocity);
        var actualList = bulletManager.Bullets;

        // Assert
        Assert.Single(actualList);
        Assert.Equal(startPosition, actualList[0].Position);

        // Output
        output.WriteLine($"Bullet count after CreateBullet: {actualList.Count}");
        output.WriteLine("Result: Bullet successfully created and added ✔");
    }

    [Theory]
    [InlineData(-10, -10)]
    [InlineData(-10, 0)]
    [InlineData(0, -10)]
    public void EnemyBulletManager_ShouldRemoveBulletsSpawningOutOfBounds
        (int x, int y)
    {
        // Arrange
        using var manager = new BulletManager<Enemy>();
        var startPosition = new Vector2(x, y);
        var velocity = new Vector2(10, 10);
        manager.CreateBullet(startPosition, velocity);

        int screenWith = 800;
        int screenHeight = 600;
        var gameTime = new GameTime();

        // Act
        manager.Update(gameTime, screenWith, screenHeight);

        // Assert
        Assert.Empty(manager.Bullets);
    }


    [Fact]
    public void EnemyBulletManager_ShouldUpdateAllBullets()
    {
        // Arrange
        using var manager = new BulletManager<Enemy>();
        var startingPosition = new Vector2(10, 10);
        var velocity = new Vector2(10, 10);

        manager.CreateBullet(startingPosition, velocity);

        var deltaTime = 1 / 60f;
        var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(deltaTime));
        var expectedPos = startingPosition + velocity * deltaTime;

        // Act
        manager.Update(gameTime, 800, 600);
        var actualPos = manager.Bullets[0].Position;

        // Assert
        var precision = 4;
        Assert.Equal(expectedPos.X, actualPos.X, precision);
        Assert.Equal(expectedPos.Y, actualPos.Y, precision);

        // output
        output.WriteLine($"Start position:    {startingPosition}");
        output.WriteLine($"Expected position: {expectedPos.X} - {expectedPos.Y}");
        output.WriteLine($"Actual position: {actualPos.X} - {actualPos.Y}");
        output.WriteLine("Result: EnemyBullet updated correctly ✔");
    }
}
