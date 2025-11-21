using Microsoft.Xna.Framework;

namespace BulletHell.Interfaces;

/// <summary>
/// Interface for UI elements that can be navigated with keyboard
/// </summary>
public interface INavigable
{
    /// <summary>
    /// Sets whether this item is currently selected in the navigation
    /// </summary>
    void SetSelected(bool isSelected);

    /// <summary>
    /// Activates the item (e.g., clicks a button, focuses an input field)
    /// </summary>
    void Activate();

    /// <summary>
    /// Gets the bounds of the navigable element for potential future use
    /// </summary>
    Rectangle Bounds { get; }
}
