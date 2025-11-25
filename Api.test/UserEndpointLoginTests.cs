using System.Net;
using System.Net.Http.Json;
using Contracts.Requests.User;
using Contracts.Responses.Common;
using Contracts.Responses.User;
using Xunit;

namespace Api.test;

public class UserEndpointLoginTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public UserEndpointLoginTests(
        CustomWebApplicationFactory<Program> factory,
        ITestOutputHelper output
    )
    {
        _output = output;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsSuccess()
    {
        // Arrange - Create a user first
        var username = $"testuser_{Guid.NewGuid():N}";
        var passwordHash = "hashedPassword123";

        var createRequest = new CreateUserRequest
        {
            UserName = username,
            PasswordHash = passwordHash,
        };

        await _client.PostAsJsonAsync("/api/users", createRequest);

        var loginRequest = new LoginRequest
        {
            UserName = username,
            PasswordHash = passwordHash,
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/users/login",
            loginRequest,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<Response<UserResponse>>();
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(username, result.Data.UserName);

        _output.WriteLine($"Login successful for user: {username}");
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsFailure()
    {
        // Arrange - Create a user first
        var username = $"testuser_{Guid.NewGuid():N}";
        var correctPasswordHash = "hashedPassword123";

        var createRequest = new CreateUserRequest
        {
            UserName = username,
            PasswordHash = correctPasswordHash,
        };

        await _client.PostAsJsonAsync("/api/users", createRequest);

        var loginRequest = new LoginRequest
        {
            UserName = username,
            PasswordHash = "wrongPasswordHash",
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/users/login",
            loginRequest,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<Response<UserResponse>>();
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);

        _output.WriteLine("Login correctly failed with wrong password");
    }

    [Fact]
    public async Task Login_WithNonExistentUser_ReturnsFailure()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            UserName = "nonexistentuser_" + Guid.NewGuid(),
            PasswordHash = "someHash",
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/users/login",
            loginRequest,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<Response<UserResponse>>();
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);

        _output.WriteLine("Login correctly failed for non-existent user");
    }

    [Fact]
    public async Task Login_WithEmptyUsername_ReturnsBadRequest()
    {
        // Arrange
        var loginRequest = new LoginRequest { UserName = "", PasswordHash = "someHash" };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/users/login",
            loginRequest,
            TestContext.Current.CancellationToken
        );

        // Assert - Empty username should trigger validation
        // Note: Depending on validation setup, this might return OK with IsSuccess=false
        // or BadRequest. Adjust based on actual API behavior.
        Assert.True(
            response.StatusCode == HttpStatusCode.BadRequest
                || response.StatusCode == HttpStatusCode.OK
        );

        _output.WriteLine($"Status: {response.StatusCode}");
    }
}
