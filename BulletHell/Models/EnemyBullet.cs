using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class EnemyBullet
{
    private Vector2 _velocity;

    public Vector2 Position { get; private set; }

    public EnemyBullet(Vector2 startPosition, Vector2 velocity)
    {
        Position = startPosition;
        _velocity = velocity;
    }

    public void LoadContent(Texture2D bulletTexture)
    {
    }

    public void Update(GameTime gameTime)
    {
    }

    public void Draw(SpriteBatch spriteBatch)
    {
    }

    public bool IsOutOfBounds(int screenWidth, int screenHeight)
    {
        return Position.X < 0
               || Position.X > screenWidth
               || Position.Y < 0
               || Position.Y > screenHeight;
    }
}