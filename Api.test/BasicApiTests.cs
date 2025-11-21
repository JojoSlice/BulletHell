using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Api.Tests
{
    public class BasicApiTests(ITestOutputHelper output)
    {
        [Fact]
        public async Task Api_Starts_WithoutErrors()
        {
            // Arrange
            await using var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/users");

            // Assert
            Assert.True(response.IsSuccessStatusCode);

            // Output
            output.WriteLine($"StatusCode: {(int)response.StatusCode} ({response.StatusCode})");

        }
    }
}