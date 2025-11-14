namespace Repository.Repositories;

using Data;
using Domain.Entities;
using Interfaces;
using Microsoft.EntityFrameworkCore;

public class HighScoreRepository(MyDbContext context) : IRepository<HighScore>
{
    private readonly MyDbContext _db = context;

    public Task<List<HighScore>> GetAsync() => _db.Highscores.ToListAsync();

    public Task<HighScore?> GetByIdAsync(int id) => _db.Highscores.FindAsync(id).AsTask();

    public async Task<HighScore> CreateAsync(HighScore highScore)
    {
        _db.Highscores.Add(highScore);
        await _db.SaveChangesAsync();
        return highScore;
    }

    public async Task UpdateAsync(HighScore highScore)
    {
        _db.Highscores.Update(highScore);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var highScore =
            await _db.Highscores.FindAsync(id)
            ?? throw new KeyNotFoundException($"Highscore with ID {id} not found.");

        _db.Highscores.Remove(highScore);
        await _db.SaveChangesAsync();
    }
}

