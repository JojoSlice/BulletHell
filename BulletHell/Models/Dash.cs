using System;
using BulletHell.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Models;

public class Dash : IDisposable
{
    private readonly ISpriteHelper _sprite;
    private Vector2 _velocity;
    private bool _disposed;

    public Vector2 Position { get; private set; }
    public int Width => _sprite.Width;
    public int Height => _sprite.Height;
    public float Opacity { get; set; } = 0.5f;

    public Dash(Vector2 startPosition, ISpriteHelper sprite)
    {
        ArgumentNullException.ThrowIfNull(sprite);
        Position = startPosition;
        _sprite = sprite;
        _velocity = Vector2.UnitY * 1200f;
    }

    public void LoadContent(Texture2D dashTexture)
    {
        ArgumentNullException.ThrowIfNull(dashTexture);
        _sprite.LoadSpriteSheet(dashTexture, 5, 300, 0.1f);
    }

    public void Update(GameTime gameTime)
    {
        ArgumentNullException.ThrowIfNull(gameTime);
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Position += _velocity * deltaTime;
        _sprite.Update(gameTime);
    }

    public bool ShouldBeRemoved(int screenWidth, int screenHeight)
    {
        return Position.Y > screenHeight + 50 || Position.X < -50 || Position.X > screenWidth + 50;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        ArgumentNullException.ThrowIfNull(spriteBatch);
        _sprite.Draw(spriteBatch, Position, Color.White * Opacity, 0f, 0.5f);
    }

    public void Reset(Vector2 position)
    {
        Position = position;
        _velocity = Vector2.UnitY * 1200f;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _sprite?.Dispose();
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