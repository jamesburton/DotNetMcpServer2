# AOT Compilation Guide for DotNetMcpServer2

## Overview

This document provides detailed information about Ahead-of-Time (AOT) compilation for the DotNetMcpServer2 project, including configuration, troubleshooting, and optimization strategies.

## AOT Configuration Details

### Project File Settings

The `DotNetMcpServer2.csproj` file includes comprehensive AOT settings:

```xml
<!-- Core AOT Settings -->
<PublishAot>true</PublishAot>
<PublishSingleFile>true</PublishSingleFile>
<PublishTrimmed>true</PublishTrimmed>
<SelfContained>true</SelfContained>

<!-- Optimization Settings -->
<TrimMode>full</TrimMode>
<OptimizationPreference>Size</OptimizationPreference>
<IlcOptimizationPreference>Size</IlcOptimizationPreference>
<IlcFoldIdenticalMethodBodies>true</IlcFoldIdenticalMethodBodies>

<!-- Runtime Optimizations -->
<InvariantGlobalization>true</InvariantGlobalization>
<UseSystemResourceKeys>true</UseSystemResourceKeys>
<DebuggerSupport>false</DebuggerSupport>
<EnableUnsafeUTF7Encoding>false</EnableUnsafeUTF7Encoding>
<EventSourceSupport>false</EventSourceSupport>
<HttpActivityPropagationSupport>false</HttpActivityPropagationSupport>
<UseNativeHttpHandler>true</UseNativeHttpHandler>
<MetadataUpdaterSupport>false</MetadataUpdaterSupport>
```

### Analysis and Warnings

```xml
<!-- Enable AOT analysis -->
<EnableAotAnalyzer>true</EnableAotAnalyzer>
<EnableTrimAnalyzer>true</EnableTrimAnalyzer>
<EnableSingleFileAnalyzer>true</EnableSingleFileAnalyzer>
<SuppressTrimAnalysisWarnings>false</SuppressTrimAnalysisWarnings>
```

## Supported Runtime Targets

- **Windows**: `win-x64`, `win-arm64`
- **Linux**: `linux-x64`, `linux-arm64`
- **macOS**: `osx-x64`, `osx-arm64`

## Build Commands

### Basic AOT Build
```bash
dotnet publish -c Release -r win-x64 --self-contained true
```

### Optimized AOT Build
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishAot=true -p:OptimizationPreference=Size
```

### Cross-Platform Build Script
```bash
# Use the provided PowerShell script
.\build-aot.ps1 -Runtime win-x64
```

## Performance Characteristics

### Expected Build Times
- **First build**: 2-5 minutes (includes NuGet restore and native compilation)
- **Incremental builds**: 1-3 minutes
- **Clean builds**: 2-4 minutes

### Expected Output Sizes
- **Windows x64**: ~15-25 MB
- **Linux x64**: ~12-20 MB
- **macOS ARM64**: ~10-18 MB

### Runtime Performance
- **Startup time**: <100ms (vs ~500ms for JIT)
- **Memory usage**: ~20-40 MB working set
- **Execution speed**: 10-30% faster than JIT for CPU-intensive operations

## Troubleshooting Common Issues

### 1. Trim Warnings (IL2XXX)

**Issue**: Warnings about potentially unsafe trimming
```
warning IL2026: Using member 'System.Reflection.MethodInfo.Invoke(Object, Object[])' which has 'RequiresUnreferencedCodeAttribute'
```

**Solution**: 
- Add types to `ILLink.Descriptors.xml`
- Use `[DynamicallyAccessedMembers]` attribute
- Consider using source generators instead of reflection

### 2. AOT Analysis Warnings (AOT0XXX)

**Issue**: Warnings about AOT incompatibility
```
warning AOT0101: Type 'SomeType' was not found in the trimmed application
```

**Solution**:
- Ensure all required types are preserved
- Update the trimmer descriptors
- Use `[RequiresUnreferencedCode]` for incompatible code paths

### 3. JSON Serialization Issues

**Issue**: JSON serialization fails at runtime

**Solution**: 
- Use the provided `AppJsonSerializerContext`
- Add types to the JSON context:
```csharp
[JsonSerializable(typeof(YourType))]
internal partial class AppJsonSerializerContext : JsonSerializerContext { }
```

### 4. Missing Dependencies

**Issue**: Application fails to start due to missing dependencies

**Solution**:
- Verify all package references are compatible with AOT
- Check that `SelfContained=true` is set
- Ensure no P/Invoke dependencies are missing

### 5. Large Output Size

**Issue**: AOT executable is larger than expected

**Solutions**:
- Enable more aggressive optimizations:
  ```xml
  <OptimizationPreference>Size</OptimizationPreference>
  <IlcOptimizationPreference>Size</IlcOptimizationPreference>
  ```
- Remove unused features:
  ```xml
  <EventSourceSupport>false</EventSourceSupport>
  <DebuggerSupport>false</DebuggerSupport>
  ```
- Use invariant globalization:
  ```xml
  <InvariantGlobalization>true</InvariantGlobalization>
  ```

## MCP-Specific Considerations

### Tool Discovery
The MCP tool discovery mechanism uses reflection to find `[McpServerTool]` attributes. This is preserved through:

1. **ILLink Descriptors**: All MCP tool classes are marked with `preserve="all"`
2. **Assembly Scanning**: The ModelContextProtocol library's assembly scanning is AOT-compatible
3. **Attribute Preservation**: All required attributes are preserved in the trimmer descriptors

### Stdio Communication
The stdio transport used by MCP is fully AOT-compatible and doesn't require special configuration.

### JSON Serialization
MCP protocol messages use JSON serialization, which is handled by the AOT-compatible JSON source generator.

## Validation and Testing

### Automated Testing
Use the provided test script:
```bash
.\test-aot.ps1 -Runtime win-x64
```

### Manual Testing
1. **Build verification**: Ensure the executable is created and is self-contained
2. **Startup test**: Verify the application starts quickly
3. **MCP functionality**: Test that all MCP tools are discoverable
4. **CLI execution**: Verify that dotnet commands work correctly

### Performance Benchmarking
```bash
# Measure startup time
Measure-Command { .\DotNetMcpServer2.exe --version }

# Memory usage monitoring
Get-Process -Name "DotNetMcpServer2" | Select-Object WorkingSet64, VirtualMemorySize64
```

## Optimization Strategies

### Size Optimization
1. Enable aggressive trimming
2. Use invariant globalization
3. Disable unnecessary features
4. Remove debug information

### Speed Optimization
1. Use `OptimizationPreference=Speed`
2. Enable method body folding
3. Use ReadyToRun images for frequently used assemblies

### Compatibility Optimization
1. Keep trimming warnings enabled during development
2. Use conservative trimming settings
3. Thoroughly test all code paths

## Deployment Considerations

### Single File Deployment
The AOT build produces a single executable file that includes:
- Application code
- .NET runtime
- Required libraries
- Native dependencies

### System Requirements
- **Windows**: Windows 10 version 1607+
- **Linux**: glibc 2.17+ (most modern distributions)
- **macOS**: macOS 10.15+

### Security Considerations
- AOT binaries are harder to reverse engineer
- No JIT compilation at runtime (security benefit)
- Reduced attack surface due to trimmed dependencies

## Best Practices

### Development
1. Build and test AOT regularly during development
2. Fix trim and AOT warnings immediately
3. Use static analysis tools to catch potential issues early
4. Test on target deployment platforms

### CI/CD Integration
1. Include AOT builds in your build pipeline
2. Run automated tests on AOT binaries
3. Compare performance metrics between JIT and AOT builds
4. Monitor output size trends over time

### Monitoring
1. Track startup times in production
2. Monitor memory usage patterns
3. Log any runtime failures specific to AOT builds
4. Compare performance with JIT versions

This guide should help you successfully build, deploy, and maintain AOT-compiled versions of DotNetMcpServer2.
