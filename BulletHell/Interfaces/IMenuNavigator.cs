using Microsoft.Xna.Framework.Input;

namespace BulletHell.Interfaces;

/// <summary>
/// Interface for managing keyboard navigation through menu items
/// </summary>
public interface IMenuNavigator
{
    /// <summary>
    /// Gets the currently selected index
    /// </summary>
    int SelectedIndex { get; }

    /// <summary>
    /// Adds a navigable item to the navigation system
    /// </summary>
    void AddItem(INavigable item);

    /// <summary>
    /// Moves the selection up (to previous item)
    /// </summary>
    void NavigateUp();

    /// <summary>
    /// Moves the selection down (to next item)
    /// </summary>
    void NavigateDown();

    /// <summary>
    /// Activates the currently selected item
    /// </summary>
    void ActivateSelected();

    /// <summary>
    /// Updates the navigator with current keyboard state
    /// </summary>
    void Update(KeyboardState currentKeyState);

    /// <summary>
    /// Clears all items from the navigator
    /// </summary>
    void Clear();
}
