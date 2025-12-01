namespace BulletHell.DebugUI;

public class DebugInfo
{
    public required string SceneName { get; set; }
    public int EnemyCount { get; set; }
    public int BulletCount { get; set; }
    public int Fps { get; set; }
}