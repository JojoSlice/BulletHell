using System.Collections.Generic;
using BulletHell.Interfaces;
using BulletHell.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Managers;

public class EnemyManager : IEnemyManager
{
    private readonly List<Enemy>  _enemies = new();

    public IReadOnlyList<Enemy> Enemies => _enemies;

    public void LoadContent(Texture2D enemyTexture)
    {
        
    }

    public void AddEnemy(Enemy enemy)
    {
        _enemies.Add(enemy);
    }

    public void Update(GameTime gameTime, int screenWidth, int screenHeight)
    {
        _enemies.RemoveAll(e =>
            e.Position.X < 0 ||
            e.Position.Y < 0 ||
            e.Position.X > screenWidth ||
            e.Position.Y > screenHeight
        );
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        
    }
}