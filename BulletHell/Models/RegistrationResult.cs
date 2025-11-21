namespace BulletHell.Models;

public record RegistrationResult
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public int? UserId { get; init; }
}
