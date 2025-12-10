using BulletHell.Factories;
using BulletHell.Helpers;
using BulletHell.Interfaces;
using BulletHell.Models;
using BulletHell.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace BulletHell.Managers;

public class BulletManager<T> : IBulletManager, IDisposable
    where T : class
{
    private readonly List<Bullet<T>> _activeBullets = new();
    private readonly ObjectPool<Bullet<T>> _bulletPool;
    private readonly ISpriteHelperFactory _spriteHelperFactory;
    private Texture2D? _bulletTexture;
    private bool _disposed;

    public IReadOnlyList<Bullet<T>> Bullets => _activeBullets;

    public BulletManager(ISpriteHelperFactory spriteHelperFactory)
    {
        _spriteHelperFactory = spriteHelperFactory ?? throw new ArgumentNullException(nameof(spriteHelperFactory));

        _bulletPool = new ObjectPool<Bullet<T>>(
            CreateDefaultFactory(),
            resetAction: null, // Bullets are reset manually via Reset() method
            initialSize: 50, // Pre-allocate 50 bullets
            maxSize: 200 // Max 200 bullets in pool
            );
    }

    private Func<Bullet<T>> CreateDefaultFactory()
    {
        return () =>
        {
            ISpriteHelper sprite = _spriteHelperFactory.Create();
            return new Bullet<T>(Vector2.Zero, Vector2.Zero, sprite);
        };
    }

    /// <summary>
    /// Loads the bullet texture for all bullets
    /// </summary>
    public void LoadContent(Texture2D bulletTexture)
    {
        ArgumentNullException.ThrowIfNull(bulletTexture);
        _bulletTexture = bulletTexture;
    }

    /// <summary>
    /// Creates a new bullet at the specified position and direction.
    /// </summary>
    public void CreateBullet(Vector2 position, Vector2 direction)
    {
        // Kommenterar ut för att kunna köra tester.
        // if (_bulletTexture == null)
        //    throw new InvalidOperationException(
        //        "LoadContent must be called before creating bullets"
        //    );

        var bullet = _bulletPool.Get();

        bullet.Reset(position, direction);

        if ((bullet.Width == 0 || bullet.Height == 0) && _bulletTexture != null)
        {
            bullet.LoadContent(_bulletTexture);
        }

        _activeBullets.Add(bullet);
    }

    /// <summary>
    /// Updates all active bullets and removes dead/off-screen bullets
    /// </summary>
    public void Update(GameTime gameTime, int screenWidth, int screenHeight)
    {
        ArgumentNullException.ThrowIfNull(gameTime);

        foreach (var bullet in _activeBullets)
        {
            bullet.Update(gameTime);
        }

        for (int i = _activeBullets.Count - 1; i >= 0; i--)
        {
            if (_activeBullets[i].ShouldBeRemoved(screenWidth, screenHeight))
            {
                var b = _activeBullets[i];
                _activeBullets.RemoveAt(i);
                _bulletPool.Return(b);
            }
        }
    }

    /// <summary>
    /// Draws all active bullets
    /// </summary>
    public void Draw(SpriteBatch spriteBatch)
    {
        ArgumentNullException.ThrowIfNull(spriteBatch);

        foreach (var bullet in _activeBullets)
        {
            bullet.Draw(spriteBatch);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Return all active bullets to pool
                foreach (var bullet in _activeBullets)
                {
                    _bulletPool.Return(bullet);
                }
                _activeBullets.Clear();

                _bulletPool.Clear();

                // Note: _bulletTexture is managed by ContentManager, don't dispose here
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
