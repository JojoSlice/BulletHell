using System;
using BulletHell.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BulletHell.Models;

public class Button : INavigable
{
    private readonly Rectangle _bounds;
    private readonly string _text;
    private readonly SpriteFont _font;
    private readonly Texture2D _texture;

    private bool _isHovered;
    private bool _wasPressed;
    private bool _isSelected;

    public event Action? OnClick;
    public Rectangle Bounds => _bounds;

    public Button(GraphicsDevice graphicsDevice, SpriteFont font, string text, Rectangle bounds)
    {
        _font = font;
        _text = text;
        _bounds = bounds;

        _texture = new Texture2D(graphicsDevice, 1, 1);
        _texture.SetData(new[] { Color.White });
    }

    public void SetSelected(bool isSelected)
    {
        _isSelected = isSelected;
    }

    public void Activate()
    {
        OnClick?.Invoke();
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
        // Bakgrundsfärg baserat på state
        Color bgColor = _isHovered ? Color.Gray : Color.DarkGray;
        if (_wasPressed)
            bgColor = Color.DimGray;

        // Rita bakgrund
        spriteBatch.Draw(_texture, _bounds, bgColor);

        // Rita ram - grön om selected, annars vit
        Color borderColor = _isSelected ? Color.Lime : Color.White;
        DrawBorder(spriteBatch, _bounds, borderColor, 2);

        // Centrera text
        Vector2 textSize = _font.MeasureString(_text);
        Vector2 textPos = new(
            _bounds.X + _bounds.Width / 2 - textSize.X / 2,
            _bounds.Y + _bounds.Height / 2 - textSize.Y / 2
        );
        spriteBatch.DrawString(_font, _text, textPos, Color.White);
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
