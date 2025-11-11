using BulletHell.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Models;

public class Player(Vector2 startPosition, IInputProvider input, ISpriteHelper sprite)
{
    public Vector2 Position { get; set; } = startPosition;
    private readonly float speed = 300f;
    private readonly ISpriteHelper _sprite = sprite;

    private readonly IInputProvider _input = input;
    public int Width => _sprite?.Width ?? 0;
    public int Height => _sprite?.Height ?? 0;

    public void LoadContent(Texture2D playerTexture, int frameWidth = 32, int frameHeight = 32)
    {
        _sprite.LoadSpriteSheet(playerTexture, frameWidth, frameHeight, 0.1f);
    }

    public void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        Vector2 direction = _input.GetDirection();
        Move(direction, deltaTime);

        _sprite?.Update(gameTime);
    }

    private void Move(Vector2 direction, float deltaTime)
    {
        if (direction != Vector2.Zero)
            direction.Normalize();

        Position += direction * speed * deltaTime;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _sprite?.Draw(spriteBatch, Position, 0f, 1f);
    }
}
