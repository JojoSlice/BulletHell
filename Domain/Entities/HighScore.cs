namespace Domain.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class HighScore
{
    public int Id { get; set; }

    public int Score { get; set; } = 0;

    public int UserId { get; set; }

    public User? User { get; set; }
}
