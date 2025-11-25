using BulletHell.UI.Components;
using Microsoft.Testing.Platform.Extensions.OutputDevice;

namespace BulletHell.test.UI;

public class HUDTests
{
    private readonly ITestOutputHelper _output;

    public HUDTests(ITestOutputHelper output)
    {
        _output = output;
    }

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

    [Fact]
    public void HUD_ShouldUpdateMessage_WhenHPChanges()
    {
        // Arrange
        var expected = "HP: 50";
        var hud = new HUD();

        // Act
        hud.HP = 50;
        var actual = hud.Message;

        // Assert
        Assert.Equal(expected, actual);

        // Output
        _output.WriteLine($"Expected: {expected}");
        _output.WriteLine($"Actual:   {actual}");
    }
}