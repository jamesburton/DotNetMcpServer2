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

## Deployment Options

### 1. AOT (Ahead-of-Time) Compilation - **Recommended for Production**

Single-file AOT compilation provides maximum performance and minimal deployment footprint:

- **Fast Startup**: ~100ms vs ~500ms for JIT compilation
- **Small Footprint**: Single executable (~15-25MB)
- **No Runtime Dependencies**: Self-contained with .NET runtime included
- **Better Performance**: 10-30% faster execution for CPU-intensive operations

### 2. Docker Containers - **Recommended for Containerized Environments**

Ultra-lightweight Docker containers with AOT-compiled binaries:

- **Minimal Images**: As small as 30-50MB total
- **Fast Container Startup**: ~300ms including container overhead
- **Security**: Distroless/minimal base images with non-root execution
- **Scalability**: Perfect for microservices and cloud deployment

### 3. Traditional JIT - **Recommended for Development**

Standard .NET runtime for development and debugging.

## Quick Start

### Option 1: AOT Executable (Fastest)

```bash
# Build AOT executable
.\build-aot.ps1

# Use in Claude Desktop
{
  "mcpServers": {
    "dotnet-mcp-server2": {
      "command": "C:\\path\\to\\publish\\win-x64\\DotNetMcpServer2.exe"
    }
  }
}
```

### Option 2: Docker Container (Most Portable)

```bash
# Build Docker image
.\build-docker.ps1

# Use in Claude Desktop
{
  "mcpServers": {
    "dotnet-mcp-server2": {
      "command": "docker",
      "args": ["run", "--rm", "-i", "dotnet-mcp-server2:latest"]
    }
  }
}
```

### Option 3: Development Mode

```bash
# Run directly
cd DotNetMcpServer2
dotnet run

# Use in Claude Desktop
{
  "mcpServers": {
    "dotnet-mcp-server2": {
      "command": "dotnet",
      "args": ["run", "--project", "C:\\path\\to\\DotNetMcpServer2.csproj"]
    }
  }
}
```

## AOT (Ahead-of-Time) Compilation

### Building AOT Executable

#### Using Build Scripts (Recommended)

**Cross-platform PowerShell:**
```bash
.\build-aot.ps1                    # Interactive runtime selection
.\build-aot.ps1 -Runtime win-x64   # Specific runtime
```

**Windows Batch:**
```bash
.\build-aot-simple.bat             # Interactive menu
```

#### Manual Build

```bash
# Windows x64
dotnet publish -c Release -r win-x64 --self-contained true

# Linux x64  
dotnet publish -c Release -r linux-x64 --self-contained true

# macOS ARM64 (Apple Silicon)
dotnet publish -c Release -r osx-arm64 --self-contained true
```

### Supported Platforms
- **Windows**: x64, ARM64
- **Linux**: x64, ARM64  
- **macOS**: x64, ARM64 (Apple Silicon)

### Testing AOT Build

```bash
.\test-aot.ps1 -Runtime win-x64
```

## Docker Deployment

### Building Docker Images

```bash
# Standard image (~80MB)
docker build -t dotnet-mcp-server2:latest .

# Minimal image (~50MB)  
docker build -f Dockerfile.minimal -t dotnet-mcp-server2:minimal .

# Using build script (recommended)
.\build-docker.ps1                        # Standard build
.\build-docker.ps1 -Minimal              # Minimal build
```

### Running Docker Container

```bash
# Basic run
docker run --rm dotnet-mcp-server2:latest --version

# For MCP communication
docker run --rm -i dotnet-mcp-server2:latest

# With project volume mounting
docker run --rm -i -v "$(pwd):/workspace" -w /workspace dotnet-mcp-server2:latest

# Using Docker Compose
docker-compose up
```

### Claude Desktop Docker Configuration

```json
{
  "mcpServers": {
    "dotnet-mcp-server2": {
      "command": "docker",
      "args": [
        "run",
        "--rm",
        "--interactive",
        "--init",
        "dotnet-mcp-server2:latest"
      ],
      "env": {
        "DOTNET_CLI_TELEMETRY_OPTOUT": "1"
      }
    }
  }
}
```

For project access with Docker:
```json
{
  "mcpServers": {
    "dotnet-mcp-server2": {
      "command": "docker",
      "args": [
        "run",
        "--rm",
        "--interactive",
        "--init",
        "--volume", "C:\\Projects:/workspace:ro",
        "--workdir", "/workspace",
        "dotnet-mcp-server2:latest"
      ]
    }
  }
}
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

## Performance Comparison

| Deployment Method | Startup Time | Memory Usage | Disk Space | Best For |
|------------------|--------------|--------------|------------|----------|
| AOT Executable   | ~100ms       | ~25MB        | ~20MB      | Desktop/CLI |
| Docker Standard  | ~500ms       | ~45MB        | ~80MB      | Containers |
| Docker Minimal   | ~300ms       | ~35MB        | ~50MB      | Production |
| JIT Development  | ~800ms       | ~60MB        | ~200MB     | Development |

## Prerequisites

- .NET 9.0 SDK or later
- Docker (for container deployment)
- Entity Framework CLI tools (for EF commands): `dotnet tool install --global dotnet-ef`

## Available MCP Tools

All tools provide comprehensive .NET development capabilities:

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

### **AOT & Container Performance**
- **Native Compilation**: Faster startup and execution
- **Optimized Size**: Minimal deployment footprint
- **Self-Contained**: No runtime dependencies
- **Container Ready**: Optimized for containerized environments

## Documentation

- **[AOT Guide](AOT-GUIDE.md)**: Comprehensive AOT compilation guide
- **[MCP Configuration Examples](MCP-CONFIG-EXAMPLES.md)**: Various deployment configurations
- **[Docker Examples](MCP-CONFIG-EXAMPLES.md#docker-commands)**: Container deployment scenarios

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

## Troubleshooting

### AOT Issues
If you encounter issues with AOT compilation:

1. **Check for reflection usage**: Review warnings from `EnableAotAnalyzer`
2. **Verify trim compatibility**: Use `EnableTrimAnalyzer` warnings as guidance
3. **Update ILLink descriptors**: Add any missing types to `ILLink.Descriptors.xml`
4. **Test thoroughly**: AOT can change runtime behavior, so test all functionality

### Docker Issues
For Docker-related problems:

1. **Large image size**: Use `Dockerfile.minimal` for smaller images
2. **Permission errors**: Ensure proper file permissions in container
3. **Slow startup**: Check resource limits and use AOT builds
4. **Network issues**: Verify container networking configuration

## Contributing

This implementation provides comprehensive .NET CLI functionality with a modern, maintainable architecture using Microsoft's official MCP library, optimized for both AOT compilation and containerized deployment.
