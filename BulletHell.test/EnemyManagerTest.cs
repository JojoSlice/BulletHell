using BulletHell.Configurations;
using BulletHell.Models;
using BulletHell.Interfaces;
using BulletHell.Managers;
using Microsoft.Xna.Framework;
using NSubstitute;
using Xunit;

namespace BulletHell.test;

public class EnemyManagerTest
{
    private readonly ITestOutputHelper _output;

    public EnemyManagerTest(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [Fact]
    public void EnemyManager_ShouldRemoveDeadEnemies()
    {
        // Arrange
        var sprite = Substitute.For<ISpriteHelper>();
        var enemy = new Enemy(new Vector2(-100, -100), sprite);

        var manager = new EnemyManager();
        manager.AddEnemy(enemy);

        // Act
        var before = manager.Enemies.Count;
        manager.Update(new GameTime(), 800, 600);
        var after = manager.Enemies.Count;

        // Assert
        Assert.Empty(manager.Enemies);
        
        // Output
        _output.WriteLine($"Enemies before update: {before}");
        _output.WriteLine($"Enemies after update:  {after}");
        _output.WriteLine("Result: Enemy removed because it was out of bounds ✔");
    }

    [Fact]
    public void EnemyManager_ShouldUpdateAllEnemies()
    {
        // Arrange
        var sprite = Substitute.For<ISpriteHelper>();
        var enemy = new Enemy(new Vector2(0, 0), sprite);
        
        var startPosition = enemy.Position; // endast för output
        
        var manager = new EnemyManager();
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

}