using System;
using BulletHell.Interfaces;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace BulletHell.Inputs;

/// <summary>
/// Handles keyboard input for text fields
/// </summary>
public class KeyboardTextInputHandler : ITextInputHandler
{
    public TextInputResult ProcessInput(
        KeyboardState currentKeyState,
        KeyboardState previousKeyState,
        string currentText
    )
    {
        string newText = currentText;
        bool shouldUnfocus = false;
        bool inputProcessed = false;

        Keys[] pressedKeys = currentKeyState.GetPressedKeys();

        foreach (Keys key in pressedKeys.Where(key => previousKeyState.IsKeyUp(key)))
        {
            inputProcessed = true;

            if (key == Keys.Escape || key == Keys.Enter)
            {
                shouldUnfocus = true;
                break;
            }

            newText = ProcessTextKey(key, newText);
        }

        return new TextInputResult
        {
            Text = newText,
            ShouldUnfocus = shouldUnfocus,
            InputProcessed = inputProcessed,
        };
    }

    private string ProcessTextKey(Keys key, string currentText)
    {
        if (key == Keys.Back && currentText.Length > 0)
        {
            return currentText[..^1];
        }

        if (key == Keys.Space)
        {
            return currentText + " ";
        }

        string keyString = key.ToString();

        // Handle letter keys (A-Z)
        if (keyString.Length == 1)
        {
            char c = keyString[0];
            if (char.IsLetter(c))
            {
                // Check if Shift or CapsLock is pressed for uppercase
                bool shiftPressed =
                    Keyboard.GetState().IsKeyDown(Keys.LeftShift)
                    || Keyboard.GetState().IsKeyDown(Keys.RightShift);

                // Check CapsLock state (Windows only, defaults to false on other platforms)
                bool capsLockOn = false;
                try
                {
                    if (OperatingSystem.IsWindows())
                    {
                        capsLockOn = Console.CapsLock;
                    }
                }
                catch (PlatformNotSupportedException)
                {
                    // CapsLock not supported on this platform, default to false
                }
                catch (InvalidOperationException)
                {
                    // Console I/O error, default to false
                }

                // XOR: uppercase if either shift OR capslock is on, but not both
                bool shouldBeUppercase = shiftPressed ^ capsLockOn;

                return currentText + (shouldBeUppercase ? c : char.ToLower(c));
            }
        }

        // Handle digit keys (D0-D9)
        if (keyString.Length == 2 && keyString[0] == 'D' && char.IsDigit(keyString[1]))
        {
            // Check for shift to get special characters
            bool shiftPressed =
                Keyboard.GetState().IsKeyDown(Keys.LeftShift)
                || Keyboard.GetState().IsKeyDown(Keys.RightShift);

            if (shiftPressed)
            {
                // Map shifted number keys to special characters
                return currentText
                    + keyString[1] switch
                    {
                        '1' => '!',
                        '2' => '@',
                        '3' => '#',
                        '4' => '$',
                        '5' => '%',
                        '6' => '^',
                        '7' => '&',
                        '8' => '*',
                        '9' => '(',
                        '0' => ')',
                        _ => keyString[1],
                    };
            }

            return currentText + keyString[1];
        }

        // Handle NumPad keys (NumPad0-NumPad9)
        if (keyString.StartsWith("NumPad") && keyString.Length > 6)
        {
            return currentText + keyString[^1];
        }

        // Handle common punctuation keys
        return currentText
            + key switch
            {
                Keys.OemPeriod => '.',
                Keys.OemComma => ',',
                Keys.OemQuestion => '/',
                Keys.OemSemicolon => ';',
                Keys.OemQuotes => '\'',
                Keys.OemOpenBrackets => '[',
                Keys.OemCloseBrackets => ']',
                Keys.OemPipe => '\\',
                Keys.OemMinus => '-',
                Keys.OemPlus => '=',
                _ => "",
            };
    }
}
