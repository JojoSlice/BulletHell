using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Api.test;

public class HighScoreEndPointTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public HighScoreEndPointTests(CustomWebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        _output = output;
        _client = factory.CreateClient();
    }
    [Fact]
    public async Task GetHighScores_Returns200Ok()
    {
         // Act
         var response = await _client.GetAsync("/api/highscores", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Output
        _output.WriteLine($"Status: {response.StatusCode}");
    }
}