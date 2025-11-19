namespace Application.ServiceCollection;

using Contracts.Responses.HighScore;
using Contracts.Responses.User;
using Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Services;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IHighScoreService<HighScoreResponse>, HighScoreService>();
        services.AddScoped<IUserService<UserResponse>, UserService>();
        return services;
    }
}