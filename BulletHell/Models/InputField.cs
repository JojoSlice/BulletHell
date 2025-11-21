using System;
using BulletHell.Interfaces;
using BulletHell.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BulletHell.Models;

public class InputField : INavigable, IDisposable
{
    public const int DefaultMaxLength = 32;
    private const double CursorBlinkInterval = 0.5;
    private const int TextPaddingX = 10;
    private const int CursorWidth = 2;
    private const int LabelOffsetY = 25;
    private const int BorderThickness = 2;

    private readonly Rectangle _bounds;
    private readonly string _label;
    private readonly SpriteFont _font;
    private readonly Texture2D _texture;
    private readonly bool _isPassword;
    private readonly ITextInputHandler _textInputHandler;
    private readonly int _maxLength;

    private string _text = "";
    private bool _isFocused;
    private bool _isSelected;
    private bool _wasPressed;
    private KeyboardState _previousKeyState;
    private double _cursorBlinkTimer;
    private bool _showCursor = true;
    private bool _disposed;

    private string _cachedDisplayText = "";
    private float _cachedTextWidth;
    private bool _textChanged = true;

    public string Text => _text;
    public bool IsFocused => _isFocused;
    public Rectangle Bounds => _bounds;

    public InputField(
        SpriteFont font,
        string label,
        Rectangle bounds,
        ITextInputHandler textInputHandler,
        Texture2D whiteTexture,
        bool isPassword = false,
        int maxLength = DefaultMaxLength
    )
    {
        _font = font;
        _label = label;
        _bounds = bounds;
        _isPassword = isPassword;
        _textInputHandler = textInputHandler;
        _texture = whiteTexture;
        _maxLength = maxLength;
        _previousKeyState = Keyboard.GetState();
    }

    public void SetSelected(bool isSelected)
    {
        _isSelected = isSelected;
    }

    public void Activate()
    {
        _isFocused = true;
    }

    public void Update(GameTime gameTime, MouseState mouseState)
    {
        bool isHovered = _bounds.Contains(mouseState.Position);
        bool isPressed = mouseState.LeftButton == ButtonState.Pressed;

        if (_wasPressed && !isPressed)
        {
            _isFocused = isHovered;
        }

        _wasPressed = isPressed;

        if (_isFocused)
        {
            KeyboardState currentKeyState = Keyboard.GetState();

            var result = _textInputHandler.ProcessInput(currentKeyState, _previousKeyState, _text);

            string newText = result.Text;

            if (newText.Length > _maxLength)
            {
                newText = newText[.._maxLength];
            }

            if (_text != newText)
            {
                _text = newText;
                _textChanged = true;
            }

            if (result.ShouldUnfocus)
            {
                _isFocused = false;
            }

            _previousKeyState = currentKeyState;

            _cursorBlinkTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (_cursorBlinkTimer >= CursorBlinkInterval)
            {
                _showCursor = !_showCursor;
                _cursorBlinkTimer = 0;
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Color bgColor = _isFocused ? Color.LightGray : Color.DarkGray;
        spriteBatch.Draw(_texture, _bounds, bgColor);

        Color borderColor = _isFocused ? Color.Yellow : (_isSelected ? Color.Lime : Color.White);
        DrawingHelpers.DrawBorder(spriteBatch, _texture, _bounds, borderColor, BorderThickness);

        Vector2 labelPos = new(_bounds.X, _bounds.Y - LabelOffsetY);
        spriteBatch.DrawString(_font, _label, labelPos, Color.White);

        if (_textChanged)
        {
            _cachedDisplayText = _isPassword ? new string('*', _text.Length) : _text;
            _cachedTextWidth = _font.MeasureString(_cachedDisplayText).X;
            _textChanged = false;
        }

        float fontHeight = _font.MeasureString("A").Y;
        Vector2 textPos = new(
            _bounds.X + TextPaddingX,
            _bounds.Y + ((float)_bounds.Height / 2) - fontHeight / 2
        );
        spriteBatch.DrawString(_font, _cachedDisplayText, textPos, Color.Black);

        if (_isFocused && _showCursor)
        {
            float cursorX = textPos.X + _cachedTextWidth;
            spriteBatch.Draw(
                _texture,
                new Rectangle((int)cursorX, (int)textPos.Y, CursorWidth, (int)fontHeight),
                Color.Black
            );
        }
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
