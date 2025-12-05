namespace BulletHell.Interfaces
{
// Interface för alla objekt som har hälsa, kan ta skada och kan dö.
    public interface IHealth
    {
        int Health { get; }
        void TakeDamage(int amount);
        bool IsAlive { get; }
    }
}