using System;
using System.Collections.Generic;
using BulletHell.Interfaces;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace BulletHell.Managers;

/// <summary>
/// Manages keyboard navigation through menu items
/// </summary>
public class MenuNavigator : IMenuNavigator, IDisposable
{
    private readonly List<INavigable> _items = [];
    private int _selectedIndex;
    private KeyboardState _previousKeyState;
    private bool _disposed;

    public int SelectedIndex => _selectedIndex;

    public MenuNavigator()
    {
        _previousKeyState = Keyboard.GetState();
    }

    public void AddItem(INavigable item)
    {
        _items.Add(item);

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

        _items[_selectedIndex].SetSelected(false);

        _selectedIndex--;
        if (_selectedIndex < 0)
            _selectedIndex = _items.Count - 1;

        _items[_selectedIndex].SetSelected(true);
    }

    public void NavigateDown()
    {
        if (_items.Count == 0)
            return;

        _items[_selectedIndex].SetSelected(false);

        _selectedIndex++;
        if (_selectedIndex >= _items.Count)
            _selectedIndex = 0;

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
        if (IsKeyPressed(currentKeyState, Keys.W) || IsKeyPressed(currentKeyState, Keys.Up))
        {
            NavigateUp();
        }

        if (IsKeyPressed(currentKeyState, Keys.S) || IsKeyPressed(currentKeyState, Keys.Down))
        {
            NavigateDown();
        }

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

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                foreach (var disposable in _items.OfType<IDisposable>())
                {
                    disposable.Dispose();
                }

                _items.Clear();
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
