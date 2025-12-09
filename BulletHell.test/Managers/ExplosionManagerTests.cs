using BulletHell.Interfaces;
using BulletHell.Models;
using Microsoft.Xna.Framework;
using Moq;

namespace BulletHell.Managers;

public class ExplosionManagerTests
{
    private readonly ITestOutputHelper _output;

    public ExplosionManagerTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Add_ShouldAddExplosionsToList()
    {
        // Arrange
        var spriteMock = new Mock<ISpriteHelper>();
        var explosion = new Explosion(Vector2.Zero, spriteMock.Object);

        var manager = new ExplosionManager();

        bool expected = true;

        // Act
        manager.Add(explosion);
        bool actual = manager.Explosions.Contains(explosion);

        // Assert
        Assert.Equal(expected, actual);

        // Output
        _output.WriteLine($"Explosion in list: {expected}, Actual: {actual}");
    }

    [Fact]
    public void Update_ShouldRemoveDeadExplosions()
    {
        // Arrange
        var spriteMock = new Mock<ISpriteHelper>();
        spriteMock.SetupGet(s => s.IsAnimationFinished).Returns(true);

        var explosion = new Explosion(Vector2.Zero, spriteMock.Object);

        var manager = new ExplosionManager();
        manager.Add(explosion);

        int expected = 0;

        // Act
        manager.Update(new GameTime());
        int actual = manager.Explosions.Count;

        // Assert
        Assert.Equal(expected, actual);

        // Output
        _output.WriteLine(
            $"Remaining explosions after update: {expected}, Actual: {actual}"
        );
    }
}