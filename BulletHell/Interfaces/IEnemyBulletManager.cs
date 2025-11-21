using System.Collections.Generic;
using BulletHell.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Interfaces;

public interface IEnemyBulletManager
{
    IReadOnlyList<EnemyBullet> Bullets { get; }

    void CreateBullet(Vector2 position, Vector2 velocity);

    void Update(GameTime gameTime, int screenWidth, int screenHeight);

    void Draw(SpriteBatch spriteBatch);

    void LoadContent(Texture2D texture);
}
