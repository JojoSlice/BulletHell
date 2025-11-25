using BulletHell.UI.Components;

namespace BulletHell.test.UI;

public class HUDTests
{
    [Fact]
    public void HUD_ShouldStoreAndReturnMessage()
    {
        // Arrange
        var expected = "HP: 50";

        // Act
        var hud = new HUD();
        hud.Message = expected;
        var actual = hud.Message;

        // Assert
        Assert.Equal(expected, actual);
    }
}