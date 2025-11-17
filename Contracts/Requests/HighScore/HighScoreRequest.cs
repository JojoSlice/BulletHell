namespace Contracts.Requests.HighScore;

public record HighScoreRequest
{
    public int? Score { get; init; }
    public int? UserId { get; init; }
}