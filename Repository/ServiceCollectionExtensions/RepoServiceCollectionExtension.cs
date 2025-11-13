namespace Repository.ServiceCollectionExtensions;

using Data;
using Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Repositories;

public static class RepoServiceCollectionExtension
{
    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        services.AddScoped<IRepository<UserRepository>>();
        services.AddScoped<IRepository<HighScoreRepository>>();
        services.AddScoped<MyDbContext>();
        return services;
    }
}