using System;
using BulletHell.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BulletHell.Models;

public class InputField : INavigable
{
    private readonly Rectangle _bounds;
    private readonly string _label;
    private readonly SpriteFont _font;
    private readonly Texture2D _texture;
    private readonly bool _isPassword;

    private string _text = "";
    private bool _isFocused;
    private bool _isSelected;
    private bool _wasPressed;
    private KeyboardState _previousKeyState;
    private double _cursorBlinkTimer;
    private bool _showCursor = true;

    public string Text => _text;
    public bool IsFocused => _isFocused;
    public Rectangle Bounds => _bounds;

    public InputField(
        GraphicsDevice graphicsDevice,
        SpriteFont font,
        string label,
        Rectangle bounds,
        bool isPassword = false
    )
    {
        _font = font;
        _label = label;
        _bounds = bounds;
        _isPassword = isPassword;

        _texture = new Texture2D(graphicsDevice, 1, 1);
        _texture.SetData(new[] { Color.White });
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
            Keys[] pressedKeys = currentKeyState.GetPressedKeys();

            foreach (Keys key in pressedKeys)
            {
                if (_previousKeyState.IsKeyUp(key))
                {
                    // ESC or Enter unfocuses the input field
                    if (key == Keys.Escape || key == Keys.Enter)
                    {
                        _isFocused = false;
                        break;
                    }

                    HandleKeyPress(key);
                }
            }

            _previousKeyState = currentKeyState;

            _cursorBlinkTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (_cursorBlinkTimer >= 0.5)
            {
                _showCursor = !_showCursor;
                _cursorBlinkTimer = 0;
            }
        }
    }

    private void HandleKeyPress(Keys key)
    {
        if (key == Keys.Back && _text.Length > 0)
        {
            _text = _text[..^1];
        }
        else if (key == Keys.Space)
        {
            _text += " ";
        }
        else
        {
            string keyString = key.ToString();

            if (keyString.Length == 1)
            {
                char c = keyString[0];
                if (char.IsLetterOrDigit(c))
                {
                    _text += char.ToLower(c);
                }
            }
            else if (keyString.StartsWith("NumPad"))
            {
                _text += keyString[^1];
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // Rita bakgrund
        Color bgColor = _isFocused ? Color.LightGray : Color.DarkGray;
        spriteBatch.Draw(_texture, _bounds, bgColor);

        // Rita ram - gul om focused, grön om selected, annars vit
        Color borderColor = _isFocused ? Color.Yellow : (_isSelected ? Color.Lime : Color.White);
        DrawBorder(spriteBatch, _bounds, borderColor, 2);

        // Rita label
        Vector2 labelPos = new(_bounds.X, _bounds.Y - 25);
        spriteBatch.DrawString(_font, _label, labelPos, Color.White);

        // Rita text (maskerad om password)
        string displayText = _isPassword ? new string('*', _text.Length) : _text;
        Vector2 textPos = new(
            _bounds.X + 10,
            _bounds.Y + _bounds.Height / 2 - _font.MeasureString("A").Y / 2
        );
        spriteBatch.DrawString(_font, displayText, textPos, Color.Black);

        // Rita cursor om fokuserad
        if (_isFocused && _showCursor)
        {
            float cursorX = textPos.X + _font.MeasureString(displayText).X;
            spriteBatch.Draw(
                _texture,
                new Rectangle((int)cursorX, (int)textPos.Y, 2, (int)_font.MeasureString("A").Y),
                Color.Black
            );
        }
    }

    private void DrawBorder(SpriteBatch spriteBatch, Rectangle rect, Color color, int thickness)
    {
        // Topp
        spriteBatch.Draw(_texture, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
        // Botten
        spriteBatch.Draw(
            _texture,
            new Rectangle(rect.X, rect.Bottom - thickness, rect.Width, thickness),
            color
        );
        // Vänster
        spriteBatch.Draw(_texture, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
        // Höger
        spriteBatch.Draw(
            _texture,
            new Rectangle(rect.Right - thickness, rect.Y, thickness, rect.Height),
            color
        );
    }
}
