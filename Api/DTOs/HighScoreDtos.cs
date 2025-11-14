using System.ComponentModel.DataAnnotations;

namespace Api.DTOs;

public class HighScoreDto
{
    public int Id { get; set; }
    public int Score { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
}

public class CreateHighScoreDto
{
    [Required]
    public int Score { get; set; }

    [Required]
    public int UserId { get; set; }
}
