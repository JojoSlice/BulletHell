namespace BulletHell.Configurations;

/// <summary>
/// Interface for providing game configuration.
/// Allows different configuration sources (JSON, XML, database, etc.)
/// </summary>
public interface IConfigurationProvider
{
    /// <summary>
    /// Gets the game configuration.
    /// </summary>
    /// <returns>Complete game configuration with all settings</returns>
    GameConfiguration GetConfiguration();
}
