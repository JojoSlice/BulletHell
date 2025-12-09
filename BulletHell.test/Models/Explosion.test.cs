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
}