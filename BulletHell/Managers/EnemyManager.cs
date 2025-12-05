using BulletHell.Helpers;
using BulletHell.Interfaces;
using BulletHell.Models;
using BulletHell.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BulletHell.Managers;

public class EnemyManager : IEnemyManager
{
    private readonly BulletManager<Enemy> _enemyBulletManager;
    private readonly List<Enemy> _enemies = new();
    private readonly ObjectPool<Enemy> _enemyPool;
    private readonly Random _rand = new();
    private Texture2D? _enemyTexture;

    public EnemyManager(BulletManager<Enemy> bulletManager)
    {
        _enemyBulletManager = bulletManager;
        _enemyPool = new ObjectPool<Enemy>(
            CreateEnemyFactory(),
            resetAction: null, // Enemies are reset manually via Reset() method
            initialSize: 10, // Pre-allocate 10 enemies
            maxSize: 15 // Max 15 enemies in pool
        );
    }

    private Func<Enemy> CreateEnemyFactory()
    {
        return () =>
        {
            ISpriteHelper sprite = new SpriteHelper();
            return new Enemy(Vector2.Zero, sprite);
        };
    }

    public IReadOnlyList<Enemy> Enemies => _enemies;

    public void LoadContent(Texture2D enemyTexture)
    {
        _enemyTexture = enemyTexture;
        foreach (var enemy in _enemies)
        {
            enemy.LoadContent(enemyTexture);
        }
    }

    public void AddEnemy(Enemy enemy)
    {
        _enemies.Add(enemy);
    }

    public void TryShootEnemies()
    {
        _enemies
            .Select(enemy => enemy.TryShoot())
            .Where(shootData => shootData.HasValue)
            .Select(shootData => shootData!.Value)
            .ToList()
            .ForEach(data =>
            {
                var (position, velocity) = data;
                _enemyBulletManager.CreateBullet(position, velocity);
            });
    }

    public void Update(GameTime gameTime, int screenWidth, int screenHeight)
    {
        if (_enemies.Count < 1)
        {
            int spawnX = _rand.Next(50, screenWidth - 50);
            var spawnPos = new Vector2(spawnX, -50);

            var enemy = _enemyPool.Get();
            enemy.Reset(spawnPos);

            if ((enemy.Width == 0 || enemy.Height == 0) && _enemyTexture != null)
            {
                enemy.LoadContent(_enemyTexture);
            }

            _enemies.Add(enemy);
        }

        foreach (var enemy in _enemies)
        {
            enemy.Update(gameTime);
        }

        TryShootEnemies();

        // Return removed enemies to pool
        for (int i = _enemies.Count - 1; i >= 0; i--)
        {
            if (_enemies[i].ShouldBeRemoved(screenWidth, screenHeight))
            {
                var enemy = _enemies[i];
                _enemies.RemoveAt(i);
                _enemyPool.Return(enemy);
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var enemy in _enemies)
        {
            enemy.Draw(spriteBatch);
        }
    }
}
