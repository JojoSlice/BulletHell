namespace Contracts.Requests.User;

using System.ComponentModel.DataAnnotations;

public record UserRequest
{
    [MaxLength(50)]
    public required string UserName { get; init; }
    public required string PasswordHash { get; init; }
}