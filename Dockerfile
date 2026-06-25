# ==============================================================================
# Inventory Management System — multi-stage Dockerfile (.NET 8)
# ==============================================================================
# Build:   docker build -t inventorymanagement .
# Run:     docker run -d -p 8080:80 --name inventory inventorymanagement
# Browse:  http://localhost:8080
# ==============================================================================

# ------------------------------------------------------------------------------
# Stage 1 — Build
# ------------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first to leverage Docker layer caching.
COPY InventoryManagementSystem.sln ./
COPY src/InventoryManagement.Domain/InventoryManagement.Domain.csproj src/InventoryManagement.Domain/
COPY src/InventoryManagement.Application/InventoryManagement.Application.csproj src/InventoryManagement.Application/
COPY src/InventoryManagement.Infrastructure/InventoryManagement.Infrastructure.csproj src/InventoryManagement.Infrastructure/
COPY src/InventoryManagement.Web/InventoryManagement.Web.csproj src/InventoryManagement.Web/
COPY tests/InventoryManagement.Tests/InventoryManagement.Tests.csproj tests/InventoryManagement.Tests/

# Restore dependencies for the solution.
RUN dotnet restore InventoryManagementSystem.sln

# Copy the rest of the source code.
COPY . .

# Build the solution in Release mode.
RUN dotnet build InventoryManagementSystem.sln --configuration Release --no-restore

# Publish the Web project to /app/publish.
RUN dotnet publish src/InventoryManagement.Web/InventoryManagement.Web.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish

# ------------------------------------------------------------------------------
# Stage 2 — Runtime
# ------------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create a non-root user for runtime hardening.
RUN groupadd --system --gid 1001 inventory \
    && useradd  --system --uid 1001 --gid inventory --home /app inventory

# Copy the published application from the build stage.
COPY --from=build /app/publish ./

# Ensure the non-root user owns the application files.
RUN chown -R inventory:inventory /app

USER inventory

# SQLite database lives in /app/data inside the container.
RUN mkdir -p /app/data && chown -R inventory:inventory /app/data
ENV ASPNETCORE_URLS=http://+:80 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    ConnectionStrings__DefaultConnection="Data Source=/app/data/inventory.db"

EXPOSE 80

# Health-check probes the dashboard route.
HEALTHCHECK --interval=30s --timeout=5s --start-period=20s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:80/ || exit 1

ENTRYPOINT ["dotnet", "InventoryManagement.Web.dll"]
