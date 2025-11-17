namespace Repository.Repositories;

using Data;
using Domain.Entities;
using Interfaces;
using Microsoft.EntityFrameworkCore;

public class HighScoreRepository(MyDbContext context) : IRepository<HighScore>
{
    private readonly MyDbContext _db = context;

    public async Task<List<HighScore>> GetAsync() => await _db.Highscores.ToListAsync();

    public async Task<HighScore?> GetByIdAsync(int id) => await _db.Highscores.FindAsync(id);

    public async Task<HighScore> CreateAsync(HighScore highScore)
    {
        _db.Highscores.Add(highScore);
        await _db.SaveChangesAsync();
        return highScore;
    }

    public async Task<HighScore> UpdateAsync(HighScore highScore)
    {
        _db.Highscores.Update(highScore);
        await _db.SaveChangesAsync();
        return highScore;
    }

    public async Task<string> DeleteAsync(int id)
    {
        var highScore =
            await _db.Highscores.FindAsync(id)
            ?? throw new KeyNotFoundException($"Highscore with ID {id} not found.");

        _db.Highscores.Remove(highScore);
        await _db.SaveChangesAsync();
        return $"HighScore with id {id} was deleted.";
    }
}

