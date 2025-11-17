namespace Repository.Repositories;

using Data;
using Domain.Entities;
using Interfaces;
using Microsoft.EntityFrameworkCore;

public class UserRepository(MyDbContext context) : IRepository<User>
{
    private readonly MyDbContext _db = context;

    public async Task<List<User>> GetAllAsync() => await _db.Users.ToListAsync();

    public async Task<User?> GetByIdAsync(int id) => await _db.Users.FindAsync(id);

    public async Task<User> CreateAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<string> DeleteAsync(int id)
    {
        var user =
            await _db.Users.FindAsync(id)
            ?? throw new KeyNotFoundException($"User with ID {id} not found.");

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return $"User with id {id} was deleted.";
    }
}
