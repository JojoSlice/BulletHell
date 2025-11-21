using BulletHell.Constants;
using BulletHell.Interfaces;
using BulletHell.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NSubstitute;
using Xunit;

namespace BulletHell.test.Scenes;

public class MenuSceneTests
{
    [Fact]
    public void Constructor_ShouldInitializeInLoginMode()
    {
        // Arrange & Act
        var menuScene = CreateMenuScene();

        // Assert
        Assert.Equal(RegistrationMode.Login, menuScene.CurrentMode);
    }

    [Fact]
    public void ToggleMode_ShouldSwitchFromLoginToRegister()
    {
        // Arrange
        var menuScene = CreateMenuScene();
        Assert.Equal(RegistrationMode.Login, menuScene.CurrentMode);

        // Act
        menuScene.ToggleMode();

        // Assert
        Assert.Equal(RegistrationMode.Register, menuScene.CurrentMode);
    }

    [Fact]
    public void ToggleMode_ShouldSwitchFromRegisterToLogin()
    {
        // Arrange
        var menuScene = CreateMenuScene();
        menuScene.ToggleMode(); // Nu i Register-mode

        // Act
        menuScene.ToggleMode();

        // Assert
        Assert.Equal(RegistrationMode.Login, menuScene.CurrentMode);
    }

    [Fact]
    public void GetActionButtonText_ShouldReturnLogIn_WhenInLoginMode()
    {
        // Arrange
        var menuScene = CreateMenuScene();

        // Act
        var buttonText = menuScene.GetActionButtonText();

        // Assert
        Assert.Equal("Log In", buttonText);
    }

    [Fact]
    public void GetActionButtonText_ShouldReturnRegister_WhenInRegisterMode()
    {
        // Arrange
        var menuScene = CreateMenuScene();
        menuScene.ToggleMode();

        // Act
        var buttonText = menuScene.GetActionButtonText();

        // Assert
        Assert.Equal("Register", buttonText);
    }

    private MenuScene CreateMenuScene()
    {
        var game = Substitute.For<Game1>();
        Texture2D? texture = null; // Texture2D cannot be mocked, but we don't need it for these tests
        SpriteFont? font = null; // SpriteFont cannot be mocked either, but we don't need it for these tests
        var inputProvider = Substitute.For<IMenuInputProvider>();
        var textInputHandler = Substitute.For<ITextInputHandler>();
        var apiClient = Substitute.For<IUserApiClient>();
        var passwordHasher = Substitute.For<IPasswordHasher>();

        // Use the internal test constructor that doesn't access Game1.Content or GraphicsDevice
        return new MenuScene(
            game,
            texture!,
            font!,
            800, // screenWidth
            600, // screenHeight
            inputProvider,
            textInputHandler,
            apiClient,
            passwordHasher
        );
    }
}
