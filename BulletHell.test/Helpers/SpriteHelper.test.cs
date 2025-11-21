using BulletHell.Helpers;

namespace BulletHell.test.Helpers;

/// <summary>
/// SpriteHelper tests require integration testing due to MonoGame limitations.
/// Texture2D and SpriteBatch are sealed classes that cannot be mocked with NSubstitute.
///
/// The following SpriteHelper functionality should be tested in integration tests:
/// - Constructor initialization
/// - LoadSpriteSheet() frame calculation for various texture sizes
/// - LoadSpriteSheet() with different frame dimensions
/// - Width and Height properties returning correct frame dimensions
/// - Update() animation frame advancement over time
/// - Update() frame looping (wraps back to 0)
/// - Update() respecting animation speed
/// - Update() with null/empty frames (should not crash)
/// - Draw() rendering current frame at correct position
/// - Draw() with color parameter
/// - Draw() with rotation and scale
/// - Draw() centering sprite correctly
/// - Draw() overload with default white color
/// - Dispose() clearing frames
/// - Dispose() not disposing shared texture
/// - Edge cases: single frame sprite, zero/negative animation speed
///
/// Total tests needed: ~12-15 integration tests
/// </summary>
public class SpriteHelperTests
{
    // All SpriteHelper tests require integration testing with real MonoGame objects
}
