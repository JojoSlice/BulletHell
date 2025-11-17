namespace Application.Services;

using Contracts.Responses.HighScore;
using Contracts.Responses.Shared;
using Domain.Entities;
using Interfaces;
using Mapping;
using Repository.Repositories;

public class HighScoreService(HighScoreRepository repository) : IHighScoreService
{
    public async Task<Response<GetAllResponse<GetHighScoreResponse>>> GetAll() =>
        (await repository.GetAsync()).MapToResponse();

    public async Task<Response<GetHighScoreResponse?>> GetById(int id) =>
        (await repository.GetByIdAsync(id)).MapToResponse();

    public Response<CreateHighScoreResponse> Create(HighScore highScore)
    {
        //TODO: Implement Create
        throw new NotImplementedException();
    }

    public Response<UpdateHighScoreResponse> Update(int id, HighScore highScore)
    {
        //TODO: Implement Update
        throw new NotImplementedException();
    }

    public Response<string> Delete(int id)
    {
        //TODO: Implement Delete
        throw new NotImplementedException();
    }
}