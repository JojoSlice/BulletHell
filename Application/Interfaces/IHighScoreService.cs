using Contracts.Requests.HighScore;
using Contracts.Responses.Common;

namespace Application.Interfaces;

public interface IHighScoreService<T> where T : class
{
    Task<Response<List<T>>> GetAll();
    Task<Response<T?>> GetById(int id);
    Task<Response<T>> Create(CreateHighScoreRequest highScore);
    Task<Response<T>> Update(UpdateHighScoreRequest highScore);
    Task<Response<string>> Delete(int id);
}
