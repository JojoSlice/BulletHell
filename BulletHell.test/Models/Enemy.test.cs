using BulletHell.Configurations;
using BulletHell.Interfaces;
using BulletHell.Models;
using BulletHell.test.TestUtilities;
using Microsoft.Xna.Framework;
using NSubstitute;

namespace BulletHell.test.Models;

public class EnemyTest(ITestOutputHelper output)
{
    [Fact]
    public void Update_ShouldMoveDownwards()
    {
        // Arrange
        var startPosition = new Vector2(10, 10);
        var spriteMock = Substitute.For<ISpriteHelper>();
        var enemy = new Enemy(startPosition, spriteMock);
        var deltaTime = 1 / 60f;
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

    // --- collider related tests ---

    [Fact]
    public void Constructor_ShouldInitializeCollider()
    {
        // Arrange
        var startPosition = new Vector2(10, 20);
        var mockSprite = Substitute.For<ISpriteHelper>();

        // Act
        var enemy = new Enemy(startPosition, mockSprite);

        // Assert
        Assert.NotNull(enemy.Collider);
        Assert.Equal(typeof(Enemy), enemy.Collider.ColliderType);
        Assert.Equal(startPosition, enemy.Collider.Position);
    }

    [Fact]
    public void Update_ShouldKeepColliderInSyncWithPosition()
    {
        // Arrange
        var startPosition = new Vector2(0, 0);
        var mockSprite = Substitute.For<ISpriteHelper>();
        mockSprite.Width.Returns(16);
        mockSprite.Height.Returns(16);
        var enemy = new Enemy(startPosition, mockSprite);

        var gameTime = TestDataBuilders.OneFrame;

        // Act
        enemy.Update(gameTime);

        // Assert
        Assert.Equal(enemy.Position, enemy.Collider.Position);
    }

    [Fact]
    public void UpdateColliderRadiusFromSprite_ShouldSetRadiusBasedOnSprite()
    {
        // Arrange
        var mockSprite = Substitute.For<ISpriteHelper>();
        mockSprite.Width.Returns(20);
        mockSprite.Height.Returns(12);
        var enemy = new Enemy(Vector2.Zero, mockSprite);

        // Act
        // Radius should be initialized in constructor based on sprite dimensions

        // Assert
        var expected = Math.Max(20, 12) / 2f;
        Assert.Equal(expected, enemy.Collider.Radius);
    }

    [Fact]
    public void TakeDamage_WhenHealthReachesZero_ShouldSetIsAliveFalse()
    {
        // Arrange
        var sprite = Substitute.For<ISpriteHelper>();
        var enemy = new Enemy(new Vector2(0, 0), sprite);

        int startingHealth = enemy.Health;
        int lethalDamage = startingHealth; // Enough to kill

        var expectedHealth = 0;

        // Act
        enemy.TakeDamage(lethalDamage);
        var actualHealth = enemy.Health;

        // Assert
        Assert.False(enemy.IsAlive);
        Assert.Equal(expectedHealth, actualHealth);

        //Output
        output.WriteLine($"Starting Health: {startingHealth}");
        output.WriteLine($"Expected Health after death: {expectedHealth}");
        output.WriteLine($"Actual Health: {actualHealth} ✔️");
        output.WriteLine($"Enemy alive status correctly false ✔️");
    }

    [Fact]
    public void ShouldBeRemoved_WhenEnemyIsDead_ShouldReturnTrue()
    {
        // Arrange
        var sprite = Substitute.For<ISpriteHelper>();
        var enemy = new Enemy(new Vector2(100, 100), sprite);

        var startingHealth = enemy.Health;

        // dödar fienden
        enemy.TakeDamage(enemy.Health);

        // Act
        bool removed = enemy.ShouldBeRemoved(800, 600);

        // Assert
        Assert.True(removed);

        // Output
        output.WriteLine($"Enemy health before death: {startingHealth}");
        output.WriteLine($"Enemy is alive after damage: {enemy.IsAlive}");
        output.WriteLine($"ShouldBeRemoved returned: {removed} ✔️");
    }
    [Fact]
    public void Enemy_TakeDamage_ShouldReduceHealth()
    {
        // Arrange
        var sprite = Substitute.For<ISpriteHelper>();
        var enemy = new Enemy(new Vector2(0, 0), sprite);

        int startHealth = enemy.Health;
        int damage = 1;
        int expected = startHealth - damage;

        // Act
        enemy.TakeDamage(damage);
        int actual = enemy.Health;

        // Assert
        Assert.Equal(expected, actual);

        // Output
        output.WriteLine($"Start Health:   {startHealth}");
        output.WriteLine($"Damage Taken:   {damage}");
        output.WriteLine($"Expected Health:{expected}");
        output.WriteLine($"Actual Health:  {actual}");
    }
}
