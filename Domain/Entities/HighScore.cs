namespace Domain.Entities;

public class HighScore
{
    public int Id { get; set; }

    public int Score { get; set; } = 0;

    public int UserId { get; set; }

    public User? User { get; set; }
}
