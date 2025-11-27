using System;
using BulletHell.Configurations;
using BulletHell.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Models;

/// <summary>
/// Represents the player character
/// </summary>
public class Player : IDisposable
{
    private readonly float _speed = PlayerConfig.Speed;
    private readonly ISpriteHelper _sprite;
    private readonly IInputProvider _input;
    private readonly Collider _collider;
    private float _shootCooldown;
    private int _screenWidth;
    private int _screenHeight;
    private bool _disposed;

    public Vector2 Position { get; private set; }
    public int Width => _sprite.Width;
    public int Height => _sprite.Height;

    public Collider Collider => _collider;

    public Player(Vector2 startPosition, IInputProvider input, ISpriteHelper sprite)
    {
        ArgumentNullException.ThrowIfNull(input, nameof(input));
        ArgumentNullException.ThrowIfNull(sprite, nameof(sprite));

        Position = startPosition;
        _input = input;
        _sprite = sprite;

        var initialRadius = Math.Max(_sprite.Width, _sprite.Height) / 2f;
        _collider = new Collider(Position, typeof(Player), initialRadius);

        _shootCooldown = 0f;
    }

    /// <summary>
    /// Sets the screen boundaries for clamping player position
    /// </summary>
    public void SetScreenBounds(int screenWidth, int screenHeight)
    {
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
    }

    /// <summary>
    /// Loads the player texture and initializes the sprite
    /// </summary>
    public void LoadContent(Texture2D playerTexture)
    {
        ArgumentNullException.ThrowIfNull(playerTexture);

        if (PlayerConfig.SpriteWidth <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(PlayerConfig.SpriteWidth),
                PlayerConfig.SpriteWidth,
                "SpriteWidth must be positive"
            );

        if (PlayerConfig.SpriteHeight <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(PlayerConfig.SpriteHeight),
                PlayerConfig.SpriteHeight,
                "SpriteHeight must be positive"
            );

        if (PlayerConfig.AnimationSpeed < 0)
            throw new ArgumentOutOfRangeException(
                nameof(PlayerConfig.AnimationSpeed),
                PlayerConfig.AnimationSpeed,
                "AnimationSpeed must be non-negative"
            );

        _sprite.LoadSpriteSheet(
            playerTexture,
            PlayerConfig.SpriteWidth,
            PlayerConfig.SpriteHeight,
            PlayerConfig.AnimationSpeed
        );
    }

    /// <summary>
    /// Updates player position and animation
    /// </summary>
    public void Update(GameTime gameTime)
    {
        ArgumentNullException.ThrowIfNull(gameTime);

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        Vector2 direction = _input.GetDirection();
        Move(direction, deltaTime);

        if (_shootCooldown > 0)
            _shootCooldown -= deltaTime;

        _sprite.Update(gameTime);
    }

    private void Move(Vector2 direction, float deltaTime)
    {
        if (direction != Vector2.Zero)
            direction.Normalize();

        Vector2 newPosition = Position + direction * _speed * deltaTime;

        if (_screenWidth > 0 && _screenHeight > 0)
        {
            float halfWidth = Width / 2f;
            float halfHeight = Height / 2f;

            newPosition.X = Math.Clamp(newPosition.X, halfWidth, _screenWidth - halfWidth);
            newPosition.Y = Math.Clamp(newPosition.Y, halfHeight, _screenHeight - halfHeight);
        }

        Position = newPosition;
        _collider.Position = Position;
    }

    /// <summary>
    /// Attempts to shoot a bullet if cooldown has elapsed
    /// </summary>
    public (Vector2 position, Vector2 direction)? TryShoot()
    {
        if (_shootCooldown <= 0 && _input.IsShootPressed())
        {
            _shootCooldown = BulletConfig.FireCooldown;

            Vector2 bulletDirection = -Vector2.UnitY;
            Vector2 bulletStartPosition = Position;

            return (bulletStartPosition, bulletDirection);
        }
        return null;
    }

    /// <summary>
    /// Draws the player to the screen
    /// </summary>
    public void Draw(SpriteBatch spriteBatch)
    {
        ArgumentNullException.ThrowIfNull(spriteBatch);

        _sprite.Draw(spriteBatch, Position, 0f, 1f);
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
