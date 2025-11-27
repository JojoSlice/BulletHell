using System;
using BulletHell.Configurations;
using BulletHell.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Models;

/// <summary>
/// Represents a bullet in the game
/// </summary>
public class Bullet : IDisposable
{
    private readonly float _speed = BulletConfig.Speed;
    private readonly ISpriteHelper _sprite;
    private readonly Collider _collider;
    private Vector2 _direction;
    private float _timeAlive;
    private bool _disposed;

    public Vector2 Position { get; private set; }

    public Collider Collider => _collider;
    public bool IsAlive => _timeAlive < BulletConfig.Lifetime;
    public int Width => _sprite.Width;
    public int Height => _sprite.Height;

    public Bullet(Vector2 startPosition, Vector2 direction, ISpriteHelper sprite)
    {
        ArgumentNullException.ThrowIfNull(sprite, nameof(sprite));

        Position = startPosition;
        _sprite = sprite;

        var initialRadius = Math.Max(_sprite.Width, _sprite.Height) / 2f;
        _collider = new Collider(Position, typeof(Bullet), initialRadius);

        if (direction != Vector2.Zero)
        {
            direction.Normalize();
        }
        _direction = direction;
        _timeAlive = 0f;
    }

    public bool IsOutOfBounds(int screenWidth, int screenHeight)
    {
        return Position.X < 0
            || Position.X > screenWidth
            || Position.Y < 0
            || Position.Y > screenHeight;
    }

    public bool ShouldBeRemoved(int screenWidth, int screenHeight)
    {
        return !IsAlive || IsOutOfBounds(screenWidth, screenHeight);
    }

    /// <summary>
    /// Loads the bullet texture and initializes the sprite
    /// </summary>
    public void LoadContent(Texture2D bulletTexture)
    {
        ArgumentNullException.ThrowIfNull(bulletTexture);

        if (BulletConfig.SpriteWidth <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(BulletConfig.SpriteWidth),
                BulletConfig.SpriteWidth,
                "SpriteWidth must be positive"
            );
        if (BulletConfig.SpriteHeight <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(BulletConfig.SpriteHeight),
                BulletConfig.SpriteHeight,
                "SpriteHeight must be positive"
            );
        if (BulletConfig.AnimationSpeed <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(BulletConfig.AnimationSpeed),
                BulletConfig.AnimationSpeed,
                "AnimationSpeed must be positive"
            );

        _sprite.LoadSpriteSheet(
            bulletTexture,
            BulletConfig.SpriteWidth,
            BulletConfig.SpriteHeight,
            BulletConfig.AnimationSpeed
        );
    }

    /// <summary>
    /// Updates bullet position and animation
    /// </summary>
    public void Update(GameTime gameTime)
    {
        ArgumentNullException.ThrowIfNull(gameTime);

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        Position += _direction * _speed * deltaTime;
        _collider.Position = Position;
        _timeAlive += deltaTime;

        _sprite.Update(gameTime);
    }

    /// <summary>
    /// Draws the bullet to the screen
    /// </summary>
    public void Draw(SpriteBatch spriteBatch)
    {
        ArgumentNullException.ThrowIfNull(spriteBatch);

        _sprite.Draw(spriteBatch, Position, 0f, 1f);
    }

    /// <summary>
    /// Resets the bullet state for reuse in object pooling
    /// </summary>
    public void Reset(Vector2 position, Vector2 direction)
    {
        Position = position;
        if (direction != Vector2.Zero)
        {
            direction.Normalize();
        }
        _collider.Position = Position;
        _direction = direction;
        _timeAlive = 0f;
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
