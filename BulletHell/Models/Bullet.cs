using System;
using BulletHell.Configurations;
using BulletHell.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Models;

public class Bullet(Vector2 startPosition, Vector2 direction, ISpriteHelper sprite)
{
    public Vector2 Position { get; private set; } = startPosition;
    private readonly Vector2 _direction = direction;
    private readonly float _speed = BulletConfig.Speed;
    private readonly ISpriteHelper _sprite =
        sprite ?? throw new ArgumentNullException(nameof(sprite));
    private float _timeAlive = 0f;

    public bool IsAlive => _timeAlive < BulletConfig.Lifetime;
    public int Width => _sprite?.Width ?? 0;
    public int Height => _sprite?.Height ?? 0;

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

    public void LoadContent(Texture2D bulletTexture)
    {
        ArgumentNullException.ThrowIfNull(bulletTexture);

        if (BulletConfig.SpriteWidth <= 0)
            throw new InvalidOperationException(
                $"SpriteWidth must be positive, got {BulletConfig.SpriteWidth}"
            );
        if (BulletConfig.SpriteHeight <= 0)
            throw new InvalidOperationException(
                $"SpriteHeight must be positive, got {BulletConfig.SpriteHeight}"
            );
        if (BulletConfig.AnimationSpeed <= 0)
            throw new InvalidOperationException(
                $"AnimationSpeed must be positive, got {BulletConfig.AnimationSpeed}"
            );

        _sprite.LoadSpriteSheet(
            bulletTexture,
            BulletConfig.SpriteWidth,
            BulletConfig.SpriteHeight,
            BulletConfig.AnimationSpeed
        );
    }

    public void Update(GameTime gameTime)
    {
        ArgumentNullException.ThrowIfNull(gameTime);

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (deltaTime < 0)
            throw new InvalidOperationException($"Delta time cannot be negative, got {deltaTime}");

        Position += _direction * _speed * deltaTime;

        _timeAlive += deltaTime;

        _sprite.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        ArgumentNullException.ThrowIfNull(spriteBatch);

        _sprite.Draw(spriteBatch, Position, 0f, 1f);
    }
}
