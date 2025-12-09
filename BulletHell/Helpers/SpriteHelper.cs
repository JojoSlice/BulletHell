using System;
using BulletHell.Configurations;
using BulletHell.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Helpers;

/// <summary>
/// Helper class for managing sprite sheets and animations
/// </summary>
public class SpriteHelper : ISpriteHelper, IDisposable
{
    private enum AnimationMode
    {
        Looping,
        SequenceWithExit,
    }

    private enum AnimationPhase
    {
        Intro,
        Loop,
        Exit,
    }

    private Texture2D? _texture;
    private Rectangle[]? _frames;
    private int _currentFrame;
    private float _frameTime;
    private float _timeElapsed;
    private int _frameWidth;
    private int _frameHeight;
    private bool _disposed;

    // Animation state fields
    private AnimationMode _mode = AnimationMode.Looping;
    private AnimationPhase _phase = AnimationPhase.Intro;
    private int _introEnd = -1;
    private int _loopStart = -1;
    private int _loopEnd = -1;

    public int Width => _frameWidth;
    public int Height => _frameHeight;

    public SpriteHelper()
    {
        _currentFrame = 0;
        _timeElapsed = 0f;
    }

    public void LoadSpriteSheet(
        Texture2D texture,
        int frameWidth = SpriteDefaults.FrameWidth,
        int frameHeight = SpriteDefaults.FrameHeight,
        float animationSpeed = SpriteDefaults.AnimationSpeed
    )
    {
        _texture = texture;
        _frameWidth = frameWidth;
        _frameHeight = frameHeight;
        _frameTime = animationSpeed;

        int columns = texture.Width / frameWidth;
        int rows = texture.Height / frameHeight;
        int totalFrames = columns * rows;

        _frames = new Rectangle[totalFrames];
        int frameIndex = 0;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                _frames[frameIndex] = new Rectangle(
                    col * frameWidth,
                    row * frameHeight,
                    frameWidth,
                    frameHeight
                );
                frameIndex++;
            }
        }
    }

    public void SetTexture(Texture2D texture)
    {
        if (texture == null)
            return;

        _texture = texture;

        int columns = texture.Width / _frameWidth;
        int rows = texture.Height / _frameHeight;
        int totalFrames = columns * rows;

        _frames = new Rectangle[totalFrames];
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

    public void SetSequenceAnimation(int introEnd, int loopStart, int loopEnd)
    {
        _mode = AnimationMode.SequenceWithExit;
        _phase = AnimationPhase.Intro;
        _introEnd = introEnd;
        _loopStart = loopStart;
        _loopEnd = loopEnd;
    }

    public void ResetToLooping()
    {
        _mode = AnimationMode.Looping;
        _phase = AnimationPhase.Intro;
        _introEnd = -1;
        _loopStart = -1;
        _loopEnd = -1;
    }

    public void ResetAnimation()
    {
        _currentFrame = 0;
        _timeElapsed = 0f;
        if (_mode == AnimationMode.SequenceWithExit)
        {
            _phase = AnimationPhase.Intro;
        }
    }

    public void StartExitSequence()
    {
        if (_mode == AnimationMode.SequenceWithExit)
        {
            _phase = AnimationPhase.Exit;
            _currentFrame = _introEnd;
            _timeElapsed = 0f;
        }
    }

    public bool IsExitComplete()
    {
        return _mode == AnimationMode.SequenceWithExit && _phase == AnimationPhase.Exit && _currentFrame == 0;
    }

    public void Update(GameTime gameTime)
    {
        if (_frames == null || _frames.Length == 0)
            return;

        _timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_timeElapsed >= _frameTime)
        {
            _timeElapsed = 0f;

            switch (_mode)
            {
                case AnimationMode.Looping:
                    _currentFrame = (_currentFrame + 1) % _frames.Length;
                    break;

                case AnimationMode.SequenceWithExit:
                    UpdateSequenceAnimation();
                    break;
            }
        }
    }

    private void UpdateSequenceAnimation()
    {
        switch (_phase)
        {
            case AnimationPhase.Intro:
                if (_currentFrame < _introEnd)
                {
                    _currentFrame++;
                }
                else
                {
                    _phase = AnimationPhase.Loop;
                    _currentFrame = _loopStart;
                }
                break;

            case AnimationPhase.Loop:
                _currentFrame++;
                if (_currentFrame > _loopEnd)
                {
                    _currentFrame = _loopStart;
                }
                break;

            case AnimationPhase.Exit:
                if (_currentFrame > 0)
                {
                    _currentFrame--;
                }
                break;
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
            new Vector2((float)_frameWidth / 2, (float)_frameHeight / 2), // Center sprite
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

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Note: We don't dispose _texture here as it's managed by ContentManager
                // and owned by the caller. Only dispose resources we own.
                _frames = null;
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
