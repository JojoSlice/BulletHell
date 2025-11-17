namespace Application.Services;

using Contracts.Responses.HighScore;
using Contracts.Responses.Shared;
using Domain.Entities;
using Interfaces;
using Mapping;
using Repository.Repositories;

public class HighScoreService(HighScoreRepository repository) : IHighScoreService
{
    public async Task<Response<List<HighScoreResponse>>> GetAll() =>
        (await repository.GetAsync()).MapToResponse();

    public async Task<Response<HighScoreResponse?>> GetById(int id) =>
        (await repository.GetByIdAsync(id)).MapToResponse();

    public async Task<Response<HighScoreResponse>> Create(HighScore highScore) =>
        (await repository.CreateAsync(highScore)).MapToResponse()!;

    public async Task<Response<HighScoreResponse>> Update(HighScore highScore) =>
        (await repository.UpdateAsync(highScore)).MapToResponse()!;

    public async Task<Response<string>> Delete(int id) =>
        (await repository.DeleteAsync(id)).MapToResponse();
}