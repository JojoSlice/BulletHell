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

    [Fact]
    public void HUD_ShouldStoreAndReturnAmmo()
    {
        // Arrange
        var expected = 30;

        // Act
        var hud = new HUD();
        hud.Ammo = expected;
        var actual = hud.Ammo;

        // Assert
        Assert.Equal(expected, actual);

        // Output
        _output.WriteLine($"Expected: {expected}");
        _output.WriteLine($"Actual:   {actual}");
    }

    [Fact]
    public void HUD_ShouldUpdateMessage_WhenAmmoChanges()
    {
        // Arrange
        var expected = "Ammo: 30";
        var hud = new HUD();

        // Act
        hud.Ammo = 30;
        var actual = hud.Message;

        // Assert
        Assert.Equal(expected, actual);

        // Output
        _output.WriteLine($"Expected: {expected}");
        _output.WriteLine($"Actual:   {actual}");
    }
}