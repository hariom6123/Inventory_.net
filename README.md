# 📦 Inventory Management System

An enterprise-style **ASP.NET Core MVC (.NET 8)** reference application built with
**Clean Architecture**, **Entity Framework Core (SQLite)**, **xUnit**, **Docker**, and a
full **GitHub Actions CI/CD** pipeline with **Trivy** security scanning.

This solution is designed to demonstrate real-world software engineering and DevOps best
practices: layered architecture, dependency injection, repository + service patterns,
automated testing, containerization, and continuous security scanning.

---

## 🧭 Table of Contents

1. [Project Overview](#-project-overview)
2. [Architecture Diagram](#-architecture-diagram)
3. [Tech Stack](#-tech-stack)
4. [Folder Structure](#-folder-structure)
5. [Business Features](#-business-features)
6. [Local Setup Instructions](#-local-setup-instructions)
7. [SQLite Setup](#-sqlite-setup)
8. [Running the Tests](#-running-the-tests)
9. [Docker Instructions](#-docker-instructions)
10. [GitHub Actions CI/CD Flow](#-github-actions-cicd-flow)
11. [Security Practices](#-security-practices)
12. [License](#-license)

---

## 🎯 Project Overview

The Inventory Management System is a small but realistic business application that
manages products, categories, and inventory transactions. It is deliberately written to
be a **demonstrable reference** for:

| Concern | How it is demonstrated |
|---|---|
| **ASP.NET Core MVC development** | `InventoryManagement.Web` project with controllers, view models, Razor views, Bootstrap UI |
| **Clean Architecture** | Strict layer boundaries: `Domain` ← `Application` ← `Infrastructure` → `Web` |
| **Entity Framework Core** | `ApplicationDbContext`, Fluent API configurations, code-first migrations, `EnsureCreated` seed |
| **SQLite** | File-based `inventory.db` (created automatically on first run) |
| **Unit Testing** | xUnit suite covering `ProductService`, `CategoryService`, `InventoryService` |
| **Docker** | Multi-stage `Dockerfile` (SDK → ASP.NET runtime), non-root user, health check |
| **GitHub Actions CI/CD** | Six-stage pipeline: Build → Tests → Trivy FS → Docker Build → Trivy Image → Publish |
| **Trivy Security Scanning** | Filesystem scan + image scan, SARIF uploaded to GitHub Security tab |
| **DevOps Best Practices** | Cached NuGet, GHCR layer cache, SARIF, scoped permissions, concurrency control |

---

## 🏛 Architecture Diagram

```
                    ┌─────────────────────────────────────────────┐
                    │              Presentation Layer             │
                    │                                             │
                    │   InventoryManagement.Web (ASP.NET MVC)     │
                    │   • Controllers, ViewModels, Razor Views    │
                    │   • Bootstrap 5 UI, jQuery validation       │
                    └────────────────────┬────────────────────────┘
                                         │ depends on
                                         ▼
                    ┌─────────────────────────────────────────────┐
                    │              Application Layer              │
                    │                                             │
                    │   InventoryManagement.Application           │
                    │   • DTOs, Service interfaces                │
                    │   • ProductService, CategoryService,        │
                    │     InventoryService, DashboardService      │
                    │   • Validators, AutoMapper profile          │
                    └────────────────────┬────────────────────────┘
                                         │ depends on
                                         ▼
                    ┌─────────────────────────────────────────────┐
                    │                Domain Layer                 │
                    │                                             │
                    │   InventoryManagement.Domain                │
                    │   • Entities: Product, Category,            │
                    │     InventoryTransaction                    │
                    │   • Enums: StockStatus, TransactionType     │
                    │   • Interfaces: IRepository<T>, IUnitOfWork │
                    └────────────────────▲────────────────────────┘
                                         │ implements interfaces
                                         │
                    ┌────────────────────┴────────────────────────┐
                    │            Infrastructure Layer             │
                    │                                             │
                    │   InventoryManagement.Infrastructure        │
                    │   • ApplicationDbContext (EF Core / SQLite) │
                    │   • Fluent API entity configurations        │
                    │   • Generic Repository<T>, UnitOfWork       │
                    │   • SeedData                                │
                    └─────────────────────────────────────────────┘
```

Dependency rule: **dependencies point inward**. `Domain` has no external dependencies;
`Application` depends only on `Domain`; `Infrastructure` implements `Application` and
`Domain` interfaces; `Web` wires everything up through `Program.cs` and DI.

---

## 🧰 Tech Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 8 (LTS) |
| Web framework | ASP.NET Core MVC |
| ORM | Entity Framework Core 8 |
| Database | SQLite (`Microsoft.EntityFrameworkCore.Sqlite`) |
| Object mapping | AutoMapper |
| UI | Bootstrap 5 + Bootstrap Icons + jQuery + jQuery Validation |
| Testing | xUnit + EF Core InMemory |
| Container | Docker (multi-stage build) |
| CI/CD | GitHub Actions |
| Security scanning | Trivy (filesystem + image) |
| Container registry | GitHub Container Registry (GHCR) |

---

## 📁 Folder Structure

```
Inventory_.net/
│
├── InventoryManagementSystem.sln          # Solution file
├── Directory.Build.props                   # Central MSBuild properties
├── README.md                               # ← this file
├── Dockerfile                              # Multi-stage Docker build
├── .dockerignore                           # Docker build exclusions
│
├── .github/
│   └── workflows/
│       └── ci-cd.yml                       # GitHub Actions pipeline (6 stages)
│
├── src/
│   ├── InventoryManagement.Domain/
│   │   ├── Entities/
│   │   │   ├── Product.cs
│   │   │   ├── Category.cs
│   │   │   └── InventoryTransaction.cs
│   │   ├── Enums/
│   │   │   ├── StockStatus.cs
│   │   │   └── TransactionType.cs
│   │   └── Interfaces/
│   │       ├── IRepository.cs
│   │       └── IUnitOfWork.cs
│   │
│   ├── InventoryManagement.Application/
│   │   ├── DTOs/
│   │   │   ├── ProductDto.cs
│   │   │   ├── CategoryDto.cs
│   │   │   ├── InventoryTransactionDto.cs
│   │   │   ├── PaginatedResultDto.cs
│   │   │   └── DashboardSummaryDto.cs
│   │   ├── Interfaces/
│   │   │   ├── IProductService.cs
│   │   │   ├── ICategoryService.cs
│   │   │   └── IInventoryService.cs
│   │   ├── Services/
│   │   │   ├── ProductService.cs
│   │   │   ├── CategoryService.cs
│   │   │   ├── InventoryService.cs
│   │   │   └── DashboardService.cs
│   │   ├── Common/
│   │   │   ├── ProductValidator.cs
│   │   │   ├── CategoryValidator.cs
│   │   │   ├── ValidationResult.cs
│   │   │   └── ValidationException.cs
│   │   ├── Mappings/
│   │   │   └── MappingProfile.cs
│   │   └── ApplicationServiceCollectionExtensions.cs
│   │
│   ├── InventoryManagement.Infrastructure/
│   │   ├── Data/
│   │   │   ├── ApplicationDbContext.cs
│   │   │   └── SeedData.cs
│   │   ├── Configurations/
│   │   │   ├── ProductConfiguration.cs
│   │   │   ├── CategoryConfiguration.cs
│   │   │   └── InventoryTransactionConfiguration.cs
│   │   ├── Repositories/
│   │   │   ├── Repository.cs
│   │   │   └── UnitOfWork.cs
│   │   └── InfrastructureServiceCollectionExtensions.cs
│   │
│   └── InventoryManagement.Web/
│       ├── Controllers/
│       │   ├── DashboardController.cs
│       │   ├── ProductsController.cs
│       │   ├── CategoriesController.cs
│       │   ├── InventoryController.cs
│       │   └── ErrorController.cs
│       ├── ViewModels/
│       │   ├── DashboardViewModel.cs
│       │   ├── ProductIndexViewModel.cs
│       │   ├── InventoryTransactionViewModel.cs
│       │   └── InventoryHistoryViewModel.cs
│       ├── Views/
│       │   ├── Dashboard/Index.cshtml
│       │   ├── Products/  (Index, Create, Edit, Delete, Details)
│       │   ├── Categories/ (Index, Create, Edit, Delete)
│       │   ├── Inventory/ (StockIn, StockOut, Adjust, History)
│       │   ├── Error/     (Index, NotFound)
│       │   ├── Shared/_Layout.cshtml
│       │   ├── Shared/_ValidationScriptsPartial.cshtml
│       │   ├── _ViewImports.cshtml
│       │   └── _ViewStart.cshtml
│       ├── wwwroot/        (Bootstrap, jQuery, site.css, site.js)
│       ├── Program.cs
│       ├── GlobalUsings.cs
│       ├── appsettings.json
│       └── appsettings.Development.json
│
└── tests/
    └── InventoryManagement.Tests/
        ├── Helpers/TestHelpers.cs
        └── Services/
            ├── ProductServiceTests.cs
            ├── CategoryServiceTests.cs
            └── InventoryServiceTests.cs
```

---

## 🚀 Business Features

### 1. Dashboard
- Total Products
- Total Categories
- Low-Stock Products (≤ threshold)
- Out-of-Stock Products (Quantity = 0)
- Total Inventory Value (Σ Price × Quantity)
- Recent inventory transactions

### 2. Product Management
- Full CRUD with model validation
- Fields: `ProductId`, `ProductName`, `SKU`, `Description`, `Price`, `Quantity`, `CategoryId`, `CreatedDate`
- Pagination on the list view
- Search by name and SKU, filter by category and stock status
- Validation: name required, SKU required, price > 0, quantity ≥ 0

### 3. Category Management
- Full CRUD
- Fields: `CategoryId`, `CategoryName`, `Description`
- Cascade-protection: cannot delete a category that still owns products

### 4. Inventory Operations
- **Stock In** — adds quantity and logs an `InventoryTransaction` (type = `StockIn`)
- **Stock Out** — removes quantity and logs an `InventoryTransaction` (type = `StockOut`)
- **Adjustment** — sets a new absolute quantity with notes
- **History** — paginated log of every transaction
- Business rules: stock **cannot** become negative; every change is logged with timestamp and optional notes

### 5. Search & Filtering
- Search products by name and SKU
- Filter by category
- Filter by stock status (`All`, `In Stock`, `Low Stock`, `Out of Stock`)

---

## 🛠 Local Setup Instructions

### Prerequisites

| Tool | Version |
|---|---|
| .NET SDK | **8.0.x** ([download](https://dotnet.microsoft.com/download/dotnet/8.0)) |
| Git | latest |
| Docker Desktop (optional) | latest |
| OS | Windows / macOS / Linux |

### Clone & build

```bash
git clone <your-repo-url> InventoryManagementSystem
cd InventoryManagementSystem

# Restore + build
dotnet restore InventoryManagementSystem.sln
dotnet build  InventoryManagementSystem.sln --configuration Release
```

### Run locally

```bash
dotnet run --project src/InventoryManagement.Web/InventoryManagement.Web.csproj
```

The app will listen on:
- `http://localhost:5xxx` (assigned by launch settings) — see `Properties/launchSettings.json`
- or the URL printed in the console

Navigate to it in your browser. On first run the SQLite database
(`inventory.db`) is created automatically and seeded with the demo data.

### Default seed data

**Categories:** Electronics, Furniture, Accessories

**Products:**

| Name | SKU | Category | Price | Quantity |
|---|---|---|---|---|
| Laptop | ELEC-LAP-001 | Electronics | $1299.99 | 25 |
| Monitor | ELEC-MON-001 | Electronics | $399.99 | 40 |
| Keyboard | ACC-KBD-001 | Accessories | $89.99 | 60 |
| Mouse | ACC-MOU-001 | Accessories | $29.99 | 8 (low stock) |
| Office Chair | FUR-CHR-001 | Furniture | $249.99 | 0 (out of stock) |

---

## 🗄 SQLite Setup

The application uses **SQLite** via EF Core. There is nothing to install — the provider
ships as the `Microsoft.EntityFrameworkCore.Sqlite` NuGet package and the database file
is created on first run.

### Connection string

Defined in `src/InventoryManagement.Web/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=inventory.db"
  }
}
```

- In development the database lives next to the web app's working directory.
- Inside the Docker container the database lives at `/app/data/inventory.db` and the
  Dockerfile sets
  `ConnectionStrings__DefaultConnection="Data Source=/app/data/inventory.db"`.

### Migrating / resetting the schema

The project uses `EnsureCreated` for first-run simplicity. To reset:

```bash
# Stop the app, then delete the database file
rm inventory.db                       # macOS / Linux / Git Bash
del inventory.db                      # Windows cmd

# Re-run; the DB and seed data are recreated automatically
dotnet run --project src/InventoryManagement.Web/InventoryManagement.Web.csproj
```

### Adding EF Core migrations (optional)

If you want to switch from `EnsureCreated` to migrations:

```bash
dotnet tool install --global dotnet-ef

dotnet ef migrations add InitialCreate \
  --project src/InventoryManagement.Infrastructure \
  --startup-project src/InventoryManagement.Web

dotnet ef database update \
  --project src/InventoryManagement.Infrastructure \
  --startup-project src/InventoryManagement.Web
```

---

## 🧪 Running the Tests

```bash
# Run the entire xUnit suite
dotnet test InventoryManagementSystem.sln

# With code coverage
dotnet test InventoryManagementSystem.sln \
  --collect:"XPlat Code Coverage"

# Filter to one class
dotnet test --filter "FullyQualifiedName~ProductServiceTests"
```

The tests cover:

| Service | Scenarios |
|---|---|
| `ProductServiceTests` | Add product, update product, delete product, search, validation failures |
| `CategoryServiceTests` | Create category, update category, delete category (incl. cascade protection) |
| `InventoryServiceTests` | Stock in, stock out, **prevent negative inventory**, history query |

Tests run against an in-memory EF Core database (no SQLite file required for CI).

---

## 🐳 Docker Instructions

### Build the image

```bash
docker build -t inventorymanagement .
```

The `Dockerfile` is a **multi-stage build**:

1. **`build`** stage — `mcr.microsoft.com/dotnet/sdk:8.0`
   - restores the whole solution
   - publishes `InventoryManagement.Web` to `/app/publish`
2. **`runtime`** stage — `mcr.microsoft.com/dotnet/aspnet:8.0`
   - copies the publish output
   - creates a non-root `inventory` user (UID 1001)
   - exposes **port 80**
   - configures the SQLite DB path to `/app/data/inventory.db`
   - defines a `HEALTHCHECK` against `/`

### Run the container

```bash
docker run -d \
  --name inventory \
  -p 8080:80 \
  inventorymanagement
```

Open **http://localhost:8080** in your browser.

To persist the SQLite database outside the container:

```bash
docker run -d \
  --name inventory \
  -p 8080:80 \
  -v inventory-data:/app/data \
  inventorymanagement
```

### Useful commands

```bash
docker logs -f inventory            # tail logs
docker exec -it inventory bash      # shell inside container
docker stop inventory               # stop
docker rm inventory                 # remove
```

---

## ⚙️ GitHub Actions CI/CD Flow

The pipeline at `.github/workflows/ci-cd.yml` runs **six sequential / parallel stages**
on every push, pull request, and tag:

```
                       ┌──────────────┐
                       │   Checkout   │
                       └──────┬───────┘
                              │
              ┌───────────────┼───────────────┐
              ▼               ▼               ▼
        ┌──────────┐    ┌──────────┐    (other jobs)
        │  Build   │    │  Unit    │
        │          │    │  Tests   │
        └────┬─────┘    └────┬─────┘
             │               │
             ▼               │
        ┌─────────────┐      │
        │ Trivy FS    │◄─────┘
        │   Scan      │
        └──────┬──────┘
               │
               ▼
        ┌─────────────┐
        │ Docker      │
        │ Build + Push│  → ghcr.io/<owner>/inventorymanagement
        └──────┬──────┘
               │
               ▼
        ┌─────────────┐
        │ Trivy Image │
        │   Scan      │
        └──────┬──────┘
               │
               ▼
        ┌─────────────┐
        │ Publish     │  → artifact bundle (30-day retention)
        │ Artifact    │
        └─────────────┘
```

| # | Stage | Purpose | Triggered on |
|---|---|---|---|
| 1 | **Build** | `dotnet restore` + `dotnet build --configuration Release` + `dotnet publish` → upload `webapp-publish` artifact | every run |
| 2 | **Unit Tests** | `dotnet test` with TRX logger + XPlat coverage → upload `test-results` + `code-coverage` artifacts | every run |
| 3 | **Trivy FS Scan** | `aquasecurity/trivy-action@0.24.0` with `scan-type: fs`, severity `CRITICAL,HIGH`. Uploads SARIF to **GitHub Security tab** | after Build |
| 4 | **Docker Build** | `docker/build-push-action@v5` with GHCR layer cache. Pushes to `ghcr.io/<owner>/inventorymanagement` (skipped on PRs) | after Build + Tests |
| 5 | **Trivy Image Scan** | Scans the freshly built image (`${{ env.REGISTRY_IMAGE }}:${{ github.sha }}`) | after Docker Build |
| 6 | **Publish Artifact** | Aggregates a markdown `SUMMARY.md` with build metadata and uploads as `inventory-management-artifact` (30-day retention) | after all five |

### Required permissions / secrets

- `GITHUB_TOKEN` (provided automatically by GitHub) — used to log in to GHCR.
- Forks do not have `packages: write`; the workflow **skips image push on PRs** by
  checking `github.event_name != 'pull_request'`.

### Triggering a release

```bash
git tag v1.0.0
git push origin v1.0.0
```

The pipeline automatically tags the image with `v1.0.0`, `1.0`, and `latest`.

---

## 🔐 Security Practices

The codebase demonstrates the following security hygiene:

- **Input validation** at both the entity level (`[Required]`, `[StringLength]`) and the
  service level (`ProductValidator`, `CategoryValidator`).
- **Anti-forgery tokens** on every state-changing form (`@Html.AntiForgeryToken()` and the
  `[ValidateAntiForgeryToken]` attribute).
- **Dependency Injection** throughout — no `new`-ing of services or DbContexts in
  controllers.
- **Secure configuration** — secrets belong in environment variables or user secrets,
  never in source. The Docker image sets `ConnectionStrings__DefaultConnection` via the
  `ENV` directive.
- **Non-root container** — the runtime stage creates a `inventory` user (UID 1001) and
  drops privileges with `USER inventory`.
- **Trivy scanning** on every pipeline run, with results published to the GitHub Security
  tab so vulnerabilities appear alongside CodeQL findings.
- **Scoped permissions** in the workflow file (`contents: read`, `packages: write`,
  `security-events: write`).
- **Concurrency cancellation** so a force-push doesn't leave stale secrets or images
  behind.

---

## 📝 License

This project is provided as an **architectural reference** for educational and
demonstration purposes. You are free to adapt, extend, and use it as a starting point
for your own inventory or stock-management systems.

---

> Built with ASP.NET Core 8, EF Core, SQLite, xUnit, Docker, and GitHub Actions.
