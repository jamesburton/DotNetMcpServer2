using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;

var builder = Host.CreateApplicationBuilder(args);

// Configure logging to stderr to avoid interfering with MCP communication
builder.Logging.AddConsole(cfg =>
{
    cfg.LogToStandardErrorThreshold = LogLevel.Trace;
});

// Add MCP server with stdio transport and auto-discover tools from assembly
// Using the assembly method since tool classes are static
// The IL2026 warning is suppressed because we have ILLink.Descriptors.xml preserving the metadata
[UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", 
    Justification = "MCP tool metadata is preserved in ILLink.Descriptors.xml")]
static void ConfigureMcpServer(IServiceCollection services)
{
    services
        .AddMcpServer()
        .WithStdioServerTransport()
        .WithToolsFromAssembly(Assembly.GetExecutingAssembly());
}

ConfigureMcpServer(builder.Services);

await builder.Build().RunAsync();

// Configure JSON serialization for AOT
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(object))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}
