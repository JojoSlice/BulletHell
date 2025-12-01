using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Helpers;

public static class TextureHelper
{
    private static Texture2D? _whitePixel;

    public static Texture2D WhitePixel(GraphicsDevice graphicsDevice)
    {
        if (_whitePixel == null)
        {
            _whitePixel = new Texture2D(graphicsDevice, 1, 1);
            _whitePixel.SetData(new[] { Color.White });
        }
        return _whitePixel;
    }
}