using BulletHell.Constants;
using BulletHell.Interfaces;
using BulletHell.Models;
using BulletHell.Scenes;
using Microsoft.Xna.Framework.Graphics;
using NSubstitute;

namespace BulletHell.test.Scenes;

public class MenuSceneTests
{
    [Fact]
    public void Constructor_ShouldInitializeInLoginMode()
    {
        // Arrange & Act
        using var menuScene = CreateMenuScene();

        // Assert
        Assert.Equal(RegistrationMode.Login, menuScene.CurrentMode);
    }

    [Fact]
    public void ToggleMode_ShouldSwitchFromLoginToRegister()
    {
        // Arrange
        using var menuScene = CreateMenuScene();
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
        using var menuScene = CreateMenuScene();
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
        using var menuScene = CreateMenuScene();

        // Act
        var buttonText = menuScene.GetActionButtonText();

        // Assert
        Assert.Equal("Log In", buttonText);
    }

    [Fact]
    public void GetActionButtonText_ShouldReturnRegister_WhenInRegisterMode()
    {
        // Arrange
        using var menuScene = CreateMenuScene();
        menuScene.ToggleMode();

        // Act
        var buttonText = menuScene.GetActionButtonText();

        // Assert
        Assert.Equal("Register", buttonText);
    }

    [Fact]
    public async Task RegisterUser_ShouldCallApiWithHashedPassword()
    {
        // Arrange
        var apiClient = Substitute.For<IApiClient>();
        var passwordHasher = Substitute.For<IPasswordHasher>();
        using var menuScene = CreateMenuSceneWithMocks(apiClient, passwordHasher);

        var username = "newuser";
        var password = "SecurePass123";
        var hashedPassword = "$2a$12$hashedpassword";

        passwordHasher.HashPassword(password).Returns(hashedPassword);
        apiClient
            .RegisterUserAsync(username, hashedPassword)
            .Returns(
                new RegistrationResult
                {
                    Success = true,
                    UserId = 1,
                    Message = "Success",
                }
            );

        // Act
        var result = await menuScene.RegisterUserAsync(username, password);

        // Assert
        Assert.True(result.Success);
        passwordHasher.Received(1).HashPassword(password);
        await apiClient.Received(1).RegisterUserAsync(username, hashedPassword);
    }

    [Theory]
    [InlineData("", "password", "Användarnamn får inte vara tomt")]
    [InlineData("user", "", "Lösenord får inte vara tomt")]
    [InlineData("ab", "pass", "Användarnamn måste vara minst 3 tecken")]
    [InlineData("user", "123", "Lösenord måste vara minst 6 tecken")]
    public async Task RegisterUser_ShouldValidateInput(
        string username,
        string password,
        string expectedError
    )
    {
        // Arrange
        using var menuScene = CreateMenuScene();

        // Act
        var result = await menuScene.RegisterUserAsync(username, password);

        // Assert
        Assert.False(result.Success);
        Assert.Contains(expectedError, result.Message);
    }

    [Fact]
    public async Task RegisterUser_ShouldReturnFailure_WhenApiCallFails()
    {
        // Arrange
        var apiClient = Substitute.For<IApiClient>();
        var passwordHasher = Substitute.For<IPasswordHasher>();
        using var menuScene = CreateMenuSceneWithMocks(apiClient, passwordHasher);

        passwordHasher.HashPassword(Arg.Any<string>()).Returns("$2a$12$hash");
        apiClient
            .RegisterUserAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(new RegistrationResult { Success = false, Message = "Användarnamn upptaget" });

        // Act
        var result = await menuScene.RegisterUserAsync("testuser", "password123");

        // Assert
        Assert.False(result.Success);
        Assert.Contains("upptaget", result.Message);
    }

    private MenuScene CreateMenuScene()
    {
        var game = Substitute.For<Game1>();
        Texture2D? texture = null; // Texture2D cannot be mocked, but we don't need it for these tests
        SpriteFont? font = null; // SpriteFont cannot be mocked either, but we don't need it for these tests
        var inputProvider = Substitute.For<IMenuInputProvider>();
        var textInputHandler = Substitute.For<ITextInputHandler>();
        var apiClient = Substitute.For<IApiClient>();
        var passwordHasher = Substitute.For<IPasswordHasher>();

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

    private MenuScene CreateMenuSceneWithMocks(
        IApiClient apiClient,
        IPasswordHasher passwordHasher
    )
    {
        var game = Substitute.For<Game1>();
        Texture2D? texture = null;
        SpriteFont? font = null;
        var inputProvider = Substitute.For<IMenuInputProvider>();
        var textInputHandler = Substitute.For<ITextInputHandler>();

        return new MenuScene(
            game,
            texture!,
            font!,
            800,
            600,
            inputProvider,
            textInputHandler,
            apiClient,
            passwordHasher
        );
    }
}