using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Interfaces;

namespace Repository.Repositories;

public class UserRepository(MyDbContext context) : IRepository<User>
{
    public async Task<List<User>> GetAllAsync() => await context.Users.ToListAsync();

    public async Task<User?> GetByIdAsync(int id) => await context.Users.FindAsync(id);

    public async Task<User> CreateAsync(User user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<string> DeleteAsync(int id)
    {
        var user =
            await context.Users.FindAsync(id)
            ?? throw new KeyNotFoundException($"User with ID {id} not found.");

        context.Users.Remove(user);
        await context.SaveChangesAsync();
        return $"User with id {id} was deleted.";
    }
}
