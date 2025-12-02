using BulletHell.Configurations;
using BulletHell.Interfaces;
using BulletHell.Managers;
using BulletHell.Models;
using Microsoft.Xna.Framework;
using NSubstitute;

namespace BulletHell.test.Managers;

public class EnemyManagerTest
{
    private readonly ITestOutputHelper _output;

    public EnemyManagerTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Theory]
    [InlineData(-50, 100)]   // Left of screen
    [InlineData(900, 100)]   // Right of screen (800 wide)
    [InlineData(100, 700)]   // Below screen (600 high)
    public void EnemyManager_ShouldRemoveOutOfBoundsEnemies(float x, float y)
    {
        // Arrange
        var sprite = Substitute.For<ISpriteHelper>();
        var enemy = new Enemy(new Vector2(x, y), sprite);

        var bulletManager = new BulletManager<Enemy>();
        var manager = new EnemyManager(bulletManager);

        manager.AddEnemy(enemy);

        int screenWidth = 800;
        int screenHeight = 600;

        int expectedCount = 0;

        // Act
        manager.Update(new GameTime(), screenWidth, screenHeight);
        int actualCount = manager.Enemies.Count;

        // Assert
        Assert.Equal(expectedCount, actualCount);

        // Output
        _output.WriteLine($"Enemy pos:  ({x}, {y})");
        _output.WriteLine($"Expected count:   {expectedCount}");
        _output.WriteLine($"Actual count:     {actualCount}");
        _output.WriteLine("Result: Enemy removed ✔");
    }

    [Theory]
    [InlineData(100, 100)]
    [InlineData(799, 599)]
    [InlineData(400, 0)]
    [InlineData(0, 300)]
    public void EnemyManager_ShouldKeepEnemy_WhenInsideBounds(float x, float y)
    {
        // Arrange
        var sprite = Substitute.For<ISpriteHelper>();
        var enemy = new Enemy(new Vector2(x, y), sprite);

        var bulletManager = new BulletManager<Enemy>();
        var manager = new EnemyManager(bulletManager);

        manager.AddEnemy(enemy);

        int screenWidth = 800;
        int screenHeight = 600;

        int expectedCount = 1;

        // Act
        manager.Update(new GameTime(), screenWidth, screenHeight);
        int actualCount = manager.Enemies.Count;

        // Assert
        Assert.Equal(expectedCount, actualCount);

        // Output
        _output.WriteLine($"Enemy pos:  ({x}, {y})");
        _output.WriteLine($"Expected count:   {expectedCount}");
        _output.WriteLine($"Actual count:     {actualCount}");
        _output.WriteLine("Result: Enemy spoted! ✔");
    }

    [Fact]
    public void EnemyManager_ShouldUpdateAllEnemies()
    {
        // Arrange
        var sprite = Substitute.For<ISpriteHelper>();
        var enemy = new Enemy(new Vector2(0, 0), sprite);

        var startPosition = enemy.Position; // endast för output

        var bulletManager = new BulletManager<Enemy>();
        var manager = new EnemyManager(bulletManager);

        manager.AddEnemy(enemy);

        var deltaTime = 1 / 60f;
        var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(deltaTime));
        var expectedPosition = new Vector2(0, EnemyConfig.Speed * deltaTime);
        int precision = 4;

        // Act
        manager.Update(gameTime, 800, 600);
        var actual = enemy.Position;

        // Assert
        Assert.Equal(expectedPosition.X, actual.X, precision);
        Assert.Equal(expectedPosition.Y, actual.Y, precision);

        // output
        _output.WriteLine($"Start position:    {startPosition}");
        _output.WriteLine($"Expected position: {expectedPosition}");
        _output.WriteLine($"Actual position:   {actual}");
        _output.WriteLine("Result: Enemy updated correctly ✔");

    }

    [Fact]
    public void EnemyManager_TryShootEnemies_ShouldCreateBulletsViaBulletManager()
    {
        // Arrange
        var bulletManager = new BulletManager<Enemy>();
        var manager = new EnemyManager(bulletManager);

        var sprite = Substitute.For<ISpriteHelper>();
        var enemy = new Enemy(new Vector2(100, 100), sprite);

        manager.AddEnemy(enemy);

        // Act
        manager.TryShootEnemies();

        // Assert
        Assert.NotEmpty(bulletManager.Bullets);

        // output
        _output.WriteLine($"Enemy count: {manager.Enemies.Count}");
        _output.WriteLine($"Bullets created: {bulletManager.Bullets.Count}");
        _output.WriteLine("Result: EnemyManager.TryShootEnemies() created bullets via bullet manager ✔");
    }
}
