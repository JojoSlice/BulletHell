using BulletHell.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace BulletHell.Managers;

public class ExplosionManager
{
    private readonly List<Explosion> _explosions = new();

    public IReadOnlyList<Explosion> Explosions => _explosions; // för tests skull

    private Texture2D _explosionTexture = null!;

    public void LoadContent(Texture2D texture)
    {
        _explosionTexture = texture;
    }

    public void Add(Explosion explosion)
    {
        explosion.LoadContent(_explosionTexture);
        _explosions.Add(explosion);
    }

    public void Update(GameTime gameTime)
    {
        foreach (var exp in _explosions)
        {
            exp.Update(gameTime);
        }

        _explosions.RemoveAll(e => !e.IsAlive);
    }

    public void Draw(SpriteBatch spriteBatch) // utan test
    {
        foreach (var exp in _explosions)
        {
            exp.Draw(spriteBatch);
        }
    }
}