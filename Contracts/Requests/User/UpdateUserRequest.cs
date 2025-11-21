namespace Contracts.Requests.User;

public record UpdateUserRequest : UserRequest
{
    public required int Id { get; init; }
}
