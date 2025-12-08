using BulletHell.Interfaces;
using BulletHell.Models;
using Microsoft.Xna.Framework;
using Moq;

namespace BulletHell.test.Interfaces
{
    public class IHealthTests
    {
        private readonly ITestOutputHelper _output;

        public IHealthTests(ITestOutputHelper output)
        {
            _output = output;
        }

        // ------------------------------------------------------------
        // Health ska minska när TakeDamage anropas
        // ------------------------------------------------------------
        [Fact]
        public void IHealth_TakeDamage_ShouldReduceHealth()
        {
            // Arrange
            var spriteMock = new Mock<ISpriteHelper>();
            IHealth sut = new Enemy(Vector2.Zero, spriteMock.Object);

            int startHealth = sut.Health;
            int damage = 1;
            int expectedHealth = startHealth - damage;

            // Act
            sut.TakeDamage(damage);
            int actualHealth = sut.Health;

            // Assert
            Assert.Equal(expectedHealth, actualHealth);

            // Output
            _output.WriteLine(
                $"Start: {startHealth}," +
                $" Damage: {damage}," +
                $" Expected: {expectedHealth}," +
                $" Actual: {actualHealth}");
        }

        // ------------------------------------------------------------
        // IsAlive ska bli false när Health går till noll
        // ------------------------------------------------------------
        [Fact]
        public void IHealth_TakeDamage_ShouldSetIsAliveToFalse_WhenHealthReachesZero()
        {
            // Arrange
            var  spriteMock = new Mock<ISpriteHelper>();
            IHealth sut = new Enemy(Vector2.Zero, spriteMock.Object);

            bool alive = sut.IsAlive;   // bara för output
            int damage = sut.Health + 10;   // garanterad död
            bool expected = false;

            // Act
            sut.TakeDamage(damage);
            bool actual = sut.IsAlive;

            // Assert
            Assert.Equal(expected, actual);

            // Output
            _output.WriteLine(
                $"Before Damage (Alive): {alive}, " +
                $"Damage Applied: {damage}, " +
                $"Expected Alive: {expected}, " +
                $"Actual Alive: {actual}");
        }

        // ------------------------------------------------------------
        // Health ska inte bli negativ (om designen tillåter)
        // ------------------------------------------------------------
        [Fact]
        public void IHealth_Health_ShouldNotBecomeNegative()
        {
            // Arrange
            var  spriteMock = new Mock<ISpriteHelper>();
            IHealth sut = new Enemy(Vector2.Zero, spriteMock.Object);
            int startHealth = sut.Health;

            int damage = startHealth + 10;  // overkill
            int expected = 0;

            // Act
            sut.TakeDamage(damage);
            int actual = sut.Health;

            // Assert
            Assert.Equal(expected, actual);

            // Output
            _output.WriteLine(
                $"Start Health: {startHealth}, " +
                $"Damage: {damage}, " +
                $"Expected Health: {expected}, " +
                $"Actual Health: {actual}");
        }
    }
}
