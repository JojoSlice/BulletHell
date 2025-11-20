namespace Application.Services;

using Contracts.Requests.HighScore;
using Contracts.Responses.HighScore;
using Contracts.Responses.Shared;
using Domain.Entities;
using Interfaces;
using Mapping;
using Repository.Interfaces;

public class HighScoreService(IRepository<HighScore> repository) : IHighScoreService<HighScoreResponse>
{
    public async Task<Response<List<HighScoreResponse>>> GetAll() =>
        (await repository.GetAllAsync()).MapToResponse();

    public async Task<Response<HighScoreResponse?>> GetById(int id) =>
        (await repository.GetByIdAsync(id)).MapToResponse();

    public async Task<Response<HighScoreResponse>> Create(CreateHighScoreRequest highScore) =>
        (await repository.CreateAsync(highScore.MapToDomain())).MapToResponse()!;

    public async Task<Response<HighScoreResponse>> Update(UpdateHighScoreRequest highScore) =>
        (await repository.UpdateAsync(highScore.MapToDomain())).MapToResponse()!;

    public async Task<Response<string>> Delete(int id) =>
        (await repository.DeleteAsync(id)).MapToResponse();
}