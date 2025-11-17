using BulletHell.Configurations;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Models;
using BulletHell.Interfaces;
using Microsoft.Xna.Framework;

public class Enemy
{
    public Vector2 Position { get; private set; }
    private Vector2 _velocity;
    private readonly ISpriteHelper _sprite;

    public int Width => _sprite.Width;
    public int Height => _sprite.Height;

    public bool IsAlive { get; private set; } =  true;

    public Enemy(Vector2 startPosition, ISpriteHelper sprite)
    {
        Position = startPosition;
        _sprite =  sprite;
        _velocity = Vector2.Zero;
    }

    public void Update(GameTime gameTime)
    {
        var movement = Vector2.UnitY * EnemyConfig.Speed * (float)(gameTime.ElapsedGameTime.TotalSeconds);
        var newPosition = Position + movement;
        Position = newPosition;
        
        _sprite.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        
    }

    public void LoadContent(Texture2D texture)
    {
        
    }
}