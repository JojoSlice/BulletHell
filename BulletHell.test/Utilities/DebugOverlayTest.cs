using BulletHell.DebugUI;

namespace BulletHell.test.Utilities;

public class DebugOverlayTest
{
    private readonly ITestOutputHelper _output;

    public DebugOverlayTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Toggle_ShouldFlipVisibility()
    {
        // Arrange
        var overlay = new DebugOverlay(font: null, bgTexture: null);

        // Act
        var initial = overlay.IsVisible;
        overlay.Toggle();
        var afterFirst = overlay.IsVisible;
        overlay.Toggle();
        var afterSecond = overlay.IsVisible;

        // Assert
        Assert.NotEqual(initial, afterFirst);
        Assert.Equal(initial, afterSecond);

        // Output
        _output.WriteLine($"Initial: {initial}");
        _output.WriteLine($"After 1st Toggle: {afterFirst}");
        _output.WriteLine($"After 2nd Toggle: {afterSecond}");
        _output.WriteLine("✔ Toggle logic verified.");
    }
}