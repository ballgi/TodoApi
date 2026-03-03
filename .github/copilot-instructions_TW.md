# TodoApi Copilot 指南

## 架構概述

**模式**: Repository-Service-Controller（三層架構）
- **控制器** (Controllers/TodoController.cs): ASP.NET Core API 端點、路由、HTTP 相關事務
- **服務** (Services/TodoService.cs): 業務邏輯層，使用日誌進行審計追蹤（繁體中文）
- **存儲庫** (Repositories/TodoRepository.cs): 資料存取抽象層，使用 Entity Framework Core
- **DbContext** (Data/TodoDbContext.cs): SQL Server 資料庫，`CreatedAt` 有預設 SQL 時間戳

**核心原則**: 三層都使用介面 (`ITodoService`、`ITodoRepository`) 並透過 DI 註冊。業務邏輯不能在控制器中。

## 資料模型與資料庫

- 模型: Models/Todo.cs - `Context` 屬性（已從 `Title` 重新命名）、`IsComplete`、`CreatedAt`（SQL 預設）、`UpdatedAt`（可為 null）
- 資料庫: 透過 `appsettings.json` 中的連線字串 `DefaultConnection` 連接 SQL Server
- 框架: Entity Framework Core 9.0.2，遷移位於 `Migrations/` 資料夾
- **遷移工作流程**: 修改 `Todo` 模型 → 透過 `dotnet ef migrations add <名稱>` 新增遷移 → 更新資料庫

## 快速總結

- **模式**: 控制器 → 服務 → 存儲庫（使用 `ITodoService` / `ITodoRepository` 進行 DI）
- **技術**: .NET 8 (net8.0)、EF Core 9.0.2、Serilog 日誌。參考 `TodoApi.csproj`
- **資料庫**: 透過 `appsettings*.json` 中的 `DefaultConnection` 連接 SQL Server；遷移位於 `Migrations/` 資料夾

## 具體慣例與模式

- 模型欄位: 使用 `Context`（非 `Title`） — 參考 Models/Todo.cs
- 日期欄位: `CreatedAt` 已存在且有 SQL 預設值；服務在建立時設定 `CreatedAt`，在更新時設定 `UpdatedAt`。新增建立/更新流程時維持此模式
- 錯誤對應: 服務返回 `null` 或拋出 `KeyNotFoundException`；控制器轉譯為 `NotFound()` 或 `NoContent()`
- **日誌**: 使用 `ILogger<T>` 和繁體中文訊息，與現有條目保持一致

## 新增端點 — 最小化步驟

1. 在 `Interfaces/ITodoService.cs` 新增方法簽名（必要時在 `ITodoRepository.cs` 也新增）
2. 在 `Services/TodoService.cs` 實現業務邏輯，使用 `_logger.LogInformation/Warning`
3. 在 `Repositories/TodoRepository.cs` 新增 EF 呼叫，使用 `_context.Todos` 和 `SaveChangesAsync()`
4. 在 `Controllers/TodoController.cs` 公開，返回 `ActionResult<T>` 和適當的狀態碼

## 建置/執行/資料庫指令

- **建置**: `dotnet build TodoApi.csproj`
- **執行**: `dotnet run --project TodoApi.csproj`
- **遷移**:
  - 新增: `dotnet ef migrations add <名稱>`
  - 更新資料庫: `dotnet ef database update`

## 重要檔案

- `Program.cs` — 服務註冊、Serilog、CORS
- `Controllers/TodoController.cs` — 端點慣例和狀態碼
- `Services/TodoService.cs` — 業務規則和日誌位置
- `Repositories/TodoRepository.cs` — EF Core 使用模式
- `Data/TodoDbContext.cs` & `Models/Todo.cs` — 模型約束和資料庫預設值
