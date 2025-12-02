using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using BulletHell.Interfaces;
using BulletHell.Models;

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
            var request = new { userName = username, passwordHash = passwordHash };

            var response = await _httpClient.PostAsJsonAsync("/api/users", request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var userResponse = JsonSerializer.Deserialize<UserResponse>(content, _jsonOptions);

                return new RegistrationResult
                {
                    Success = true,
                    Message = "User created successfully!",
                    UserId = userResponse?.Id,
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
            var request = new { userName = username, passwordHash = passwordHash };

            var response = await _httpClient.PostAsJsonAsync("/api/users/login", request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<UserResponse>>(
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

    private record UserResponse
    {
        public int Id { get; init; }
        public string UserName { get; init; } = string.Empty;
    }

    private record ApiResponse<T>
    {
        public bool IsSuccess { get; init; }
        public T? Data { get; init; }
    }

    private record HighScoreResponse
    {
        public int Id { get; init; }
        public int Score { get; init; }
        public int UserId { get; init; }
    }

    public async Task<HighScoreListResult> GetAllHighScoresAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/highscores");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<HighScoreResponse>>>(
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
            var response = await _httpClient.GetAsync($"/api/highscores/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<HighScoreResponse>>(
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
            var request = new { score, userId };

            var response = await _httpClient.PostAsJsonAsync("/api/highscores", request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<HighScoreResponse>>(
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
            var request = new { id, score, userId };

            var response = await _httpClient.PutAsJsonAsync("/api/highscores", request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<HighScoreResponse>>(
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
            var response = await _httpClient.DeleteAsync($"/api/highscores/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<string>>(
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
