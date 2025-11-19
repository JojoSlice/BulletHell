namespace Application.Interfaces;

using Contracts.Requests.HighScore;
using Contracts.Responses.Shared;

public interface IHighScoreService<T> where T : class
{
    public Task<Response<List<T>>> GetAll();
    public Task<Response<T?>> GetById(int id);
    public Task<Response<T>> Create(CreateHighScoreRequest highScore);
    public Task<Response<T>> Update(UpdateHighScoreRequest highScore);
    public Task<Response<string>> Delete(int id);
}