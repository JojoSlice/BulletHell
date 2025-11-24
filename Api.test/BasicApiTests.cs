using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Api.test
{
    public class BasicApiTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;

        public BasicApiTests(CustomWebApplicationFactory<Program> factory, ITestOutputHelper output)
        {
            _output = output;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Api_Starts_WithoutErrors()
        {
            // Act
            var response = await _client.GetAsync("/api/users");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
            Assert.True(response.Content.Headers.ContentLength > 0);

            // Output
            _output.WriteLine($"StatusCode: {(int)response.StatusCode} ({response.StatusCode})");

        }
    }
}