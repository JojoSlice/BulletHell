namespace Contracts.Responses.Shared;

public record GetAllResponse<T>
{
    public required IEnumerable<T?> Items { get; init; }
}