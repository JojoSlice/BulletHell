namespace Application.Services;

using Contracts.Responses.Shared;
using Contracts.Responses.User;
using Domain.Entities;
using Interfaces;
using Mapping;
using Repository.Interfaces;

public class UserService(IRepository<User> repository) : IService<UserResponse, User>
{
    public async Task<Response<List<UserResponse>>> GetAll() =>
        (await repository.GetAllAsync()).MapToResponse();

    public async Task<Response<UserResponse?>> GetById(int id) =>
        (await repository.GetByIdAsync(id)).MapToResponse();

    public async Task<Response<UserResponse>> Create(User user) =>
        (await repository.CreateAsync(user)).MapToResponse()!;

    public async Task<Response<UserResponse>> Update(User user) =>
        (await repository.UpdateAsync(user)).MapToResponse()!;

    public async Task<Response<string>> Delete(int id) =>
        (await repository.DeleteAsync(id)).MapToResponse();
}