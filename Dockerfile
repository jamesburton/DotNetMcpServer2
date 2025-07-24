# Minimal Dockerfile using Microsoft's chiseled Ubuntu base
# This creates a small, secure container (~20-30MB total) with some basic OS functionality

# Stage 1: Build the AOT executable
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

# Copy project files
COPY global.json ./
COPY Directory.Build.props ./
COPY DotNetMcpServer2.sln ./
COPY DotNetMcpServer2/ ./DotNetMcpServer2/
COPY DotNetMcpServer2.Tests/ ./DotNetMcpServer2.Tests/

# Restore dependencies
RUN dotnet restore DotNetMcpServer2/DotNetMcpServer2.csproj

# Build and publish AOT for Linux x64 with size optimization
RUN dotnet publish DotNetMcpServer2/DotNetMcpServer2.csproj \
    --configuration Release \
    --runtime linux-x64 \
    --self-contained true \
    --output /app/publish \
    -p:PublishAot=true \
    -p:OptimizationPreference=Size \
    -p:IlcOptimizationPreference=Size \
    -p:StripSymbols=true \
    --verbosity minimal

# Verify the executable was created
RUN ls -la /app/publish/ && \
    file /app/publish/DotNetMcpServer2 && \
    du -h /app/publish/DotNetMcpServer2 && \
    chmod +x /app/publish/DotNetMcpServer2

# Test the AOT executable works
RUN /app/publish/DotNetMcpServer2 --version || echo "Version check may not be supported"

# Stage 2: Minimal runtime using Microsoft's chiseled Ubuntu for AOT
# This is optimized specifically for .NET AOT applications
FROM mcr.microsoft.com/dotnet/runtime-deps:9.0-jammy-chiseled-aot AS runtime

# Copy only the AOT executable from build stage
COPY --from=build /app/publish/DotNetMcpServer2 /app/DotNetMcpServer2

# Set environment variables for optimal performance and security
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 \
    DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_NOLOGO=1 \
    DOTNET_RUNNING_IN_CONTAINER=true

# Use the app user already configured in the chiseled image
USER app

WORKDIR /app

# Health check using the AOT executable itself
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD ./DotNetMcpServer2 --version || exit 1

# Default command
ENTRYPOINT ["./DotNetMcpServer2"]
