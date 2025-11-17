namespace Application.Mapping;

using Contracts.Responses.HighScore;
using Contracts.Responses.Shared;
using Domain.Entities;

public static class MappingExtensions
{
    
    private static HighScoreResponse MapHighScoreToResponse(this HighScore hs) => new()
    {
        Id = hs.Id,
        Score = hs.Score,
        UserId = hs.UserId
    };
    
    public static Response<List<HighScoreResponse>> MapToResponse(this List<HighScore> highScores)
    {
        return highScores.Count != 0 ? new Response<List<HighScoreResponse>>
        {
            IsSuccess = highScores.Count != 0,
            Data = highScores.Select(hs => hs.MapHighScoreToResponse()).ToList()
        }
        : new Response<List<HighScoreResponse>>
        {
            IsSuccess = false,
            Data = null
        };
    }

    public static Response<HighScoreResponse?> MapToResponse(this HighScore? highScore) => new()
    {
        IsSuccess = true,
        Data = highScore?.MapHighScoreToResponse()
    };
    
    public static Response<string> MapToResponse(this string message) => new()
    {
        IsSuccess = true,
        Data = message
    };
}