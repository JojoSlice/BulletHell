using System;
using BulletHell.Configurations;
using BulletHell.Helpers;
using BulletHell.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Models;

/// <summary>
/// Represents the player character.
/// </summary>
public class Player : IDisposable, IHealth, ICollidable
{
    private enum TurnState
    {
        None,
        TurningLeft,
        TurningRight,
        ExitingTurn,
    }

    private readonly float _speed = PlayerConfig.Speed;
    private readonly ISpriteHelper _sprite;
    private readonly IInputProvider _input;
    private readonly Collider _collider;
    private float _shootCooldown;
    private int _screenHeight;
    private int _screenWidth;
    private bool _disposed;

    // Knockback state
    private Vector2 _knockbackVelocity = Vector2.Zero;
    private float _knockbackTimer = 0f;

    // Turn animation state
    private Vector2 _lastDirection = Vector2.Zero;
    private Texture2D? _mainTexture;
    private Texture2D? _turnLeftTexture;
    private Texture2D? _turnRightTexture;
    private TurnState _turnState = TurnState.None;

    public Vector2 Position { get; private set; }
    public int Width => _sprite.Width;
    public int Height => _sprite.Height;
    public int MaxLives => PlayerConfig.Lives;
    public int Lives { get; private set; } = PlayerConfig.Lives;
    public bool IsAlive => Lives > 0;
    public int Health { get; private set; } = PlayerConfig.MaxHealth;
    public int Score { get; private set; }

    public void AddScore(int value)
    {
        Score += value;
    }

    public Collider Collider => _collider;

    public Player(Vector2 startPosition, IInputProvider input, ISpriteHelper sprite)
    {
        ArgumentNullException.ThrowIfNull(input, nameof(input));
        ArgumentNullException.ThrowIfNull(sprite, nameof(sprite));

        Position = startPosition;
        _input = input;
        _sprite = sprite;

        _shootCooldown = 0f;

        var initialRadius = Math.Max(Width, Height) / 2f;
        _collider = new Collider(Position, typeof(Player), initialRadius);
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
    public void LoadContent(
        Texture2D playerTexture,
        Texture2D? turnLeftTexture = null,
        Texture2D? turnRightTexture = null)
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

        _mainTexture = playerTexture;
        _turnLeftTexture = turnLeftTexture;
        _turnRightTexture = turnRightTexture;

        _sprite.LoadSpriteSheet(
            playerTexture,
            PlayerConfig.SpriteWidth,
            PlayerConfig.SpriteHeight,
            PlayerConfig.AnimationSpeed
        );

        _collider.Radius = Math.Max(Width, Height) / 2f;
    }

    /// <summary>
    /// Updates player position and animation
    /// </summary>
    public void Update(GameTime gameTime)
    {
        ArgumentNullException.ThrowIfNull(gameTime);

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_knockbackTimer > 0f)
        {
            // Apply smooth knockback over time
            Position += _knockbackVelocity * deltaTime;
            _knockbackTimer -= deltaTime;

            if (_knockbackTimer <= 0f)
            {
                _knockbackTimer = 0f;
                _knockbackVelocity = Vector2.Zero;
            }

            // Clamp to screen bounds if set
            if (_screenWidth > 0 && _screenHeight > 0)
            {
                float halfWidth = Width / 2f;
                float halfHeight = Height / 2f;

                Position = Position with
                {
                    X = Math.Clamp(Position.X, halfWidth, _screenWidth - halfWidth),
                    Y = Math.Clamp(Position.Y, halfHeight, _screenHeight - halfHeight),
                };
            }

            _collider.Position = Position;
        }
        else
        {
            Vector2 direction = _input.GetDirection();
            UpdateTurnAnimation(direction);
            Move(direction, deltaTime);
            _lastDirection = direction;
        }

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

    private TurnState DetectTurnDirection(Vector2 current, Vector2 last)
    {
        if (current == Vector2.Zero)
            return TurnState.None;

        if (current.X < -0.1f && last.X >= -0.1f)
            return TurnState.TurningLeft;

        if (current.X > 0.1f && last.X <= 0.1f)
            return TurnState.TurningRight;

        if (current.X < -0.1f)
            return TurnState.TurningLeft;

        if (current.X > 0.1f)
            return TurnState.TurningRight;

        return TurnState.None;
    }

    private void UpdateTurnAnimation(Vector2 currentDirection)
    {
        if (_turnLeftTexture == null || _turnRightTexture == null)
            return;

        var spriteHelper = _sprite as SpriteHelper;
        if (spriteHelper == null)
            return;

        var detectedTurn = DetectTurnDirection(currentDirection, _lastDirection);

        if (_turnState == TurnState.None || _turnState == TurnState.ExitingTurn)
        {
            if (detectedTurn == TurnState.TurningLeft || detectedTurn == TurnState.TurningRight)
            {
                _turnState = detectedTurn;
                var turnTexture = (detectedTurn == TurnState.TurningLeft)
                    ? _turnLeftTexture
                    : _turnRightTexture;

                spriteHelper.SetTexture(turnTexture);
                spriteHelper.SetSequenceAnimation(introEnd: 2, loopStart: 3, loopEnd: 7);
                spriteHelper.ResetAnimation();
            }
        }
        else if (_turnState == TurnState.TurningLeft || _turnState == TurnState.TurningRight)
        {
            if (detectedTurn == TurnState.None)
            {
                _turnState = TurnState.ExitingTurn;
                spriteHelper.StartExitSequence();
            }
            else if (detectedTurn != _turnState)
            {
                _turnState = detectedTurn;
                var turnTexture = (detectedTurn == TurnState.TurningLeft)
                    ? _turnLeftTexture
                    : _turnRightTexture;

                spriteHelper.SetTexture(turnTexture);
                spriteHelper.ResetAnimation();
            }
        }

        if (_turnState == TurnState.ExitingTurn)
        {
            if (spriteHelper.IsExitComplete())
            {
                if (detectedTurn == TurnState.None)
                {
                    spriteHelper.SetTexture(_mainTexture!);
                    spriteHelper.ResetToLooping();
                    spriteHelper.ResetAnimation();
                    _turnState = TurnState.None;
                }
                else
                {
                    _turnState = detectedTurn;
                    var turnTexture = (detectedTurn == TurnState.TurningLeft)
                        ? _turnLeftTexture
                        : _turnRightTexture;

                    spriteHelper.SetTexture(turnTexture);
                    spriteHelper.SetSequenceAnimation(introEnd: 2, loopStart: 3, loopEnd: 7);
                    spriteHelper.ResetAnimation();
                }
            }
            else if (detectedTurn != TurnState.None && detectedTurn != _turnState)
            {
                _turnState = detectedTurn;
                var turnTexture = (detectedTurn == TurnState.TurningLeft)
                    ? _turnLeftTexture
                    : _turnRightTexture;

                spriteHelper.SetTexture(turnTexture);
                spriteHelper.SetSequenceAnimation(introEnd: 2, loopStart: 3, loopEnd: 7);
                spriteHelper.ResetAnimation();
            }
        }
    }

    /// <summary>
    /// Attempts to shoot a bullet if cooldown has elapsed
    /// </summary>
    public (Vector2 position, Vector2 direction)? TryShoot()
    {
        if (_shootCooldown <= 0 && _input.IsShootPressed())
        {
            _shootCooldown = BulletConfig.Player.FireCooldown;

            Vector2 bulletDirection = -Vector2.UnitY;
            Vector2 bulletStartPosition = Position;

            return (bulletStartPosition, bulletDirection);
        }

        return null;
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            Lives--;
            Health = PlayerConfig.MaxHealth;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        ArgumentNullException.ThrowIfNull(spriteBatch);

        _sprite.Draw(spriteBatch, Position, 0f, 0.8f);
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

    /// <summary>
    /// Applies a smooth knockback over the given duration. The provided force is interpreted as
    /// initial speed (pixels per second). The knockback will move the player by
    /// approximately force * duration pixels over the duration.
    /// </summary>
    public void ApplyKnockback(Vector2 direction, float force, float duration)
    {
        if (duration <= 0f || force <= 0f)
            return;

        if (direction == Vector2.Zero)
            direction = -Vector2.UnitY;

        direction.Normalize();
        _knockbackVelocity = direction * force;
        _knockbackTimer = duration;
    }
}
