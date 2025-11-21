using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Api.test;

public class HighScoreEndpointTests(ITestOutputHelper output)
{
    [Fact]
    public async Task GetHighScores_Returns200Ok()
    {
        // Arrange
        await using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/highscores");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Output
        output.WriteLine($"Status: {response.StatusCode}");
    }
}