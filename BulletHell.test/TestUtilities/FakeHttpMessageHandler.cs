using System.Net;
using System.Text;
using System.Text.Json;

namespace BulletHell.test.TestUtilities;

/// <summary>
/// Fake HttpMessageHandler för att mocka HTTP-anrop i tester
/// </summary>
public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _responseBuilder;

    public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseBuilder)
    {
        _responseBuilder = responseBuilder;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        return Task.FromResult(_responseBuilder(request));
    }
}

/// <summary>
/// Builder för att skapa fake HTTP-responses
/// </summary>
public static class FakeHttpResponseBuilder
{
    public static HttpResponseMessage CreateUserCreatedResponse(int userId)
    {
        var response = new { id = userId, userName = "testuser" };

        return new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(response),
                Encoding.UTF8,
                "application/json"
            ),
        };
    }

    public static HttpResponseMessage CreateBadRequestResponse(string message)
    {
        var response = new { error = message };

        return new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(response),
                Encoding.UTF8,
                "application/json"
            ),
        };
    }

    public static HttpResponseMessage CreateServerErrorResponse()
    {
        return new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("Internal Server Error"),
        };
    }
}
