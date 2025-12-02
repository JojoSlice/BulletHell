using System.Collections.Generic;

namespace BulletHell.Models;

public record HighScoreListResult
{
    public bool Success { get; init; }
    public required string Message { get; init; }
    public List<HighScoreResult>? Data { get; init; }
}
