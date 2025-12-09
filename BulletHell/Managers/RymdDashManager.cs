using BulletHell.Models;
using BulletHell.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace BulletHell.Managers;

public class RymdDashManager : IDisposable
{
    private readonly List<RymdDash> _activeDashes = new();
    private readonly ObjectPool<RymdDash>[] _dashPools = new ObjectPool<RymdDash>[3];
    private readonly Random _rand = new();
    private readonly float[] _speeds = { 200f, 300f, 400f };
    private readonly Texture2D?[] _textures = new Texture2D?[3];
    private bool _disposed;

    public RymdDashManager()
    {
        // Create 3 pools, one for each rymddash type
        for (int i = 0; i < 3; i++)
        {
            int typeIndex = i; // Capture for closure
            _dashPools[i] = new ObjectPool<RymdDash>(
                () => new RymdDash(Vector2.Zero, _speeds[typeIndex]),
                resetAction: null,
                initialSize: 5,
                maxSize: 5
            );
        }
    }

    public void LoadContent(Texture2D texture1, Texture2D texture2, Texture2D texture3)
    {
        ArgumentNullException.ThrowIfNull(texture1);
        ArgumentNullException.ThrowIfNull(texture2);
        ArgumentNullException.ThrowIfNull(texture3);

        _textures[0] = texture1;
        _textures[1] = texture2;
        _textures[2] = texture3;
    }

    public void SpawnRymdDash(int screenWidth)
    {
        // Randomly pick one of the 3 types
        int type = _rand.Next(0, 3);

        if (_textures[type] == null)
            throw new InvalidOperationException("LoadContent must be called first");

        // Spawn at random X position above screen
        int spawnX = _rand.Next(-screenWidth / 2, screenWidth / 2);
        var spawnPos = new Vector2(spawnX, -screenWidth);

        var dash = _dashPools[type].Get();
        dash.Reset(spawnPos, _speeds[type]);

        if (dash.Width == 0 && _textures[type] != null)
        {
            dash.LoadContent(_textures[type]!);
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

                // Return to appropriate pool based on speed
                for (int poolIndex = 0; poolIndex < 3; poolIndex++)
                {
                    if (Math.Abs(dash.Speed - _speeds[poolIndex]) < 0.01f)
                    {
                        _dashPools[poolIndex].Return(dash);
                        break;
                    }
                }
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch, int screenWidth, int screenHeight)
    {
        ArgumentNullException.ThrowIfNull(spriteBatch);

        foreach (var dash in _activeDashes)
        {
            dash.Draw(spriteBatch, screenWidth, screenHeight);
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
                    // Return to pools
                    for (int i = 0; i < 3; i++)
                    {
                        _dashPools[i].Return(dash);
                    }
                }
                _activeDashes.Clear();

                foreach (var pool in _dashPools)
                {
                    pool.Clear();
                }
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
