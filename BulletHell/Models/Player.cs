using System;
using BulletHell.Configurations;
using BulletHell.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Models;

public class Player(Vector2 startPosition, IInputProvider input, ISpriteHelper sprite)
{
    public Vector2 Position { get; set; } = startPosition;
    private readonly float _speed = PlayerConfig.Speed;
    private readonly ISpriteHelper _sprite =
        sprite ?? throw new ArgumentNullException(nameof(sprite));
    private readonly IInputProvider _input =
        input ?? throw new ArgumentNullException(nameof(input));
    public int Width => _sprite?.Width ?? 0;
    public int Height => _sprite?.Height ?? 0;

    public void LoadContent(Texture2D playerTexture)
    {
        ArgumentNullException.ThrowIfNull(playerTexture);

        if (PlayerConfig.SpriteWidth <= 0)
            throw new InvalidOperationException(
                $"SpriteWidth must be positive, got {PlayerConfig.SpriteWidth}"
            );

        if (PlayerConfig.SpriteHeight <= 0)
            throw new InvalidOperationException(
                $"SpriteHeight must be positive, got {PlayerConfig.SpriteHeight}"
            );

        if (PlayerConfig.AnimationSpeed < 0)
            throw new InvalidOperationException(
                $"AnimationSpeed must be non-negative, got {PlayerConfig.AnimationSpeed}"
            );

        _sprite.LoadSpriteSheet(
            playerTexture,
            PlayerConfig.SpriteWidth,
            PlayerConfig.SpriteHeight,
            PlayerConfig.AnimationSpeed
        );
    }

    public void Update(GameTime gameTime)
    {
        ArgumentNullException.ThrowIfNull(gameTime);

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (deltaTime < 0)
            throw new InvalidOperationException($"Delta time cannot be negative, got {deltaTime}");

        Vector2 direction = _input.GetDirection();
        Move(direction, deltaTime);

        _sprite?.Update(gameTime);
    }

    private void Move(Vector2 direction, float deltaTime)
    {
        if (direction != Vector2.Zero)
            direction.Normalize();

        Position += direction * _speed * deltaTime;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        ArgumentNullException.ThrowIfNull(spriteBatch);

        _sprite.Draw(spriteBatch, Position, 0f, 1f);
    }
}
