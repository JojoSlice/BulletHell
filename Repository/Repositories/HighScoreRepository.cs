using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Interfaces;

namespace Repository.Repositories;

public class HighScoreRepository(MyDbContext context) : IRepository<HighScore>
{
    public async Task<List<HighScore>> GetAllAsync() => await context.Highscores.ToListAsync();

    public async Task<HighScore?> GetByIdAsync(int id) => await context.Highscores.FindAsync(id);

    public async Task<HighScore> CreateAsync(HighScore highScore)
    {
        context.Highscores.Add(highScore);
        await context.SaveChangesAsync();
        return highScore;
    }

    public async Task<HighScore> UpdateAsync(HighScore highScore)
    {
        context.Highscores.Update(highScore);
        await context.SaveChangesAsync();
        return highScore;
    }

    public async Task<string> DeleteAsync(int id)
    {
        var highScore =
            await context.Highscores.FindAsync(id)
            ?? throw new KeyNotFoundException($"Highscore with ID {id} not found.");

        context.Highscores.Remove(highScore);
        await context.SaveChangesAsync();
        return $"HighScore with id {id} was deleted.";
    }
}
