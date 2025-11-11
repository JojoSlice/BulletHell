using BulletHell.Configurations;
using BulletHell.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Helpers;

public class SpriteHelper : ISpriteHelper
{
    private Texture2D? _texture;
    private Rectangle[]? _frames;
    private int _currentFrame;
    private float _frameTime;
    private float _timeElapsed;
    private int _frameWidth;
    private int _frameHeight;

    public int Width => _frameWidth;
    public int Height => _frameHeight;

    public SpriteHelper()
    {
        _currentFrame = 0;
        _timeElapsed = 0f;
    }

    public void LoadSpriteSheet(
        Texture2D _texture,
        int _frameWidth = SpriteDefaults.FrameWidth,
        int _frameHeight = SpriteDefaults.FrameHeight,
        float animationSpeed = SpriteDefaults.AnimationSpeed
    )
    {
        this._texture = _texture;
        this._frameWidth = _frameWidth;
        this._frameHeight = _frameHeight;
        this._frameTime = animationSpeed;

        int columns = _texture.Width / _frameWidth;
        int rows = _texture.Height / _frameHeight;
        int total_frames = columns * rows;

        _frames = new Rectangle[total_frames];
        int frameIndex = 0;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                _frames[frameIndex] = new Rectangle(
                    col * _frameWidth,
                    row * _frameHeight,
                    _frameWidth,
                    _frameHeight
                );
                frameIndex++;
            }
        }
    }

    public void Update(GameTime gameTime)
    {
        if (_frames == null || _frames.Length == 0)
            return;

        _timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_timeElapsed >= _frameTime)
        {
            _timeElapsed = 0f;
            _currentFrame = (_currentFrame + 1) % _frames.Length;
        }
    }

    public void Draw(
        SpriteBatch spriteBatch,
        Vector2 position,
        Color? color = null,
        float rotation = 0f,
        float scale = 1f
    )
    {
        if (_texture == null || _frames == null || _frames.Length == 0)
            return;

        spriteBatch.Draw(
            _texture,
            position,
            _frames[_currentFrame],
            color ?? Color.White,
            rotation,
            new Vector2(_frameWidth / 2, _frameHeight / 2), // Centrera sprite
            scale,
            SpriteEffects.None,
            0f
        );
    }

    public void Draw(
        SpriteBatch spriteBatch,
        Vector2 position,
        float rotation = 0f,
        float scale = 1f
    )
    {
        Draw(spriteBatch, position, Color.White, rotation, scale);
    }
}
