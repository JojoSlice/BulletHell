using BulletHell.UI.Components;

namespace BulletHell.test.UI;

public class HUDtests
{
    [Fact]
    public void HUD_UpdateLives_ShouldStoreCorrectValue()
    {
        // Arrange
        var hud = new HUD();
        var expectedLives = 2;

        // Act
        hud.UpdateLives(expectedLives);
        var actualLives = hud.Lives;

        // Assert
        Assert.Equal(expectedLives, actualLives);
    }

    [Fact]
    public void HUD_UpdateScore_ShouldStoreCorrectValue()
    {
        // Arrange
        var hud = new HUD();
        int expected = 123;

        // Act
        hud.UpdateScore(expected);
        int actual = hud.Score;

        // Assert
        Assert.Equal(expected, actual);
    }
}