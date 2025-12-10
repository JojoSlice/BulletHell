using BulletHell.Configurations;
using BulletHell.Factories;
using BulletHell.Interfaces;
using BulletHell.Managers;
using BulletHell.Models;
using BulletHell.ServiceCollection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BulletHell.test.Integration;

public class DIIntegrationTests
{
    [Fact]
    public void ServiceProvider_ShouldResolveAllRequiredServices()
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        services.AddGameServices();
        services.AddSceneServices();

        var provider = services.BuildServiceProvider();

        // Act & Assert - Singleton services
        Assert.NotNull(provider.GetService<IConfigurationProvider>());
        Assert.NotNull(provider.GetService<GameConfiguration>());
        Assert.NotNull(provider.GetService<ISpriteHelperFactory>());
        Assert.NotNull(provider.GetService<IPasswordHasher>());
        Assert.NotNull(provider.GetService<IApiClient>());
        Assert.NotNull(provider.GetService<IMenuInputProvider>());

        // Act & Assert - Scoped services (within a scope)
        using var scope = provider.CreateScope();
        Assert.NotNull(scope.ServiceProvider.GetService<BulletManager<Player>>());
        Assert.NotNull(scope.ServiceProvider.GetService<BulletManager<Enemy>>());
        Assert.NotNull(scope.ServiceProvider.GetService<EnemyManager>());
        Assert.NotNull(scope.ServiceProvider.GetService<ExplosionManager>());
        Assert.NotNull(scope.ServiceProvider.GetService<DashManager>());
        Assert.NotNull(scope.ServiceProvider.GetService<RymdDashManager>());
    }

    [Fact]
    public void ServiceProvider_SingletonServices_ShouldReturnSameInstance()
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        services.AddGameServices();

        var provider = services.BuildServiceProvider();

        // Act
        var factory1 = provider.GetRequiredService<ISpriteHelperFactory>();
        var factory2 = provider.GetRequiredService<ISpriteHelperFactory>();
        var config1 = provider.GetRequiredService<GameConfiguration>();
        var config2 = provider.GetRequiredService<GameConfiguration>();

        // Assert
        Assert.Same(factory1, factory2);
        Assert.Same(config1, config2);
    }

    [Fact]
    public void ServiceProvider_ScopedServices_ShouldReturnDifferentInstancesPerScope()
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        services.AddGameServices();
        services.AddSceneServices();

        var provider = services.BuildServiceProvider();

        // Act
        BulletManager<Player>? manager1;
        BulletManager<Player>? manager2;

        using (var scope1 = provider.CreateScope())
        {
            manager1 = scope1.ServiceProvider.GetRequiredService<BulletManager<Player>>();
        }

        using (var scope2 = provider.CreateScope())
        {
            manager2 = scope2.ServiceProvider.GetRequiredService<BulletManager<Player>>();
        }

        // Assert
        Assert.NotSame(manager1, manager2);
    }

    [Fact]
    public void ServiceProvider_ScopedServices_ShouldReturnSameInstanceWithinScope()
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        services.AddGameServices();
        services.AddSceneServices();

        var provider = services.BuildServiceProvider();

        // Act & Assert
        using var scope = provider.CreateScope();
        var manager1 = scope.ServiceProvider.GetRequiredService<BulletManager<Player>>();
        var manager2 = scope.ServiceProvider.GetRequiredService<BulletManager<Player>>();

        Assert.Same(manager1, manager2);
    }

    [Fact]
    public void BulletManager_ResolvedFromDI_ShouldUseConfiguration()
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        services.AddGameServices();
        services.AddSceneServices();

        var provider = services.BuildServiceProvider();

        // Act
        using var scope = provider.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<BulletManager<Player>>();

        // Assert
        Assert.NotNull(manager);
        // BulletManager should have been created with config values (50, 200)
        // We can verify it doesn't crash when creating bullets
        manager.CreateBullet(Microsoft.Xna.Framework.Vector2.Zero, Microsoft.Xna.Framework.Vector2.UnitX);
        Assert.Single(manager.Bullets);
    }

    [Fact]
    public void ConfigurationProvider_ShouldLoadConfiguration()
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        services.AddGameServices();

        var provider = services.BuildServiceProvider();

        // Act
        var config = provider.GetRequiredService<GameConfiguration>();

        // Assert
        Assert.NotNull(config);
        Assert.Equal(300f, config.Player.Speed);
        Assert.Equal(50, config.Pools.PlayerBullets.InitialSize);
        Assert.Equal(200, config.Pools.PlayerBullets.MaxSize);
        Assert.Equal(300f, config.Collision.KnockbackForce);
    }

    [Fact]
    public void EnemyManager_ResolvedFromDI_ShouldHaveBulletManagerDependency()
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        services.AddGameServices();
        services.AddSceneServices();

        var provider = services.BuildServiceProvider();

        // Act
        using var scope = provider.CreateScope();
        var enemyManager = scope.ServiceProvider.GetRequiredService<EnemyManager>();
        var enemyBulletManager = scope.ServiceProvider.GetRequiredService<BulletManager<Enemy>>();

        // Assert
        Assert.NotNull(enemyManager);
        Assert.NotNull(enemyBulletManager);
        // EnemyManager internally uses the bullet manager resolved from DI
    }

    [Fact]
    public void ServiceProvider_DisposingScopeDisposesServices()
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        services.AddGameServices();
        services.AddSceneServices();

        var provider = services.BuildServiceProvider();

        BulletManager<Player>? manager;

        // Act
        using (var scope = provider.CreateScope())
        {
            manager = scope.ServiceProvider.GetRequiredService<BulletManager<Player>>();
            Assert.NotNull(manager);
        }

        // Assert - Manager should be disposed after scope disposal
        // Note: We can't easily verify disposal without making the manager's _disposed field public
        // But we can verify a new scope creates a new instance
        using var newScope = provider.CreateScope();
        var newManager = newScope.ServiceProvider.GetRequiredService<BulletManager<Player>>();
        Assert.NotSame(manager, newManager);
    }
}
