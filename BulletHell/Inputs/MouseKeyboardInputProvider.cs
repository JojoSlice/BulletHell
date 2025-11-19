using BulletHell.Interfaces;
using Microsoft.Xna.Framework.Input;

namespace BulletHell.Inputs;

/// <summary>
/// Provides mouse and keyboard input state from MonoGame's input system
/// </summary>
public class MouseKeyboardInputProvider : IMenuInputProvider
{
    public MouseState GetMouseState()
    {
        return Mouse.GetState();
    }

    public KeyboardState GetKeyboardState()
    {
        return Keyboard.GetState();
    }
}
