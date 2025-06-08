using System.Diagnostics;
using System.Text;

namespace DotNetMcpServer2.Core;

/// <summary>
/// Provides utilities for executing dotnet CLI commands
/// </summary>
public static class DotNetExecutor
{
    /// <summary>
    /// Executes a dotnet command and returns the result
    /// </summary>
    /// <param name="arguments">The dotnet command arguments</param>
    /// <param name="workingDirectory">Optional working directory</param>
    /// <returns>Command execution result</returns>
    public static async Task<CommandResult> ExecuteAsync(string arguments, string? workingDirectory = null)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory
        };

        var outputBuilder = new StringBuilder();
        var errorBuilder = new StringBuilder();

        using var process = new Process { StartInfo = processStartInfo };
        
        process.OutputDataReceived += (sender, e) =>
        {
            if (e.Data != null)
                outputBuilder.AppendLine(e.Data);
        };
        
        process.ErrorDataReceived += (sender, e) =>
        {
            if (e.Data != null)
                errorBuilder.AppendLine(e.Data);
        };

        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            
            await process.WaitForExitAsync();
            stopwatch.Stop();

            return new CommandResult
            {
                ExitCode = process.ExitCode,
                StandardOutput = outputBuilder.ToString().Trim(),
                StandardError = errorBuilder.ToString().Trim(),
                ExecutionTime = stopwatch.Elapsed,
                Command = $"dotnet {arguments}",
                WorkingDirectory = processStartInfo.WorkingDirectory
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return new CommandResult
            {
                ExitCode = -1,
                StandardOutput = "",
                StandardError = $"Failed to execute command: {ex.Message}",
                ExecutionTime = stopwatch.Elapsed,
                Command = $"dotnet {arguments}",
                WorkingDirectory = processStartInfo.WorkingDirectory
            };
        }
    }

    /// <summary>
    /// Validates that the dotnet CLI is available
    /// </summary>
    /// <returns>True if dotnet CLI is available</returns>
    public static async Task<bool> IsAvailableAsync()
    {
        try
        {
            var result = await ExecuteAsync("--version");
            return result.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }
}

/// <summary>
/// Represents the result of a command execution
/// </summary>
public class CommandResult
{
    public int ExitCode { get; init; }
    public string StandardOutput { get; init; } = string.Empty;
    public string StandardError { get; init; } = string.Empty;
    public TimeSpan ExecutionTime { get; init; }
    public string Command { get; init; } = string.Empty;
    public string WorkingDirectory { get; init; } = string.Empty;

    public bool IsSuccess => ExitCode == 0;

    public override string ToString()
    {
        var result = new StringBuilder();
        result.AppendLine($"Command: {Command}");
        result.AppendLine($"Working Directory: {WorkingDirectory}");
        result.AppendLine($"Exit Code: {ExitCode}");
        result.AppendLine($"Execution Time: {ExecutionTime.TotalMilliseconds:F2}ms");
        
        if (!string.IsNullOrEmpty(StandardOutput))
        {
            result.AppendLine("Standard Output:");
            result.AppendLine(StandardOutput);
        }
        
        if (!string.IsNullOrEmpty(StandardError))
        {
            result.AppendLine("Standard Error:");
            result.AppendLine(StandardError);
        }
        
        return result.ToString();
    }
}
