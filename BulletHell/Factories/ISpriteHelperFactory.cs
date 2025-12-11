using BulletHell.Interfaces;

namespace BulletHell.Factories;

/// <summary>
/// Factory interface for creating ISpriteHelper instances.
/// Enables dependency injection and testability by abstracting SpriteHelper creation.
/// </summary>
public interface ISpriteHelperFactory
{
    /// <summary>
    /// Creates a new ISpriteHelper instance.
    /// </summary>
    /// <returns>A new sprite helper instance</returns>
    ISpriteHelper Create();
}
