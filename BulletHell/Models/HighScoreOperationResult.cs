namespace BulletHell.Models;

public record HighScoreOperationResult
{
    public bool Success { get; init; }
    public required string Message { get; init; }
    public HighScoreResult? Data { get; init; }
}
