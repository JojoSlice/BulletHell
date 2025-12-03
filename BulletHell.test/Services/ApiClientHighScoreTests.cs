using BulletHell.Interfaces;
using BulletHell.Services;
using BulletHell.test.TestUtilities;

namespace BulletHell.test.Services;

public class ApiClientHighScoreTests
{
    // GetAllHighScoresAsync Tests
    [Fact]
    public async Task GetAllHighScoresAsync_ShouldReturnSuccess_WhenApiReturnsData()
    {
        // Arrange
        var highScores = new List<(int id, int score, int userId)>
        {
            (1, 1000, 10),
            (2, 2000, 20),
            (3, 3000, 30),
        };

        var fakeHandler = new FakeHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.Equal("/api/highscores", request.RequestUri?.AbsolutePath);

            return FakeHttpResponseBuilder.CreateHighScoreListResponse(highScores);
        });

        var httpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("http://localhost:5000"),
        };

        var apiClient = new ApiClient(httpClient);

        // Act
        var result = await apiClient.GetAllHighScoresAsync();

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(3, result.Data.Count);
        Assert.Equal(1000, result.Data[0].Score);
    }

    [Fact]
    public async Task GetAllHighScoresAsync_ShouldReturnEmptyList_WhenNoHighScoresExist()
    {
        // Arrange
        var highScores = new List<(int id, int score, int userId)>();

        var fakeHandler = new FakeHttpMessageHandler(request =>
            FakeHttpResponseBuilder.CreateHighScoreListResponse(highScores)
        );

        var httpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("http://localhost:5000"),
        };

        var apiClient = new ApiClient(httpClient);

        // Act
        var result = await apiClient.GetAllHighScoresAsync();

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task GetAllHighScoresAsync_ShouldHandleServerError_Gracefully()
    {
        // Arrange
        var fakeHandler = new FakeHttpMessageHandler(request =>
            FakeHttpResponseBuilder.CreateServerErrorResponse()
        );

        var httpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("http://localhost:5000"),
        };

        var apiClient = new ApiClient(httpClient);

        // Act
        var result = await apiClient.GetAllHighScoresAsync();

        // Assert
        Assert.False(result.Success);
        Assert.Contains("error", result.Message.ToLower());
    }

    [Fact]
    public async Task GetAllHighScoresAsync_ShouldHandleNetworkError_Gracefully()
    {
        // Arrange
        var fakeHandler = new FakeHttpMessageHandler(request =>
            throw new HttpRequestException("Network error")
        );

        var httpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("http://localhost:5000"),
        };

        var apiClient = new ApiClient(httpClient);

        // Act
        var result = await apiClient.GetAllHighScoresAsync();

        // Assert
        Assert.False(result.Success);
        Assert.Contains("connect", result.Message.ToLower());
    }

    // GetHighScoreByIdAsync Tests
    [Fact]
    public async Task GetHighScoreByIdAsync_ShouldReturnSuccess_WhenHighScoreExists()
    {
        // Arrange
        var fakeHandler = new FakeHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.Equal("/api/highscores/1", request.RequestUri?.AbsolutePath);

            return FakeHttpResponseBuilder.CreateHighScoreResponse(1, 1000, 10);
        });

        var httpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("http://localhost:5000"),
        };

        var apiClient = new ApiClient(httpClient);

        // Act
        var result = await apiClient.GetHighScoreByIdAsync(1);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.Id);
        Assert.Equal(1000, result.Data.Score);
        Assert.Equal(10, result.Data.UserId);
    }

    [Fact]
    public async Task GetHighScoreByIdAsync_ShouldReturnFailure_WhenHighScoreNotFound()
    {
        // Arrange
        var fakeHandler = new FakeHttpMessageHandler(request =>
            FakeHttpResponseBuilder.CreateNotFoundResponse()
        );

        var httpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("http://localhost:5000"),
        };

        var apiClient = new ApiClient(httpClient);

        // Act
        var result = await apiClient.GetHighScoreByIdAsync(999);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("not found", result.Message.ToLower());
    }

    [Fact]
    public async Task GetHighScoreByIdAsync_ShouldHandleServerError_Gracefully()
    {
        // Arrange
        var fakeHandler = new FakeHttpMessageHandler(request =>
            FakeHttpResponseBuilder.CreateServerErrorResponse()
        );

        var httpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("http://localhost:5000"),
        };

        var apiClient = new ApiClient(httpClient);

        // Act
        var result = await apiClient.GetHighScoreByIdAsync(1);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("error", result.Message.ToLower());
    }

    [Fact]
    public async Task GetHighScoreByIdAsync_ShouldHandleTimeout_Gracefully()
    {
        // Arrange
        var fakeHandler = new FakeHttpMessageHandler(request =>
            throw new TaskCanceledException("Request timed out")
        );

        var httpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("http://localhost:5000"),
        };

        var apiClient = new ApiClient(httpClient);

        // Act
        var result = await apiClient.GetHighScoreByIdAsync(1);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("timed out", result.Message.ToLower());
    }

    // CreateHighScoreAsync Tests
    [Fact]
    public async Task CreateHighScoreAsync_ShouldReturnSuccess_WhenHighScoreIsCreated()
    {
        // Arrange
        var fakeHandler = new FakeHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("/api/highscores", request.RequestUri?.AbsolutePath);

            return FakeHttpResponseBuilder.CreateHighScoreCreatedResponse(5, 2000, 15);
        });

        var httpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("http://localhost:5000"),
        };

        var apiClient = new ApiClient(httpClient);

        // Act
        var result = await apiClient.CreateHighScoreAsync(2000, 15);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(5, result.Data.Id);
        Assert.Equal(2000, result.Data.Score);
        Assert.Equal(15, result.Data.UserId);
    }

    [Fact]
    public async Task CreateHighScoreAsync_ShouldReturnFailure_WhenValidationFails()
    {
        // Arrange
        var fakeHandler = new FakeHttpMessageHandler(request =>
            FakeHttpResponseBuilder.CreateBadRequestResponse("Invalid score")
        );

        var httpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("http://localhost:5000"),
        };

        var apiClient = new ApiClient(httpClient);

        // Act
        var result = await apiClient.CreateHighScoreAsync(-100, 15);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("validation", result.Message.ToLower());
    }

    [Fact]
    public async Task CreateHighScoreAsync_ShouldHandleServerError_Gracefully()
    {
        // Arrange
        var fakeHandler = new FakeHttpMessageHandler(request =>
            FakeHttpResponseBuilder.CreateServerErrorResponse()
        );

        var httpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("http://localhost:5000"),
        };

        var apiClient = new ApiClient(httpClient);

        // Act
        var result = await apiClient.CreateHighScoreAsync(2000, 15);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("error", result.Message.ToLower());
    }

    [Fact]
    public async Task CreateHighScoreAsync_ShouldHandleNetworkError_Gracefully()
    {
        // Arrange
        var fakeHandler = new FakeHttpMessageHandler(request =>
            throw new HttpRequestException("Network error")
        );

        var httpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("http://localhost:5000"),
        };

        var apiClient = new ApiClient(httpClient);

        // Act
        var result = await apiClient.CreateHighScoreAsync(2000, 15);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("connect", result.Message.ToLower());
    }

    // UpdateHighScoreAsync Tests
    [Fact]
    public async Task UpdateHighScoreAsync_ShouldReturnSuccess_WhenHighScoreIsUpdated()
    {
        // Arrange
        var fakeHandler = new FakeHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.Equal("/api/highscores", request.RequestUri?.AbsolutePath);

            return FakeHttpResponseBuilder.CreateHighScoreCreatedResponse(1, 5000, 10);
        });

        var httpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("http://localhost:5000"),
        };

        var apiClient = new ApiClient(httpClient);

        // Act
        var result = await apiClient.UpdateHighScoreAsync(1, 5000, 10);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.Id);
        Assert.Equal(5000, result.Data.Score);
    }

    [Fact]
    public async Task UpdateHighScoreAsync_ShouldReturnFailure_WhenHighScoreNotFound()
    {
        // Arrange
        var fakeHandler = new FakeHttpMessageHandler(request =>
            FakeHttpResponseBuilder.CreateNotFoundResponse()
        );

        var httpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("http://localhost:5000"),
        };

        var apiClient = new ApiClient(httpClient);

        // Act
        var result = await apiClient.UpdateHighScoreAsync(999, 5000, 10);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("not found", result.Message.ToLower());
    }

    [Fact]
    public async Task UpdateHighScoreAsync_ShouldReturnFailure_WhenValidationFails()
    {
        // Arrange
        var fakeHandler = new FakeHttpMessageHandler(request =>
            FakeHttpResponseBuilder.CreateBadRequestResponse("Invalid data")
        );

        var httpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("http://localhost:5000"),
        };

        var apiClient = new ApiClient(httpClient);

        // Act
        var result = await apiClient.UpdateHighScoreAsync(1, -100, 10);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("validation", result.Message.ToLower());
    }

    [Fact]
    public async Task UpdateHighScoreAsync_ShouldHandleServerError_Gracefully()
    {
        // Arrange
        var fakeHandler = new FakeHttpMessageHandler(request =>
            FakeHttpResponseBuilder.CreateServerErrorResponse()
        );

        var httpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("http://localhost:5000"),
        };

        var apiClient = new ApiClient(httpClient);

        // Act
        var result = await apiClient.UpdateHighScoreAsync(1, 5000, 10);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("error", result.Message.ToLower());
    }

    [Fact]
    public async Task UpdateHighScoreAsync_ShouldHandleTimeout_Gracefully()
    {
        // Arrange
        var fakeHandler = new FakeHttpMessageHandler(request =>
            throw new TaskCanceledException("Request timed out")
        );

        var httpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("http://localhost:5000"),
        };

        var apiClient = new ApiClient(httpClient);

        // Act
        var result = await apiClient.UpdateHighScoreAsync(1, 5000, 10);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("timed out", result.Message.ToLower());
    }

    // DeleteHighScoreAsync Tests
    [Fact]
    public async Task DeleteHighScoreAsync_ShouldReturnSuccess_WhenHighScoreIsDeleted()
    {
        // Arrange
        var fakeHandler = new FakeHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Delete, request.Method);
            Assert.Equal("/api/highscores/1", request.RequestUri?.AbsolutePath);

            return FakeHttpResponseBuilder.CreateHighScoreDeletedResponse();
        });

        var httpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("http://localhost:5000"),
        };

        var apiClient = new ApiClient(httpClient);

        // Act
        var result = await apiClient.DeleteHighScoreAsync(1);

        // Assert
        Assert.True(result.Success);
        Assert.Contains("deleted", result.Message.ToLower());
    }

    [Fact]
    public async Task DeleteHighScoreAsync_ShouldReturnFailure_WhenHighScoreNotFound()
    {
        // Arrange
        var fakeHandler = new FakeHttpMessageHandler(request =>
            FakeHttpResponseBuilder.CreateNotFoundResponse()
        );

        var httpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("http://localhost:5000"),
        };

        var apiClient = new ApiClient(httpClient);

        // Act
        var result = await apiClient.DeleteHighScoreAsync(999);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("not found", result.Message.ToLower());
    }

    [Fact]
    public async Task DeleteHighScoreAsync_ShouldHandleServerError_Gracefully()
    {
        // Arrange
        var fakeHandler = new FakeHttpMessageHandler(request =>
            FakeHttpResponseBuilder.CreateServerErrorResponse()
        );

        var httpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("http://localhost:5000"),
        };

        var apiClient = new ApiClient(httpClient);

        // Act
        var result = await apiClient.DeleteHighScoreAsync(1);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("error", result.Message.ToLower());
    }

    [Fact]
    public async Task DeleteHighScoreAsync_ShouldHandleNetworkError_Gracefully()
    {
        // Arrange
        var fakeHandler = new FakeHttpMessageHandler(request =>
            throw new HttpRequestException("Network error")
        );

        var httpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("http://localhost:5000"),
        };

        var apiClient = new ApiClient(httpClient);

        // Act
        var result = await apiClient.DeleteHighScoreAsync(1);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("connect", result.Message.ToLower());
    }
}
