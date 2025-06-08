using System.ComponentModel;
using DotNetMcpServer2.Core;
using ModelContextProtocol.Server;

namespace DotNetMcpServer2.Services;

/// <summary>
/// Provides solution and NuGet management commands as MCP tools
/// </summary>
[McpServerToolType]
public static class SolutionCommands
{
    [McpServerTool, Description("Lists projects in a solution")]
    public static async Task<string> ListProjectsAsync(
        [Description("Solution file path")] string? solutionPath = null,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = "sln";
        if (!string.IsNullOrEmpty(solutionPath))
            args += $" {solutionPath}";
        args += " list";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Adds projects to a solution")]
    public static async Task<string> AddProjectToSolutionAsync(
        [Description("Project path to add")] string projectPath,
        [Description("Solution file path")] string? solutionPath = null,
        [Description("Solution folder")] string? solutionFolder = null,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = "sln";
        if (!string.IsNullOrEmpty(solutionPath))
            args += $" {solutionPath}";
        args += $" add {projectPath}";
        if (!string.IsNullOrEmpty(solutionFolder))
            args += $" --solution-folder {solutionFolder}";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Removes projects from a solution")]
    public static async Task<string> RemoveProjectFromSolutionAsync(
        [Description("Project path to remove")] string projectPath,
        [Description("Solution file path")] string? solutionPath = null,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = "sln";
        if (!string.IsNullOrEmpty(solutionPath))
            args += $" {solutionPath}";
        args += $" remove {projectPath}";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Pushes a package to a NuGet source")]
    public static async Task<string> NuGetPushAsync(
        [Description("Package file path")] string packagePath,
        [Description("NuGet source URL")] string? source = null,
        [Description("API key")] string? apiKey = null,
        [Description("Symbol source URL")] string? symbolSource = null,
        [Description("Symbol API key")] string? symbolApiKey = null,
        [Description("Timeout in seconds")] int? timeout = null,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = $"nuget push {packagePath}";
        
        if (!string.IsNullOrEmpty(source))
            args += $" --source {source}";
        if (!string.IsNullOrEmpty(apiKey))
            args += $" --api-key {apiKey}";
        if (!string.IsNullOrEmpty(symbolSource))
            args += $" --symbol-source {symbolSource}";
        if (!string.IsNullOrEmpty(symbolApiKey))
            args += $" --symbol-api-key {symbolApiKey}";
        if (timeout.HasValue)
            args += $" --timeout {timeout.Value}";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Deletes a package from a NuGet source")]
    public static async Task<string> NuGetDeleteAsync(
        [Description("Package name")] string packageName,
        [Description("Package version")] string version,
        [Description("NuGet source URL")] string? source = null,
        [Description("API key")] string? apiKey = null,
        [Description("Non-interactive mode")] bool nonInteractive = false,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = $"nuget delete {packageName} {version}";
        
        if (!string.IsNullOrEmpty(source))
            args += $" --source {source}";
        if (!string.IsNullOrEmpty(apiKey))
            args += $" --api-key {apiKey}";
        if (nonInteractive)
            args += " --non-interactive";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Clears or lists local NuGet caches")]
    public static async Task<string> NuGetLocalsAsync(
        [Description("Cache location (all, http-cache, global-packages, temp, plugins-cache)")] string cacheLocation,
        [Description("List cache contents instead of clearing")] bool list = false,
        [Description("Clear cache")] bool clear = false,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = $"nuget locals {cacheLocation}";
        
        if (list)
            args += " --list";
        else if (clear)
            args += " --clear";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Adds a NuGet source")]
    public static async Task<string> NuGetAddSourceAsync(
        [Description("Source URL")] string source,
        [Description("Source name")] string? name = null,
        [Description("Username")] string? username = null,
        [Description("Password")] string? password = null,
        [Description("Store password in clear text")] bool storePasswordInClearText = false,
        [Description("Valid authentication types")] string? validAuthenticationTypes = null,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = $"nuget add source {source}";
        
        if (!string.IsNullOrEmpty(name))
            args += $" --name {name}";
        if (!string.IsNullOrEmpty(username))
            args += $" --username {username}";
        if (!string.IsNullOrEmpty(password))
            args += $" --password {password}";
        if (storePasswordInClearText)
            args += " --store-password-in-clear-text";
        if (!string.IsNullOrEmpty(validAuthenticationTypes))
            args += $" --valid-authentication-types {validAuthenticationTypes}";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Removes a NuGet source")]
    public static async Task<string> NuGetRemoveSourceAsync(
        [Description("Source name or URL")] string source,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = $"nuget remove source {source}";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Lists NuGet sources")]
    public static async Task<string> NuGetListSourceAsync(
        [Description("Format (Detailed, Short)")] string? format = null,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = "nuget list source";
        
        if (!string.IsNullOrEmpty(format))
            args += $" --format {format}";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Updates a NuGet source")]
    public static async Task<string> NuGetUpdateSourceAsync(
        [Description("Source name")] string name,
        [Description("Source URL")] string? source = null,
        [Description("Username")] string? username = null,
        [Description("Password")] string? password = null,
        [Description("Store password in clear text")] bool storePasswordInClearText = false,
        [Description("Valid authentication types")] string? validAuthenticationTypes = null,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = $"nuget update source {name}";
        
        if (!string.IsNullOrEmpty(source))
            args += $" --source {source}";
        if (!string.IsNullOrEmpty(username))
            args += $" --username {username}";
        if (!string.IsNullOrEmpty(password))
            args += $" --password {password}";
        if (storePasswordInClearText)
            args += " --store-password-in-clear-text";
        if (!string.IsNullOrEmpty(validAuthenticationTypes))
            args += $" --valid-authentication-types {validAuthenticationTypes}";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Enables a NuGet source")]
    public static async Task<string> NuGetEnableSourceAsync(
        [Description("Source name")] string name,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = $"nuget enable source {name}";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Disables a NuGet source")]
    public static async Task<string> NuGetDisableSourceAsync(
        [Description("Source name")] string name,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = $"nuget disable source {name}";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }
}
