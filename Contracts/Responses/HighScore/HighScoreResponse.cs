namespace Contracts.Responses.HighScore;

public record HighScoreResponse
{
    public required int Id { get; init; }
    public required int Score { get; init; }
    public required int UserId { get; init; }
    public required string UserName { get; init; }
}
