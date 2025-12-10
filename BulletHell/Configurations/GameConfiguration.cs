namespace BulletHell.Configurations;

/// <summary>
/// Root configuration class for the entire game.
/// Contains all configurable settings organized by category.
/// </summary>
public class GameConfiguration
{
    public PlayerConfiguration Player { get; set; } = new();
    public EnemyConfiguration Enemy { get; set; } = new();
    public BulletConfiguration Bullets { get; set; } = new();
    public PoolConfiguration Pools { get; set; } = new();
    public CollisionConfiguration Collision { get; set; } = new();
    public SpawnConfiguration Spawn { get; set; } = new();
    public EffectsConfiguration Effects { get; set; } = new();
}

/// <summary>
/// Player-related configuration settings.
/// </summary>
public class PlayerConfiguration
{
    public float Speed { get; set; } = 300f;
    public int SpriteWidth { get; set; } = 128;
    public int SpriteHeight { get; set; } = 128;
    public float AnimationSpeed { get; set; } = 0.1f;
    public int MaxHealth { get; set; } = 100;
    public int Lives { get; set; } = 3;
    public int TurnAnimationIntroEnd { get; set; } = 2;
    public int TurnAnimationLoopStart { get; set; } = 3;
    public int TurnAnimationLoopEnd { get; set; } = 7;
    public float TurnDetectionThreshold { get; set; } = 0.1f;
}

/// <summary>
/// Enemy-related configuration settings.
/// </summary>
public class EnemyConfiguration
{
    public float Speed { get; set; } = 50f;
    public int SpriteWidth { get; set; } = 32;
    public int SpriteHeight { get; set; } = 32;
    public float AnimationSpeed { get; set; } = 0f;
    public int MaxHealth { get; set; } = 20;
    public int ScoreValue { get; set; } = 1;
}

/// <summary>
/// Bullet configuration for both player and enemy bullets.
/// </summary>
public class BulletConfiguration
{
    public PlayerBulletConfig Player { get; set; } = new();
    public EnemyBulletConfig Enemy { get; set; } = new();

    public class PlayerBulletConfig
    {
        public float Speed { get; set; } = 600f;
        public int SpriteWidth { get; set; } = 8;
        public int SpriteHeight { get; set; } = 8;
        public float AnimationSpeed { get; set; } = 0.05f;
        public float Lifetime { get; set; } = 3f;
        public float FireCooldown { get; set; } = 0.2f;
        public int Damage { get; set; } = 10;
    }

    public class EnemyBulletConfig
    {
        public float Speed { get; set; } = 100f;
        public int SpriteWidth { get; set; } = 8;
        public int SpriteHeight { get; set; } = 8;
        public float AnimationSpeed { get; set; } = 0.05f;
        public float FireCooldown { get; set; } = 1.5f;
        public int Damage { get; set; } = 10;
    }
}

/// <summary>
/// Object pool size configuration for various pooled objects.
/// </summary>
public class PoolConfiguration
{
    public PoolSizeConfig PlayerBullets { get; set; } = new() { InitialSize = 50, MaxSize = 200 };
    public PoolSizeConfig EnemyBullets { get; set; } = new() { InitialSize = 50, MaxSize = 200 };
    public PoolSizeConfig Enemies { get; set; } = new() { InitialSize = 10, MaxSize = 15 };
    public PoolSizeConfig Dashes { get; set; } = new() { InitialSize = 10, MaxSize = 10 };
    public PoolSizeConfig RymdDashes { get; set; } = new() { InitialSize = 5, MaxSize = 5 };

    public class PoolSizeConfig
    {
        public int InitialSize { get; set; }
        public int MaxSize { get; set; }
    }
}

/// <summary>
/// Collision detection and knockback configuration.
/// </summary>
public class CollisionConfiguration
{
    public float DistanceCheckThreshold { get; set; } = 10f;
    public float KnockbackForce { get; set; } = 300f;
    public float KnockbackDuration { get; set; } = 0.18f;
}

/// <summary>
/// Spawn position configuration for enemies and effects.
/// </summary>
public class SpawnConfiguration
{
    public int EnemySpawnMargin { get; set; } = 50;
    public int EnemySpawnOffsetY { get; set; } = -50;
    public int DashSpawnOffsetY { get; set; } = -30;
}

/// <summary>
/// Visual effects configuration.
/// </summary>
public class EffectsConfiguration
{
    public float DashSpawnInterval { get; set; } = 0.2f;
    public float RymdDashSpawnInterval { get; set; } = 0.7f;
    public float[] RymdDashSpeeds { get; set; } = new[] { 200f, 300f, 400f };
}
