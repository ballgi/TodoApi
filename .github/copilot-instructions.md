# TodoApi Copilot Instructions

## Architecture Overview

**Pattern**: Repository-Service-Controller (3-layer)
- **Controllers** ([Controllers/TodoController.cs](Controllers/TodoController.cs)): ASP.NET Core API endpoints, routing, HTTP concerns only
- **Services** ([Services/TodoService.cs](Services/TodoService.cs)): Business logic layer, uses logging for audit trails (Chinese locale)
- **Repositories** ([Repositories/TodoRepository.cs](Repositories/TodoRepository.cs)): Data access abstraction using Entity Framework Core
- **DbContext** ([Data/TodoDbContext.cs](Data/TodoDbContext.cs)): SQL Server database with default SQL timestamp on `CreatedAt`

**Key Principle**: All three layers are interface-based (`ITodoService`, `ITodoRepository`) and DI-registered. No business logic belongs in controllers.

## Data Model & Database

- Model: [Models/Todo.cs](Models/Todo.cs) - `Context` property (renamed from `Title`), `IsComplete`, `CreatedAt` (SQL default), `UpdatedAt` (nullable)
- Database: SQL Server via connection string `DefaultConnection` in `appsettings.json`
- Framework: Entity Framework Core 9.0.2, migrations in `Migrations/` folder
- **Migration Workflow**: When changing `Todo` model → add migration via `dotnet ef migrations add <name>` → update database

## Build & Development Workflow

**Build**: `dotnet build TodoApi.csproj` (via VS Code build task)

**Database Setup**:
1. Update connection string in [appsettings.json](appsettings.json) or [appsettings.Development.json](appsettings.Development.json)
2. Run migrations: `dotnet ef database update`

**Logging Configuration**:
- Primary: Serilog (configured in `Program.cs`) outputs to Console
- Secondary: NLog available but commented out in `Program.cs` (kept for reference)
- Log config: [nlog.config](nlog.config)
- Services use `ILogger<T>` injected, with Chinese log messages (example: "正在獲取所有待辦事項")

**CORS Policy** ([Program.cs](Program.cs#L18-L26)): "AllowAll" enables any origin—restrict before production.

## Common Patterns

### Adding New Endpoints
1. Add method to [Interfaces/ITodoService.cs](Interfaces/ITodoService.cs)
2. Implement in [Services/TodoService.cs](Services/TodoService.cs) with logging
3. Add repository method to [Interfaces/ITodoRepository.cs](Interfaces/ITodoRepository.cs)
4. Implement in [Repositories/TodoRepository.cs](Repositories/TodoRepository.cs) using `_context` DbSet
5. Add controller action in [Controllers/TodoController.cs](Controllers/TodoController.cs), return `ActionResult<T>`

### Error Handling Convention
- Service returns `null` for "not found" (e.g., `GetTodoByIdAsync`)
- Controller maps `null` to `NotFound()` (HTTP 404)
- Service logs warnings for missing entities
- Validation exceptions: Log and return appropriate status code (see Update action pattern)

### DateTime Handling
- `CreatedAt`: Set by SQL Server (`GETDATE()`)—**do not set in service**
- `UpdatedAt`: Set by service before update (example in [Services/TodoService.cs](Services/TodoService.cs#L53))

## API Endpoints

```
GET    /todo          → GetAll
GET    /todo/{id}     → GetById (404 if not found)
POST   /todo          → Create (returns 201 with location header)
PUT    /todo/{id}     → Update (404 if not found, 400 if invalid)
DELETE /todo/{id}     → Delete (404 if not found)
```

Test via [TodoApi.http](TodoApi.http) (VS Code REST Client extension).

## Dependencies

- ASP.NET Core 8.0
- Entity Framework Core 9.0.2 (SQL Server provider)
- Serilog 9.0.0 + Console sink
- Swashbuckle 6.4.0 (Swagger)
- Nullable reference types enabled—all parameters/returns should be marked `?` if nullable

## Constraints & Notes

- **Internationalization**: Logs and error messages use Chinese (zh-TW)—maintain consistency
- **Naming**: Property `Context` (not `Title`) per recent migration
- **Testing**: No test project yet; add via separate `.csproj` if needed
- **Docker**: [Dockerfile](Dockerfile) and [docker-compose.yml](docker-compose.yml) present—verify SQL Server connection on deploy
