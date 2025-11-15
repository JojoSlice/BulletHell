using BulletHell.Configurations;
using BulletHell.Interfaces;
using BulletHell.Models;
using Microsoft.Xna.Framework;
using NSubstitute;

namespace BulletHell.tests;

public class PlayerTest
{
    [Fact]
    public void Update_WithZeroDirection_ShouldNotMovePlayer()
    {
        // Arrange
        var startPosition = new Vector2(100, 100);
        var mockInput = Substitute.For<IInputProvider>();
        var mockSprite = Substitute.For<ISpriteHelper>();
        mockInput.GetDirection().Returns(Vector2.Zero);

        var player = new Player(startPosition, mockInput, mockSprite);
        var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1.0 / 60.0));

        // Act
        player.Update(gameTime);

        // Assert
        Assert.Equal(startPosition, player.Position);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(-1, -1)]
    [InlineData(-1, 1)]
    [InlineData(1, -1)]
    public void Update_WithDiagonalDirection_ShouldNormalizeAndMove(int x, int y)
    {
        // Arrange
        var startPosition = new Vector2(100, 100);
        var mockInput = Substitute.For<IInputProvider>();
        var mockSprite = Substitute.For<ISpriteHelper>();
        mockInput.GetDirection().Returns(new Vector2(x, y));

        var player = new Player(startPosition, mockInput, mockSprite);
        var deltaTime = 1.0f / 60.0f;
        var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(deltaTime));

        // Act
        player.Update(gameTime);

        // Assert
        var normalizedDirection = Vector2.Normalize(new Vector2(x, y));
        var expectedPosition = startPosition + normalizedDirection * PlayerConfig.Speed * deltaTime;

        Assert.Equal(expectedPosition.X, player.Position.X, 2);
        Assert.Equal(expectedPosition.Y, player.Position.Y, 2);
    }

    [Fact]
    public void Update_ShouldCallSpriteUpdate()
    {
        // Arrange
        var startPosition = new Vector2(100, 100);
        var mockInput = Substitute.For<IInputProvider>();
        var mockSprite = Substitute.For<ISpriteHelper>();
        mockInput.GetDirection().Returns(Vector2.Zero);

        var player = new Player(startPosition, mockInput, mockSprite);
        var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1.0 / 60.0));

        // Act
        player.Update(gameTime);

        // Assert
        mockSprite.Received(1).Update(gameTime);
    }

    [Theory]
    [InlineData(1, 0, 100)]
    [InlineData(-1, 0, 100)]
    [InlineData(0, 1, 100)]
    [InlineData(0, -1, 100)]
    public void Update_WithCardinalDirections_ShouldMoveCorrectly(float x, float y, float startPos)
    {
        // Arrange
        var startPosition = new Vector2(startPos, startPos);
        var mockInput = Substitute.For<IInputProvider>();
        var mockSprite = Substitute.For<ISpriteHelper>();
        var direction = new Vector2(x, y);
        mockInput.GetDirection().Returns(direction);

        var player = new Player(startPosition, mockInput, mockSprite);
        var deltaTime = 1.0f / 60.0f;
        var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(deltaTime));

        // Act
        player.Update(gameTime);

        // Assert
        var expectedPosition = startPosition + direction * PlayerConfig.Speed * deltaTime;
        Assert.Equal(expectedPosition.X, player.Position.X, 2);
        Assert.Equal(expectedPosition.Y, player.Position.Y, 2);
    }
}
