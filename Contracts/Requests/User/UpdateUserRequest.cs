namespace Contracts.Requests.User;

internal record UpdateUserRequest : UserRequest
{
    public required int Id { get; init; }
}
