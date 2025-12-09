using BulletHell.Configurations;
using BulletHell.Interfaces;
using BulletHell.Models;
using Microsoft.Xna.Framework;
using Moq;

namespace BulletHell.test.Interfaces
{
    public class IDamageDealerTests
    {
        private readonly ITestOutputHelper _output;

        public IDamageDealerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        // ------------------------------------------------------------
        // Damage ska alltid vara > 0
        // ------------------------------------------------------------
        [Fact]
        public void IDamageDealer_Damage_ShouldBeGreaterThanZero()
        {
            // Arrange
            var spriteMock = new Mock<ISpriteHelper>();
            IDamageDealer sut = new Bullet<Player>(Vector2.Zero, Vector2.UnitY, spriteMock.Object);

            // Act
            int actualDamage = sut.Damage;

            // Assert
            Assert.True(actualDamage > 0);

            // Output
            _output.WriteLine($"Damage: {actualDamage}");
        }

        // ------------------------------------------------------------
        // PlayerBullet ska använda BulletConfig.Player.Damage
        // ------------------------------------------------------------
        [Fact]
        public void IDamageDealer_PlayerBullet_ShouldUsePlayerConfigDamage()
        {
            // Arrange
            var spriteMock = new Mock<ISpriteHelper>();
            var bullet = new Bullet<Player>(Vector2.Zero, Vector2.UnitY, spriteMock.Object);

            bullet.Reset(Vector2.Zero, Vector2.UnitY);

            IDamageDealer sut = bullet;

            int expectedDamage = BulletConfig.Player.Damage;

            // Act
            int actualDamage = sut.Damage;

            // Assert
            Assert.Equal(expectedDamage, actualDamage);

            // Output
            _output.WriteLine($"Expected: {expectedDamage}, Actual: {actualDamage}");
        }

        // ------------------------------------------------------------
        // EnemyBullet ska använda BulletConfig.Enemy.Damage
        // ------------------------------------------------------------
        [Fact]
        public void IDamageDealer_EnemyBullet_ShouldUseEnemyConfigDamage()
        {
            // Arrange
            var spriteMock = new Mock<ISpriteHelper>();
            var bullet = new Bullet<Enemy>(Vector2.Zero, Vector2.UnitY, spriteMock.Object);

            bullet.Reset(Vector2.Zero, Vector2.UnitY);

            IDamageDealer sut = bullet;

            int expectedDamage = BulletConfig.Enemy.Damage;

            // Act
            int actualDamage = sut.Damage;

            // Assert
            Assert.Equal(expectedDamage, actualDamage);

            // Output
            _output.WriteLine($"Expected: {expectedDamage}, Actual: {actualDamage}");
        }
    }
}
