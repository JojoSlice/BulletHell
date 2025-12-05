namespace BulletHell.Interfaces
{
    // Interface för objekt som kan orsaka skada (t.ex. bullets).
    public interface IDamageDealer
    {
        int Damage { get; }
    }
}