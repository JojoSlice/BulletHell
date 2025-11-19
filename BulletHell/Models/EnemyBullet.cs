using System;
using BulletHell.Configurations;
using BulletHell.Helpers;
using BulletHell.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Models;

public class EnemyBullet : IDisposable
{
    private readonly ISpriteHelper _sprite;
    private float _timeAlive = 0f;
    private Vector2 _velocity;

    public Vector2 Position { get; private set; }
    public int Width => _sprite.Width;
    public int Height => _sprite.Height;

    public EnemyBullet(Vector2 startPosition, Vector2 velocity, ISpriteHelper sprite)
    {
        Position = startPosition;
        _velocity = velocity;
        _sprite = sprite;
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
    }

    public void Update(GameTime gameTime)
    {
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Position += _velocity * deltaTime;
        _timeAlive += deltaTime;

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
