using BulletHell.Configurations;
using BulletHell.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BulletHell.Models;

public class Bullet<T> : IDisposable, IDamageDealer, ICollidable
{
    private readonly ISpriteHelper _sprite;
    private readonly Collider _collider;
    private Vector2 _direction;
    private bool _hitTarget;
    private float _timeAlive;
    private bool _disposed;

    public Vector2 Position { get; private set; }

    public Collider Collider => _collider;

    public bool IsAlive
    {
        get
        {
            if (typeof(T) == typeof(Player))
                return _timeAlive < BulletConfig.Player.Lifetime;

            return true;
        }
    }

    public int Width => _sprite.Width;
    public int Height => _sprite.Height;
    public int Damage { get; private set; } = 1;
    // 1 är endast ett failsafe. Det riktiga Damage-värdet sätts i Reset() baserat på BulletConfig.

    public Bullet(Vector2 startPosition, Vector2 direction, ISpriteHelper sprite)
    {
        ArgumentNullException.ThrowIfNull(sprite);

        Position = startPosition;
        _sprite = sprite;
        _timeAlive = 0f;

        // Determine initial velocity based on bullet type
        if (typeof(T) == typeof(Player))
        {
            var dir = direction;
            if (dir != Vector2.Zero)
                dir.Normalize();
            _direction = dir * BulletConfig.Player.Speed;
        }
        else
        {
            _direction = direction;
        }

        var initialRadius = Math.Max(Width, Height) / 2f;
        _collider = new Collider(Position, typeof(Bullet<T>), initialRadius);
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
        return !IsAlive || IsOutOfBounds(screenWidth, screenHeight) || _hitTarget;
    }

    public void MarkHit() => _hitTarget = true;

    public void LoadContent(Texture2D bulletTexture)
    {
        ArgumentNullException.ThrowIfNull(bulletTexture);

        int frameWidth;
        int frameHeight;
        float animationSpeed;

        if (typeof(T) == typeof(Player))
        {
            frameWidth = BulletConfig.Player.SpriteWidth;
            frameHeight = BulletConfig.Player.SpriteHeight;
            animationSpeed = BulletConfig.Player.AnimationSpeed;

            if (frameWidth <= 0)
                throw new ArgumentOutOfRangeException(nameof(frameWidth));
            if (frameHeight <= 0)
                throw new ArgumentOutOfRangeException(nameof(frameHeight));
            if (animationSpeed <= 0)
                throw new ArgumentOutOfRangeException(nameof(animationSpeed));
        }
        else
        {
            frameWidth = BulletConfig.Enemy.SpriteWidth;
            frameHeight = BulletConfig.Enemy.SpriteHeight;
            animationSpeed = BulletConfig.Enemy.AnimationSpeed;

            if (frameWidth <= 0)
                throw new ArgumentOutOfRangeException(nameof(frameWidth));
            if (frameHeight <= 0)
                throw new ArgumentOutOfRangeException(nameof(frameHeight));
            if (animationSpeed <= 0)
                throw new ArgumentOutOfRangeException(nameof(animationSpeed));
        }

        _sprite.LoadSpriteSheet(bulletTexture, frameWidth, frameHeight, animationSpeed);

        _collider.Radius = Math.Max(Width, Height) / 2f;
    }

    public void Update(GameTime gameTime)
    {
        ArgumentNullException.ThrowIfNull(gameTime);

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        Position += _direction * deltaTime;
        _collider.Position = Position;
        _timeAlive += deltaTime;

        _sprite.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        ArgumentNullException.ThrowIfNull(spriteBatch);

        float rotation = MathF.Atan2(_direction.Y, _direction.X) + MathF.PI / 2f;

        _sprite.Draw(spriteBatch, Position, rotation, 1f);
    }

    public void Reset(Vector2 position, Vector2 directionOrVelocity, int damage = 1)
    {
        Position = position;

        if (typeof(T) == typeof(Player))
        {
            var dir = directionOrVelocity;
            if (dir != Vector2.Zero)
                dir.Normalize();
            _direction = dir * BulletConfig.Player.Speed;
            Damage = BulletConfig.Player.Damage;
        }
        else
        {
            _direction = directionOrVelocity;
            Damage = BulletConfig.Enemy.Damage;
        }

        _timeAlive = 0f;
        _collider.Position = Position;
        _hitTarget = false;
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