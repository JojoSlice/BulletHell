using System.Collections.Generic;
using BulletHell.Interfaces;
using Microsoft.Xna.Framework.Input;

namespace BulletHell.Managers;

/// <summary>
/// Manages keyboard navigation through menu items
/// </summary>
public class MenuNavigator : IMenuNavigator
{
    private readonly List<INavigable> _items = [];
    private int _selectedIndex;
    private KeyboardState _previousKeyState;

    public int SelectedIndex => _selectedIndex;

    public MenuNavigator()
    {
        _previousKeyState = Keyboard.GetState();
    }

    public void AddItem(INavigable item)
    {
        _items.Add(item);

        // If this is the first item, select it
        if (_items.Count == 1)
        {
            _selectedIndex = 0;
            item.SetSelected(true);
        }
    }

    public void NavigateUp()
    {
        if (_items.Count == 0)
            return;

        // Deselect current item
        _items[_selectedIndex].SetSelected(false);

        // Move up with wrapping
        _selectedIndex--;
        if (_selectedIndex < 0)
            _selectedIndex = _items.Count - 1;

        // Select new item
        _items[_selectedIndex].SetSelected(true);
    }

    public void NavigateDown()
    {
        if (_items.Count == 0)
            return;

        // Deselect current item
        _items[_selectedIndex].SetSelected(false);

        // Move down with wrapping
        _selectedIndex++;
        if (_selectedIndex >= _items.Count)
            _selectedIndex = 0;

        // Select new item
        _items[_selectedIndex].SetSelected(true);
    }

    public void ActivateSelected()
    {
        if (_items.Count == 0)
            return;

        _items[_selectedIndex].Activate();
    }

    public void Update(KeyboardState currentKeyState)
    {
        // Check for navigation up (W or Up arrow)
        if (IsKeyPressed(currentKeyState, Keys.W) || IsKeyPressed(currentKeyState, Keys.Up))
        {
            NavigateUp();
        }

        // Check for navigation down (S or Down arrow)
        if (IsKeyPressed(currentKeyState, Keys.S) || IsKeyPressed(currentKeyState, Keys.Down))
        {
            NavigateDown();
        }

        // Check for activation (Enter or Space)
        if (IsKeyPressed(currentKeyState, Keys.Enter) || IsKeyPressed(currentKeyState, Keys.Space))
        {
            ActivateSelected();
        }

        _previousKeyState = currentKeyState;
    }

    public void Clear()
    {
        _items.Clear();
        _selectedIndex = 0;
    }

    private bool IsKeyPressed(KeyboardState currentKeyState, Keys key)
    {
        return currentKeyState.IsKeyDown(key) && _previousKeyState.IsKeyUp(key);
    }
}
