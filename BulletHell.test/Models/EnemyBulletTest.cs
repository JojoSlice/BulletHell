using BulletHell.Interfaces;
using BulletHell.Models;
using BulletHell.test.TestUtilities;
using Microsoft.Xna.Framework;
using NSubstitute;

namespace BulletHell.test.Models;

public class EnemyBulletTest
{
    [Fact]
    public void EnemyBullet_ShouldMoveAccordingToVelocity()
    {
        // Arrange
        var startPosition = new Vector2(10, 10);
        var velocity = new Vector2(10, 10);
        var mockSprite = Substitute.For<ISpriteHelper>();
        mockSprite.Width.Returns(32);
        mockSprite.Height.Returns(32);

        var enemyBullet = new EnemyBullet(startPosition, velocity, mockSprite);

        var deltaTime = 1/60f;
        var gameTime = new GameTime(
            TimeSpan.Zero,
            TimeSpan.FromSeconds(deltaTime)
        );

        var expectedPosition = startPosition + velocity * deltaTime;

        // Act
        enemyBullet.Update(gameTime);
        var actualPosition = enemyBullet.Position;

        // Assert
        Assert.Equal(expectedPosition.X, actualPosition.X, 4);
        Assert.Equal(expectedPosition.Y, actualPosition.Y, 4);
        mockSprite.Received(1).Update(gameTime);
    }

    [Fact]
    public void Constructor_ShouldInitializeCollider()
    {
        // Arrange
        var startPosition = new Vector2(10, 20);
        var mockSprite = Substitute.For<ISpriteHelper>();

        // Act
        var bullet = new EnemyBullet(startPosition, Vector2.UnitY, mockSprite);

        // Assert
        Assert.NotNull(bullet.Collider);
        Assert.Equal(typeof(EnemyBullet), bullet.Collider.ColliderType);
        Assert.Equal(startPosition, bullet.Collider.Position);
    }

    [Fact]
    public void Update_ShouldKeepColliderInSyncWithPosition()
    {
        // Arrange
        var startPosition = new Vector2(50, 50);
        var velocity = new Vector2(0, 1);
        var mockSprite = Substitute.For<ISpriteHelper>();
        mockSprite.Width.Returns(8);
        mockSprite.Height.Returns(8);

        var bullet = new EnemyBullet(startPosition, velocity, mockSprite);

        // Act
        bullet.Update(TestDataBuilders.OneFrame);

        // Assert
        Assert.Equal(bullet.Position, bullet.Collider.Position);
    }

    [Fact]
    public void UpdateColliderRadiusFromSprite_ShouldSetRadiusBasedOnSprite()
    {
        // Arrange
        var mockSprite = Substitute.For<ISpriteHelper>();
        mockSprite.Width.Returns(9);
        mockSprite.Height.Returns(15);
        var bullet = new EnemyBullet(Vector2.Zero, Vector2.UnitY, mockSprite);

        // Act
        bullet.UpdateColliderRadiusFromSprite();

        // Assert
        var expected = Math.Max(9, 15) / 2f;
        Assert.Equal(expected, bullet.Collider.Radius);
    }
}
