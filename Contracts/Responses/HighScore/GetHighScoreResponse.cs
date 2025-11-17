namespace Contracts.Responses.HighScore;

public record GetHighScoreResponse : HighScoreResponse
{
    public int Id { get; init; }
}