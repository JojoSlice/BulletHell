using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace BulletHell.Interfaces;

public interface IEnemyManager
{
    void LoadContent(Texture2D enemyTexture);
    void AddEnemy(Vector2 position); // här kan man lägga till pattern eller annat skoj sen
    void Update(GameTime gameTime, int screenWidth, int screenHeight);
    void Draw(SpriteBatch spriteBatch);
}