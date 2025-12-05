using BulletHell.Models;

namespace BulletHell.Interfaces;
// Interface för alla objekt som kan kollidera i spelet.
public interface ICollidable
{
    Collider Collider { get; }
}