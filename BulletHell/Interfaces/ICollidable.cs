using BulletHell.Models;

namespace BulletHell.Interfaces;

public interface ICollidable
{
    Collider Collider { get; }
}