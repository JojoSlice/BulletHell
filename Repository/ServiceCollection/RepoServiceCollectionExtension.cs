using Microsoft.Extensions.DependencyInjection;
using Interfaces;
using Domain.Entities;
using Repository.Data;
using Repository.Interfaces;
using Repository.Repositories;

namespace Repository.ServiceCollectionExtensions;

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
