using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BulletHell.Models;

public class RymdDash : IDisposable
{
    private Vector2 _velocity;
    private Texture2D? _texture;
    private bool _disposed;

    public Vector2 Position { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public float Speed { get; private set; }

    public RymdDash(Vector2 startPosition, float speed)
    {
        Position = startPosition;
        Speed = speed;
        _velocity = Vector2.UnitY * speed;
    }

    public void LoadContent(Texture2D texture)
    {
        ArgumentNullException.ThrowIfNull(texture);
        _texture = texture;
        Width = texture.Width;
        Height = texture.Height;
    }

    public void Update(GameTime gameTime)
    {
        ArgumentNullException.ThrowIfNull(gameTime);
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Position += _velocity * deltaTime;
    }

    public bool ShouldBeRemoved(int screenWidth, int screenHeight)
    {
        return Position.Y > screenHeight + Height ||
               Position.X < -Width ||
               Position.X > screenWidth + Width;
    }

    public void Draw(SpriteBatch spriteBatch, int screenWidth, int screenHeight)
    {
        ArgumentNullException.ThrowIfNull(spriteBatch);
        if (_texture == null)
        {
            return;
        }

        // Draw at background size (1.5x screen)
        Rectangle destRect = new Rectangle(
            (int)Position.X,
            (int)Position.Y,
            (int)(screenWidth * 1.5f),
            (int)(screenHeight * 1.5f)
        );
        spriteBatch.Draw(_texture, destRect, Color.White);
    }

    public void Reset(Vector2 position, float speed)
    {
        Position = position;
        Speed = speed;
        _velocity = Vector2.UnitY * speed;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Texture managed by ContentManager
                _texture = null;
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
