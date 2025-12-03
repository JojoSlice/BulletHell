using BulletHell.Interfaces;
using BulletHell.Managers;
using BulletHell.Models;
using BulletHell.test.TestUtilities;
using Microsoft.Xna.Framework;

namespace BulletHell.test.Managers;

public class PlayerBulletManagerTests
{
    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Act
        var bulletManager = new BulletManager<Player>();

        // Assert
        Assert.NotNull(bulletManager);
    }

    // Note: LoadContent with valid texture requires integration testing
    // due to MonoGame Texture2D being non-mockable

    [Fact]
    public void LoadContent_WithNullTexture_ShouldThrowArgumentNullException()
    {
        // Arrange
        var bulletManager = new BulletManager<Player>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => bulletManager.LoadContent(null!));
    }

    /// <summary>
    /// Kommenterar ut pga ett annorlunda test behövs här då dethär testet var byggt på att CreateBullet
    /// började med att kolla ifall _bulletTexture va null, vilket den alltid va när metoden startade.
    /// </summary>
    //[Fact]
    //public void CreateBullet_BeforeLoadContent_ShouldThrowInvalidOperationException()
    //{
    //    // Arrange
    //    var bulletManager = new BulletManager<Player>();

    //    // Act & Assert
    //    var exception = Assert.Throws<InvalidOperationException>(
    //        () => ((IBulletManager)bulletManager).CreateBullet(Vector2.Zero, Vector2.UnitY)
    //    );
    //    Assert.Contains("LoadContent must be called", exception.Message);
    //}

    // Note: Tests below require integration testing due to MonoGame Texture2D being non-mockable
    // - CreateBullet_AfterLoadContent_ShouldNotThrow
    // - CreateBullet_ShouldCreateBulletAtSpecifiedPosition
    // - CreateBullet_MultipleTimes_ShouldCreateMultipleBullets

    [Fact]
    public void Update_WithNullGameTime_ShouldThrowArgumentNullException()
    {
        // Arrange
        var bulletManager = new BulletManager<Player>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => bulletManager.Update(null!, 800, 600));
    }

    [Fact]
    public void Update_WithNoBullets_ShouldNotThrow()
    {
        // Arrange
        var bulletManager = new BulletManager<Player>();
        var gameTime = TestDataBuilders.OneFrame;

        // Act & Assert
        var exception = Record.Exception(() => bulletManager.Update(gameTime, 800, 600));
        Assert.Null(exception);
    }

    [Fact]
    public void Update_CalledMultipleTimes_ShouldNotThrow()
    {
        // Arrange
        var bulletManager = new BulletManager<Player>();
        var gameTime = TestDataBuilders.OneFrame;

        // Act & Assert
        for (int i = 0; i < 100; i++)
        {
            var exception = Record.Exception(() => bulletManager.Update(gameTime, 800, 600));
            Assert.Null(exception);
        }
    }

    // Note: Requires integration testing - Update_ShouldRemoveOutOfBoundsBullets

    [Fact]
    public void Draw_WithNullSpriteBatch_ShouldThrowArgumentNullException()
    {
        // Arrange
        var bulletManager = new BulletManager<Player>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => bulletManager.Draw(null!));
    }

    // Note: Following tests require integration testing due to MonoGame limitations:
    // - Draw_WithNoBullets_ShouldNotThrow
    // - Draw_WithActiveBullets_ShouldNotThrow
    // - ObjectPool_ShouldReuseBullets
    // - CreateBullet_ManyBullets_ShouldHandleLargeNumbers
    // - Dispose_ShouldClearAllBullets

    [Fact]
    public void Dispose_CalledMultipleTimes_ShouldNotThrow()
    {
        // Arrange
        var bulletManager = new BulletManager<Player>();

        // Act & Assert
        bulletManager.Dispose();
        var exception = Record.Exception(() => bulletManager.Dispose());
        Assert.Null(exception);
    }

    // Note: Requires integration testing - Update_ShouldUpdateAllActiveBullets
}
