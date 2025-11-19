using System;
using System.Collections.Generic;
using BulletHell.Helpers;
using BulletHell.Interfaces;
using BulletHell.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Managers;

public class EnemyManager : IEnemyManager
{
    private readonly EnemyBulletManager _enemyBulletManager;
    private readonly List<Enemy> _enemies = new();
    private readonly Random _rand = new();
    private Texture2D? _enemyTexture;
    private float _shootTimer = 0f;

    public EnemyManager(EnemyBulletManager bulletManager)
    {
        _enemyBulletManager = bulletManager;
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

    public IEnumerable<EnemyBullet> TryShoot()
    {
        foreach (var enemy in _enemies)
        {
            yield return enemy.Shoot();
        }
    }

    public void Update(GameTime gameTime, int screenWidth, int screenHeight)
    {
        if (_enemies.Count < 1)
        {
            int spawnX = _rand.Next(50, screenWidth - 50);
            var spawnPos = new Vector2(spawnX, -50);
            var enemy = new Enemy(spawnPos, new SpriteHelper());
            enemy.LoadContent(_enemyTexture!);

            _enemies.Add(enemy);
        }

        foreach (var enemy in _enemies)
        {
            enemy.Update(gameTime);
        }

        _shootTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_shootTimer <= 0f)
        {
            foreach (var bullet in TryShoot())
            {
                _enemyBulletManager.AddBullet(bullet);
            }

            _shootTimer = 1.5f;
        }

        _enemies.RemoveAll(e => e.ShouldBeRemoved(screenWidth, screenHeight));
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var enemy in _enemies)
        {
            enemy.Draw(spriteBatch);
        }
    }
}
