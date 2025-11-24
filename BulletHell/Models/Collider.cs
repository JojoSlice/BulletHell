using System;
using Microsoft.Xna.Framework;

namespace BulletHell.Models;

public class Collider(Vector2 pos)
{
    public float Radius { get; set; }
    public Vector2 Position { get; set; } = pos;
    public Type ColliderType { get; set; }
}
