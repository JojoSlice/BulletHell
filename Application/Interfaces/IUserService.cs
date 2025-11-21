using Contracts.Requests.User;
using Contracts.Responses.Common;

namespace Application.Interfaces;

public interface IUserService<T>
    where T : class
{
    Task<Response<List<T>>> GetAll();
    Task<Response<T?>> GetById(int id);
    Task<Response<T>> Create(CreateUserRequest user);
    Task<Response<T>> Update(UpdateUserRequest user);
    Task<Response<string>> Delete(int id);
}
