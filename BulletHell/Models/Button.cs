using System;
using BulletHell.Interfaces;
using BulletHell.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BulletHell.Models;

public class Button : INavigable, IDisposable
{
    private const int BorderThickness = 2;

    private readonly Rectangle _bounds;
    private readonly SpriteFont _font;
    private readonly Texture2D _texture;
    private string _text;

    private bool _isHovered;
    private bool _wasPressed;
    private bool _isSelected;
    private bool _disposed;

    private Vector2 _cachedTextSize;
    private Vector2 _cachedTextPos;
    private bool _textMeasured;

    public event Action? OnClick;
    public Rectangle Bounds => _bounds;

    public Button(SpriteFont font, string text, Rectangle bounds, Texture2D whiteTexture)
    {
        _font = font;
        _text = text;
        _bounds = bounds;
        _texture = whiteTexture;
    }

    public void SetSelected(bool isSelected)
    {
        _isSelected = isSelected;
    }

    public void Activate()
    {
        OnClick?.Invoke();
    }

    public void UpdateText(string newText)
    {
        _text = newText;
        _textMeasured = false; // Force re-measurement of text
    }

    public void Update(MouseState mouseState)
    {
        _isHovered = _bounds.Contains(mouseState.Position);

        bool isPressed = mouseState.LeftButton == ButtonState.Pressed;

        if (_wasPressed && !isPressed && _isHovered)
        {
            OnClick?.Invoke();
        }

        _wasPressed = isPressed && _isHovered;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Color bgColor = _wasPressed ? Color.DimGray : (_isHovered ? Color.Gray : Color.DarkGray);

        spriteBatch.Draw(_texture, _bounds, bgColor);

        Color borderColor = _isSelected ? Color.Lime : Color.White;
        DrawingHelpers.DrawBorder(spriteBatch, _texture, _bounds, borderColor, BorderThickness);

        if (!_textMeasured)
        {
            _cachedTextSize = _font.MeasureString(_text);
            _cachedTextPos = new Vector2(
                _bounds.X + (float)_bounds.Width / 2 - _cachedTextSize.X / 2,
                _bounds.Y + (float)_bounds.Height / 2 - _cachedTextSize.Y / 2
            );
            _textMeasured = true;
        }

        spriteBatch.DrawString(_font, _text, _cachedTextPos, Color.White);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Note: We don't dispose _texture here as it's a shared resource
                // owned by Game1 and will be disposed there
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
