using BulletHell.Inputs;
using Microsoft.Xna.Framework.Input;

namespace BulletHell.test.Helpers;

public class KeyboardTextInputHandlerTests
{
    private readonly KeyboardTextInputHandler _handler = new();

    [Fact]
    public void ProcessInput_WithNoKeysPressed_ShouldReturnUnchangedText()
    {
        // Arrange
        var currentKeyState = new KeyboardState();
        var previousKeyState = new KeyboardState();
        var currentText = "hello";

        // Act
        var result = _handler.ProcessInput(currentKeyState, previousKeyState, currentText);

        // Assert
        Assert.Equal("hello", result.Text);
        Assert.False(result.ShouldUnfocus);
        Assert.False(result.InputProcessed);
    }

    [Theory]
    [InlineData(Keys.A, "a")]
    [InlineData(Keys.B, "b")]
    [InlineData(Keys.Z, "z")]
    [InlineData(Keys.M, "m")]
    public void ProcessInput_WithLetterKey_ShouldAddLowercaseLetter(Keys key, string expected)
    {
        // Arrange
        var currentKeyState = new KeyboardState(key);
        var previousKeyState = new KeyboardState();
        var currentText = "";

        // Act
        var result = _handler.ProcessInput(currentKeyState, previousKeyState, currentText);

        // Assert
        Assert.Equal(expected, result.Text);
        Assert.False(result.ShouldUnfocus);
        Assert.True(result.InputProcessed);
    }

    [Theory]
    [InlineData(Keys.D0, "0")]
    [InlineData(Keys.D1, "1")]
    [InlineData(Keys.D5, "5")]
    [InlineData(Keys.D9, "9")]
    public void ProcessInput_WithDigitKey_ShouldAddDigit(Keys key, string expected)
    {
        // Arrange
        var currentKeyState = new KeyboardState(key);
        var previousKeyState = new KeyboardState();
        var currentText = "";

        // Act
        var result = _handler.ProcessInput(currentKeyState, previousKeyState, currentText);

        // Assert
        Assert.Equal(expected, result.Text);
        Assert.False(result.ShouldUnfocus);
        Assert.True(result.InputProcessed);
    }

    [Theory]
    [InlineData(Keys.NumPad0, "0")]
    [InlineData(Keys.NumPad1, "1")]
    [InlineData(Keys.NumPad5, "5")]
    [InlineData(Keys.NumPad9, "9")]
    public void ProcessInput_WithNumPadKey_ShouldAddDigit(Keys key, string expected)
    {
        // Arrange
        var currentKeyState = new KeyboardState(key);
        var previousKeyState = new KeyboardState();
        var currentText = "";

        // Act
        var result = _handler.ProcessInput(currentKeyState, previousKeyState, currentText);

        // Assert
        Assert.Equal(expected, result.Text);
        Assert.False(result.ShouldUnfocus);
        Assert.True(result.InputProcessed);
    }

    [Fact]
    public void ProcessInput_WithSpaceKey_ShouldAddSpace()
    {
        // Arrange
        var currentKeyState = new KeyboardState(Keys.Space);
        var previousKeyState = new KeyboardState();
        var currentText = "hello";

        // Act
        var result = _handler.ProcessInput(currentKeyState, previousKeyState, currentText);

        // Assert
        Assert.Equal("hello ", result.Text);
        Assert.False(result.ShouldUnfocus);
        Assert.True(result.InputProcessed);
    }

    [Fact]
    public void ProcessInput_WithBackspace_ShouldRemoveLastCharacter()
    {
        // Arrange
        var currentKeyState = new KeyboardState(Keys.Back);
        var previousKeyState = new KeyboardState();
        var currentText = "hello";

        // Act
        var result = _handler.ProcessInput(currentKeyState, previousKeyState, currentText);

        // Assert
        Assert.Equal("hell", result.Text);
        Assert.False(result.ShouldUnfocus);
        Assert.True(result.InputProcessed);
    }

    [Fact]
    public void ProcessInput_WithBackspaceOnEmptyString_ShouldReturnEmptyString()
    {
        // Arrange
        var currentKeyState = new KeyboardState(Keys.Back);
        var previousKeyState = new KeyboardState();
        var currentText = "";

        // Act
        var result = _handler.ProcessInput(currentKeyState, previousKeyState, currentText);

        // Assert
        Assert.Equal("", result.Text);
        Assert.False(result.ShouldUnfocus);
        Assert.True(result.InputProcessed);
    }

    [Fact]
    public void ProcessInput_WithEscapeKey_ShouldSetShouldUnfocusToTrue()
    {
        // Arrange
        var currentKeyState = new KeyboardState(Keys.Escape);
        var previousKeyState = new KeyboardState();
        var currentText = "hello";

        // Act
        var result = _handler.ProcessInput(currentKeyState, previousKeyState, currentText);

        // Assert
        Assert.Equal("hello", result.Text); // Text should not change
        Assert.True(result.ShouldUnfocus);
        Assert.True(result.InputProcessed);
    }

    [Fact]
    public void ProcessInput_WithEnterKey_ShouldSetShouldUnfocusToTrue()
    {
        // Arrange
        var currentKeyState = new KeyboardState(Keys.Enter);
        var previousKeyState = new KeyboardState();
        var currentText = "hello";

        // Act
        var result = _handler.ProcessInput(currentKeyState, previousKeyState, currentText);

        // Assert
        Assert.Equal("hello", result.Text);
        Assert.True(result.ShouldUnfocus);
        Assert.True(result.InputProcessed);
    }

    [Fact]
    public void ProcessInput_WithKeyAlreadyPressed_ShouldNotProcessInput()
    {
        // Arrange
        var currentKeyState = new KeyboardState(Keys.A);
        var previousKeyState = new KeyboardState(Keys.A);
        var currentText = "hello";

        // Act
        var result = _handler.ProcessInput(currentKeyState, previousKeyState, currentText);

        // Assert
        Assert.Equal("hello", result.Text);
        Assert.False(result.ShouldUnfocus);
        Assert.False(result.InputProcessed);
    }

    [Fact]
    public void ProcessInput_WithMultipleNewKeys_ShouldProcessAllKeys()
    {
        // Arrange
        var currentKeyState = new KeyboardState(Keys.A, Keys.B, Keys.C);
        var previousKeyState = new KeyboardState();
        var currentText = "";

        // Act
        var result = _handler.ProcessInput(currentKeyState, previousKeyState, currentText);

        // Assert
        Assert.Equal(3, result.Text.Length);
        Assert.Contains('a', result.Text);
        Assert.Contains('b', result.Text);
        Assert.Contains('c', result.Text);
        Assert.False(result.ShouldUnfocus);
        Assert.True(result.InputProcessed);
    }

    [Fact]
    public void ProcessInput_WithMixOfNewAndHeldKeys_ShouldOnlyProcessNewKeys()
    {
        // Arrange
        var currentKeyState = new KeyboardState(Keys.A, Keys.B, Keys.C);
        var previousKeyState = new KeyboardState(Keys.A); // A was already held
        var currentText = "";

        // Act
        var result = _handler.ProcessInput(currentKeyState, previousKeyState, currentText);

        // Assert
        Assert.Equal(2, result.Text.Length);
        Assert.DoesNotContain('a', result.Text);
        Assert.Contains('b', result.Text);
        Assert.Contains('c', result.Text);
        Assert.False(result.ShouldUnfocus);
        Assert.True(result.InputProcessed);
    }

    [Fact]
    public void ProcessInput_WithSpecialKeys_ShouldNotAddToText()
    {
        // Arrange
        var currentKeyState = new KeyboardState(Keys.Tab, Keys.LeftShift, Keys.RightControl);
        var previousKeyState = new KeyboardState();
        var currentText = "";

        // Act
        var result = _handler.ProcessInput(currentKeyState, previousKeyState, currentText);

        // Assert
        Assert.Equal("", result.Text);
        Assert.False(result.ShouldUnfocus);
        Assert.True(result.InputProcessed);
    }

    [Fact]
    public void ProcessInput_WithEscapeAndOtherKeys_ShouldUnfocusImmediately()
    {
        // Arrange
        var currentKeyState = new KeyboardState(Keys.Escape, Keys.A, Keys.B);
        var previousKeyState = new KeyboardState();
        var currentText = "test";

        // Act
        var result = _handler.ProcessInput(currentKeyState, previousKeyState, currentText);

        // Assert
        Assert.True(result.ShouldUnfocus);
        Assert.True(result.InputProcessed);
    }

    [Fact]
    public void ProcessInput_WithMixedLettersAndNumbers_ShouldAddAll()
    {
        // Arrange
        var currentKeyState = new KeyboardState(Keys.A, Keys.D1, Keys.B, Keys.D2);
        var previousKeyState = new KeyboardState();
        var currentText = "";

        // Act
        var result = _handler.ProcessInput(currentKeyState, previousKeyState, currentText);

        // Assert
        Assert.Equal(4, result.Text.Length);
        Assert.Contains('a', result.Text);
        Assert.Contains('1', result.Text);
        Assert.Contains('b', result.Text);
        Assert.Contains('2', result.Text);
        Assert.True(result.InputProcessed);
    }

    [Fact]
    public void ProcessInput_WithOnlyModifierKeys_ShouldNotChangeText()
    {
        // Arrange
        var currentKeyState = new KeyboardState(
            Keys.LeftShift,
            Keys.RightShift,
            Keys.LeftControl,
            Keys.RightControl,
            Keys.LeftAlt,
            Keys.RightAlt
        );
        var previousKeyState = new KeyboardState();
        var currentText = "test";

        // Act
        var result = _handler.ProcessInput(currentKeyState, previousKeyState, currentText);

        // Assert
        Assert.Equal("test", result.Text);
        Assert.False(result.ShouldUnfocus);
        Assert.True(result.InputProcessed);
    }
}
