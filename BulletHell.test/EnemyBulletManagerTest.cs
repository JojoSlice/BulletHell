using System.Numerics;
using BulletHell.Managers;
using BulletHell.Models;

namespace BulletHell.test;

public class EnemyBulletManagerTest(ITestOutputHelper output)
{
    [Fact]
    public void EnemyBulletManager_ShouldAddBulletToList()
    {
        // Arrange
        var bulletManager = new EnemyBulletManager();
        var startPosition = new Vector2(10, 10);
        var velocity = new Vector2(10, 10);
        var enemyBullet= new EnemyBullet(startPosition, velocity);
        var expected = enemyBullet;

        // Act
        bulletManager.AddBullet(enemyBullet);
        var actualList = bulletManager.Bullets;; 

        // Assert
        Assert.Single(actualList);
        Assert.Equal(expected, actualList[0]);
        
        // Output
        output.WriteLine($"Bullet count after AddBullet: {actualList.Count}");
        output.WriteLine("Result: Bullet successfully added ✔");
        
    }
}

// AddBullet ska lägga in en bullet i listan
// Count ska vara 1

// EnemyBulletManager_ShouldRemoveOutOfBoundsBullets
// bullet spawna utanför → manager.Update → tas bort

// EnemyBulletManager_ShouldUpdateAllBullets
// loopar lista
// kallar bullet.Update på alla

// EnemyBulletManager_ShouldDrawAllBullets
// Draw anropas en gång per bullet
// Ofta med NSubstitute:
// mocka EnemyBullet → expect Received(1).Draw