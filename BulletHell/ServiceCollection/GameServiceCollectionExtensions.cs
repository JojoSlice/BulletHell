using BulletHell.Configurations;
using BulletHell.Factories;
using BulletHell.Inputs;
using BulletHell.Interfaces;
using BulletHell.Managers;
using BulletHell.Models;
using BulletHell.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace BulletHell.ServiceCollection;

/// <summary>
/// Extension methods for configuring game services in the dependency injection container.
/// </summary>
public static class GameServiceCollectionExtensions
{
    /// <summary>
    /// Adds core game services that are shared across the entire application lifetime.
    /// These services are registered as singletons.
    /// </summary>
    public static IServiceCollection AddGameServices(this IServiceCollection services)
    {
        // Configuration - Singleton (loaded once, shared everywhere)
        services.AddSingleton<IConfigurationProvider, JsonConfigurationProvider>();
        services.AddSingleton(sp => sp.GetRequiredService<IConfigurationProvider>().GetConfiguration());

        // Factories - Singleton (stateless, safe to share)
        services.AddSingleton<ISpriteHelperFactory, SpriteHelperFactory>();

        // API Services - Singleton (HttpClient should be singleton or use IHttpClientFactory)
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton<HttpClient>(sp => new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5111")
        });
        services.AddSingleton<IApiClient, ApiClient>();

        // Input providers - Singleton for menu (stateful but manages its own state)
        services.AddSingleton<IMenuInputProvider, MouseKeyboardInputProvider>();

        return services;
    }

    /// <summary>
    /// Adds scene-scoped services that are created per scene and disposed when the scene changes.
    /// These services are registered as scoped, meaning each scene gets fresh instances.
    /// </summary>
    public static IServiceCollection AddSceneServices(this IServiceCollection services)
    {
        // Managers - Scoped (recreated for each scene)
        services.AddScoped<BulletManager<Player>>(sp =>
        {
            var config = sp.GetRequiredService<GameConfiguration>();
            var factory = sp.GetRequiredService<ISpriteHelperFactory>();
            return new BulletManager<Player>(factory, config.Pools.PlayerBullets);
        });

        services.AddScoped<BulletManager<Enemy>>(sp =>
        {
            var config = sp.GetRequiredService<GameConfiguration>();
            var factory = sp.GetRequiredService<ISpriteHelperFactory>();
            return new BulletManager<Enemy>(factory, config.Pools.EnemyBullets);
        });

        services.AddScoped<EnemyManager>(sp =>
        {
            var config = sp.GetRequiredService<GameConfiguration>();
            var factory = sp.GetRequiredService<ISpriteHelperFactory>();
            var bulletManager = sp.GetRequiredService<BulletManager<Enemy>>();
            return new EnemyManager(bulletManager, factory, config.Pools.Enemies, config.Spawn);
        });

        services.AddScoped<ExplosionManager>();

        services.AddScoped<DashManager>(sp =>
        {
            var config = sp.GetRequiredService<GameConfiguration>();
            return new DashManager(config.Pools.Dashes);
        });

        services.AddScoped<RymdDashManager>(sp =>
        {
            var config = sp.GetRequiredService<GameConfiguration>();
            return new RymdDashManager(config.Pools.RymdDashes, config.Effects);
        });

        return services;
    }
}
