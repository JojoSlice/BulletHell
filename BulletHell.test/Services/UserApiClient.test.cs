using BulletHell.Interfaces;
using BulletHell.test.TestUtilities;

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
        var fakeHandler = new FakeHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("/api/users", request.RequestUri?.AbsolutePath);

            return FakeHttpResponseBuilder.CreateUserCreatedResponse(userId: 123);
        });

        var httpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("http://localhost:5000"),
        };

        var apiClient = new UserApiClient(httpClient);

        // Act
        var result = await apiClient.RegisterUserAsync("testuser", "$2a$12$hash");

        // Assert
        Assert.True(result.Success);
        Assert.Equal(123, result.UserId);
        Assert.Contains("success", result.Message.ToLower());
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldReturnFailure_WhenApiReturns400()
    {
        // Arrange
        var fakeHandler = new FakeHttpMessageHandler(request =>
            FakeHttpResponseBuilder.CreateBadRequestResponse("Username är obligatoriskt")
        );

        var httpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("http://localhost:5000"),
        };

        var apiClient = new UserApiClient(httpClient);

        // Act
        var result = await apiClient.RegisterUserAsync("", "$2a$12$hash");

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Username är obligatoriskt", result.Message);
        Assert.Null(result.UserId);
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldReturnFailure_WhenApiReturns500()
    {
        // Arrange
        var fakeHandler = new FakeHttpMessageHandler(request =>
            FakeHttpResponseBuilder.CreateServerErrorResponse()
        );

        var httpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("http://localhost:5000"),
        };

        var apiClient = new UserApiClient(httpClient);

        // Act
        var result = await apiClient.RegisterUserAsync("testuser", "$2a$12$hash");

        // Assert
        Assert.False(result.Success);
        Assert.Contains("server", result.Message.ToLower());
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
