namespace BulletHell.Configurations;

public static class BulletConfig
{
    public static class Player
    {
        public const float Speed = 600f;
        public const int SpriteWidth = 8;
        public const int SpriteHeight = 8;
        public const float AnimationSpeed = 0.05f;
        public const float Lifetime = 3f;
        public const float FireCooldown = 0.2f;
        public const int Damage = 10;
    }
    public static class Enemy
    {
        public const float Speed = 100f;
        public const int SpriteWidth = 8;
        public const int SpriteHeight = 8;
        public const float AnimationSpeed = 0.05f;
        public const float Lifetime = 10f;
        public const float FireCooldown = 1.5f;
        public const int Damage = 10;
    }
}
