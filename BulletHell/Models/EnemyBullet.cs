using BulletHell.Configurations;
using BulletHell.Helpers;
using BulletHell.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class EnemyBullet
{
    private Vector2 _velocity;
    private readonly ISpriteHelper _sprite;
    private float _timeAlive = 0f;

    public Vector2 Position { get; private set; }

    public EnemyBullet(Vector2 startPosition, Vector2 velocity)
    {
        Position = startPosition;
        _velocity = velocity;
        _sprite = new SpriteHelper();
    }
    
    public bool ShouldBeRemoved(int screenWidth, int screenHeight)
    {
        return _timeAlive >= EnemyBulletConfig.Lifetime
               || IsOutOfBounds(screenWidth, screenHeight);
    }

    public void LoadContent(Texture2D bulletTexture)
    {
        _sprite.LoadSpriteSheet(
            bulletTexture,
            8,
            8,
            0.05f
        );

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
}