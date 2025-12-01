using BulletHell.Configurations;
using BulletHell.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BulletHell.Models;

public class Enemy
{
    private readonly ISpriteHelper _sprite;
    private readonly Collider _collider;
    private Vector2 _velocity;
    private float _shootCooldown = 0f;

    public Vector2 Position { get; private set; }

    public int Width => _sprite.Width;

    public int Height => _sprite.Height;

    public bool IsAlive { get; private set; } = true;

    public Collider Collider => _collider;

    public Enemy(Vector2 startPosition, ISpriteHelper sprite)
    {
        Position = startPosition;
        _sprite = sprite;
        _velocity = Vector2.Zero;

        var initialRadius = Math.Max(_sprite.Width, _sprite.Height) / 2f;
        _collider = new Collider(Position, typeof(Enemy), initialRadius);
    }

    public bool IsOutOfBounds(int screenWidth, int screenHeight)
    {
        return Position.X < 0 ||
               Position.X > screenWidth ||
               Position.Y > screenHeight;
    }

    public bool ShouldBeRemoved(int screenWidth, int screenHeight)
    {
        return IsOutOfBounds(screenWidth, screenHeight);
    }

    public void LoadContent(Texture2D enemyTexture)
    {
        // NOTE: Går inte att enhetstesta just nu eftersom-
        // SpriteHelper kräver en riktig Texture2D.
        // Funktionaliteten verifieras istället i spelet.
        _sprite.LoadSpriteSheet(
            enemyTexture,
            SpriteDefaults.FrameWidth,
            SpriteDefaults.FrameHeight,
            SpriteDefaults.AnimationSpeed);
    }

    public void Update(GameTime gameTime)
    {
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        var movement = Vector2.UnitY * EnemyConfig.Speed * deltaTime;
        var newPosition = Position + movement;
        Position = newPosition;

        // Keep collider in sync with logical position
        _collider.Position = Position;

        if (_shootCooldown > 0)
        {
            _shootCooldown -= deltaTime;
        }

        _sprite.Update(gameTime);
    }

    public (Vector2 position, Vector2 velocity)? TryShoot()
    {
        if (_shootCooldown <= 0)
        {
            _shootCooldown = EnemyBulletConfig.FireCooldown;
            Vector2 bulletVelocity = new Vector2(0, 1) * EnemyBulletConfig.Speed;
            Vector2 bulletStartPosition = Position;
            return (bulletStartPosition, bulletVelocity);
        }
        return null;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // NOTE: Draw anropas endast grafiskt och enhetstestas inte.
        // Renderingen verifieras visuellt i spelet.
        _sprite.Draw(spriteBatch, Position, 0f, 1f);
    }
}
