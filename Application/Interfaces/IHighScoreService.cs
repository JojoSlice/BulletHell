namespace Application.Interfaces;

using Contracts.Responses.HighScore;
using Contracts.Responses.Shared;
using Domain.Entities;

public interface IHighScoreService
{
    public Task<Response<List<HighScoreResponse>>> GetAll();
    public Task<Response<HighScoreResponse?>> GetById(int id);
    public Task<Response<HighScoreResponse>> Create(HighScore highScore);
    public Task<Response<HighScoreResponse>> Update(HighScore highScore);
    public Task<Response<string>> Delete(int id);
}