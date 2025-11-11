using BulletHell.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BulletHell.Inputs;

public class KeyboardInputProvider : IInputProvider
{
    public Vector2 GetDirection()
    {
        KeyboardState keyState = Keyboard.GetState();
        Vector2 direction = Vector2.Zero;

        if (keyState.IsKeyDown(Keys.W) || keyState.IsKeyDown(Keys.Up))
            direction.Y -= 1;
        if (keyState.IsKeyDown(Keys.S) || keyState.IsKeyDown(Keys.Down))
            direction.Y += 1;
        if (keyState.IsKeyDown(Keys.A) || keyState.IsKeyDown(Keys.Left))
            direction.X -= 1;
        if (keyState.IsKeyDown(Keys.D) || keyState.IsKeyDown(Keys.Right))
            direction.X += 1;

        return direction;
    }

    public bool IsShootPressed()
    {
        KeyboardState keyState = Keyboard.GetState();
        return keyState.IsKeyDown(Keys.Space);
    }
}
