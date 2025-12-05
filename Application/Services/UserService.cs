using Application.Interfaces;
using Application.Mapping;
using Contracts.Requests.User;
using Contracts.Responses.Common;
using Contracts.Responses.User;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using BCrypt.Net;

namespace Application.Services;

public class UserService(MyDbContext dbContext) : IUserService<UserResponse>
{
    public async Task<Response<List<UserResponse>>> GetAll() =>
        (await dbContext.Users.ToListAsync()).MapToResponse();

    public async Task<Response<UserResponse?>> GetById(int id) =>
        (await dbContext.Users.FindAsync(id)).MapToResponse();

    public async Task<Response<UserResponse>> Create(CreateUserRequest user)
    {
        var entry = await dbContext.Users.AddAsync(user.MapToDomain());
        await dbContext.SaveChangesAsync();
        return entry.Entity.MapToResponse()!;
    }

    public async Task<Response<UserResponse>> Update(UpdateUserRequest user)
    {
        var entry = dbContext.Users.Update(user.MapToDomain());
        await dbContext.SaveChangesAsync();
        return entry.Entity.MapToResponse()!;
    }

    public async Task<Response<string>> Delete(int id)
    {
        var entity = await dbContext.Users.FindAsync(id);
        if (entity == null)
        {
            return new Response<string>
            {
                IsSuccess = false,
                Data = null,
            };
        }

        dbContext.Users.Remove(entity);
        await dbContext.SaveChangesAsync();

        return new Response<string>
        {
            IsSuccess = true,
            Data = "Deleted",
        };
    }

    public async Task<Response<UserResponse?>> Login(LoginRequest request)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);

        if (user == null)
        {
            return new Response<UserResponse?>
            {
                IsSuccess = false,
                Data = null,
            };
        }

        // Verify password using BCrypt
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
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
