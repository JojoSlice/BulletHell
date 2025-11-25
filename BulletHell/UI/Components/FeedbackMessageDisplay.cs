using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.UI.Components;

public class FeedbackMessageDisplay
{
    private string _message = string.Empty;
    private Color _color = Color.White;
    private float _timer = 0f;
    private float _duration = 0f;

    public bool IsVisible => _timer > 0 && !string.IsNullOrEmpty(_message);

    public void Show(string message, Color color, float duration)
    {
        _message = message;
        _color = color;
        _duration = duration;
        _timer = duration;
    }

    public void Clear()
    {
        _message = string.Empty;
        _timer = 0f;
    }

    public void Update(GameTime gameTime)
    {
        if (_timer > 0)
        {
            _timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_timer < 0)
            {
                Clear();
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch, SpriteFont font, int screenWidth, int yPosition)
    {
        if (!IsVisible)
        {
            return;
        }

        var messageSize = font.MeasureString(_message);
        var messagePosition = new Vector2((float)screenWidth / 2 - messageSize.X / 2, yPosition);

        spriteBatch.DrawString(font, _message, messagePosition, _color);
    }
}
