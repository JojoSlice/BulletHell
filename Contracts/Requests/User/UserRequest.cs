using System.ComponentModel.DataAnnotations;

namespace Contracts.Requests.User;

public record UserRequest
{
    [MaxLength(50)]
    public required string UserName { get; init; }
    public required string PasswordHash { get; init; }
}
