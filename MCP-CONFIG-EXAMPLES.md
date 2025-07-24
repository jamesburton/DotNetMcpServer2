# MCP Server Configuration Examples - Updated with Minimal Docker Images

This document provides comprehensive configuration examples for using DotNetMcpServer2 as an MCP server in different environments, including ultra-minimal Docker containers.

## Docker Image Size Comparison

| Docker Type | Base Image | Size | Security | Best For |
|-------------|------------|------|----------|----------|
| **Scratch** | `scratch` | ~10-15 MB | Maximum | Ultra-minimal production |
| **Distroless** | `gcr.io/distroless/static` | ~12-18 MB | Very High | Secure production |
| **Standard** | `mcr.microsoft.com/dotnet/runtime-deps:9.0-jammy-chiseled-aot` | ~20-30 MB | High | Enterprise production |
| **Traditional** | `mcr.microsoft.com/dotnet/runtime-deps:9.0-alpine` | ~50-80 MB | Medium | Legacy compatibility |

## Claude Desktop Configuration

### 1. Local AOT Executable (Fastest - Recommended for Desktop)

```json
{
  "mcpServers": {
    "dotnet-mcp-server2": {
      "command": "C:\\Development\\MCP\\DotNetMcpServer2\\publish\\win-x64\\DotNetMcpServer2.exe",
      "env": {
        "DOTNET_CLI_TELEMETRY_OPTOUT": "1",
        "DOTNET_NOLOGO": "1"
      }
    }
  }
}
```

### 2. Ultra-Minimal Docker (Scratch Base - Production)

```json
{
  "mcpServers": {
    "dotnet-mcp-server2-scratch": {
      "command": "docker",
      "args": [
        "run",
        "--rm",
        "--interactive",
        "--init",
        "dotnet-mcp-server2:scratch"
      ],
      "env": {
        "DOTNET_CLI_TELEMETRY_OPTOUT": "1"
      }
    }
  }
}
```

### 3. Minimal Docker with Security (Distroless - Recommended for Containers)

```json
{
  "mcpServers": {
    "dotnet-mcp-server2-distroless": {
      "command": "docker",
      "args": [
        "run",
        "--rm",
        "--interactive",
        "--init",
        "dotnet-mcp-server2:distroless"
      ],
      "env": {
        "DOTNET_CLI_TELEMETRY_OPTOUT": "1"
      }
    }
  }
}
```

### 4. Standard Minimal Docker (Microsoft Chiseled - Enterprise)

```json
{
  "mcpServers": {
    "dotnet-mcp-server2-standard": {
      "command": "docker",
      "args": [
        "run",
        "--rm",
        "--interactive",
        "--init",
        "dotnet-mcp-server2:minimal"
      ],
      "env": {
        "DOTNET_CLI_TELEMETRY_OPTOUT": "1"
      }
    }
  }
}
```

### 5. Docker with Project Access (Volume Mounting)

```json
{
  "mcpServers": {
    "dotnet-mcp-server2-with-projects": {
      "command": "docker",
      "args": [
        "run",
        "--rm",
        "--interactive",
        "--init",
        "--volume", "C:\\Projects:/workspace:ro",
        "--workdir", "/workspace",
        "dotnet-mcp-server2:distroless"
      ],
      "env": {
        "DOTNET_CLI_TELEMETRY_OPTOUT": "1"
      }
    }
  }
}
```

### 6. Development Mode (JIT Compilation)

```json
{
  "mcpServers": {
    "dotnet-mcp-server2-dev": {
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

## Docker Commands

### Building the Images

```bash
# Build all variants and compare sizes
.\build-all-docker.ps1

# Build specific variants
.\build-docker.ps1 -Type scratch        # Ultra-minimal (~10-15 MB)
.\build-docker.ps1 -Type distroless     # Secure minimal (~12-18 MB)  
.\build-docker.ps1 -Type standard       # Standard minimal (~20-30 MB)

# Build without cache
.\build-docker.ps1 -Type scratch -NoBuildCache

# Build and push to registry
.\build-docker.ps1 -Type distroless -Push -Registry "myregistry.azurecr.io"
```

### Manual Docker Build Commands

```bash
# Ultra-minimal (scratch base)
docker build -f Dockerfile.scratch -t dotnet-mcp-server2:scratch .

# Secure minimal (Google Distroless)  
docker build -f Dockerfile.distroless -t dotnet-mcp-server2:distroless .

# Standard minimal (Microsoft Chiseled)
docker build -f Dockerfile -t dotnet-mcp-server2:minimal .
```

### Running the Containers

```bash
# Basic test runs
docker run --rm dotnet-mcp-server2:scratch --version
docker run --rm dotnet-mcp-server2:distroless --version
docker run --rm dotnet-mcp-server2:minimal --version

# Interactive MCP mode
docker run --rm -i dotnet-mcp-server2:scratch
docker run --rm -i dotnet-mcp-server2:distroless
docker run --rm -i dotnet-mcp-server2:minimal

# With volume mounting for project access
docker run --rm -i -v "$(pwd):/workspace" -w /workspace dotnet-mcp-server2:distroless

# Background execution with auto-restart
docker run -d --name mcp-server --restart unless-stopped dotnet-mcp-server2:distroless

# Using Docker Compose
docker-compose up
```

### Container Management

```bash
# View all MCP server images
docker images dotnet-mcp-server2

# Compare image sizes
docker images dotnet-mcp-server2 --format "table {{.Repository}}\t{{.Tag}}\t{{.Size}}"

# Inspect specific image
docker inspect dotnet-mcp-server2:scratch

# Check running containers
docker ps --filter "ancestor=dotnet-mcp-server2"

# View container logs
docker logs mcp-server
```

## Production Deployment

### 1. Kubernetes Deployment (Ultra-Minimal)

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: dotnet-mcp-server2-scratch
  labels:
    app: dotnet-mcp-server2
    variant: scratch
spec:
  replicas: 3
  selector:
    matchLabels:
      app: dotnet-mcp-server2
      variant: scratch
  template:
    metadata:
      labels:
        app: dotnet-mcp-server2
        variant: scratch
    spec:
      containers:
      - name: mcp-server
        image: dotnet-mcp-server2:scratch
        resources:
          limits:
            memory: "64Mi"
            cpu: "200m"
          requests:
            memory: "32Mi"
            cpu: "50m"
        env:
        - name: DOTNET_CLI_TELEMETRY_OPTOUT
          value: "1"
        - name: DOTNET_NOLOGO
          value: "1"
        securityContext:
          runAsNonRoot: false  # scratch limitation
          readOnlyRootFilesystem: true
          allowPrivilegeEscalation: false
          capabilities:
            drop:
            - ALL
---
apiVersion: v1
kind: Service
metadata:
  name: dotnet-mcp-server2-service
spec:
  selector:
    app: dotnet-mcp-server2
  ports:
  - protocol: TCP
    port: 80
    targetPort: 8080
```

### 2. Kubernetes Deployment (Distroless - Recommended)

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: dotnet-mcp-server2-distroless
  labels:
    app: dotnet-mcp-server2
    variant: distroless
spec:
  replicas: 3
  selector:
    matchLabels:
      app: dotnet-mcp-server2
      variant: distroless
  template:
    metadata:
      labels:
        app: dotnet-mcp-server2
        variant: distroless
    spec:
      containers:
      - name: mcp-server
        image: dotnet-mcp-server2:distroless
        resources:
          limits:
            memory: "64Mi"
            cpu: "200m"
          requests:
            memory: "32Mi"
            cpu: "50m"
        env:
        - name: DOTNET_CLI_TELEMETRY_OPTOUT
          value: "1"
        - name: DOTNET_NOLOGO
          value: "1"
        securityContext:
          runAsNonRoot: true
          runAsUser: 65532  # nonroot user in distroless
          readOnlyRootFilesystem: true
          allowPrivilegeEscalation: false
          capabilities:
            drop:
            - ALL
        livenessProbe:
          exec:
            command:
            - /DotNetMcpServer2
            - --version
          initialDelaySeconds: 10
          periodSeconds: 30
        readinessProbe:
          exec:
            command:
            - /DotNetMcpServer2
            - --version
          initialDelaySeconds: 5
          periodSeconds: 10
```

### 3. Docker Swarm Service

```bash
# Ultra-minimal scratch variant
docker service create \
  --name dotnet-mcp-server2-scratch \
  --replicas 3 \
  --limit-memory 64m \
  --limit-cpu 0.2 \
  --reserve-memory 32m \
  --reserve-cpu 0.05 \
  --env DOTNET_CLI_TELEMETRY_OPTOUT=1 \
  --env DOTNET_NOLOGO=1 \
  dotnet-mcp-server2:scratch

# Secure distroless variant
docker service create \
  --name dotnet-mcp-server2-distroless \
  --replicas 3 \
  --limit-memory 64m \
  --limit-cpu 0.2 \
  --reserve-memory 32m \
  --reserve-cpu 0.05 \
  --env DOTNET_CLI_TELEMETRY_OPTOUT=1 \
  --env DOTNET_NOLOGO=1 \
  --user 65532:65532 \
  dotnet-mcp-server2:distroless
```

### 4. Azure Container Instances

```bash
# Scratch variant (ultra-minimal)
az container create \
  --resource-group myResourceGroup \
  --name dotnet-mcp-server2-scratch \
  --image dotnet-mcp-server2:scratch \
  --cpu 0.1 \
  --memory 0.125 \
  --environment-variables \
    DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_NOLOGO=1

# Distroless variant (recommended)
az container create \
  --resource-group myResourceGroup \
  --name dotnet-mcp-server2-distroless \
  --image dotnet-mcp-server2:distroless \
  --cpu 0.1 \
  --memory 0.125 \
  --environment-variables \
    DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_NOLOGO=1
```

## Performance Comparison

### Resource Usage by Variant

| Variant | Container Size | Memory Usage | CPU Usage | Startup Time | Security |
|---------|---------------|--------------|-----------|--------------|----------|
| **Scratch** | ~10-15 MB | ~20-30 MB | Low | ~100ms | Maximum |
| **Distroless** | ~12-18 MB | ~25-35 MB | Low | ~150ms | Very High |
| **Standard** | ~20-30 MB | ~30-40 MB | Low | ~200ms | High |
| **Traditional** | ~50-80 MB | ~40-60 MB | Medium | ~300ms | Medium |

### Startup Time Comparison

```bash
# Benchmark startup times
time docker run --rm dotnet-mcp-server2:scratch --version
time docker run --rm dotnet-mcp-server2:distroless --version
time docker run --rm dotnet-mcp-server2:minimal --version
```

### Memory Usage Analysis

```bash
# Monitor memory usage during runtime
docker stats dotnet-mcp-server2-scratch --no-stream
docker stats dotnet-mcp-server2-distroless --no-stream
docker stats dotnet-mcp-server2-minimal --no-stream
```

## Security Configuration

### Ultra-Minimal Security (Scratch)

```dockerfile
# Minimal attack surface - no OS, no shell, no packages
FROM scratch
COPY --from=build /app/publish/DotNetMcpServer2 /DotNetMcpServer2
ENTRYPOINT ["/DotNetMcpServer2"]
```

**Security Benefits:**
- ✅ Minimal attack surface (no OS components)
- ✅ No shell access
- ✅ No package vulnerabilities
- ⚠️ Runs as root (scratch limitation)

### Secure Minimal (Distroless)

```dockerfile
# Balanced security with essential features
FROM gcr.io/distroless/static-debian12:nonroot
COPY --from=build --chown=nonroot:nonroot /app/publish/DotNetMcpServer2 /DotNetMcpServer2
USER nonroot
ENTRYPOINT ["/DotNetMcpServer2"]
```

**Security Benefits:**
- ✅ Non-root execution
- ✅ No shell access
- ✅ No package manager
- ✅ CA certificates included
- ✅ Minimal attack surface

### Runtime Security Options

```bash
# Maximum security container execution
docker run --rm -i \
  --read-only \
  --security-opt no-new-privileges:true \
  --cap-drop ALL \
  --user 65532 \
  --tmpfs /tmp:noexec,nosuid,size=10m \
  dotnet-mcp-server2:distroless
```

## Monitoring and Logging

### Structured Logging

```yaml
# Docker Compose with logging
version: '3.8'
services:
  mcp-server-scratch:
    image: dotnet-mcp-server2:scratch
    logging:
      driver: "json-file"
      options:
        max-size: "5m"
        max-file: "3"
    deploy:
      resources:
        limits:
          memory: 64M
          cpus: '0.2'
        reservations:
          memory: 32M
          cpus: '0.05'

  mcp-server-distroless:
    image: dotnet-mcp-server2:distroless
    logging:
      driver: "json-file"
      options:
        max-size: "5m"
        max-file: "3"
    deploy:
      resources:
        limits:
          memory: 64M
          cpus: '0.2'
        reservations:
          memory: 32M
          cpus: '0.05'
```

### Health Monitoring

```bash
# Health check script
#!/bin/bash
CONTAINER_NAME="mcp-server"
IMAGE_VARIANT="distroless"  # or scratch, minimal

# Check container health
if docker ps --filter "name=$CONTAINER_NAME" --filter "status=running" | grep -q $CONTAINER_NAME; then
    echo "✅ Container $CONTAINER_NAME is running"
    
    # Check memory usage
    MEMORY=$(docker stats $CONTAINER_NAME --no-stream --format "{{.MemUsage}}")
    echo "📊 Memory usage: $MEMORY"
    
    # Check if responsive (for distroless/minimal variants)
    if [[ "$IMAGE_VARIANT" != "scratch" ]]; then
        if docker exec $CONTAINER_NAME /DotNetMcpServer2 --version >/dev/null 2>&1; then
            echo "✅ Service is responsive"
        else
            echo "⚠️ Service may not be responding"
        fi
    fi
else
    echo "❌ Container $CONTAINER_NAME is not running"
    exit 1
fi
```

## Troubleshooting

### Common Issues by Variant

#### Scratch Variant Issues
1. **Permission denied errors**
   - Scratch images run as root by default
   - Some operations may fail due to missing user context

2. **Missing CA certificates**
   - No SSL/TLS verification possible
   - Add certificates manually if needed

#### Distroless Variant Issues
1. **Debugging difficulties**
   - No shell access for debugging
   - Use debug variants or external tools

2. **Time zone issues**
   - UTC only unless timezone data is included
   - Use variants with timezone support if needed

#### General Container Issues
1. **Container won't start**
   ```bash
   # Check container logs
   docker logs dotnet-mcp-server2
   
   # Try interactive mode
   docker run --rm -it --entrypoint /bin/sh dotnet-mcp-server2:minimal
   ```

2. **High memory usage**
   ```bash
   # Monitor real-time usage
   docker stats --no-stream
   
   # Check for memory leaks
   docker exec container-name cat /proc/meminfo
   ```

3. **Slow startup in container**
   ```bash
   # Compare variants
   time docker run --rm dotnet-mcp-server2:scratch --version
   time docker run --rm dotnet-mcp-server2:distroless --version
   ```

## Recommendations

### Production Deployment Strategy

1. **For maximum security and minimal footprint**: Use **scratch** variant
2. **For balanced security and features**: Use **distroless** variant (recommended)
3. **For enterprise environments**: Use **standard** variant
4. **For development**: Use local AOT executable or JIT mode

### Configuration Selection Guide

Choose your deployment method based on:

- **Desktop development**: Local AOT executable
- **Container development**: Standard minimal Docker
- **Production (security-first)**: Distroless Docker
- **Production (size-first)**: Scratch Docker
- **Enterprise production**: Standard minimal Docker
- **Cloud functions/serverless**: Scratch or Distroless Docker

This comprehensive guide covers all deployment scenarios and should help you choose the optimal configuration for your specific use case and security requirements.
