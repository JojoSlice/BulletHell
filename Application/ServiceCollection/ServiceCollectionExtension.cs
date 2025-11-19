namespace Application.ServiceCollection;

using Contracts.Responses.HighScore;
using Contracts.Responses.User;
using Domain.Entities;
using Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Services;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IService<HighScoreResponse,HighScore>, HighScoreService>();
        services.AddScoped<IService<UserResponse, User>, UserService>();
        return services;
    }
}