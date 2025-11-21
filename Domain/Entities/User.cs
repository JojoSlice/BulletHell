using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class User
{
    public int Id { get; init; }

    [Required]
    [MaxLength(50)]
    public string UserName { get; init; } = string.Empty;

    [Required]
    public string PasswordHash { get; init; } = string.Empty;

    public List<HighScore> HighScores { get; init; } = [];
}
