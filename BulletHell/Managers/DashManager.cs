using BulletHell.Configurations;
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

public class DashManager : IDisposable
{
    private readonly List<Dash> _activeDashes = new();
    private readonly ObjectPool<Dash> _dashPool;
    private readonly ISpriteHelperFactory _spriteHelperFactory;
    private readonly Random _rand = new();
    private Texture2D? _dashTexture;
    private bool _disposed;

    public DashManager(PoolConfiguration.PoolSizeConfig? poolConfig = null, ISpriteHelperFactory? spriteHelperFactory = null)
    {
        _spriteHelperFactory = spriteHelperFactory ?? new SpriteHelperFactory();

        var pool = poolConfig ?? new PoolConfiguration.PoolSizeConfig { InitialSize = 10, MaxSize = 10 };

        _dashPool = new ObjectPool<Dash>(
            CreateDashFactory(),
            resetAction: null,
            initialSize: pool.InitialSize,
            maxSize: pool.MaxSize
        );
    }

    private Func<Dash> CreateDashFactory()
    {
        return () =>
        {
            ISpriteHelper sprite = _spriteHelperFactory.Create();
            return new Dash(Vector2.Zero, sprite);
        };
    }

    public void LoadContent(Texture2D dashTexture)
    {
        ArgumentNullException.ThrowIfNull(dashTexture);
        _dashTexture = dashTexture;
    }

    public void SpawnDash(int screenWidth)
    {
        if (_dashTexture == null)
            throw new InvalidOperationException("LoadContent must be called first");

        int spawnX = _rand.Next(0, screenWidth);
        var spawnPos = new Vector2(spawnX, -30);

        var dash = _dashPool.Get();
        dash.Reset(spawnPos);

        if (dash.Width == 0)
        {
            dash.LoadContent(_dashTexture);
        }

        _activeDashes.Add(dash);
    }

    public void Update(GameTime gameTime, int screenWidth, int screenHeight)
    {
        ArgumentNullException.ThrowIfNull(gameTime);

        foreach (var dash in _activeDashes)
        {
            dash.Update(gameTime);
        }

        for (int i = _activeDashes.Count - 1; i >= 0; i--)
        {
            if (_activeDashes[i].ShouldBeRemoved(screenWidth, screenHeight))
            {
                var dash = _activeDashes[i];
                _activeDashes.RemoveAt(i);
                _dashPool.Return(dash);
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        ArgumentNullException.ThrowIfNull(spriteBatch);

        foreach (var dash in _activeDashes)
        {
            dash.Draw(spriteBatch);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                foreach (var dash in _activeDashes)
                {
                    _dashPool.Return(dash);
                }
                _activeDashes.Clear();
                _dashPool.Clear();
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
