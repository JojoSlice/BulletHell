using BulletHell.Helpers;
using BulletHell.Interfaces;

namespace BulletHell.Factories;

/// <summary>
/// Default implementation of ISpriteHelperFactory that creates SpriteHelper instances.
/// </summary>
public class SpriteHelperFactory : ISpriteHelperFactory
{
    /// <summary>
    /// Creates a new SpriteHelper instance.
    /// </summary>
    /// <returns>A new SpriteHelper instance</returns>
    public ISpriteHelper Create()
    {
        return new SpriteHelper();
    }
}
