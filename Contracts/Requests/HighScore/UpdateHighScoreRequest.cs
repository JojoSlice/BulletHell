namespace Contracts.Requests.HighScore;

public record UpdateHighScoreRequest : HighScoreRequest
{
    public required int Id { get; init; }
}
