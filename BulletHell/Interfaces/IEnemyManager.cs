using BulletHell.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Interfaces;

public interface IEnemyManager
{
    void LoadContent(Texture2D enemyTexture);

    void AddEnemy(Enemy enemy); // här kan man lägga till pattern eller annat skoj sen

    void Update(GameTime gameTime, int screenWidth, int screenHeight);

    void Draw(SpriteBatch spriteBatch);
}
