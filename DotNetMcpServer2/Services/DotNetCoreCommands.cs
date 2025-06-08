using System.ComponentModel;
using DotNetMcpServer2.Core;
using ModelContextProtocol.Server;

namespace DotNetMcpServer2.Services;

/// <summary>
/// Provides core .NET CLI commands as MCP tools
/// </summary>
[McpServerToolType]
public static class DotNetCoreCommands
{
    [McpServerTool, Description("Gets the installed .NET version")]
    public static async Task<string> GetVersionAsync()
    {
        var result = await DotNetExecutor.ExecuteAsync("--version");
        return result.ToString();
    }

    [McpServerTool, Description("Displays .NET information")]
    public static async Task<string> GetInfoAsync()
    {
        var result = await DotNetExecutor.ExecuteAsync("--info");
        return result.ToString();
    }

    [McpServerTool, Description("Lists installed .NET SDKs")]
    public static async Task<string> ListSdksAsync()
    {
        var result = await DotNetExecutor.ExecuteAsync("--list-sdks");
        return result.ToString();
    }

    [McpServerTool, Description("Lists installed .NET runtimes")]
    public static async Task<string> ListRuntimesAsync()
    {
        var result = await DotNetExecutor.ExecuteAsync("--list-runtimes");
        return result.ToString();
    }

    [McpServerTool, Description("Lists available project templates")]
    public static async Task<string> ListTemplatesAsync()
    {
        var result = await DotNetExecutor.ExecuteAsync("new list");
        return result.ToString();
    }

    [McpServerTool, Description("Creates a new project from template")]
    public static async Task<string> CreateProjectAsync(
        [Description("Template name (e.g., console, webapi, classlib)")] string template,
        [Description("Project name")] string? name = null,
        [Description("Output directory")] string? output = null,
        [Description("Target framework (e.g., net9.0)")] string? framework = null,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = $"new {template}";
        
        if (!string.IsNullOrEmpty(name))
            args += $" --name {name}";
        if (!string.IsNullOrEmpty(output))
            args += $" --output {output}";
        if (!string.IsNullOrEmpty(framework))
            args += $" --framework {framework}";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Restores project dependencies")]
    public static async Task<string> RestoreAsync(
        [Description("Project path")] string? projectPath = null,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = "restore";
        if (!string.IsNullOrEmpty(projectPath))
            args += $" {projectPath}";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Builds a project")]
    public static async Task<string> BuildAsync(
        [Description("Project path")] string? projectPath = null,
        [Description("Build configuration (Debug/Release)")] string? configuration = null,
        [Description("Target framework")] string? framework = null,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = "build";
        if (!string.IsNullOrEmpty(projectPath))
            args += $" {projectPath}";
        if (!string.IsNullOrEmpty(configuration))
            args += $" --configuration {configuration}";
        if (!string.IsNullOrEmpty(framework))
            args += $" --framework {framework}";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Runs a project")]
    public static async Task<string> RunAsync(
        [Description("Project path")] string? projectPath = null,
        [Description("Build configuration (Debug/Release)")] string? configuration = null,
        [Description("Target framework")] string? framework = null,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = "run";
        if (!string.IsNullOrEmpty(projectPath))
            args += $" --project {projectPath}";
        if (!string.IsNullOrEmpty(configuration))
            args += $" --configuration {configuration}";
        if (!string.IsNullOrEmpty(framework))
            args += $" --framework {framework}";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Runs tests in a project")]
    public static async Task<string> TestAsync(
        [Description("Project path")] string? projectPath = null,
        [Description("Build configuration (Debug/Release)")] string? configuration = null,
        [Description("Target framework")] string? framework = null,
        [Description("Test filter expression")] string? filter = null,
        [Description("Skip building the project")] bool noBuild = false,
        [Description("Skip restoring packages")] bool noRestore = false,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = "test";
        if (!string.IsNullOrEmpty(projectPath))
            args += $" {projectPath}";
        if (!string.IsNullOrEmpty(configuration))
            args += $" --configuration {configuration}";
        if (!string.IsNullOrEmpty(framework))
            args += $" --framework {framework}";
        if (!string.IsNullOrEmpty(filter))
            args += $" --filter {filter}";
        if (noBuild)
            args += " --no-build";
        if (noRestore)
            args += " --no-restore";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Publishes a project for deployment")]
    public static async Task<string> PublishAsync(
        [Description("Project path")] string? projectPath = null,
        [Description("Build configuration (Debug/Release)")] string? configuration = null,
        [Description("Target framework")] string? framework = null,
        [Description("Target runtime identifier")] string? runtime = null,
        [Description("Output directory")] string? output = null,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = "publish";
        if (!string.IsNullOrEmpty(projectPath))
            args += $" {projectPath}";
        if (!string.IsNullOrEmpty(configuration))
            args += $" --configuration {configuration}";
        if (!string.IsNullOrEmpty(framework))
            args += $" --framework {framework}";
        if (!string.IsNullOrEmpty(runtime))
            args += $" --runtime {runtime}";
        if (!string.IsNullOrEmpty(output))
            args += $" --output {output}";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Cleans build outputs")]
    public static async Task<string> CleanAsync(
        [Description("Project path")] string? projectPath = null,
        [Description("Build configuration (Debug/Release)")] string? configuration = null,
        [Description("Target framework")] string? framework = null,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = "clean";
        if (!string.IsNullOrEmpty(projectPath))
            args += $" {projectPath}";
        if (!string.IsNullOrEmpty(configuration))
            args += $" --configuration {configuration}";
        if (!string.IsNullOrEmpty(framework))
            args += $" --framework {framework}";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Creates NuGet packages")]
    public static async Task<string> PackAsync(
        [Description("Project path")] string? projectPath = null,
        [Description("Build configuration (Debug/Release)")] string? configuration = null,
        [Description("Output directory")] string? output = null,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = "pack";
        if (!string.IsNullOrEmpty(projectPath))
            args += $" {projectPath}";
        if (!string.IsNullOrEmpty(configuration))
            args += $" --configuration {configuration}";
        if (!string.IsNullOrEmpty(output))
            args += $" --output {output}";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Adds a NuGet package reference to a project")]
    public static async Task<string> AddPackageAsync(
        [Description("Package name")] string packageName,
        [Description("Package version")] string? version = null,
        [Description("Project path")] string? projectPath = null,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = "add";
        if (!string.IsNullOrEmpty(projectPath))
            args += $" {projectPath}";
        args += $" package {packageName}";
        if (!string.IsNullOrEmpty(version))
            args += $" --version {version}";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Removes a NuGet package reference from a project")]
    public static async Task<string> RemovePackageAsync(
        [Description("Package name")] string packageName,
        [Description("Project path")] string? projectPath = null,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = "remove";
        if (!string.IsNullOrEmpty(projectPath))
            args += $" {projectPath}";
        args += $" package {packageName}";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Lists package references in a project")]
    public static async Task<string> ListPackagesAsync(
        [Description("Project path")] string? projectPath = null,
        [Description("Include outdated packages")] bool outdated = false,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = "list";
        if (!string.IsNullOrEmpty(projectPath))
            args += $" {projectPath}";
        args += " package";
        if (outdated)
            args += " --outdated";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Adds a project reference")]
    public static async Task<string> AddReferenceAsync(
        [Description("Reference project path")] string referencePath,
        [Description("Project path")] string? projectPath = null,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = "add";
        if (!string.IsNullOrEmpty(projectPath))
            args += $" {projectPath}";
        args += $" reference {referencePath}";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Removes a project reference")]
    public static async Task<string> RemoveReferenceAsync(
        [Description("Reference project path")] string referencePath,
        [Description("Project path")] string? projectPath = null,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = "remove";
        if (!string.IsNullOrEmpty(projectPath))
            args += $" {projectPath}";
        args += $" reference {referencePath}";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Lists project references")]
    public static async Task<string> ListReferencesAsync(
        [Description("Project path")] string? projectPath = null,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = "list";
        if (!string.IsNullOrEmpty(projectPath))
            args += $" {projectPath}";
        args += " reference";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }
}
