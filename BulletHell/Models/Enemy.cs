using BulletHell.Configurations;
using BulletHell.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BulletHell.Models;

public class Enemy : IHealth, ICollidable
{
    private readonly ISpriteHelper _sprite;
    private readonly Collider _collider;
    private Vector2 _velocity;
    private float _shootCooldown = 0f;

    public Vector2 Position { get; private set; }

    public int Width => _sprite.Width;

    public int Height => _sprite.Height;
    public int Health { get; private set; } = EnemyConfig.MaxHealth;

    public bool IsAlive { get; private set; } = true;
    public int CollisionDamage { get; set; } = 25;

    public Collider Collider => _collider;

    public Enemy(Vector2 startPosition, ISpriteHelper sprite)
    {
        Position = startPosition;
        _sprite = sprite;
        _velocity = Vector2.Zero;

        var initialRadius = Math.Max(Width, Height) / 2f;
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
        return !IsAlive || IsOutOfBounds(screenWidth, screenHeight);
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

        _collider.Radius = Math.Max(Width, Height) / 2f;
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
            _shootCooldown = BulletConfig.Enemy.FireCooldown;
            Vector2 bulletVelocity = new Vector2(0, 1) * BulletConfig.Enemy.Speed;
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

    public void TakeDamage(int damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            Health = 0;
            IsAlive = false;
        }
    }
}
