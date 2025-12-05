using BulletHell.Configurations;
using BulletHell.Interfaces;
using BulletHell.Models;
using BulletHell.test.TestUtilities;
using Microsoft.Xna.Framework;
using NSubstitute;

namespace BulletHell.test.Models;

public class PlayerBulletTests(ITestOutputHelper output)
{
    [Theory]
    [InlineData(-5, 5, true)]
    [InlineData(5, -5, true)]
    [InlineData(105, 50, true)]
    [InlineData(50, 105, true)]
    [InlineData(5, 5, false)]
    public void IsOutOfBounds_ReturnsValidBoolean(int x, int y, bool expected)
    {
        // Arrange
        Vector2 startPosition = new(x, y);
        Vector2 direction = new(x, y);
        ISpriteHelper bulletSprite = Substitute.For<ISpriteHelper>();

        using var bullet = new Bullet<Player>(startPosition, direction, bulletSprite);

        // Act
        var actual = bullet.IsOutOfBounds(100, 100);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Update_ShouldMoveBulletInDirection()
    {
        // Arrange
        var startPosition = new Vector2(100, 100);
        var direction = Vector2.UnitY;
        using var bullet = TestDataBuilders.CreateTestBullet(startPosition, direction);
        var gameTime = TestDataBuilders.OneFrame;

        // Act
        bullet.Update(gameTime);

        // Assert
        var expectedPosition = startPosition + direction * BulletConfig.Player.Speed * (1.0f / 60.0f);
        Assert.Equal(expectedPosition.X, bullet.Position.X, 2);
        Assert.Equal(expectedPosition.Y, bullet.Position.Y, 2);
    }

    [Fact]
    public void Update_ShouldIncrementTimeAlive()
    {
        // Arrange
        using var bullet = TestDataBuilders.CreateTestBullet();
        var gameTime = TestDataBuilders.OneFrame;

        // Act
        bullet.Update(gameTime);

        // Assert
        Assert.True(bullet.IsAlive); // Should still be alive after one frame
    }

    [Fact]
    public void Update_ShouldCallSpriteUpdate()
    {
        // Arrange
        var mockSprite = Substitute.For<ISpriteHelper>();
        using var bullet = TestDataBuilders.CreateTestBullet(sprite: mockSprite);
        var gameTime = TestDataBuilders.OneFrame;

        // Act
        bullet.Update(gameTime);

        // Assert
        mockSprite.Received(1).Update(gameTime);
    }

    [Fact]
    public void Update_WithNullGameTime_ShouldThrowArgumentNullException()
    {
        // Arrange
        using var bullet = TestDataBuilders.CreateTestBullet();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => bullet.Update(null!));
    }

    [Fact]
    public void Constructor_ShouldNormalizeDirection()
    {
        // Arrange
        var startPosition = new Vector2(100, 100);
        var direction = new Vector2(3, 4); // Length = 5
        var mockSprite = Substitute.For<ISpriteHelper>();

        // Act
        using var bullet = new Bullet<Player>(startPosition, direction, mockSprite);
        bullet.Update(TestDataBuilders.OneFrame);

        // Assert - if direction is normalized, movement should be based on unit vector
        var expectedMovement = Vector2.Normalize(direction) * BulletConfig.Player.Speed * (1.0f / 60.0f);
        var expectedPosition = startPosition + expectedMovement;
        Assert.Equal(expectedPosition.X, bullet.Position.X, 2);
        Assert.Equal(expectedPosition.Y, bullet.Position.Y, 2);
    }

    [Fact]
    public void Reset_ShouldUpdatePositionAndDirection()
    {
        // Arrange
        using var bullet = TestDataBuilders.CreateTestBullet(new Vector2(100, 100), Vector2.UnitY);
        var newPosition = new Vector2(200, 200);
        var newDirection = Vector2.UnitX;

        // Act
        bullet.Reset(newPosition, newDirection);

        // Assert
        Assert.Equal(newPosition, bullet.Position);
    }

    [Fact]
    public void Reset_ShouldResetTimeAlive()
    {
        // Arrange
        using var bullet = TestDataBuilders.CreateTestBullet();

        // Update bullet many times to increase time alive
        for (int i = 0; i < 100; i++)
        {
            bullet.Update(TestDataBuilders.OneFrame);
        }

        // Act
        bullet.Reset(Vector2.Zero, Vector2.UnitY);

        // Assert
        Assert.True(bullet.IsAlive);
    }

    [Fact]
    public void Reset_ShouldNormalizeDirection()
    {
        // Arrange
        using var bullet = TestDataBuilders.CreateTestBullet();
        var newDirection = new Vector2(3, 4); // Length = 5

        // Act
        bullet.Reset(new Vector2(100, 100), newDirection);
        var startPosition = bullet.Position;
        bullet.Update(TestDataBuilders.OneFrame);

        // Assert
        var expectedMovement = Vector2.Normalize(newDirection) * BulletConfig.Player.Speed * (1.0f / 60.0f);
        var expectedPosition = startPosition + expectedMovement;
        Assert.Equal(expectedPosition.X, bullet.Position.X, 2);
        Assert.Equal(expectedPosition.Y, bullet.Position.Y, 2);
    }

    [Fact]
    public void IsAlive_WhenTimeAliveExceedsLifetime_ShouldReturnFalse()
    {
        // Arrange
        using var bullet = TestDataBuilders.CreateTestBullet();

        // Act - update for longer than lifetime
        for (int i = 0; i < 200; i++)
        {
            bullet.Update(TestDataBuilders.OneFrame);
        }

        // Assert
        Assert.False(bullet.IsAlive);
    }

    [Fact]
    public void IsAlive_WhenTimeAliveWithinLifetime_ShouldReturnTrue()
    {
        // Arrange
        using var bullet = TestDataBuilders.CreateTestBullet();

        // Act
        bullet.Update(TestDataBuilders.OneFrame);

        // Assert
        Assert.True(bullet.IsAlive);
    }

    [Fact]
    public void ShouldBeRemoved_WhenOutOfBounds_ShouldReturnTrue()
    {
        // Arrange
        using var bullet = TestDataBuilders.CreateTestBullet(new Vector2(-10, 50), Vector2.UnitY);

        // Act
        var result = bullet.ShouldBeRemoved(100, 100);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ShouldBeRemoved_WhenNotAlive_ShouldReturnTrue()
    {
        // Arrange
        using var bullet = TestDataBuilders.CreateTestBullet();

        // Update until no longer alive
        for (int i = 0; i < 200; i++)
        {
            bullet.Update(TestDataBuilders.OneFrame);
        }

        // Act
        var result = bullet.ShouldBeRemoved(800, 600);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ShouldBeRemoved_WhenAliveAndInBounds_ShouldReturnFalse()
    {
        // Arrange
        using var bullet = TestDataBuilders.CreateTestBullet(new Vector2(50, 50), Vector2.UnitY);

        // Act
        var result = bullet.ShouldBeRemoved(100, 100);

        // Assert
        Assert.False(result);
    }

    // Note: LoadContent with valid texture requires integration testing
    // due to MonoGame Texture2D being non-mockable

    [Fact]
    public void LoadContent_WithNullTexture_ShouldThrowArgumentNullException()
    {
        // Arrange
        using var bullet = TestDataBuilders.CreateTestBullet();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => bullet.LoadContent(null!));
    }

    [Fact]
    public void LoadContent_ShouldSetColliderRadius_BasedOnSpriteSize()
    {
        // Arrange
        int frameWidth = 10;
        int frameHeight = 6;
        var mockSprite = MockFactories.CreateMockSpriteHelper(width: frameWidth, height: frameHeight);
        using var bullet = TestDataBuilders.CreateTestBullet(sprite: mockSprite);

        // Act
        // Radius should be initialized in constructor based on sprite dimensions

        // Assert
        var expected = Math.Max(frameWidth, frameHeight) / 2f;
        Assert.Equal(expected, bullet.Collider.Radius);
    }

    // Note: Draw with valid SpriteBatch requires integration testing
    // due to MonoGame SpriteBatch being non-mockable

    [Fact]
    public void Draw_WithNullSpriteBatch_ShouldThrowArgumentNullException()
    {
        // Arrange
        using var bullet = TestDataBuilders.CreateTestBullet();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => bullet.Draw(null!));
    }

    [Fact]
    public void Dispose_ShouldDisposeSprite()
    {
        // Arrange
        var mockSprite = Substitute.For<ISpriteHelper>();
        using var bullet = TestDataBuilders.CreateTestBullet(sprite: mockSprite);

        // Act
        bullet.Dispose();

        // Assert
        mockSprite.Received(1).Dispose();
    }

    [Fact]
    public void Dispose_CalledMultipleTimes_ShouldOnlyDisposeOnce()
    {
        // Arrange
        var mockSprite = Substitute.For<ISpriteHelper>();
        using var bullet = TestDataBuilders.CreateTestBullet(sprite: mockSprite);

        // Act
        bullet.Dispose();
        bullet.Dispose();

        // Assert
        mockSprite.Received(1).Dispose();
    }

    [Fact]
    public void Constructor_ShouldInitializeCollider()
    {
        // Arrange
        var startPosition = new Vector2(10, 20);
        var mockSprite = Substitute.For<ISpriteHelper>();

        // Act
        using var bullet = new Bullet<Player>(startPosition, Vector2.UnitY, mockSprite);

        // Assert
        Assert.NotNull(bullet.Collider);
        Assert.Equal(typeof(Bullet<Player>), bullet.Collider.ColliderType);
        Assert.Equal(startPosition, bullet.Collider.Position);
    }

    [Fact]
    public void Update_ShouldKeepColliderInSyncWithPosition()
    {
        // Arrange
        var startPosition = new Vector2(100, 100);
        var direction = Vector2.UnitY;
        var mockSprite = Substitute.For<ISpriteHelper>();
        using var bullet = new Bullet<Player>(startPosition, direction, mockSprite);

        // Act
        bullet.Update(TestDataBuilders.OneFrame);

        // Assert
        Assert.Equal(bullet.Position, bullet.Collider.Position);
    }

    [Fact]
    public void Reset_ShouldUpdateColliderPosition()
    {
        // Arrange
        var mockSprite = Substitute.For<ISpriteHelper>();
        using var bullet = TestDataBuilders.CreateTestBullet(sprite: mockSprite);

        var newPosition = new Vector2(250, 250);

        // Act
        bullet.Reset(newPosition, Vector2.UnitX);

        // Assert
        Assert.Equal(newPosition, bullet.Collider.Position);
    }
    [Fact]
    public void PlayerBullet_Reset_ShouldAssignPlayerDamage()
    {
        // Arrange
        var sprite = Substitute.For<ISpriteHelper>();
        using var bullet = new Bullet<Player>(Vector2.Zero, Vector2.UnitY, sprite);

        var expected = BulletConfig.Player.Damage;

        // Act
        bullet.Reset(Vector2.Zero, Vector2.UnitY);
        var actual = bullet.Damage;

        // Assert
        Assert.Equal(expected, actual);

        // Output
        output.WriteLine($"Expected Damage: {expected}");
        output.WriteLine($"Actual Damage:   {actual}");
    }
}
