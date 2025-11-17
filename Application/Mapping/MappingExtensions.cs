namespace Application.Mapping;

using Contracts.Responses.HighScore;
using Contracts.Responses.Shared;
using Domain.Entities;

public static class MappingExtensions
{
    private static GetHighScoreResponse MapHighScoreToResponse(this HighScore hs) => new()
    {
        Id = hs.Id,
        Score = hs.Score,
        UserId = hs.UserId
    };
    
    public static Response<GetAllResponse<GetHighScoreResponse>> MapToResponse(this List<HighScore> highScores)
    {
        return highScores.Count != 0 ? new Response<GetAllResponse<GetHighScoreResponse>>
        {
            IsSuccess = highScores.Count != 0,
            Data = new GetAllResponse<GetHighScoreResponse>
            {
                Items = highScores.Select(hs => hs.MapHighScoreToResponse())
            }
        }
        : new Response<GetAllResponse<GetHighScoreResponse>>
        {
            IsSuccess = false,
            Data = null
        };
    }

    public static Response<GetHighScoreResponse?> MapToResponse(this HighScore? highScore) => new()
    {
        IsSuccess = true,
        Data = highScore?.MapHighScoreToResponse()
    };
}