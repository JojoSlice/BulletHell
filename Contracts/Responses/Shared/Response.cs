namespace Contracts.Responses.Shared;

public record Response<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
}