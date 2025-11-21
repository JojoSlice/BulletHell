namespace Contracts.Responses.Common;

public record GetAllResponse<T>
{
    public required IEnumerable<T?> Items { get; init; }
}
