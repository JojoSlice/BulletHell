using Contracts.Responses.Common;
using Contracts.Responses.HighScore;
using Contracts.Responses.User;
using Domain.Entities;

namespace Application.Mapping;

public static class MappingExtensions
{
    #region HighScore extension methods

    private static HighScoreResponse MapHighScoreToResponse(this HighScore hs) => new()
    {
        Id = hs.Id,
        Score = hs.Score,
        UserId = hs.UserId
    };

    public static Response<List<HighScoreResponse>> MapToResponse(this List<HighScore> highScores)
    {
        return highScores.Count != 0
            ? new Response<List<HighScoreResponse>>
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

    #endregion

    #region User extension methods

    private static UserResponse MapUserToResponse(this User user) => new()
    {
        Id = user.Id,
        UserName = user.UserName
    };

    public static Response<List<UserResponse>> MapToResponse(this List<User> users) => users.Count == 0
            ? new Response<List<UserResponse>> { IsSuccess = false, Data = null }
            : new Response<List<UserResponse>>
            {
                IsSuccess = true,
                Data = users.Select(u => u.MapUserToResponse()).ToList()
            };

    public static Response<UserResponse?> MapToResponse(this User? user) => new()
    {
        IsSuccess = true,
        Data = user?.MapUserToResponse()
    };

    #endregion
    
    public static Response<string> MapToResponse(this string message) => new()
    {
        IsSuccess = true,
        Data = message,
    };
}
