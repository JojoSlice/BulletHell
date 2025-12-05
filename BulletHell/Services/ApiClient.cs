using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using BulletHell.Interfaces;
using BulletHell.Models;
using Contracts;
using Contracts.Requests.HighScore;
using Contracts.Requests.User;
using Contracts.Responses.Common;
using Contracts.Responses.HighScore;
using Contracts.Responses.User;
using UserResponse = Contracts.Responses.User.UserResponse;

namespace BulletHell.Services;

public class ApiClient : IApiClient
{
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly HttpClient _httpClient;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<RegistrationResult> RegisterUserAsync(string username, string passwordHash)
    {
        ArgumentNullException.ThrowIfNull(username);
        ArgumentNullException.ThrowIfNull(passwordHash);

        try
        {
            var request = new CreateUserRequest { UserName = username, PasswordHash = passwordHash };

            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.User.Create, request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<Response<UserResponse>>(
                    content,
                    _jsonOptions
                );

                if (apiResponse?.IsSuccess == true && apiResponse.Data != null)
                {
                    return new RegistrationResult
                    {
                        Success = true,
                        Message = "User created successfully!",
                        UserId = apiResponse.Data.Id,
                    };
                }

                return new RegistrationResult
                {
                    Success = false,
                    Message = "Could not create user",
                };
            }

            var errorContent = await response.Content.ReadAsStringAsync();

            return new RegistrationResult
            {
                Success = false,
                Message = response.StatusCode switch
                {
                    System.Net.HttpStatusCode.BadRequest => $"Validation error: {errorContent}",
                    System.Net.HttpStatusCode.Conflict => "Username already exist",
                    _ => $"Error: {response.StatusCode}",
                },
            };
        }
        catch (HttpRequestException ex)
        {
            return new RegistrationResult
            {
                Success = false,
                Message = $"Could not connect to server: {ex.Message}",
            };
        }
        catch (TaskCanceledException ex)
        {
            return new RegistrationResult
            {
                Success = false,
                Message = $"Operation timed out: {ex.Message}",
            };
        }
        catch (JsonException ex)
        {
            return new RegistrationResult
            {
                Success = false,
                Message = $"Response parsing error: {ex.Message}",
            };
        }
    }

    public async Task<RegistrationResult> LoginAsync(string username, string passwordHash)
    {
        ArgumentNullException.ThrowIfNull(username);
        ArgumentNullException.ThrowIfNull(passwordHash);

        try
        {
            var request = new LoginRequest { UserName = username, PasswordHash = passwordHash };

            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.User.Login, request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<Response<UserResponse>>(
                    content,
                    _jsonOptions
                );

                if (apiResponse?.IsSuccess == true && apiResponse.Data != null)
                {
                    return new RegistrationResult
                    {
                        Success = true,
                        Message = "Login successful!",
                        UserId = apiResponse.Data.Id,
                    };
                }

                return new RegistrationResult
                {
                    Success = false,
                    Message = "Invalid username or password",
                };
            }

            return new RegistrationResult
            {
                Success = false,
                Message = response.StatusCode switch
                {
                    System.Net.HttpStatusCode.BadRequest => "Invalid credentials",
                    System.Net.HttpStatusCode.Unauthorized => "Invalid username or password",
                    _ => $"Error: {response.StatusCode}",
                },
            };
        }
        catch (HttpRequestException ex)
        {
            return new RegistrationResult
            {
                Success = false,
                Message = $"Could not connect to server: {ex.Message}",
            };
        }
        catch (TaskCanceledException ex)
        {
            return new RegistrationResult
            {
                Success = false,
                Message = $"Operation timed out: {ex.Message}",
            };
        }
        catch (JsonException ex)
        {
            return new RegistrationResult
            {
                Success = false,
                Message = $"Response parsing error: {ex.Message}",
            };
        }
    }

    public async Task<HighScoreListResult> GetAllHighScoresAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync(ApiEndpoints.HighScore.GetAll);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<Response<List<HighScoreResponse>>>(
                    content,
                    _jsonOptions
                );

                if (apiResponse?.IsSuccess == true && apiResponse.Data != null)
                {
                    var highScores = apiResponse.Data.Select(hs => new HighScoreResult
                    {
                        Id = hs.Id,
                        Score = hs.Score,
                        UserId = hs.UserId,
                    }).ToList();

                    return new HighScoreListResult
                    {
                        Success = true,
                        Message = "High scores retrieved",
                        Data = highScores,
                    };
                }

                return new HighScoreListResult
                {
                    Success = false,
                    Message = "Could not retrieve high scores",
                };
            }

            return new HighScoreListResult
            {
                Success = false,
                Message = $"Error: {response.StatusCode}",
            };
        }
        catch (HttpRequestException ex)
        {
            return new HighScoreListResult
            {
                Success = false,
                Message = $"Could not connect to server: {ex.Message}",
            };
        }
        catch (TaskCanceledException ex)
        {
            return new HighScoreListResult
            {
                Success = false,
                Message = $"Operation timed out: {ex.Message}",
            };
        }
        catch (JsonException ex)
        {
            return new HighScoreListResult
            {
                Success = false,
                Message = $"Response parsing error: {ex.Message}",
            };
        }
    }

    public async Task<HighScoreOperationResult> GetHighScoreByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync(ApiEndpoints.HighScore.GetById.Replace("{id}", id.ToString()));

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<Response<HighScoreResponse>>(
                    content,
                    _jsonOptions
                );

                if (apiResponse?.IsSuccess == true && apiResponse.Data != null)
                {
                    var highScore = new HighScoreResult
                    {
                        Id = apiResponse.Data.Id,
                        Score = apiResponse.Data.Score,
                        UserId = apiResponse.Data.UserId,
                    };

                    return new HighScoreOperationResult
                    {
                        Success = true,
                        Message = "High score retrieved",
                        Data = highScore,
                    };
                }

                return new HighScoreOperationResult
                {
                    Success = false,
                    Message = "High score not found",
                };
            }

            return new HighScoreOperationResult
            {
                Success = false,
                Message = response.StatusCode switch
                {
                    System.Net.HttpStatusCode.NotFound => "High score not found",
                    _ => $"Error: {response.StatusCode}",
                },
            };
        }
        catch (HttpRequestException ex)
        {
            return new HighScoreOperationResult
            {
                Success = false,
                Message = $"Could not connect to server: {ex.Message}",
            };
        }
        catch (TaskCanceledException ex)
        {
            return new HighScoreOperationResult
            {
                Success = false,
                Message = $"Operation timed out: {ex.Message}",
            };
        }
        catch (JsonException ex)
        {
            return new HighScoreOperationResult
            {
                Success = false,
                Message = $"Response parsing error: {ex.Message}",
            };
        }
    }

    public async Task<HighScoreOperationResult> CreateHighScoreAsync(int score, int userId)
    {
        try
        {
            var request = new CreateHighScoreRequest { Score = score, UserId = userId };

            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.HighScore.Create, request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<Response<HighScoreResponse>>(
                    content,
                    _jsonOptions
                );

                if (apiResponse?.IsSuccess == true && apiResponse.Data != null)
                {
                    var highScore = new HighScoreResult
                    {
                        Id = apiResponse.Data.Id,
                        Score = apiResponse.Data.Score,
                        UserId = apiResponse.Data.UserId,
                    };

                    return new HighScoreOperationResult
                    {
                        Success = true,
                        Message = "High score created",
                        Data = highScore,
                    };
                }

                return new HighScoreOperationResult
                {
                    Success = false,
                    Message = "Could not create high score",
                };
            }

            var errorContent = await response.Content.ReadAsStringAsync();

            return new HighScoreOperationResult
            {
                Success = false,
                Message = response.StatusCode switch
                {
                    System.Net.HttpStatusCode.BadRequest => $"Validation error: {errorContent}",
                    _ => $"Error: {response.StatusCode}",
                },
            };
        }
        catch (HttpRequestException ex)
        {
            return new HighScoreOperationResult
            {
                Success = false,
                Message = $"Could not connect to server: {ex.Message}",
            };
        }
        catch (TaskCanceledException ex)
        {
            return new HighScoreOperationResult
            {
                Success = false,
                Message = $"Operation timed out: {ex.Message}",
            };
        }
        catch (JsonException ex)
        {
            return new HighScoreOperationResult
            {
                Success = false,
                Message = $"Response parsing error: {ex.Message}",
            };
        }
    }

    public async Task<HighScoreOperationResult> UpdateHighScoreAsync(int id, int score, int userId)
    {
        try
        {
            var request = new UpdateHighScoreRequest { Id = id, Score = score, UserId = userId };

            var response = await _httpClient.PutAsJsonAsync(ApiEndpoints.HighScore.Update, request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<Response<HighScoreResponse>>(
                    content,
                    _jsonOptions
                );

                if (apiResponse?.IsSuccess == true && apiResponse.Data != null)
                {
                    var highScore = new HighScoreResult
                    {
                        Id = apiResponse.Data.Id,
                        Score = apiResponse.Data.Score,
                        UserId = apiResponse.Data.UserId,
                    };

                    return new HighScoreOperationResult
                    {
                        Success = true,
                        Message = "High score updated",
                        Data = highScore,
                    };
                }

                return new HighScoreOperationResult
                {
                    Success = false,
                    Message = "Could not update high score",
                };
            }

            var errorContent = await response.Content.ReadAsStringAsync();

            return new HighScoreOperationResult
            {
                Success = false,
                Message = response.StatusCode switch
                {
                    System.Net.HttpStatusCode.NotFound => "High score not found",
                    System.Net.HttpStatusCode.BadRequest => $"Validation error: {errorContent}",
                    _ => $"Error: {response.StatusCode}",
                },
            };
        }
        catch (HttpRequestException ex)
        {
            return new HighScoreOperationResult
            {
                Success = false,
                Message = $"Could not connect to server: {ex.Message}",
            };
        }
        catch (TaskCanceledException ex)
        {
            return new HighScoreOperationResult
            {
                Success = false,
                Message = $"Operation timed out: {ex.Message}",
            };
        }
        catch (JsonException ex)
        {
            return new HighScoreOperationResult
            {
                Success = false,
                Message = $"Response parsing error: {ex.Message}",
            };
        }
    }

    public async Task<HighScoreOperationResult> DeleteHighScoreAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(ApiEndpoints.HighScore.Delete.Replace("{id}", id.ToString()));

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<Response<string>>(
                    content,
                    _jsonOptions
                );

                if (apiResponse?.IsSuccess == true)
                {
                    return new HighScoreOperationResult
                    {
                        Success = true,
                        Message = "High score deleted",
                    };
                }

                return new HighScoreOperationResult
                {
                    Success = false,
                    Message = "Could not delete high score",
                };
            }

            return new HighScoreOperationResult
            {
                Success = false,
                Message = response.StatusCode switch
                {
                    System.Net.HttpStatusCode.NotFound => "High score not found",
                    _ => $"Error: {response.StatusCode}",
                },
            };
        }
        catch (HttpRequestException ex)
        {
            return new HighScoreOperationResult
            {
                Success = false,
                Message = $"Could not connect to server: {ex.Message}",
            };
        }
        catch (TaskCanceledException ex)
        {
            return new HighScoreOperationResult
            {
                Success = false,
                Message = $"Operation timed out: {ex.Message}",
            };
        }
        catch (JsonException ex)
        {
            return new HighScoreOperationResult
            {
                Success = false,
                Message = $"Response parsing error: {ex.Message}",
            };
        }
    }
}
