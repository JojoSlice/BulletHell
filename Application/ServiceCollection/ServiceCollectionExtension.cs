using Application.Interfaces;
using Application.Services;
using Contracts.Responses.HighScore;
using Contracts.Responses.User;
using Microsoft.Extensions.DependencyInjection;

namespace Application.ServiceCollection;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IHighScoreService<HighScoreResponse>, HighScoreService>();
        services.AddScoped<IUserService<UserResponse>, UserService>();
        return services;
    }
}
