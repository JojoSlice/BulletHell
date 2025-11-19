using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BulletHell.Models;

namespace BulletHell.Interfaces
{
    public interface IEnemyBulletManager
    {
        IReadOnlyList<EnemyBullet> Bullets { get; }

        void AddBullet(EnemyBullet bullet);

        void Update(GameTime gameTime, int screenWidth, int screenHeight);

        void Draw(SpriteBatch spriteBatch);
    }
}