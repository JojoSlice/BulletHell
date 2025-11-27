using Application.Interfaces;
using Application.Mapping;
using Contracts.Requests.User;
using Contracts.Responses.Common;
using Contracts.Responses.User;
using Domain.Entities;
using Repository.Interfaces;

namespace Application.Services;

public class UserService(IRepository<User> repository) : IUserService<UserResponse>
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

    public async Task<Response<UserResponse?>> Login(LoginRequest request)
    {
        var user = await repository.GetByUsernameAsync(request.UserName);

        if (user == null
        || (user.PasswordHash != request.PasswordHash))
        {
            return new Response<UserResponse?>
            {
                IsSuccess = false,
                Data = null,
            };
        }

        return user.MapToResponse();
    }
}
