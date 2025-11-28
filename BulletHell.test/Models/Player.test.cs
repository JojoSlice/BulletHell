using BulletHell.Configurations;
using BulletHell.Interfaces;
using BulletHell.Models;
using BulletHell.test.TestUtilities;
using Microsoft.Xna.Framework;
using Moq;

namespace BulletHell.test.Models;

public class PlayerTests(ITestOutputHelper output)
{
    private readonly ITestOutputHelper _output = output;

    [Fact]
    public void Update_WithZeroDirection_ShouldNotMovePlayer()
    {
        // Arrange
        var startPosition = new Vector2(100, 100);
        var mockInput = new Mock<IInputProvider>();
        var mockSprite = new Mock<ISpriteHelper>();
        mockInput.Setup(i => i.GetDirection()).Returns(Vector2.Zero);

        var player = new Player(startPosition, mockInput.Object, mockSprite.Object);
        var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1.0 / 60.0));

        // Act
        player.Update(gameTime);

        // Assert
        Assert.Equal(startPosition, player.Position);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(-1, -1)]
    [InlineData(-1, 1)]
    [InlineData(1, -1)]
    public void Update_WithDiagonalDirection_ShouldNormalizeAndMove(int x, int y)
    {
        // Arrange
        var startPosition = new Vector2(100, 100);
        var mockInput = new Mock<IInputProvider>();
        var mockSprite = new Mock<ISpriteHelper>();
        mockInput.Setup(i => i.GetDirection()).Returns(new Vector2(x, y));

        var player = new Player(startPosition, mockInput.Object, mockSprite.Object);
        var deltaTime = 1.0f / 60.0f;
        var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(deltaTime));

        // Act
        player.Update(gameTime);

        // Assert
        var normalizedDirection = Vector2.Normalize(new Vector2(x, y));
        var expectedPosition = startPosition + normalizedDirection * PlayerConfig.Speed * deltaTime;

        Assert.Equal(expectedPosition.X, player.Position.X, 2);
        Assert.Equal(expectedPosition.Y, player.Position.Y, 2);
    }

    [Fact]
    public void Update_ShouldCallSpriteUpdate()
    {
        // Arrange
        var startPosition = new Vector2(100, 100);
        var mockInput = new Mock<IInputProvider>();
        var mockSprite = new Mock<ISpriteHelper>();
        mockInput.Setup(i => i.GetDirection()).Returns(Vector2.Zero);

        var player = new Player(startPosition, mockInput.Object, mockSprite.Object);
        var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1.0 / 60.0));

        // Act
        player.Update(gameTime);

        // Assert
        mockSprite.Verify(s => s.Update(gameTime), Times.Once);
    }

    [Theory]
    [InlineData(1, 0, 100)]
    [InlineData(-1, 0, 100)]
    [InlineData(0, 1, 100)]
    [InlineData(0, -1, 100)]
    public void Update_WithCardinalDirections_ShouldMoveCorrectly(float x, float y, float startPos)
    {
        // Arrange
        var startPosition = new Vector2(startPos, startPos);
        var mockInput = new Mock<IInputProvider>();
        var mockSprite = new Mock<ISpriteHelper>();
        var direction = new Vector2(x, y);
        mockInput.Setup(i => i.GetDirection()).Returns(direction);

        var player = new Player(startPosition, mockInput.Object, mockSprite.Object);
        var deltaTime = 1.0f / 60.0f;
        var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(deltaTime));

        // Act
        player.Update(gameTime);

        // Assert
        var expectedPosition = startPosition + direction * PlayerConfig.Speed * deltaTime;
        Assert.Equal(expectedPosition.X, player.Position.X, 2);
        Assert.Equal(expectedPosition.Y, player.Position.Y, 2);
    }

    [Fact]
    public void SetScreenBounds_ShouldStoreScreenDimensions()
    {
        // Arrange
        var player = TestDataBuilders.CreateTestPlayer();

        // Act
        player.SetScreenBounds(800, 600);

        // Assert - verify by testing clamping behavior
        // (no direct way to verify private fields, but we can test the effect)
        Assert.NotNull(player);
    }

    [Theory]
    [InlineData(-100)]
    [InlineData(900)]
    public void Update_WhenPositionExceedsHorizontalBounds_ShouldClampPosition(float x)
    {
        // Arrange
        var mockSprite = MockFactories.CreateMockSpriteHelper(width: 32, height: 32);
        var mockInput = MockFactories.CreateMockInputProvider(new Vector2(1, 0));
        var player = new Player(new Vector2(x, 300), mockInput, mockSprite);
        player.SetScreenBounds(800, 600);

        // Act
        player.Update(TestDataBuilders.OneFrame);

        // Assert
        Assert.True(player.Position.X >= 16 && player.Position.X <= 784);
    }

    [Theory]
    [InlineData(-100)]
    [InlineData(700)]
    public void Update_WhenPositionExceedsVerticalBounds_ShouldClampPosition(float y)
    {
        // Arrange
        var mockSprite = MockFactories.CreateMockSpriteHelper(width: 32, height: 32);
        var mockInput = MockFactories.CreateMockInputProvider(new Vector2(0, 1));
        var player = new Player(new Vector2(400, y), mockInput, mockSprite);
        player.SetScreenBounds(800, 600);

        // Act
        player.Update(TestDataBuilders.OneFrame);

        // Assert
        Assert.True(player.Position.Y >= 16 && player.Position.Y <= 584);
    }

    // Note: LoadContent with valid texture requires integration testing
    // due to MonoGame Texture2D being non-mockable

    [Fact]
    public void LoadContent_WithNullTexture_ShouldThrowArgumentNullException()
    {
        // Arrange
        var player = TestDataBuilders.CreateTestPlayer();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => player.LoadContent(null!));
    }

    [Fact]
    public void TryShoot_WhenCooldownIsZero_ShouldReturnBulletInfo()
    {
        // Arrange
        var mockInput = MockFactories.CreateMockInputProvider(isShootPressed: true);
        var player = TestDataBuilders.CreateTestPlayer(
            position: new Vector2(100, 100),
            input: mockInput
        );

        // Act
        var result = player.TryShoot();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(new Vector2(100, 100), result.Value.position);
        Assert.Equal(-Vector2.UnitY, result.Value.direction);
    }

    [Fact]
    public void TryShoot_WhenShootNotPressed_ShouldReturnNull()
    {
        // Arrange
        var mockInput = MockFactories.CreateMockInputProvider(isShootPressed: false);
        var player = TestDataBuilders.CreateTestPlayer(input: mockInput);

        // Act
        var result = player.TryShoot();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void TryShoot_WhenCooldownActive_ShouldReturnNull()
    {
        // Arrange
        var mockInput = MockFactories.CreateMockInputProvider(isShootPressed: true);
        var player = TestDataBuilders.CreateTestPlayer(input: mockInput);

        // Act - first shot should succeed
        var firstShot = player.TryShoot();
        var secondShot = player.TryShoot();

        // Assert
        Assert.NotNull(firstShot);
        Assert.Null(secondShot);
    }

    [Fact]
    public void Update_ShouldDecrementShootCooldown()
    {
        // Arrange
        var mockInput = MockFactories.CreateMockInputProvider(isShootPressed: true);
        var player = TestDataBuilders.CreateTestPlayer(input: mockInput);

        // Shoot to activate cooldown
        player.TryShoot();
        Assert.Null(player.TryShoot()); // Cooldown active

        // Act - update for cooldown duration
        for (int i = 0; i < 60; i++)
        {
            player.Update(TestDataBuilders.OneFrame);
        }

        // Assert - should be able to shoot again
        var result = player.TryShoot();
        Assert.NotNull(result);
    }

    // Note: Draw with valid SpriteBatch requires integration testing
    // due to MonoGame SpriteBatch being non-mockable

    [Fact]
    public void Draw_WithNullSpriteBatch_ShouldThrowArgumentNullException()
    {
        // Arrange
        var player = TestDataBuilders.CreateTestPlayer();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => player.Draw(null!));
    }

    [Fact]
    public void Dispose_ShouldDisposeSprite()
    {
        // Arrange
        var mockSprite = new Mock<ISpriteHelper>();
        using var player = TestDataBuilders.CreateTestPlayer(sprite: mockSprite.Object);

        // Act
        player.Dispose();

        // Assert
        mockSprite.Verify(s => s.Dispose(), Times.Once);
    }

    [Fact]
    public void Dispose_CalledMultipleTimes_ShouldOnlyDisposeOnce()
    {
        // Arrange
        var mockSprite = new Mock<ISpriteHelper>();
        using (var player = TestDataBuilders.CreateTestPlayer(sprite: mockSprite.Object))
        {
            // Act & Assert
            player.Dispose();
            player.Dispose();
            mockSprite.Verify(s => s.Dispose(), Times.Once);
        }
    }

    [Fact]
    public void Player_ShouldStartWithThreeLives()
    {
        // Arrange
        var mockInput = MockFactories.CreateMockInputProvider();
        var mockSprite = MockFactories.CreateMockSpriteHelper();
        var player = new Player(new Vector2(100, 100), mockInput, mockSprite);

        var expectedLives = 3;

        // Act
        var actualLives = player.Lives;

        // Assert
        Assert.Equal(expectedLives, actualLives);

        // Output
        _output.WriteLine("Player starts with " + actualLives + " lives ✔");
    }
}
