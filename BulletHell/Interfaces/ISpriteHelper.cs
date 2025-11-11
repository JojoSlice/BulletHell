using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Interfaces;

public interface ISpriteHelper
{
    int Width { get; }
    int Height { get; }
    void LoadSpriteSheet(Texture2D texture, int frameWidth, int frameHeight, float animationSpeed);
    void Update(GameTime gameTime);
    void Draw(SpriteBatch spriteBatch, Vector2 position, float rotation, float scale);
}
