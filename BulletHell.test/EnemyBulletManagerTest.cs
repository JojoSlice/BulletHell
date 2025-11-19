using BulletHell.Configurations;
using BulletHell.Helpers;
using BulletHell.Interfaces;
using BulletHell.Managers;
using BulletHell.Models;
using Microsoft.Xna.Framework;
using NSubstitute;
using Vector2 = System.Numerics.Vector2;

namespace BulletHell.test;

public class EnemyBulletManagerTest(ITestOutputHelper output)
{
    [Fact]
    public void EnemyBulletManager_ShouldAddBulletToList()
    {
        // Arrange
        var bulletManager = new EnemyBulletManager();
        var startPosition = new Vector2(10, 10);
        var velocity = new Vector2(10, 10);
        var enemyBullet = new EnemyBullet(startPosition, velocity);
        var expected = enemyBullet;

        // Act
        bulletManager.AddBullet(enemyBullet);
        var actualList = bulletManager.Bullets;
        ;

        // Assert
        Assert.Single(actualList);
        Assert.Equal(expected, actualList[0]);

        // Output
        output.WriteLine($"Bullet count after AddBullet: {actualList.Count}");
        output.WriteLine("Result: Bullet successfully added ✔");
    }

    [Theory]
    [InlineData(-10, -10)]
    [InlineData(-10, 0)]
    [InlineData(0, -10)]
    public void EnemyBulletManager_ShouldRemoveBulletsSpawningOutOfBounds
        (int x, int y)
    {
        // Arrange
        var manager = new EnemyBulletManager();
        var startPosition = new Vector2(x, y);
        var velocity = new Vector2(10, 10);
        var bullet = new EnemyBullet(startPosition, velocity);
        manager.AddBullet(bullet);

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
        var manager = new EnemyBulletManager();
        var startingPosition = new Vector2(10, 10);
        var velocity = new Vector2(10, 10);
        var enemyBullet = new EnemyBullet(startingPosition, velocity);
        
        manager.AddBullet(enemyBullet);

        var deltaTime = 1 / 60f;
        var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(deltaTime));
        var expectedPos =  startingPosition + velocity * deltaTime;
        
        // Act
        manager.Update(gameTime, 800, 600);
        var actualPos = enemyBullet.Position;

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