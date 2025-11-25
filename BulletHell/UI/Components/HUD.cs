namespace BulletHell.UI.Components;

public class HUD
{
    public string Message { get; set; }
    private int _hp;
    public int HP
    {
        get => _hp;
        set
        {
            _hp = value;
            Message = $"HP: {value}";
        }
    }
}