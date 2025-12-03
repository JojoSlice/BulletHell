using System;
using System.Net.Http;
using System.Threading.Tasks;
using BulletHell.Interfaces;
using BulletHell.Services;
using Xunit;

namespace BulletHell.test.Integration;

public class UserRegistrationIntegrationTests
{
    private readonly IApiClient _apiClient;
    private readonly IPasswordHasher _passwordHasher;

    public UserRegistrationIntegrationTests()
    {
        var httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };
        _apiClient = new ApiClient(httpClient);
        _passwordHasher = new BCryptPasswordHasher();
    }

    [Fact(Skip = "Requires running API server")]
    public async Task FullRegistrationFlow_ShouldSucceed_WithRealApiCall()
    {
        // Arrange
        var username = $"testuser_{Guid.NewGuid():N}";
        var password = "SecurePassword123!";

        // Act
        var passwordHash = _passwordHasher.HashPassword(password);
        var result = await _apiClient.RegisterUserAsync(username, passwordHash);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.UserId);
        Assert.True(result.UserId > 0);
    }

    [Fact(Skip = "Requires running API server")]
    public async Task RegistrationFlow_ShouldFail_WhenUsernameIsDuplicate()
    {
        // Arrange
        var username = "duplicateuser";
        var password = "Password123!";
        var passwordHash = _passwordHasher.HashPassword(password);

        // Skapa första användaren
        await _apiClient.RegisterUserAsync(username, passwordHash);

        // Act - Försök skapa igen
        var result = await _apiClient.RegisterUserAsync(username, passwordHash);

        // Assert
        Assert.False(result.Success);
    }
}
