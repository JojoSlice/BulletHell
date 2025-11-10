using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Helpers;

public class SpriteHelper
{
    private Texture2D texture;
    private Rectangle[] frames;
    private int currentFrame;
    private float frameTime;
    private float timeElapsed;
    private int frameWidth;
    private int frameHeight;

    public int Width => frameWidth;
    public int Height => frameHeight;

    public SpriteHelper()
    {
        currentFrame = 0;
        frameTime = 0.1f; // 100ms per frame som standard
        timeElapsed = 0f;
    }

    public void LoadSpriteSheet(
        Texture2D texture,
        int frameWidth = 32,
        int frameHeight = 32,
        float animationSpeed = 0.1f
    )
    {
        this.texture = texture;
        this.frameWidth = frameWidth;
        this.frameHeight = frameHeight;
        this.frameTime = animationSpeed;

        int columns = texture.Width / frameWidth;
        int rows = texture.Height / frameHeight;
        int totalFrames = columns * rows;

        frames = new Rectangle[totalFrames];
        int frameIndex = 0;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                frames[frameIndex] = new Rectangle(
                    col * frameWidth,
                    row * frameHeight,
                    frameWidth,
                    frameHeight
                );
                frameIndex++;
            }
        }
    }

    public void Update(GameTime gameTime)
    {
        if (frames == null || frames.Length == 0)
            return;

        timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (timeElapsed >= frameTime)
        {
            timeElapsed = 0f;
            currentFrame = (currentFrame + 1) % frames.Length;
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
        if (texture == null || frames == null || frames.Length == 0)
            return;

        spriteBatch.Draw(
            texture,
            position,
            frames[currentFrame],
            color ?? Color.White,
            rotation,
            new Vector2(frameWidth / 2, frameHeight / 2), // Centrera sprite
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
