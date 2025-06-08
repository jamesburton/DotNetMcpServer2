using DotNetMcpServer2.Core;

namespace DotNetMcpServer2.Tests;

public class DotNetExecutorTests
{
    [Fact]
    public async Task ExecuteAsync_ValidCommand_ReturnsSuccessResult()
    {
        // Arrange
        var command = "--version";

        // Act
        var result = await DotNetExecutor.ExecuteAsync(command);

        // Assert
        Assert.Equal(0, result.ExitCode);
        Assert.True(result.IsSuccess);
        Assert.False(string.IsNullOrEmpty(result.StandardOutput));
        Assert.Equal("dotnet --version", result.Command);
    }

    [Fact]
    public async Task ExecuteAsync_InvalidCommand_ReturnsFailureResult()
    {
        // Arrange
        var command = "invalid-command-that-does-not-exist";

        // Act
        var result = await DotNetExecutor.ExecuteAsync(command);

        // Assert
        Assert.NotEqual(0, result.ExitCode);
        Assert.False(result.IsSuccess);
        Assert.Equal("dotnet invalid-command-that-does-not-exist", result.Command);
    }

    [Fact]
    public async Task IsAvailableAsync_DotNetInstalled_ReturnsTrue()
    {
        // Act
        var isAvailable = await DotNetExecutor.IsAvailableAsync();

        // Assert
        Assert.True(isAvailable);
    }

    [Fact]
    public void CommandResult_ToString_FormatsCorrectly()
    {
        // Arrange
        var result = new CommandResult
        {
            ExitCode = 0,
            StandardOutput = "Test output",
            StandardError = "",
            ExecutionTime = TimeSpan.FromMilliseconds(100),
            Command = "dotnet test",
            WorkingDirectory = "/test/dir"
        };

        // Act
        var output = result.ToString();

        // Assert
        Assert.Contains("Command: dotnet test", output);
        Assert.Contains("Working Directory: /test/dir", output);
        Assert.Contains("Exit Code: 0", output);
        Assert.Contains("Standard Output:", output);
        Assert.Contains("Test output", output);
    }
}
