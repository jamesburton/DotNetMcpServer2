using System.ComponentModel;
using DotNetMcpServer2.Core;
using ModelContextProtocol.Server;

namespace DotNetMcpServer2.Services;

/// <summary>
/// Provides Entity Framework CLI commands as MCP tools
/// </summary>
[McpServerToolType]
public static class EntityFrameworkCommands
{
    [McpServerTool, Description("Updates the database to the latest migration")]
    public static async Task<string> UpdateDatabaseAsync(
        [Description("Target migration")] string? targetMigration = null,
        [Description("Connection string")] string? connection = null,
        [Description("DbContext class name")] string? context = null,
        [Description("Project path")] string? project = null,
        [Description("Startup project path")] string? startupProject = null,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = "ef database update";
        
        if (!string.IsNullOrEmpty(targetMigration))
            args += $" {targetMigration}";
        if (!string.IsNullOrEmpty(connection))
            args += $" --connection \"{connection}\"";
        if (!string.IsNullOrEmpty(context))
            args += $" --context {context}";
        if (!string.IsNullOrEmpty(project))
            args += $" --project {project}";
        if (!string.IsNullOrEmpty(startupProject))
            args += $" --startup-project {startupProject}";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Drops the database")]
    public static async Task<string> DropDatabaseAsync(
        [Description("Force drop without confirmation")] bool force = false,
        [Description("Connection string")] string? connection = null,
        [Description("DbContext class name")] string? context = null,
        [Description("Project path")] string? project = null,
        [Description("Startup project path")] string? startupProject = null,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = "ef database drop";
        
        if (force)
            args += " --force";
        if (!string.IsNullOrEmpty(connection))
            args += $" --connection \"{connection}\"";
        if (!string.IsNullOrEmpty(context))
            args += $" --context {context}";
        if (!string.IsNullOrEmpty(project))
            args += $" --project {project}";
        if (!string.IsNullOrEmpty(startupProject))
            args += $" --startup-project {startupProject}";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Adds a new migration")]
    public static async Task<string> AddMigrationAsync(
        [Description("Migration name")] string name,
        [Description("Output directory for migration files")] string? outputDir = null,
        [Description("DbContext class name")] string? context = null,
        [Description("Project path")] string? project = null,
        [Description("Startup project path")] string? startupProject = null,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = $"ef migrations add {name}";
        
        if (!string.IsNullOrEmpty(outputDir))
            args += $" --output-dir {outputDir}";
        if (!string.IsNullOrEmpty(context))
            args += $" --context {context}";
        if (!string.IsNullOrEmpty(project))
            args += $" --project {project}";
        if (!string.IsNullOrEmpty(startupProject))
            args += $" --startup-project {startupProject}";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Removes the last migration")]
    public static async Task<string> RemoveMigrationAsync(
        [Description("Force removal without checking for unapplied changes")] bool force = false,
        [Description("DbContext class name")] string? context = null,
        [Description("Project path")] string? project = null,
        [Description("Startup project path")] string? startupProject = null,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = "ef migrations remove";
        
        if (force)
            args += " --force";
        if (!string.IsNullOrEmpty(context))
            args += $" --context {context}";
        if (!string.IsNullOrEmpty(project))
            args += $" --project {project}";
        if (!string.IsNullOrEmpty(startupProject))
            args += $" --startup-project {startupProject}";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Lists available migrations")]
    public static async Task<string> ListMigrationsAsync(
        [Description("Connection string")] string? connection = null,
        [Description("DbContext class name")] string? context = null,
        [Description("Project path")] string? project = null,
        [Description("Startup project path")] string? startupProject = null,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = "ef migrations list";
        
        if (!string.IsNullOrEmpty(connection))
            args += $" --connection \"{connection}\"";
        if (!string.IsNullOrEmpty(context))
            args += $" --context {context}";
        if (!string.IsNullOrEmpty(project))
            args += $" --project {project}";
        if (!string.IsNullOrEmpty(startupProject))
            args += $" --startup-project {startupProject}";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Generates SQL script for migrations")]
    public static async Task<string> ScriptMigrationsAsync(
        [Description("From migration")] string? from = null,
        [Description("To migration")] string? to = null,
        [Description("Output file path")] string? output = null,
        [Description("Generate idempotent script")] bool idempotent = false,
        [Description("Connection string")] string? connection = null,
        [Description("DbContext class name")] string? context = null,
        [Description("Project path")] string? project = null,
        [Description("Startup project path")] string? startupProject = null,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = "ef migrations script";
        
        if (!string.IsNullOrEmpty(from))
            args += $" {from}";
        if (!string.IsNullOrEmpty(to))
            args += $" {to}";
        if (!string.IsNullOrEmpty(output))
            args += $" --output {output}";
        if (idempotent)
            args += " --idempotent";
        if (!string.IsNullOrEmpty(connection))
            args += $" --connection \"{connection}\"";
        if (!string.IsNullOrEmpty(context))
            args += $" --context {context}";
        if (!string.IsNullOrEmpty(project))
            args += $" --project {project}";
        if (!string.IsNullOrEmpty(startupProject))
            args += $" --startup-project {startupProject}";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Gets information about a DbContext type")]
    public static async Task<string> GetDbContextInfoAsync(
        [Description("DbContext class name")] string? context = null,
        [Description("Project path")] string? project = null,
        [Description("Startup project path")] string? startupProject = null,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = "ef dbcontext info";
        
        if (!string.IsNullOrEmpty(context))
            args += $" --context {context}";
        if (!string.IsNullOrEmpty(project))
            args += $" --project {project}";
        if (!string.IsNullOrEmpty(startupProject))
            args += $" --startup-project {startupProject}";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Lists available DbContext types")]
    public static async Task<string> ListDbContextsAsync(
        [Description("Project path")] string? project = null,
        [Description("Startup project path")] string? startupProject = null,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = "ef dbcontext list";
        
        if (!string.IsNullOrEmpty(project))
            args += $" --project {project}";
        if (!string.IsNullOrEmpty(startupProject))
            args += $" --startup-project {startupProject}";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Scaffolds a DbContext and entity types from a database")]
    public static async Task<string> ScaffoldDbContextAsync(
        [Description("Database connection string")] string connectionString,
        [Description("Database provider (e.g., Microsoft.EntityFrameworkCore.SqlServer)")] string provider,
        [Description("Output directory")] string? outputDir = null,
        [Description("DbContext name")] string? contextName = null,
        [Description("DbContext directory")] string? contextDir = null,
        [Description("Force overwrite existing files")] bool force = false,
        [Description("Use data annotations instead of fluent API")] bool dataAnnotations = false,
        [Description("Project path")] string? project = null,
        [Description("Startup project path")] string? startupProject = null,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = $"ef dbcontext scaffold \"{connectionString}\" {provider}";
        
        if (!string.IsNullOrEmpty(outputDir))
            args += $" --output-dir {outputDir}";
        if (!string.IsNullOrEmpty(contextName))
            args += $" --context {contextName}";
        if (!string.IsNullOrEmpty(contextDir))
            args += $" --context-dir {contextDir}";
        if (force)
            args += " --force";
        if (dataAnnotations)
            args += " --data-annotations";
        if (!string.IsNullOrEmpty(project))
            args += $" --project {project}";
        if (!string.IsNullOrEmpty(startupProject))
            args += $" --startup-project {startupProject}";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Generates a compiled version of the model used by the DbContext")]
    public static async Task<string> OptimizeDbContextAsync(
        [Description("Output directory")] string? outputDir = null,
        [Description("Namespace for generated class")] string? nameSpace = null,
        [Description("DbContext class name")] string? context = null,
        [Description("Project path")] string? project = null,
        [Description("Startup project path")] string? startupProject = null,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = "ef dbcontext optimize";
        
        if (!string.IsNullOrEmpty(outputDir))
            args += $" --output-dir {outputDir}";
        if (!string.IsNullOrEmpty(nameSpace))
            args += $" --namespace {nameSpace}";
        if (!string.IsNullOrEmpty(context))
            args += $" --context {context}";
        if (!string.IsNullOrEmpty(project))
            args += $" --project {project}";
        if (!string.IsNullOrEmpty(startupProject))
            args += $" --startup-project {startupProject}";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }

    [McpServerTool, Description("Creates an executable bundle containing migrations")]
    public static async Task<string> BundleMigrationsAsync(
        [Description("Output file path")] string? output = null,
        [Description("Force overwrite existing bundle")] bool force = false,
        [Description("Self-contained bundle")] bool selfContained = false,
        [Description("Target runtime")] string? targetRuntime = null,
        [Description("DbContext class name")] string? context = null,
        [Description("Project path")] string? project = null,
        [Description("Startup project path")] string? startupProject = null,
        [Description("Working directory")] string? workingDirectory = null)
    {
        var args = "ef migrations bundle";
        
        if (!string.IsNullOrEmpty(output))
            args += $" --output {output}";
        if (force)
            args += " --force";
        if (selfContained)
            args += " --self-contained";
        if (!string.IsNullOrEmpty(targetRuntime))
            args += $" --target-runtime {targetRuntime}";
        if (!string.IsNullOrEmpty(context))
            args += $" --context {context}";
        if (!string.IsNullOrEmpty(project))
            args += $" --project {project}";
        if (!string.IsNullOrEmpty(startupProject))
            args += $" --startup-project {startupProject}";

        var result = await DotNetExecutor.ExecuteAsync(args, workingDirectory);
        return result.ToString();
    }
}
