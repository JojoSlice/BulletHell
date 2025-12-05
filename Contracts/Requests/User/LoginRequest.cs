using System.ComponentModel.DataAnnotations;

namespace Contracts.Requests.User;

public record LoginRequest
{
    [MaxLength(50)]
    public required string UserName { get; init; }
    public required string Password { get; init; }
}
