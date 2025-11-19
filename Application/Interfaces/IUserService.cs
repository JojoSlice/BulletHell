namespace Application.Interfaces;

using Contracts.Requests.User;
using Contracts.Responses.Shared;

public interface IUserService<T> where T : class
{
    public Task<Response<List<T>>> GetAll();
    public Task<Response<T?>> GetById(int id);
    public Task<Response<T>> Create(CreateUserRequest user);
    public Task<Response<T>> Update(UpdateUserRequest user);
    public Task<Response<string>> Delete(int id);
}