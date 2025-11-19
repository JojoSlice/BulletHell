using Microsoft.Extensions.DependencyInjection;
using Repository.Data;
using Repository.Interfaces;
using Repository.Repositories;

namespace Repository.ServiceCollectionExtensions;

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
