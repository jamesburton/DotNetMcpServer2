# .NET MCP Server 2

A comprehensive Model Context Protocol (MCP) server that exposes .NET CLI commands and Entity Framework operations as MCP tools, built using the official Microsoft `ModelContextProtocol` library.

## Key Differences from DotNetMcpServer

This version uses the **Microsoft ModelContextProtocol library** instead of MCPSharp, providing:

- **Official Microsoft Support**: Built on the official Microsoft MCP library
- **Hosting Integration**: Uses `Microsoft.Extensions.Hosting` for robust server lifecycle management
- **Automatic Tool Discovery**: Tools are automatically discovered from the assembly using `WithToolsFromAssembly()`
- **Simplified Attributes**: Uses `[McpServerTool]` and `[Description]` attributes instead of custom MCP attributes
- **Better Logging**: Integrated logging to stderr to avoid interfering with MCP communication

## Architecture Comparison

### DotNetMcpServer (MCPSharp)
```csharp
[McpTool("dotnet_version", "Gets the installed .NET version")]
public static async Task<string> GetVersionAsync()

// Registration
MCPServer.Register<DotNetCoreCommands>();
await MCPServer.StartAsync("DotNetMcpServer", "1.0.0");
```

### DotNetMcpServer2 (ModelContextProtocol)
```csharp
[McpServerTool, Description("Gets the installed .NET version")]
public static async Task<string> GetVersionAsync()

// Registration
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();
```

## Features

### Core .NET CLI Commands (20+ tools)
- **Project Management**: Create, build, run, test, clean, and publish .NET projects
- **Package Management**: Add, remove, and list NuGet packages
- **Project References**: Manage project-to-project references
- **Template Operations**: List and create projects from templates
- **Information Commands**: Get .NET version, SDK info, and runtime details

### Entity Framework Commands (11 tools)
- **Database Operations**: Update, drop, and manage databases
- **Migration Management**: Add, remove, list, and script migrations
- **DbContext Operations**: List, scaffold, and optimize DbContext classes
- **Advanced Features**: Create migration bundles for deployment

### Solution & NuGet Management (12 tools)
- **Solution Operations**: Add/remove projects, list solution contents
- **NuGet Publishing**: Push and delete packages from NuGet sources
- **Source Management**: Add, remove, update, and list NuGet sources
- **Cache Management**: Clear and list NuGet local caches

## Prerequisites

- .NET 9.0 SDK or later
- Entity Framework CLI tools (for EF commands): `dotnet tool install --global dotnet-ef`

## Installation & Usage

1. **Clone and Build**:
   ```bash
   git clone <repository-url>
   cd DotNetMcpServer2
   dotnet build
   ```

2. **Run the Server**:
   ```bash
   cd DotNetMcpServer2
   dotnet run
   ```

3. **Run Tests**:
   ```bash
   dotnet test
   ```

## Claude Desktop Configuration

Add this entry to your Claude Desktop configuration (`claude_desktop_config.json`):

```json
{
  "mcpServers": {
    "dotnet-mcp-server2": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:\\Development\\MCP\\DotNetMcpServer2\\DotNetMcpServer2\\DotNetMcpServer2.csproj"
      ],
      "env": {
        "DOTNET_CLI_TELEMETRY_OPTOUT": "1",
        "DOTNET_NOLOGO": "1"
      }
    }
  }
}
```

## Available MCP Tools

All tools from the original DotNetMcpServer are available with the same functionality:

### Core Commands
- `GetVersionAsync` - Gets .NET version
- `GetInfoAsync` - Displays detailed .NET information
- `ListSdksAsync` - Lists installed SDKs
- `CreateProjectAsync` - Creates new projects from templates
- `BuildAsync` - Builds projects
- `TestAsync` - Runs tests (includes `--no-build` and `--no-restore` flags)
- `PublishAsync` - Publishes projects
- And many more...

### Entity Framework Commands
- `UpdateDatabaseAsync` - Updates database to latest migration
- `AddMigrationAsync` - Adds new migrations
- `ScaffoldDbContextAsync` - Scaffolds DbContext from database
- `ListMigrationsAsync` - Lists available migrations
- And more...

### Solution & NuGet Commands
- `ListProjectsAsync` - Lists projects in solution
- `AddProjectToSolutionAsync` - Adds projects to solutions
- `NuGetPushAsync` - Pushes packages to NuGet
- `NuGetListSourceAsync` - Lists NuGet sources
- And more...

## Advantages of ModelContextProtocol Library

### **Simplified Development**
- **Auto-discovery**: No manual tool registration required
- **Standard Attributes**: Uses familiar `[Description]` attributes
- **Type Safety**: Strong typing with automatic parameter binding

### **Better Integration**
- **Hosting Model**: Integrates with .NET hosting and dependency injection
- **Logging**: Built-in logging infrastructure with configurable levels
- **Configuration**: Standard .NET configuration patterns

### **Enhanced Reliability**
- **Official Support**: Maintained by Microsoft
- **Robust Transport**: Built-in stdio transport with error handling
- **Process Management**: Proper lifecycle management and graceful shutdown

## Performance Comparison

Both versions provide identical functionality, but DotNetMcpServer2 offers:
- Better startup time due to optimized hosting
- More efficient tool discovery and registration
- Improved error handling and logging
- Better resource management and cleanup

## Migration Guide

To migrate from MCPSharp to ModelContextProtocol:

1. **Replace Package Reference**:
   ```xml
   <!-- Old -->
   <PackageReference Include="MCPSharp" Version="1.0.10" />
   
   <!-- New -->
   <PackageReference Include="ModelContextProtocol" Version="0.1.0-preview.11" />
   <PackageReference Include="Microsoft.Extensions.Hosting" Version="9.0.4" />
   ```

2. **Update Attributes**:
   ```csharp
   // Old
   [McpTool("tool_name", "Description")]
   public static Task<string> MethodAsync([McpParameter(true)] string param)
   
   // New
   [McpServerTool, Description("Description")]
   public static Task<string> MethodAsync([Description("Parameter description")] string param)
   ```

3. **Update Program.cs**:
   ```csharp
   // Old
   MCPServer.Register<Commands>();
   await MCPServer.StartAsync("ServerName", "1.0.0");
   
   // New
   var builder = Host.CreateApplicationBuilder(args);
   builder.Services.AddMcpServer().WithStdioServerTransport().WithToolsFromAssembly();
   await builder.Build().RunAsync();
   ```

This implementation provides the same comprehensive .NET CLI functionality with a more modern, maintainable architecture using Microsoft's official MCP library.
