using System;
using System.IO;
using System.Text.Json;

namespace BulletHell.Configurations;

/// <summary>
/// Provides game configuration loaded from a JSON file.
/// Falls back to default values if file is missing or malformed.
/// </summary>
public class JsonConfigurationProvider : IConfigurationProvider
{
    private readonly string _configPath;
    private GameConfiguration? _cachedConfig;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Creates a new JSON configuration provider.
    /// </summary>
    /// <param name="configPath">Path to the JSON configuration file. Defaults to "Content/gameconfig.json"</param>
    public JsonConfigurationProvider(string configPath = "Content/gameconfig.json")
    {
        _configPath = configPath ?? throw new ArgumentNullException(nameof(configPath));
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };
    }

    /// <summary>
    /// Gets the game configuration, loading from JSON file on first call.
    /// Returns cached configuration on subsequent calls.
    /// Falls back to default configuration if file is missing or invalid.
    /// </summary>
    public GameConfiguration GetConfiguration()
    {
        if (_cachedConfig != null)
            return _cachedConfig;

        try
        {
            if (File.Exists(_configPath))
            {
                var json = File.ReadAllText(_configPath);
                _cachedConfig = JsonSerializer.Deserialize<GameConfiguration>(json, _jsonOptions);

                if (_cachedConfig != null)
                {
                    Console.WriteLine($"[Config] Loaded configuration from {_configPath}");
                    return _cachedConfig;
                }
            }
            else
            {
                Console.WriteLine($"[Config] Configuration file not found at {_configPath}, using defaults");
            }
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[Config] Failed to parse JSON configuration: {ex.Message}");
            Console.WriteLine($"[Config] Using default configuration");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Config] Error loading configuration: {ex.Message}");
            Console.WriteLine($"[Config] Using default configuration");
        }

        // Fall back to defaults
        _cachedConfig = new GameConfiguration();
        return _cachedConfig;
    }

    /// <summary>
    /// Clears the cached configuration, forcing a reload on next GetConfiguration() call.
    /// </summary>
    public void ClearCache()
    {
        _cachedConfig = null;
    }
}
