using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Interfaces;

public interface IBulletManager
{
    void LoadContent(Texture2D bulletTexture);
    void CreateBullet(Vector2 position, Vector2 direction);
    void Update(GameTime gameTime, int screenWidth, int screenHeight);
    void Draw(SpriteBatch spriteBatch);
    public static class Player
    {
        public const int Damage = 10;
    }
    public static class Enemy
    {
        public const int Damage = 5;
    }
}
