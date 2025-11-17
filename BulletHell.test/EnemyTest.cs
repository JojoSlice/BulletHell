using BulletHell.Configurations;
using BulletHell.Models;
using BulletHell.Interfaces;
using Microsoft.Xna.Framework;
using NSubstitute;
using Xunit;

namespace BulletHell.test;

public class EnemyTest
{
    private readonly ITestOutputHelper _output;

    public EnemyTest(ITestOutputHelper output)
    {
        _output = output;
    }
    
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
        _output.WriteLine($"Expected: {expectedPosition}");
        _output.WriteLine($"Actual:   {actual}");
    }

    [Fact]
    public void Update_ShouldCallSpriteUpdate()
    {
        // Arrange
        var startPosition = new Vector2(10, 10);
        var spriteMock = Substitute.For<ISpriteHelper>();
        var enemy = new Enemy(startPosition, spriteMock);
        var deltaTime = 1/60f;
        var totalTime = TimeSpan.Zero;
        var elapsedTime = TimeSpan.FromSeconds(deltaTime);
        var gameTime = new GameTime(totalTime, elapsedTime);
        
        // Act
        enemy.Update(gameTime);
        
        // Assert
        spriteMock.Received(1).Update(gameTime);
        
        _output.WriteLine("sprite.Update(gameTime) was called once ✔");
    }

    [Fact]
    public void Update_ShouldUseEnemySpeed()
    {
        // Arrange


        // Act


        // Assert
    }

    [Fact]
    public void IsAlive_ShouldBeFalse_WhenOutOfBounds()
    {
        // Arrange


        // Act


        // Assert
    }
}