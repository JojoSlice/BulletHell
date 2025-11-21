using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using BulletHell.Interfaces;
using BulletHell.Models;
using BulletHell.Services;
using NSubstitute;
using Xunit;

namespace BulletHell.test.Services;

public class UserApiClientTests
{
    private readonly IUserApiClient _apiClient;
    private readonly HttpClient _httpClient;

    public UserApiClientTests()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };
        _apiClient = new UserApiClient(_httpClient);
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldReturnSuccess_WhenApiReturns201()
    {
        // Arrange
        var username = "testuser";
        var passwordHash = "$2a$12$hashedpassword";

        // Act
        var result = await _apiClient.RegisterUserAsync(username, passwordHash);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.UserId);
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldReturnFailure_WhenApiReturns400()
    {
        // Arrange
        var username = "";
        var passwordHash = "$2a$12$hashedpassword";

        // Act
        var result = await _apiClient.RegisterUserAsync(username, passwordHash);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("validation", result.Message.ToLower());
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldThrowArgumentNullException_WhenUsernameIsNull()
    {
        // Arrange
        string? username = null;
        var passwordHash = "$2a$12$hashedpassword";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _apiClient.RegisterUserAsync(username!, passwordHash)
        );
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldThrowArgumentNullException_WhenPasswordHashIsNull()
    {
        // Arrange
        var username = "testuser";
        string? passwordHash = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _apiClient.RegisterUserAsync(username, passwordHash!)
        );
    }
}
