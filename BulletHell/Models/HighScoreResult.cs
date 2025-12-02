namespace BulletHell.Models;

public record HighScoreResult
{
    public required int Id { get; init; }
    public required int Score { get; init; }
    public required int UserId { get; init; }
}
