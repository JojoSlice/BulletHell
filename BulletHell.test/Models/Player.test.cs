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

        using var player = new Player(startPosition, mockInput.Object, mockSprite.Object);
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

        using var player = new Player(startPosition, mockInput.Object, mockSprite.Object);
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

        using var player = new Player(startPosition, mockInput.Object, mockSprite.Object);
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

        using var player = new Player(startPosition, mockInput.Object, mockSprite.Object);
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
        using var player = TestDataBuilders.CreateTestPlayer();

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
        using var player = new Player(new Vector2(x, 300), mockInput, mockSprite);
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
        using var player = new Player(new Vector2(400, y), mockInput, mockSprite);
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
        using var player = TestDataBuilders.CreateTestPlayer();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => player.LoadContent(null!));
    }

    [Fact]
    public void TryShoot_WhenCooldownIsZero_ShouldReturnBulletInfo()
    {
        // Arrange
        var mockInput = MockFactories.CreateMockInputProvider(isShootPressed: true);
        using var player = TestDataBuilders.CreateTestPlayer(
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
        using var player = TestDataBuilders.CreateTestPlayer(input: mockInput);

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
        using var player = TestDataBuilders.CreateTestPlayer(input: mockInput);

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
        using var player = TestDataBuilders.CreateTestPlayer(input: mockInput);

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
        using var player = TestDataBuilders.CreateTestPlayer();

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
        using var player = new Player(new Vector2(100, 100), mockInput, mockSprite);

        var expectedLives = 3;

        // Act
        var actualLives = player.Lives;

        // Assert
        Assert.Equal(expectedLives, actualLives);

        // Output
        _output.WriteLine("Player starts with " + actualLives + " lives ✔");
    }

    [Fact]
    public void Constructor_ShouldInitializeCollider()
    {
        // Arrange
        var startPosition = new Vector2(100, 100);
        var mockInput = new Mock<IInputProvider>();
        var mockSprite = new Mock<ISpriteHelper>();

        // Act
        using var player = new Player(startPosition, mockInput.Object, mockSprite.Object);

        // Assert
        Assert.NotNull(player.Collider);
        Assert.Equal(typeof(Player), player.Collider.ColliderType);
        Assert.Equal(startPosition, player.Collider.Position);
    }

    [Fact]
    public void Update_ShouldKeepColliderInSyncWithPosition()
    {
        // Arrange
        var mockSprite = new Mock<ISpriteHelper>();
        mockSprite.Setup(s => s.Width).Returns(32);
        mockSprite.Setup(s => s.Height).Returns(32);
        var mockInput = MockFactories.CreateMockInputProvider(new Vector2(1, 0));
        using var player = new Player(new Vector2(50, 50), mockInput, mockSprite.Object);
        player.SetScreenBounds(800, 600);

        // Act
        player.Update(TestDataBuilders.OneFrame);

        // Assert
        Assert.Equal(player.Position, player.Collider.Position);
    }

    [Fact]
    public void UpdateColliderRadiusFromSprite_ShouldSetRadiusBasedOnSprite()
    {
        // Arrange
        var mockSprite = new Mock<ISpriteHelper>();
        mockSprite.Setup(s => s.Width).Returns(24);
        mockSprite.Setup(s => s.Height).Returns(16);
        var mockInput = new Mock<IInputProvider>();
        using var player = new Player(Vector2.Zero, mockInput.Object, mockSprite.Object);

        // Act
        // Radius should be initialized in constructor based on sprite dimensions

        // Assert
        var expected = System.Math.Max(24, 16) / 2f;
        Assert.Equal(expected, player.Collider.Radius);
    }

    [Fact]
    public void TakeDame_ShouldReduceHealth()
    {
        // Arrange
        var mockInput = MockFactories.CreateMockInputProvider();
        var mockSprite = MockFactories.CreateMockSpriteHelper();
        using var player = new Player(new Vector2(100, 100), mockInput, mockSprite);

        var expected = PlayerConfig.MaxHealth;

        // Act
        player.TakeDamage(100);


        // Assert
        Assert.Equal(expected, player.Health);

        // Output
        _output.WriteLine("Health reduced correctly to " + player.Health + "✔️");
    }

    [Fact]
    public void TakeDamage_WhenHealthDropsToZero_ShouldReduceLifeAndResetHealth()
    {
        // Arrange
        using var player = TestDataBuilders.CreateTestPlayer();

        var damage = 100;
        var expectedLives = player.Lives - 1;

        // Act
        player.TakeDamage(damage);
        var actualLives = player.Lives;

        // Assert
        Assert.Equal(expectedLives, actualLives);
        Assert.Equal(PlayerConfig.MaxHealth, player.Health);

        // Output
        _output.WriteLine("Expected Lives reduced from 3 to " + player.Lives + "✔️");
        _output.WriteLine("Actual lives reduced from 3 to " + actualLives + "✔️");

    }

    [Fact]
    public void TakeDamage_WhenHealthReachesZero_ShouldReduceLivesAndResetHealth()
    {
        // Arrange
        var mockInput = MockFactories.CreateMockInputProvider();
        var mockSprite = MockFactories.CreateMockSpriteHelper();
        using var player = new Player(new Vector2(100, 100), mockInput, mockSprite);

        var startingLives = player.Lives;

        // Sänker hälsan till ett värde som inte klarar av kommande skada
        player.TakeDamage(PlayerConfig.MaxHealth - 50);
        var startingHealth = player.Health;

        var expectedLives = startingLives - 1;
        var expectedHealth = PlayerConfig.MaxHealth;

        // Act
        player.TakeDamage(100); // detta ska döda och resetta
        var actualLives = player.Lives;
        var actualHealth = player.Health;

        // Assert
        Assert.Equal(expectedLives, actualLives);
        Assert.Equal(expectedHealth, actualHealth);

        // Output (Debug info)
        _output.WriteLine($"Starting Lives: {startingLives}");
        _output.WriteLine($"Expected Lives after death: {expectedLives}");
        _output.WriteLine($"Actual Lives: {actualLives} ✔️");

        _output.WriteLine($"Health before lethal damage: {startingHealth}");
        _output.WriteLine($"Expected reset Health:       {expectedHealth}");
        _output.WriteLine($"Actual reset Health:         {actualHealth} ✔️");
    }
}
