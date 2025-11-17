namespace Application.Interfaces;

using Contracts.Responses.Shared;

public interface IService<T,U> where T : class where U : class
{
    public Task<Response<List<T>>> GetAll();
    public Task<Response<T?>> GetById(int id);
    public Task<Response<T>> Create(U param);
    public Task<Response<T>> Update(U param);
    public Task<Response<string>> Delete(int id);
}