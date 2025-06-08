using DotNetMcpServer2.Services;

namespace DotNetMcpServer2.Tests;

public class DotNetCoreCommandsTests
{
    [Fact]
    public async Task GetVersionAsync_ReturnsVersionInfo()
    {
        // Act
        var result = await DotNetCoreCommands.GetVersionAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Contains("dotnet --version", result);
    }

    [Fact]
    public async Task GetInfoAsync_ReturnsDetailedInfo()
    {
        // Act
        var result = await DotNetCoreCommands.GetInfoAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Contains("dotnet --info", result);
    }

    [Fact]
    public async Task ListSdksAsync_ReturnsSdkList()
    {
        // Act
        var result = await DotNetCoreCommands.ListSdksAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Contains("dotnet --list-sdks", result);
    }

    [Fact]
    public async Task ListRuntimesAsync_ReturnsRuntimeList()
    {
        // Act
        var result = await DotNetCoreCommands.ListRuntimesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Contains("dotnet --list-runtimes", result);
    }

    [Fact]
    public async Task ListTemplatesAsync_ReturnsTemplateList()
    {
        // Act
        var result = await DotNetCoreCommands.ListTemplatesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Contains("dotnet new list", result);
    }

    [Fact]
    public async Task CreateProjectAsync_WithTemplate_BuildsCorrectCommand()
    {
        // Act
        var result = await DotNetCoreCommands.CreateProjectAsync("console", "TestProject", null, "net9.0");

        // Assert
        Assert.NotNull(result);
        Assert.Contains("dotnet new console --name TestProject --framework net9.0", result);
    }

    [Fact]
    public async Task BuildAsync_WithConfiguration_BuildsCorrectCommand()
    {
        // Act
        var result = await DotNetCoreCommands.BuildAsync(null, "Release", "net9.0");

        // Assert
        Assert.NotNull(result);
        Assert.Contains("dotnet build --configuration Release --framework net9.0", result);
    }

    [Fact]
    public async Task TestAsync_WithNoBuildFlag_BuildsCorrectCommand()
    {
        // Act
        var result = await DotNetCoreCommands.TestAsync(noBuild: true, noRestore: true);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("dotnet test --no-build --no-restore", result);
    }
}
