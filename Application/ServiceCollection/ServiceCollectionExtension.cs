using Application.Interfaces;
using Application.Services;
using Contracts.Responses.HighScore;
using Contracts.Responses.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repository.Data;

namespace Application.ServiceCollection;

public static class ServiceCollectionExtension
{
    private static readonly string _dbPath = MyDbContext.GetDefaultDatabasePath();
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddDbContext<MyDbContext>(options =>
            options.UseSqlite($"Data Source={_dbPath}"));
        services.AddScoped<IHighScoreService<HighScoreResponse>, HighScoreService>();
        services.AddScoped<IUserService<UserResponse>, UserService>();
        return services;
    }

    public static void EnsureDatabaseCreated(this IServiceProvider serviceProvider)
    {
        if (!File.Exists(_dbPath))
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();
            db.Database.EnsureCreated();
        }
    }
}
