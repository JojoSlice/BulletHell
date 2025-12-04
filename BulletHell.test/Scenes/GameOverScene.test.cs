using BulletHell.Constants;
using BulletHell.Interfaces;
using BulletHell.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moq;

namespace BulletHell.test.Scenes;

public class GameOverSceneTests
{
    private GameOverScene CreateGameOverScene(
        Game1? game = null,
        IMenuInputProvider? inputProvider = null,
        IMenuNavigator? navigator = null
    )
    {
        game ??= new Mock<Game1>().Object;
        inputProvider ??= new Mock<IMenuInputProvider>().Object;

        Texture2D? texture = null; // Can't mock sealed class
        SpriteFont? font = null; // Can't mock sealed class

        return new GameOverScene(
            game,
            texture!,
            font!,
            800, // screenWidth
            600, // screenHeight
            inputProvider,
            navigator
        );
    }

    [Fact]
    public void Update_AfterDispose_ShouldThrowObjectDisposedException()
    {
        // Arrange
        GameOverScene scene;
        using (scene = CreateGameOverScene())
        {
            scene.OnEnter();
        }

        // Act & Assert
        Assert.Throws<ObjectDisposedException>(() => scene.Update(new GameTime()));
    }

    [Fact]
    public void Dispose_AfterOnEnter_ShouldNotThrowException()
    {
        // Arrange
        using (var scene = CreateGameOverScene())
        {
            scene.OnEnter();
            // The using statement will automatically call Dispose()
            // If Dispose() throws an exception, the test will fail
        }
    }

    [Fact]
    public void OnExit_ShouldClearNavigator()
    {
        // Arrange
        var mockNavigator = new Mock<IMenuNavigator>();
        using (var scene = CreateGameOverScene(navigator: mockNavigator.Object))
        {
            scene.OnEnter();

            // Act
            scene.OnExit();

            // Assert
            mockNavigator.Verify(m => m.Clear(), Times.Once);
        }
    }

    [Fact]
    public void Update_ShouldCallGetMouseStateOnInputProvider()
    {
        // Arrange
        var mockInputProvider = new Mock<IMenuInputProvider>();
        mockInputProvider.Setup(m => m.GetMouseState()).Returns(new MouseState());
        mockInputProvider.Setup(m => m.GetKeyboardState()).Returns(new KeyboardState());

        var scene = CreateGameOverScene(inputProvider: mockInputProvider.Object);

        using (scene)
        {
            scene.OnEnter();

            // Act
            scene.Update(new GameTime());

            // Assert
            mockInputProvider.Verify(m => m.GetMouseState(), Times.Once);
        }
    }

    [Fact]
    public void Update_ShouldCallGetKeyboardStateOnInputProvider()
    {
        // Arrange
        var mockInputProvider = new Mock<IMenuInputProvider>();
        mockInputProvider.Setup(m => m.GetMouseState()).Returns(new MouseState());
        mockInputProvider.Setup(m => m.GetKeyboardState()).Returns(new KeyboardState());

        var scene = CreateGameOverScene(inputProvider: mockInputProvider.Object);

        using (scene)
        {
            scene.OnEnter();

            // Act
            scene.Update(new GameTime());

            // Assert
            mockInputProvider.Verify(m => m.GetKeyboardState(), Times.Once);
        }
    }

    [Fact]
    public void Update_ShouldPassKeyboardStateToNavigator()
    {
        // Arrange
        var mockInputProvider = new Mock<IMenuInputProvider>();
        var mockNavigator = new Mock<IMenuNavigator>();
        var keyboardState = new KeyboardState();

        mockInputProvider.Setup(m => m.GetMouseState()).Returns(new MouseState());
        mockInputProvider.Setup(m => m.GetKeyboardState()).Returns(keyboardState);

        var scene = CreateGameOverScene(
            inputProvider: mockInputProvider.Object,
            navigator: mockNavigator.Object
        );

        using (scene)
        {
            scene.OnEnter();

            // Act
            scene.Update(new GameTime());

            // Assert
            mockNavigator.Verify(m => m.Update(keyboardState), Times.Once);
        }
    }

    [Fact]
    public void OnEnter_WhenNavigatorIsNull_ShouldCreateNavigator()
    {
        // Arrange
        var mockInputProvider = new Mock<IMenuInputProvider>();
        mockInputProvider.Setup(m => m.GetMouseState()).Returns(new MouseState());
        mockInputProvider.Setup(m => m.GetKeyboardState()).Returns(new KeyboardState());

        var scene = CreateGameOverScene(inputProvider: mockInputProvider.Object, navigator: null);

        using (scene)
        {
            // Act
            scene.OnEnter();

            // Update should work without throwing (navigator created)
            var exception = Record.Exception(() => scene.Update(new GameTime()));

            // Assert
            Assert.Null(exception);
        }
    }
}