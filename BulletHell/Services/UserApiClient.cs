using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using BulletHell.Interfaces;
using BulletHell.Models;

namespace BulletHell.Services;

public class UserApiClient : IUserApiClient
{
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly HttpClient _httpClient;

    public UserApiClient(HttpClient httpClient)
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
                Message = $"Could not connetct to server: {ex.Message}",
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
                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(content, _jsonOptions);

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

    private record ApiResponse
    {
        public bool IsSuccess { get; init; }
        public UserResponse? Data { get; init; }
    }
}
