using BulletHell.Configurations;
using BulletHell.Models;
using BulletHell.Interfaces;
using Microsoft.Xna.Framework;
using NSubstitute;
using Xunit;

namespace BulletHell.test;

public class EnemyTest(ITestOutputHelper output)
{
    [Fact]
    public void Update_ShouldMoveDownwards()
    {
        // Arrange
        var startPosition = new Vector2(10, 10);
        var spriteMock = Substitute.For<ISpriteHelper>();
        var enemy = new Enemy(startPosition, spriteMock);
        var deltaTime = 1/60f;
        var totalTime = TimeSpan.Zero;
        var elapsedTime = TimeSpan.FromSeconds(deltaTime);
        var gameTime = new GameTime(totalTime, elapsedTime);
        var expectedPosition = new Vector2(startPosition.X, startPosition.Y + EnemyConfig.Speed * deltaTime);
        int precision = 4; // antalet decimaler att gämföra

        // Act
        enemy.Update(gameTime);
        var actual = enemy.Position;

        // Assert
        Assert.Equal(expectedPosition.X, actual.X, precision);
        Assert.Equal(expectedPosition.Y, actual.Y, precision);
        
        // Output
        output.WriteLine($"Expected: {expectedPosition}");
        output.WriteLine($"Actual:   {actual}");
    }

    [Fact]
    public void Update_ShouldUseEnemySpeed()
    {
        // Arrange
        var startPosition = new Vector2(0, 0);
        var spriteMock = Substitute.For<ISpriteHelper>();
        var enemy = new Enemy(startPosition, spriteMock);
        var deltaTime = 1.0f;
        var totalTime = TimeSpan.Zero;
        var elapsedTime = TimeSpan.FromSeconds(deltaTime);
        var gameTime = new GameTime(totalTime, elapsedTime);
        var expectedPosition = new Vector2(startPosition.X, startPosition.Y + EnemyConfig.Speed * deltaTime);

        // Act
        enemy.Update(gameTime);


        // Assert
        Assert.Equal(expectedPosition.X, enemy.Position.X, 4);
        Assert.Equal(expectedPosition.Y, enemy.Position.Y, 4);

        output.WriteLine($"Speed test OK ✔ Expected ΔY = {EnemyConfig.Speed * deltaTime}");
        output.WriteLine($"Actual ΔY = {enemy.Position.Y - startPosition.Y}");

    }
    
    [Theory]
    [InlineData(-50, 100, true)]
    [InlineData(900, 100, true)]
    [InlineData(100, 700, true)]
    [InlineData(100, -10, false)]
    [InlineData(100, 100, false)]
    public void Enemy_IsOutOfBounds_ReturnsValidBoolean(float x, float y, bool expected)
    {
        var sprite = Substitute.For<ISpriteHelper>();
        var enemy = new Enemy(new Vector2(x, y), sprite);

        var actual = enemy.IsOutOfBounds(800, 600);

        Assert.Equal(expected, actual);
    }

    
}