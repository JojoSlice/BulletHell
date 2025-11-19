namespace Repository.ServiceCollection;

using Microsoft.Extensions.DependencyInjection;
using Data;
using Domain.Entities;
using Interfaces;
using Repositories;

public static class RepoServiceCollectionExtension
{
    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        services.AddDbContext<MyDbContext>();
        services.AddScoped<IRepository<User>, UserRepository>();
        services.AddScoped<IRepository<HighScore>, HighScoreRepository>();
        return services;
    }
}