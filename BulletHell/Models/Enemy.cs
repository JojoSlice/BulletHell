using BulletHell.Configurations;
using BulletHell.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Models;

public class Enemy
{
    private readonly ISpriteHelper _sprite;
    private Vector2 _velocity;

    public Vector2 Position { get; private set; }

    public int Width => _sprite.Width;

    public int Height => _sprite.Height;

    public bool IsAlive { get; private set; } = true;

    public Enemy(Vector2 startPosition, ISpriteHelper sprite)
    {
        Position = startPosition;
        _sprite = sprite;
        _velocity = Vector2.Zero;
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
        var movement = Vector2.UnitY * EnemyConfig.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        var newPosition = Position + movement;
        Position = newPosition;

        _sprite.Update(gameTime);
    }

    public EnemyBullet Shoot()
    {
        var start = Position;
        Vector2 velocity = new Vector2(0, 1) * EnemyBulletConfig.Speed;

        return new EnemyBullet(start, velocity);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // NOTE: Draw anropas endast grafiskt och enhetstestas inte.
        // Renderingen verifieras visuellt i spelet.
        _sprite.Draw(spriteBatch, Position, 0f, 1f);
    }
}
