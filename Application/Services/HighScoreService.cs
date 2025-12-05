using Application.Interfaces;
using Application.Mapping;
using Contracts.Requests.HighScore;
using Contracts.Responses.Common;
using Contracts.Responses.HighScore;
using Microsoft.EntityFrameworkCore;
using Repository.Data;

namespace Application.Services;

public class HighScoreService(MyDbContext context) : IHighScoreService<HighScoreResponse>
{
    public async Task<Response<List<HighScoreResponse>>> GetAll() =>
        (await context.Highscores.ToListAsync()).MapToResponse();

    public async Task<Response<HighScoreResponse?>> GetById(int id) =>
        (await context.Highscores.FindAsync(id)).MapToResponse();

    public async Task<Response<HighScoreResponse>> Create(CreateHighScoreRequest highScore)
    {
        var newHighScore = context.Highscores.Add(highScore.MapToDomain()).Entity;
        await context.SaveChangesAsync();
        return newHighScore.MapToResponse()!;
    }

    public async Task<Response<HighScoreResponse>> Update(UpdateHighScoreRequest highScore)
    {
        var updatedHighScore = context.Highscores.Update(highScore.MapToDomain()).Entity;
        await context.SaveChangesAsync();
        return updatedHighScore.MapToResponse()!;
    }

    public async Task<Response<string>> Delete(int id)
    {
        var highScore = await context.Highscores.FindAsync(id);
        if (highScore == null)
        {
            return new Response<string>
            {
                IsSuccess = false,
                Data = null,
            };
        }

        context.Highscores.Remove(highScore);
        await context.SaveChangesAsync();

        return new Response<string>()
        {
            IsSuccess = true,
            Data = $"HighScore with id {id} was deleted."
        };
    }
}
