using BulletHell.Helpers;
using BulletHell.Managers;
using BulletHell.Models;
using Microsoft.Xna.Framework;
using NSubstitute.Routing.Handlers;

namespace BulletHell.test;

public class EnemyBulletTest()
{
    [Fact]
    public void EnemyBullet_ShouldMoveAccordingToVelocity()
    {
        // Arrange
        var startPosition = new Vector2(10, 10);
        var velocity = new Vector2(10, 10);
        var enemyBullet = new EnemyBullet(startPosition, velocity);

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
    }
}
