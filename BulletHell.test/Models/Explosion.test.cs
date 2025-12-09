using BulletHell.Interfaces;
using BulletHell.Models;
using Microsoft.Xna.Framework;
using Moq;

namespace BulletHell.test.Models;

public class ExplosionTests
{
    private readonly ITestOutputHelper _output;

    public ExplosionTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Explosion_ShouldStartAsAlive()
    {
        // Arrange
        var spriteMock = new Mock<ISpriteHelper>();
        var explosion = new Explosion(Vector2.Zero, spriteMock.Object);

        bool expected = true;

        // Act
        bool actual = explosion.IsAlive;

        // Assert
        Assert.Equal(expected, actual);

        // Output
        _output.WriteLine($"Expected: {expected}, Actual: {actual}");
    }

    [Fact]
    public void Explosion_ShouldBecomeNotAlive_WhenAnimationFinished()
    {
        // Arrange
        var spriteMock = new Mock<ISpriteHelper>();
        spriteMock.SetupGet(s => s.IsAnimationFinished).Returns(true);

        var explosion = new Explosion(Vector2.Zero, spriteMock.Object);

        bool expected = false;

        // Act
        explosion.Update(new GameTime());
        bool actual = explosion.IsAlive;

        // Assert
        Assert.Equal(expected, actual);

        // Output
        _output.WriteLine($"Expected: {expected}, Actual: {actual}");
    }

    [Fact]
    public void Explosion_Dispose_ShouldDisposeSprite()
    {
        // Arrange
        var spriteMock = new Mock<ISpriteHelper>();
        var explosion = new Explosion(Vector2.Zero, spriteMock.Object);

        bool expected = true;

        // Act
        explosion.Dispose();

        bool actual;

        try
        {
            spriteMock.Verify(s => s.Dispose(), Times.Once);
            actual = true;
        }
        catch
        {
            actual = false;
        }

        // Assert
        Assert.Equal(expected, actual);

        // Output
        _output.WriteLine($"Expected Dispose call: {expected}, Actual: {actual}");
    }
}