namespace Contracts.Responses.User;

public record UserResponse
{
    public required int Id { get; init; }
    public required string UserName { get; init; }
}
