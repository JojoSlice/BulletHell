using BulletHell.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BulletHell.Models;

public class Player
{
    public Vector2 Position { get; set; }
    private readonly float speed = 300f;
    private SpriteHelper sprite;

    public int Width => sprite?.Width ?? 0;
    public int Height => sprite?.Height ?? 0;

    public Player(Vector2 startPosition)
    {
        Position = startPosition;
        sprite = new SpriteHelper();
    }

    public void LoadContent(ContentManager content)
    {
        Texture2D playerTexture = content.Load<Texture2D>("player");

        sprite.LoadSpriteSheet(playerTexture, 32, 32, 0.1f);
    }

    public void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        Vector2 direction = GetInputDirection();
        Move(direction, deltaTime);

        sprite?.Update(gameTime);
    }

    private Vector2 GetInputDirection()
    {
        KeyboardState keyState = Keyboard.GetState();
        Vector2 direction = Vector2.Zero;

        if (keyState.IsKeyDown(Keys.W) || keyState.IsKeyDown(Keys.Up))
            direction.Y -= 1;
        if (keyState.IsKeyDown(Keys.S) || keyState.IsKeyDown(Keys.Down))
            direction.Y += 1;
        if (keyState.IsKeyDown(Keys.A) || keyState.IsKeyDown(Keys.Left))
            direction.X -= 1;
        if (keyState.IsKeyDown(Keys.D) || keyState.IsKeyDown(Keys.Right))
            direction.X += 1;

        return direction;
    }

    private void Move(Vector2 direction, float deltaTime)
    {
        if (direction != Vector2.Zero)
            direction.Normalize();

        Position += direction * speed * deltaTime;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        sprite?.Draw(spriteBatch, Position, 0f, 1f);
    }
}
