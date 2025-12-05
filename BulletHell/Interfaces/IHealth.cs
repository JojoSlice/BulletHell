namespace BulletHell.Interfaces;

public interface IHealth
{
    int Health { get; }
    void TakeDamage(int amount);
    bool IsAlive { get; }
}