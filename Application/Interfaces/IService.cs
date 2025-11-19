using Contracts.Responses.Common;

namespace Application.Interfaces;

public interface IService<T, U>
    where T : class
    where U : class
{
    Task<Response<List<T>>> GetAll();
    Task<Response<T?>> GetById(int id);
    Task<Response<T>> Create(U param);
    Task<Response<T>> Update(U param);
    Task<Response<string>> Delete(int id);
}
