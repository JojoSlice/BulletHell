using System;
using BulletHell.Configurations;
using BulletHell.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Models;

public class EnemyBullet : IDisposable
{
    private readonly ISpriteHelper _sprite;
    private float _timeAlive = 0f;
    private Vector2 _velocity;
    private Collider _collider;

    public Vector2 Position { get; private set; }
    public int Width => _sprite.Width;
    public int Height => _sprite.Height;

    public Collider Collider => _collider;

    public EnemyBullet(Vector2 startPosition, Vector2 velocity, ISpriteHelper sprite)
    {
        Position = startPosition;
        _velocity = velocity;
        _sprite = sprite;
        _collider = new Collider(Position, typeof(EnemyBullet));
    }

    public bool ShouldBeRemoved(int screenWidth, int screenHeight)
    {
        return IsOutOfBounds(screenWidth, screenHeight);
    }

    public void LoadContent(Texture2D bulletTexture)
    {
        _sprite.LoadSpriteSheet(
            bulletTexture,
            EnemyBulletConfig.SpriteWidth,
            EnemyBulletConfig.SpriteHeight,
            EnemyBulletConfig.AnimationSpeed);

        // Set collider radius based on sprite size
        UpdateColliderRadiusFromSprite();
    }

    /// <summary>
    /// Updates the collider radius from the current sprite frame size.
    /// Exposed to allow unit tests to set radius without requiring a real Texture2D.
    /// </summary>
    public void UpdateColliderRadiusFromSprite() => _collider.Radius = Math.Max(Width, Height) / 2f;

    public void Update(GameTime gameTime)
    {
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Position += _velocity * deltaTime;
        _timeAlive += deltaTime;

        // Keep collider position in sync
        _collider.Position = Position;

        _sprite.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _sprite.Draw(spriteBatch, Position, 0f, 1f);
    }

    public bool IsOutOfBounds(int screenWidth, int screenHeight)
    {
        return Position.X < 0
               || Position.X > screenWidth
               || Position.Y < 0
               || Position.Y > screenHeight;
    }

    public void Reset(Vector2 position, Vector2 velocity)
    {
        Position = position;
        _velocity = velocity;
        _timeAlive = 0f;
        _collider.Position = Position;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _sprite?.Dispose();
        }
    }
}
