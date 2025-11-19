namespace Contracts.Responses.Common;

public record Response<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
}
