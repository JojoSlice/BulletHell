using BulletHell.Configurations;
using Xunit;

namespace BulletHell.test.Configurations;

public class GameConfigurationTests
{
    [Fact]
    public void GameConfiguration_ShouldHaveCorrectDefaults()
    {
        // Act
        var config = new GameConfiguration();

        // Assert - Player defaults
        Assert.Equal(300f, config.Player.Speed);
        Assert.Equal(128, config.Player.SpriteWidth);
        Assert.Equal(100, config.Player.MaxHealth);
        Assert.Equal(3, config.Player.Lives);

        // Assert - Enemy defaults
        Assert.Equal(50f, config.Enemy.Speed);
        Assert.Equal(32, config.Enemy.SpriteWidth);
        Assert.Equal(20, config.Enemy.MaxHealth);
        Assert.Equal(1, config.Enemy.ScoreValue);

        // Assert - Pool defaults
        Assert.Equal(50, config.Pools.PlayerBullets.InitialSize);
        Assert.Equal(200, config.Pools.PlayerBullets.MaxSize);
        Assert.Equal(10, config.Pools.Enemies.InitialSize);
        Assert.Equal(15, config.Pools.Enemies.MaxSize);

        // Assert - Collision defaults
        Assert.Equal(10f, config.Collision.DistanceCheckThreshold);
        Assert.Equal(300f, config.Collision.KnockbackForce);
        Assert.Equal(0.18f, config.Collision.KnockbackDuration);

        // Assert - Spawn defaults
        Assert.Equal(50, config.Spawn.EnemySpawnMargin);
        Assert.Equal(-50, config.Spawn.EnemySpawnOffsetY);
        Assert.Equal(-30, config.Spawn.DashSpawnOffsetY);
    }

    [Fact]
    public void PlayerConfiguration_CanBeModified()
    {
        // Arrange
        var config = new PlayerConfiguration();

        // Act
        config.Speed = 500f;
        config.MaxHealth = 200;

        // Assert
        Assert.Equal(500f, config.Speed);
        Assert.Equal(200, config.MaxHealth);
    }

    [Fact]
    public void PoolConfiguration_CanBeModified()
    {
        // Arrange
        var poolConfig = new PoolConfiguration.PoolSizeConfig();

        // Act
        poolConfig.InitialSize = 100;
        poolConfig.MaxSize = 500;

        // Assert
        Assert.Equal(100, poolConfig.InitialSize);
        Assert.Equal(500, poolConfig.MaxSize);
    }

    [Fact]
    public void CollisionConfiguration_CanBeModified()
    {
        // Arrange
        var config = new CollisionConfiguration();

        // Act
        config.KnockbackForce = 500f;
        config.KnockbackDuration = 0.5f;

        // Assert
        Assert.Equal(500f, config.KnockbackForce);
        Assert.Equal(0.5f, config.KnockbackDuration);
    }
}
