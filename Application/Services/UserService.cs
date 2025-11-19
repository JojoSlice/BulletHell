namespace Application.Services;

using Contracts.Requests.User;
using Contracts.Responses.Shared;
using Contracts.Responses.User;
using Domain.Entities;
using Interfaces;
using Mapping;
using Repository.Interfaces;

public class UserService(IRepository<User> repository) : IUserService<UserResponse>
{
    public async Task<Response<List<UserResponse>>> GetAll() =>
        (await repository.GetAllAsync()).MapToResponse();

    public async Task<Response<UserResponse?>> GetById(int id) =>
        (await repository.GetByIdAsync(id)).MapToResponse();

    public async Task<Response<UserResponse>> Create(CreateUserRequest user) => //TODO: Map request to entity
        (await repository.CreateAsync(user)).MapToResponse()!;

    public async Task<Response<UserResponse>> Update(UpdateUserRequest user) => //TODO: Map request to entity
        (await repository.UpdateAsync(user)).MapToResponse()!;

    public async Task<Response<string>> Delete(int id) =>
        (await repository.DeleteAsync(id)).MapToResponse();
}