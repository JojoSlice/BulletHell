namespace Application.Interfaces;

using Contracts.Responses.HighScore;
using Contracts.Responses.Shared;
using Domain.Entities;

public interface IHighScoreService
{
    public Task<Response<GetAllResponse<GetHighScoreResponse>>> GetAll();
    public Task<Response<GetHighScoreResponse?>> GetById(int id);
    public Response<CreateHighScoreResponse> Create(HighScore highScore);
    public Response<UpdateHighScoreResponse> Update(int id, HighScore highScore);
    public Response<string> Delete(int id);
}