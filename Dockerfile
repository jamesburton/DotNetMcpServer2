# Use the official .NET 9 runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["DotNetMcpServer2/DotNetMcpServer2.csproj", "DotNetMcpServer2/"]
COPY ["global.json", "./"]
RUN dotnet restore "DotNetMcpServer2/DotNetMcpServer2.csproj"

# Copy source code and build
COPY . .
WORKDIR "/src/DotNetMcpServer2"
RUN dotnet build "DotNetMcpServer2.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "DotNetMcpServer2.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create non-root user for security
RUN adduser --disabled-password --gecos '' mcpuser && chown -R mcpuser /app
USER mcpuser

# Set environment variables
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV DOTNET_NOLOGO=1

ENTRYPOINT ["dotnet", "DotNetMcpServer2.dll"]
