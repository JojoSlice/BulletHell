using Microsoft.Extensions.DependencyInjection;
using Domain.Entities;
using Repository.Data;
using Repository.Interfaces;
using Repository.Repositories;

namespace Repository.ServiceCollection;

public static class RepoServiceCollectionExtension
{
    public static void AddRepositoryServices(this IServiceCollection services)
    {
        services.AddDbContext<MyDbContext>();
        services.AddScoped<IRepository<User>, UserRepository>();
        services.AddScoped<IRepository<HighScore>, HighScoreRepository>();
    }
}
