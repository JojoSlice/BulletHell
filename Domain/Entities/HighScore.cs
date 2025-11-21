namespace Domain.Entities;

public class HighScore
{
    public int Id { get; init; }

    public int Score { get; init; } = 0;

    public int UserId { get; init; }

    public User? User { get; init; }
}
