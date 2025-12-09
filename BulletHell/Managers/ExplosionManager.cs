using BulletHell.Models;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace BulletHell.Managers;

public class ExplosionManager
{
    public List<Explosion> Explosions { get; } = new();

    public void Add(Explosion explosion)
    {
        Explosions.Add(explosion);
    }

    public void Update(GameTime gameTime)
    {
        foreach (var exp in Explosions)
        {
            exp.Update(gameTime);
        }

        Explosions.RemoveAll(e => !e.IsAlive);
    }
}
