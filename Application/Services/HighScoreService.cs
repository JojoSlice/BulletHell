using Application.Interfaces;
using Application.Mapping;
using Contracts.Responses.Common;
using Contracts.Responses.HighScore;
using Domain.Entities;
using Repository.Repositories;

namespace Application.Services;

public class HighScoreService(HighScoreRepository repository) : IService<HighScoreResponse, HighScore>
{
    public async Task<Response<List<HighScoreResponse>>> GetAll() =>
        (await repository.GetAllAsync()).MapToResponse();

    public async Task<Response<HighScoreResponse?>> GetById(int id) =>
        (await repository.GetByIdAsync(id)).MapToResponse();

    public async Task<Response<HighScoreResponse>> Create(HighScore highScore) =>
        (await repository.CreateAsync(highScore)).MapToResponse()!;

    public async Task<Response<HighScoreResponse>> Update(HighScore highScore) =>
        (await repository.UpdateAsync(highScore)).MapToResponse()!;

    public async Task<Response<string>> Delete(int id) =>
        (await repository.DeleteAsync(id)).MapToResponse();
}
