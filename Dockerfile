# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy solution and project files
COPY ["CleanArchitectur.sln", "CleanArchitectur.sln"]
COPY ["API/API.csproj", "API/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Persistance/Persistance.csproj", "Persistance/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]

# Restore dependencies
RUN dotnet restore "CleanArchitectur.sln"

# Copy source code
COPY ["API/", "API/"]
COPY ["Application/", "Application/"]
COPY ["Domain/", "Domain/"]
COPY ["Persistance/", "Persistance/"]
COPY ["Infrastructure/", "Infrastructure/"]
COPY ["JwtProvider.cs", "."]

# Build the application
RUN dotnet build "CleanArchitectur.sln" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "API/API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published application
COPY --from=publish /app/publish .

# Create non-root user for security
RUN useradd -m -u 1001 appuser && chown -R appuser:appuser /app
USER appuser

# Expose port
EXPOSE 8080

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Run application
ENTRYPOINT ["dotnet", "API.dll"]
