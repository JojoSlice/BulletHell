using Application.Interfaces;
using Application.Mapping;
using Contracts.Requests.User;
using Contracts.Responses.Common;
using Contracts.Responses.User;
using Domain.Entities;
using Interfaces;
using Mapping;
using Repository.Interfaces;
using Repository.Repositories;

namespace Application.Services;

public class UserService(IRepository<User> repository) : IUserService<UserResponse>

public class UserService(UserRepository repository) : IService<UserResponse, User>
{
    public async Task<Response<List<UserResponse>>> GetAll() =>
        (await repository.GetAllAsync()).MapToResponse();

    public async Task<Response<UserResponse?>> GetById(int id) =>
        (await repository.GetByIdAsync(id)).MapToResponse();

    public async Task<Response<UserResponse>> Create(CreateUserRequest user) =>
        (await repository.CreateAsync(user.MapToDomain())).MapToResponse()!;

    public async Task<Response<UserResponse>> Update(UpdateUserRequest user) =>
        (await repository.UpdateAsync(user.MapToDomain())).MapToResponse()!;

    public async Task<Response<string>> Delete(int id) =>
        (await repository.DeleteAsync(id)).MapToResponse();
}
