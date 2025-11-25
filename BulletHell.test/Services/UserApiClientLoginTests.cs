using BulletHell.Interfaces;
using BulletHell.Services;
using BulletHell.test.TestUtilities;

namespace BulletHell.test.Services;

public class UserApiClientLoginTests
{
    private readonly IUserApiClient _apiClient;
    private readonly HttpClient _httpClient;

    public UserApiClientLoginTests()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };
        _apiClient = new UserApiClient(_httpClient);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnSuccess_WhenCredentialsAreValid()
    {
        // Arrange
        var fakeHandler = new FakeHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("/api/users/login", request.RequestUri?.AbsolutePath);

            return FakeHttpResponseBuilder.CreateLoginSuccessResponse(
                userId: 42,
                username: "testuser"
            );
        });

        var httpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("http://localhost:5000"),
        };

        var apiClient = new UserApiClient(httpClient);

        // Act
        var result = await apiClient.LoginAsync("testuser", "$2a$12$hash");

        // Assert
        Assert.True(result.Success);
        Assert.Equal(42, result.UserId);
        Assert.Contains("success", result.Message.ToLower());
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenCredentialsAreInvalid()
    {
        // Arrange
        var fakeHandler = new FakeHttpMessageHandler(request =>
            FakeHttpResponseBuilder.CreateLoginFailureResponse()
        );

        var httpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("http://localhost:5000"),
        };

        var apiClient = new UserApiClient(httpClient);

        // Act
        var result = await apiClient.LoginAsync("testuser", "$2a$12$wronghash");

        // Assert
        Assert.False(result.Success);
        Assert.Contains("password", result.Message.ToLower());
        Assert.Null(result.UserId);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenApiReturns400()
    {
        // Arrange
        var fakeHandler = new FakeHttpMessageHandler(request =>
            FakeHttpResponseBuilder.CreateBadRequestResponse("Invalid request")
        );

        var httpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("http://localhost:5000"),
        };

        var apiClient = new UserApiClient(httpClient);

        // Act
        var result = await apiClient.LoginAsync("testuser", "$2a$12$hash");

        // Assert
        Assert.False(result.Success);
        Assert.Contains("credentials", result.Message.ToLower());
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenApiReturns500()
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
        var result = await apiClient.LoginAsync("testuser", "$2a$12$hash");

        // Assert
        Assert.False(result.Success);
        Assert.Contains("error", result.Message.ToLower());
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowArgumentNullException_WhenUsernameIsNull()
    {
        // Arrange
        string? username = null;
        var passwordHash = "$2a$12$hashedpassword";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _apiClient.LoginAsync(username!, passwordHash)
        );
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowArgumentNullException_WhenPasswordHashIsNull()
    {
        // Arrange
        var username = "testuser";
        string? passwordHash = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _apiClient.LoginAsync(username, passwordHash!)
        );
    }

    [Fact]
    public async Task LoginAsync_ShouldHandleNetworkError_Gracefully()
    {
        // Arrange
        var fakeHandler = new FakeHttpMessageHandler(request =>
            throw new HttpRequestException("Network error")
        );

        var httpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("http://localhost:5000"),
        };

        var apiClient = new UserApiClient(httpClient);

        // Act
        var result = await apiClient.LoginAsync("testuser", "$2a$12$hash");

        // Assert
        Assert.False(result.Success);
        Assert.Contains("connect", result.Message.ToLower());
    }

    [Fact]
    public async Task LoginAsync_ShouldHandleTimeout_Gracefully()
    {
        // Arrange
        var fakeHandler = new FakeHttpMessageHandler(request =>
            throw new TaskCanceledException("Request timed out")
        );

        var httpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("http://localhost:5000"),
        };

        var apiClient = new UserApiClient(httpClient);

        // Act
        var result = await apiClient.LoginAsync("testuser", "$2a$12$hash");

        // Assert
        Assert.False(result.Success);
        Assert.Contains("timed out", result.Message.ToLower());
    }
}
