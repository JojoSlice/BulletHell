using System.Collections.Generic;
using BulletHell.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Interfaces;

public interface IEnemyBulletManager
{
    IReadOnlyList<EnemyBullet> Bullets { get; }

    void AddBullet(EnemyBullet bullet);

    void Update(GameTime gameTime, int screenWidth, int screenHeight);

    void Draw(SpriteBatch spriteBatch);
}
