using Microsoft.Xna.Framework.Input;

namespace BulletHell.Interfaces;

/// <summary>
/// Interface for providing input state for menu scenes
/// </summary>
public interface IMenuInputProvider
{
    /// <summary>
    /// Gets the current mouse state
    /// </summary>
    MouseState GetMouseState();

    /// <summary>
    /// Gets the current keyboard state
    /// </summary>
    KeyboardState GetKeyboardState();
}
