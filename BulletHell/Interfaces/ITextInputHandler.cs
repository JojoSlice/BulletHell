using Microsoft.Xna.Framework.Input;

namespace BulletHell.Interfaces;

/// <summary>
/// Interface for handling text input from keyboard
/// </summary>
public interface ITextInputHandler
{
    /// <summary>
    /// Processes keyboard input and returns the text modification result
    /// </summary>
    /// <param name="currentKeyState">Current keyboard state</param>
    /// <param name="previousKeyState">Previous keyboard state</param>
    /// <param name="currentText">Current text value</param>
    /// <returns>TextInputResult containing modified text and control commands</returns>
    TextInputResult ProcessInput(KeyboardState currentKeyState, KeyboardState previousKeyState, string currentText);
}

/// <summary>
/// Result of text input processing
/// </summary>
public record TextInputResult
{
    /// <summary>
    /// The updated text after processing input
    /// </summary>
    public string Text { get; init; } = "";

    /// <summary>
    /// Whether the input field should be unfocused
    /// </summary>
    public bool ShouldUnfocus { get; init; }

    /// <summary>
    /// Whether any input was processed
    /// </summary>
    public bool InputProcessed { get; init; }
}
