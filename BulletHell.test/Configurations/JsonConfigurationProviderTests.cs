using BulletHell.Configurations;
using System.IO;
using Xunit;

namespace BulletHell.test.Configurations;

public class JsonConfigurationProviderTests
{
    [Fact]
    public void GetConfiguration_WithMissingFile_ShouldReturnDefaults()
    {
        // Arrange
        var provider = new JsonConfigurationProvider("nonexistent.json");

        // Act
        var config = provider.GetConfiguration();

        // Assert
        Assert.NotNull(config);
        Assert.Equal(300f, config.Player.Speed);
        Assert.Equal(50, config.Pools.PlayerBullets.InitialSize);
        Assert.Equal(200, config.Pools.PlayerBullets.MaxSize);
    }

    [Fact]
    public void GetConfiguration_CalledTwice_ShouldReturnCachedInstance()
    {
        // Arrange
        var provider = new JsonConfigurationProvider("nonexistent.json");

        // Act
        var config1 = provider.GetConfiguration();
        var config2 = provider.GetConfiguration();

        // Assert
        Assert.Same(config1, config2);
    }

    [Fact]
    public void GetConfiguration_WithValidJson_ShouldLoadFromFile()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        var json = @"{
            ""player"": { ""speed"": 500.0 },
            ""pools"": {
                ""playerBullets"": { ""initialSize"": 100, ""maxSize"": 300 }
            }
        }";
        File.WriteAllText(tempFile, json);

        try
        {
            var provider = new JsonConfigurationProvider(tempFile);

            // Act
            var config = provider.GetConfiguration();

            // Assert
            Assert.NotNull(config);
            Assert.Equal(500f, config.Player.Speed);
            Assert.Equal(100, config.Pools.PlayerBullets.InitialSize);
            Assert.Equal(300, config.Pools.PlayerBullets.MaxSize);
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Fact]
    public void GetConfiguration_WithMalformedJson_ShouldReturnDefaults()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, "{ invalid json }");

        try
        {
            var provider = new JsonConfigurationProvider(tempFile);

            // Act
            var config = provider.GetConfiguration();

            // Assert - Should fall back to defaults
            Assert.NotNull(config);
            Assert.Equal(300f, config.Player.Speed);
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Fact]
    public void ClearCache_ShouldForceReload()
    {
        // Arrange
        var provider = new JsonConfigurationProvider("nonexistent.json");
        var config1 = provider.GetConfiguration();

        // Act
        provider.ClearCache();
        var config2 = provider.GetConfiguration();

        // Assert
        Assert.NotSame(config1, config2);
    }
}
