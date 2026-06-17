# Coding Standards

> Unified coding conventions for ChipmeoFoodstore — covering both Svelte 5 (frontend) and .NET Clean Architecture (backend).

---

## Table of Contents

1. [General Principles](#1-general-principles)
2. [.NET / C# Standards](#2-net--c-standards)
   - [2.1 Clean Architecture Rules](#21-clean-architecture-rules)
   - [2.2 Naming Conventions](#22-naming-conventions)
   - [2.3 Project Structure](#23-project-structure)
   - [2.4 Coding Patterns](#24-coding-patterns)
   - [2.5 Dependency Injection](#25-dependency-injection)
   - [2.6 Entity Framework Core](#26-entity-framework-core)
   - [2.7 Controllers & API](#27-controllers--api)
   - [2.8 Error Handling](#28-error-handling)
3. [Svelte 5 / Frontend Standards](#3-svelte-5--frontend-standards)
   - [3.1 Runes (Reactivity)](#31-runes-reactivity)
   - [3.2 Component Conventions](#32-component-conventions)
   - [3.3 SvelteKit Routing](#33-sveltekit-routing)
   - [3.4 State Management](#34-state-management)
   - [3.5 API Layer](#35-api-layer)
   - [3.6 TypeScript](#36-typescript)
   - [3.7 Styling](#37-styling)
   - [3.8 Naming Conventions](#38-naming-conventions)
   - [3.9 Frontend Configuration](#39-frontend-configuration-environment-variables)
   - [3.10 Flowbite Interactive Components](#310-flowbite-interactive-components)
   - [3.11 Icons (Iconify + Tabler)](#311-icons-iconify--tabler)
4. [Cross-Cutting Concerns](#4-cross-cutting-concerns)
   - [4.1 Security](#41-security)
   - [4.2 Git](#42-git)
   - [4.3 Testing](#43-testing)

---

## 1. General Principles

### SOLID

| Principle | Meaning |
|---|---|
| **S**ingle Responsibility | One class/component = one reason to change |
| **O**pen/Closed | Open for extension, closed for modification |
| **L**iskov Substitution | Subtypes must be substitutable for base types |
| **I**nterface Segregation | Small, focused interfaces over large ones |
| **D**ependency Inversion | Depend on abstractions, not concretions |

### Dependency Rule (Clean Architecture)

> Source code dependencies must point **inward**. Inner layers never know about outer layers.

```
  Web (Controllers) ──► Usecase (Services) ──► Core (Entities)
       │                        │
       ▼                        ▼
Infrastructure (EF, FTP) ──► Usecase (via DI)
```

- **Core**: zero dependencies. Pure C# classes only.
- **Usecase**: references Core only. Interfaces defined here, implementations injected.
- **Infrastructure**: references Usecase. Implements interfaces.
- **Web**: references Usecase. Composition root (DI registration).

### The Boy Scout Rule

> Leave the codebase cleaner than you found it.

---

## 2. .NET / C# Standards

### 2.1 Clean Architecture Rules

#### DO ✅

| Rule | Example |
|---|---|
| Put business logic in Usecase Services | `CreateOrderService.cs` |
| Define interfaces in Usecase layer | `IOrderRepository` in `Usecase/Interfaces/Repositories/` |
| Keep Core entities pure POCOs | No EF Core attributes, no base class dependencies |
| Use records for DTOs | `public record CreateOrderRequest(...)` |
| Register DI in each layer's `Extensions/` | `public static IServiceCollection AddInfrastructure(this IServiceCollection services, ...)` |
| Use `CancellationToken` in async methods | `Task<Order> GetByIdAsync(int id, CancellationToken ct)` |

#### DON'T ❌

| Rule | Example |
|---|---|
| Don't reference Infrastructure from Web directly | Web → Usecase only (Infrastructure is wired via DI) |
| Don't put EF Core attributes on Core entities | Use Fluent API in Infrastructure configurations |
| Don't use `AutoMapper` implicitly | Prefer manual mapping or explicit AutoMapper profiles |
| Don't access `HttpContext` in Usecase | Pass required data as parameters |
| Don't put business logic in controllers | Controllers call services, nothing more |

### 2.2 Naming Conventions

#### General

| Element | Convention | Example |
|---|---|---|
| Namespaces | `PascalCase`, dot-separated | `ChipmeoApis.Usecase.Services` |
| Classes | `PascalCase`, noun | `OrderService`, `MenuController` |
| Interfaces | `I` + `PascalCase` | `IOrderRepository`, `IAuthService` |
| Methods | `PascalCase`, verb | `CreateAsync()`, `GetByIdAsync()` |
| Method parameters | `camelCase` | `int orderId`, `string phone` |
| Local variables | `camelCase` | `var result = ...` |
| Private fields | `_camelCase` | `_repository`, `_logger` |
| Constants | `PascalCase` | `MaxRetryCount = 3` |
| Enums | `PascalCase` (singular) | `public enum OrderStatus` |
| Properties | `PascalCase` | `public int Id { get; set; }` |
| Boolean fields/methods | Start with `Is`, `Has`, `Can` | `IsActive`, `HasPermission()` |

#### Project & Folder

| Pattern | Example |
|---|---|
| `[Company].[Layer]` | `ChipmeoApis.Core` |
| `[Layer].Services` | `Usecase.Services.OrderService` |
| `[Layer].Controllers` | `Web.Controllers.OrdersController` |
| `[Layer].Interfaces.Repositories` | `Usecase.Interfaces.Repositories.IOrderRepository` |
| Plural folder names | `Entities/`, `Controllers/`, `Services/` |

### 2.3 Project Structure

Each layer project follows:

```
ChipmeoApis.{Layer}/
├── {Feature}/
│   ├── {Action}.cs
│   └── ...
├── Extensions/
│   └── DependencyInjection.cs
└── {Layer}.csproj
```

#### Usecase Layer (Application)

```
ChipmeoApis.Usecase/
├── DTOs/
│   ├── Orders/
│   │   ├── CreateOrderRequest.cs
│   │   ├── OrderResponse.cs
│   │   └── OrderStatusUpdateRequest.cs
│   └── Menu/
│       └── MenuItemResponse.cs
├── Interfaces/
│   ├── Repositories/
│   │   └── IOrderRepository.cs
│   └── Services/
│       └── IOrderService.cs
├── Services/
│   └── OrderService.cs
├── Extensions/
│   └── DependencyInjection.cs
└── Utils/
    └── ...
```

#### Infrastructure Layer

```
ChipmeoApis.Infrastructure/
├── Data/
│   ├── StoreDbContext.cs
│   └── Configurations/
│       └── OrderConfiguration.cs
├── Repositories/
│   └── OrderRepository.cs
├── Extensions/
│   └── DependencyInjection.cs
└── Handlers/
    └── MediaHandler.cs
```

### 2.4 Coding Patterns

#### Async Method Naming

```csharp
// GET / single
Task<T?> GetByIdAsync(int id, CancellationToken ct);

// GET / list
Task<List<T>> GetAllAsync(CancellationToken ct);

// POST
Task<T> CreateAsync(T entity, CancellationToken ct);

// PUT
Task<T> UpdateAsync(T entity, CancellationToken ct);

// DELETE
Task<bool> DeleteAsync(int id, CancellationToken ct);
```

#### Service Pattern

```csharp
public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OrderResponse> CreateAsync(
        CreateOrderRequest request,
        CancellationToken ct)
    {
        // 1. Validate business rules
        // 2. Map to entity
        // 3. Persist
        // 4. Return response
    }
}
```

#### Record Usage for DTOs

```csharp
// Request DTO (immutable input)
public sealed record CreateOrderRequest(
    int SourceId,
    string? CustomerPhone,
    string? Note,
    string? DiscountCode,
    List<CreateOrderItemRequest> Items
);

// Response DTO (immutable output)
public sealed record OrderResponse(
    int Id,
    decimal TotalAmount,
    decimal DiscountAmount,
    decimal FinalAmount,
    string Status,
    DateTime CreatedAt
);
```

#### Entity Design

```csharp
public class Order : IAuditableEntity
{
    public int Id { get; set; }
    public int? SourceId { get; set; }
    public int? CustomerId { get; set; }
    public string? Note { get; set; }
    public string Status { get; set; } = "pending";
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public string? DiscountCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }

    // Navigation properties
    public Source? Source { get; set; }
    public Customer? Customer { get; set; }
    public List<OrderItem> Items { get; set; } = [];
    public List<Payment> Payments { get; set; } = [];
    public List<OrderStatusHistory> StatusHistory { get; set; } = [];
}
```

#### Use `primary` constructors (C# 12+)

```csharp
// Preferred in .NET 10
public class OrderService(IOrderRepository repository, IUnitOfWork uow)
    : IOrderService
{
    public async Task<OrderResponse> CreateAsync(...)
    {
        // repository and uow available directly
    }
}
```

### 2.5 Dependency Injection

#### Registration Pattern

Each layer has its own `Extensions/DependencyInjection.cs`:

```csharp
// ChipmeoApis.Infrastructure/Extensions/DependencyInjection.cs
public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<StoreDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IOrderRepository, OrderRepository>();

        return services;
    }
}

// Program.cs — Composition Root
builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication();

// Or with extension chaining:
builder.Services
    .AddCore()
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddWeb(builder.Configuration);
```

#### Lifetime Rules

| Lifetime | When | Example |
|---|---|---|
| `Singleton` | One instance for app lifetime | Cache, configuration, logger |
| `Scoped` | One instance per HTTP request | DbContext, repositories |
| `Transient` | New instance every injection | Lightweight stateless services |

### 2.6 Entity Framework Core

#### Configuration (Fluent API)

```csharp
// Use Fluent API in Infrastructure layer, NOT attributes on Core entities
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Status)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(e => e.TotalAmount)
               .HasColumnType("decimal(18,2)");
        builder.Property(e => e.CreatedAt)
               .HasDefaultValueSql("GETUTCDATE()");

        builder.HasMany(e => e.Items)
               .WithOne(e => e.Order)
               .HasForeignKey(e => e.OrderId);

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
```

#### Query Rules

```csharp
// Use AsNoTracking for read-only queries
var orders = await _context.Orders
    .AsNoTracking()
    .Where(o => o.Status == "paid")
    .ToListAsync(ct);

// Use Include/ThenInclude for related data
var order = await _context.Orders
    .Include(o => o.Items)
        .ThenInclude(i => i.Addons)
    .Include(o => o.Payments)
    .FirstOrDefaultAsync(o => o.Id == id, ct);

// Use projection for DTOs
var summaries = await _context.Orders
    .Select(o => new OrderSummary(
        o.Id,
        o.FinalAmount,
        o.Status,
        o.CreatedAt
    ))
    .ToListAsync(ct);

// Use pagination for list endpoints
var page = await _context.Orders
    .OrderByDescending(o => o.CreatedAt)
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync(ct);
```

### 2.7 Controllers & API (v2)

#### Route Convention

> Tất cả routes tự động có prefix `/v2/` nhờ `V2RouteConvention` (xem `Conventions/V2RouteConvention.cs`).
> Không cần ghi `v2/` trong `[Route]` attribute — convention xử lý tự động.

#### Controller Structure

```csharp
[ApiController]
[Route("api/admin/orders")]     // → thực tế là /v2/api/admin/orders
[Authorize]
public class OrdersController(IOrderService service) : ControllerBase
{
    [HttpGet]
    [RequirePermission("orders.view")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var orders = await service.GetAllAsync(ct);
        return ApiResult.Success(orders);       // ← bắt buộc dùng ApiResult
    }

    [HttpGet("{id}")]
    [RequirePermission("orders.view")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var order = await service.GetByIdAsync(id, ct);
        return order is null
            ? ApiResult.NotFound("Order not found")
            : ApiResult.Success(order);
    }

    [HttpPost]
    [RequirePermission("orders.create")]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrderRequest request,
        CancellationToken ct)
    {
        var order = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        // CreatedAtAction giữ nguyên — HTTP 201, body là resource
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct)
    {
        var (items, totalCount) = await service.GetPagedAsync(page, pageSize, ct);
        return ApiResult.Paged(items, page, pageSize, totalCount);
    }
}
```

#### Route Conventions (v2)

| Controller | Route (thực tế) | Auth |
|---|---|---|
| Health | `GET /v2/api/health` | None |
| Auth | `/v2/api/auth/*` | Mixed |
| Customer API | `/v2/api/customers/*` | Mixed |
| Public Blog | `/v2/api/blog` | None (read) / Employee (write) |
| POS | `/v2/api/pos/*` | Employee |
| Kitchen | `/v2/api/kitchen/*` | Employee |
| Admin | `/v2/api/admin/*` | Employee + Permission |
| Admin Dashboard | `/v2/api/admin/dashboard/*` | Employee + Permission |
| Reports | `/v2/api/reports/*` | Employee |
| Media | `/v2/api/media/*` | Employee |

#### Response Conventions (v2)

**BẮT BUỘC** dùng `ApiResult` helper cho mọi response:

```csharp
using ChipmeoApis.Web.ApiResponse;

// ✅ Success — single resource (HTTP 200)
return ApiResult.Success(data);

// ✅ Success — collection (HTTP 200)
return ApiResult.Success(items);

// ✅ Success — paginated (HTTP 200)
// Response: { data: [...], error: null, meta: { page, pageSize, totalCount, totalPages } }
return ApiResult.Paged(items, page, pageSize, totalCount);

// ✅ Created (HTTP 201)
return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);

// ✅ No Content (HTTP 204) — cho DELETE
return ApiResult.NoContent();           // hoặc return NoContent();

// ❌ Client Error (HTTP 400)
return ApiResult.BadRequest("message", errors);    // errors: List<FieldError>?

// ❌ Not Found (HTTP 404)
return ApiResult.NotFound("message");

// ❌ Server Error (HTTP 500)
return StatusCode(500, ApiResponse<object>.Failure(
    new ErrorDetail { Code = "INTERNAL_ERROR", Message = ex.Message }
));

// ❌ Unauthorized — giữ nguyên (middleware xử lý)
return Unauthorized();
return Forbid();
```

**KHÔNG được** dùng trực tiếp:
- ❌ `return Ok(data)` — không envelope
- ❌ `return NotFound(new { ... })` — format không chuẩn
- ❌ `return BadRequest("string")` — thiếu error code
- ❌ `return StatusCode(500, new { error = ... })` — thiếu envelope

#### Response Envelope Format

Mọi response (trừ 204 No Content) đều theo format:

```json
{
  "data": { ... } | [ ... ] | null,
  "error": { "code": "...", "message": "...", "details": [...] } | null,
  "meta": { "timestamp": "2026-01-15T10:30:00Z", "requestId": "uuid" }
}
```

- Success: `data` có giá trị, `error` = `null`
- Error: `data` = `null`, `error` có giá trị
- Paginated: `data` là mảng, `meta` có `page`, `pageSize`, `totalCount`, `totalPages`

### 2.8 Error Handling (v2)

#### Service Layer — Result Pattern

```csharp
// Use Result pattern for service operations
public sealed record Result<T>
{
    public T? Value { get; init; }
    public string? Error { get; init; }
    public bool IsSuccess => Error is null;
    public bool IsFailure => Error is not null;

    public static Result<T> Success(T value) => new() { Value = value };
    public static Result<T> Failure(string error) => new() { Error = error };
}

// Services return Result instead of throwing
public async Task<Result<OrderResponse>> CreateAsync(CreateOrderRequest request, CancellationToken ct)
{
    if (request.Items.Count == 0)
        return Result<OrderResponse>.Failure("Order must have at least one item.");

    // ... create order
    return Result<OrderResponse>.Success(response);
}
```

#### Controller Layer — ApiResult

```csharp
// Controllers map Result → ApiResult envelope
var result = await orderService.CreateAsync(request, ct);
if (result.IsFailure)
    return ApiResult.BadRequest(result.Error);

return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
```

#### Exception Handling Middleware

```csharp
// Global exception handler — tự động wrap vào ApiResponse<object>.Failure
app.UseMiddleware<ExceptionHandlingMiddleware>();
```

Middleware (`Middleware/ExceptionHandlingMiddleware.cs`) tự động bắt mọi unhandled exception và trả về:
```json
{
  "data": null,
  "error": {
    "code": "INTERNAL_ERROR",
    "message": "An unexpected error occurred."
  },
  "meta": { "timestamp": "..." }
}
```

---

## 3. Svelte 5 / Frontend Standards

### 3.1 Runes (Reactivity)

Svelte 5 uses runes — explicit compiler directives prefixed with `$`.

#### `$state` — Reactive State

```svelte
<script lang="ts">
  // ✅ Correct: $state for UI-driving values
  let count = $state(0);
  let items = $state([]);

  // ✅ Correct: $state.raw for API data (replaced, not mutated)
  let users = $state.raw(await fetchUsers());

  // ❌ Wrong: plain variable for reactive state
  let count = 0; // NOT reactive
</script>
```

**When to use what:**

| Variant | Use Case |
|---|---|
| `$state(value)` | Values you mutate directly (push, splice, property assign) |
| `$state.raw(value)` | Values you replace entirely (API responses, table data) |
| `$state.snapshot(value)` | Extract a non-reactive snapshot from a reactive proxy |

#### `$derived` — Computed Values

```svelte
<script lang="ts">
  let count = $state(0);

  // ✅ Correct: declarative computed value
  let doubled = $derived(count * 2);

  // ✅ Correct: derived.by for complex logic
  let description = $derived.by(() => {
    if (count === 0) return 'Zero';
    if (count === 1) return 'One';
    return 'Many';
  });

  // ❌ Wrong: using $effect to compute
  let doubled = $state(0);
  $effect(() => { doubled = count * 2; });
</script>
```

**Rule:** Always prefer `$derived` over `$effect` for transforming values.

#### `$effect` — Side Effects (Escape Hatch)

```svelte
<script lang="ts">
  let count = $state(0);

  // ✅ Correct: syncing with external system (document.title)
  $effect(() => {
    document.title = `Count: ${count}`;
  });

  // ✅ Correct: logging
  $effect(() => {
    console.log('count changed to', count);
  });

  // ❌ Wrong: computing derived values
  $effect(() => { doubled = count * 2; });
</script>
```

**Rule:** `$effect` is for external side effects only. Never use it for computed values.

#### `$props` — Component Inputs

```svelte
<script lang="ts">
  // ✅ Correct: destructure props with $props()
  let { name, age = 18, onAction } = $props<{
    name: string;
    age?: number;
    onAction?: () => void;
  }>();

  // ✅ Correct: derived values from props
  let greeting = $derived(`Hello, ${name}!`);

  // ❌ Wrong: export let (Svelte 4 legacy)
  export let name: string;
</script>
```

#### `$bindable` — Two-Way Binding

```svelte
<script lang="ts">
  // ✅ Correct: explicit two-way binding prop
  let { value = $bindable() } = $props<{
    value: string;
  }>();
</script>

<input bind:value={value} />
```

### 3.2 Component Conventions

#### File Structure

```svelte
<!-- MyComponent.svelte -->
<script lang="ts">
  // 1. Imports
  import { onMount } from 'svelte';

  // 2. Props
  let { title, items = [] } = $props<Props>();

  // 3. State
  let isOpen = $state(false);
  let searchQuery = $state('');

  // 4. Derived
  let filteredItems = $derived(
    items.filter(i => i.name.includes(searchQuery))
  );

  // 5. Effects
  $effect(() => {
    if (isOpen) onOpen?.();
  });

  // 6. Functions
  function toggle() {
    isOpen = !isOpen;
  }

  // 7. Lifecycle
  onMount(() => { ... });
</script>

<!-- Template: semantic HTML, no wrapper divs when unnecessary -->
<div class="card">
  <h2>{title}</h2>
  {#if isOpen}
    <ul>
      {#each filteredItems as item (item.id)}
        <li>{item.name}</li>
      {/each}
    </ul>
  {/if}
  <button onclick={toggle}>Toggle</button>
</div>

<style>
  /* Scoped styles using CSS classes */
  .card { border: 1px solid var(--border); }
  h2 { font-size: 1.25rem; }
</style>
```

#### Order Within `<script>`

1. Imports (grouped: Svelte → project lib → third-party)
2. `$props()` (destructure props, but NOT inside `$derived` or `$effect`)
3. `$state()` declarations
4. `$derived()` / `$derived.by()` values
5. `$effect()` blocks
6. Regular functions (event handlers, helpers)
7. Lifecycle functions (`onMount`, `onDestroy`, etc.)

#### Event Handlers

```svelte
<script lang="ts">
  // ✅ Correct: inline arrow for simple handlers
  let count = $state(0);
</script>

<button onclick={() => count++}>Count: {count}</button>

<!-- ✅ Correct: named function for complex logic -->
<button onclick={handleSubmit}>Submit</button>

<!-- ❌ Wrong: on:click (Svelte 4 legacy) -->
<button on:click={handleSubmit}>Submit</button>
```

#### Snippets (Replacing Slots)

```svelte
<!-- Card.svelte — using snippets instead of slots -->
<script lang="ts">
  let {
    title,
    children,
    actions
  } = $props<{
    title: string;
    children?: Snippet;
    actions?: Snippet;
  }>();
</script>

<div class="card">
  <div class="card-header">
    <h3>{title}</h3>
    {#if actions}
      <div class="card-actions">
        {@render actions()}
      </div>
    {/if}
  </div>
  <div class="card-body">
    {#if children}
      {@render children()}
    {/if}
  </div>
</div>

<!-- Usage -->
<Card title="Orders">
  {#snippet children()}
    <p>Order content here</p>
  {/snippet}
  {#snippet actions()}
    <button onclick={refresh}>Refresh</button>
  {/snippet}
</Card>
```

### 3.3 SvelteKit Routing

#### File Conventions

| File | Purpose |
|---|---|
| `+page.svelte` | Page component |
| `+page.server.ts` | Server load function (runs on server) |
| `+layout.svelte` | Layout wrapper |
| `+layout.server.ts` | Server load function for layout |
| `+error.svelte` | Error boundary |
| `+server.ts` | API endpoint (no UI) |
| `+page.ts` | Universal load function (runs on server + client) |
| `hooks.server.ts` | Global server hooks (auth, session) |
| `hooks.client.ts` | Global client hooks |
| `params/` | Custom param matchers |

#### Layout Rules

```svelte
<!-- src/routes/admin/+layout.svelte -->
<script lang="ts">
  import { page } from '$app/stores';

  let { children } = $props();

  // ✅ Correct: auth check in layout
  $effect(() => {
    if (!$page.data.user) {
      goto('/login');
    }
  });
</script>

<aside class="sidebar">...</aside>
<main>
  {@render children()}
</main>
```

#### Loading Data (load functions)

```typescript
// src/routes/admin/orders/+page.server.ts
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ fetch, locals }) => {
  const res = await fetch('/admin/orders', {
    headers: { Authorization: `Bearer ${locals.user.token}` }
  });
  const orders = await res.json();

  return { orders };
};
```

### 3.4 State Management

#### Class-Based Stores with Runes

```typescript
// src/lib/utils/state.ts ✅
class AppState {
  cart = $state<CartItem[]>([]);
  isAuthModalOpen = $state(false);
  currentUser = $state.raw<User | null>(null);

  get cartTotal() {
    return $derived(
      this.cart.reduce((sum, item) => sum + item.price * item.quantity, 0)
    );
  }

  addItem(item: CartItem) {
    this.cart.push(item);
  }

  clearCart() {
    this.cart = [];
  }
}

export const appState = new AppState();
```

#### Context API for Scoped State

```typescript
// src/lib/utils/state.ts ✅
const ADMIN_KEY = Symbol('admin-context');

class AdminState {
  sidebarOpen = $state(true);
  currentTab = $state('dashboard');
}

export function setAdminContext() {
  setContext(ADMIN_KEY, new AdminState());
}

export function getAdminContext(): AdminState {
  return getContext(AdminState);
}
```

### 3.5 API Layer

#### Module Pattern

```typescript
// src/lib/api/orders.ts ✅
import { api } from './utils';

export interface OrderResponse { ... }
export interface CreateOrderRequest { ... }

export const ordersApi = {
  list: () => api.get<OrderResponse[]>('/admin/orders'),
  getById: (id: number) => api.get<OrderResponse>(`/admin/orders/${id}`),
  create: (data: CreateOrderRequest) => api.post<OrderResponse>('/pos/orders', data),
  updateStatus: (id: number, status: string) =>
    api.put(`/pos/orders/${id}/status`, { status }),
};

// Usage:
import { ordersApi } from '$lib/api/orders';
const orders = await ordersApi.list();
```

#### Request Helper

```typescript
// src/lib/api/utils.ts ✅
import { config } from '$lib/config';
import { appState } from '$lib/utils/state';

async function request<T>(
  path: string,
  options: RequestInit = {}
): Promise<T> {
  const token = appState.currentUser?.token;
  const res = await fetch(`${config.apiUrl}${path}`, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...options.headers,
    },
  });

  if (!res.ok) {
    throw new ApiError(res.status, await res.json());
  }

  return res.json();
}

export const api = {
  get: <T>(path: string) => request<T>(path),
  post: <T>(path: string, body: unknown) =>
    request<T>(path, { method: 'POST', body: JSON.stringify(body) }),
  put: <T>(path: string, body: unknown) =>
    request<T>(path, { method: 'PUT', body: JSON.stringify(body) }),
  delete: (path: string) => request(path, { method: 'DELETE' }),
};
```

### 3.6 TypeScript

#### Type Over `any`

```typescript
// ✅ Correct: explicit types for API responses
interface OrderResponse {
  id: number;
  totalAmount: number;
  status: 'pending' | 'paid' | 'preparing' | 'ready' | 'served' | 'cancelled';
  items: OrderItemResponse[];
}

// ✅ Correct: specific function type instead of `Function`
type Callback = (...args: unknown[]) => void;

// ❌ Wrong: any
const processOrder = (data: any) => { ... };
```

#### `.svelte.ts` Files

When using runes in `.svelte.ts` files, use the **class-based pattern**:

```typescript
// ✅ Correct: runes work in .svelte.ts files
export class OrderStore {
  orders = $state<Order[]>([]);
  loading = $state(false);

  get pendingCount() {
    return $derived(this.orders.filter(o => o.status === 'pending').length);
  }
}

// ❌ Wrong: module-level runes are shared across all users (SSR leak!)
export let orders = $state<Order[]>([]);
```

### 3.7 Styling

#### Tailwind CSS

```svelte
<!-- ✅ Correct: utility-first with Tailwind -->
<button
  class="inline-flex items-center gap-2 rounded-lg bg-blue-600 px-4 py-2
         text-white hover:bg-blue-700 disabled:opacity-50"
  disabled={!canSubmit}
  onclick={handleSubmit}
>
  Submit
</button>

<!-- ✅ Correct: use Tailwind @apply sparingly (prefer utility classes) -->
<!-- ✅ Correct: custom CSS only for truly unique styles -->
<style>
  .custom-gradient {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  }
</style>
```

#### Naming Classes

- Use Tailwind utility classes in markup (preferred)
- Use `kebab-case` for custom CSS classes
- Avoid BEM — Tailwind + component scoping covers most needs

#### Component Composition

```svelte
<!-- ✅ Correct: props for variant/state -->
<Button variant="primary" size="lg" disabled={isSubmitting}>
  Save
</Button>
```

### 3.8 Naming Conventions (Frontend)

| Element | Convention | Example |
|---|---|---|
| Components | PascalCase | `Button.svelte`, `OrderHistory.svelte` |
| Files (lib) | kebab-case | `custom-image.ts`, `auth.ts` |
| Files (routes) | SvelteKit standard | `+page.svelte`, `+layout.server.ts` |
| Routes | kebab-case | `/admin/role-permissions/` |
| Variables | camelCase | `const orderTotal = ...` |
| Functions | camelCase | `function calculateTotal()` |
| Constants | SCREAMING_SNAKE_CASE | `const MAX_ITEMS = 10` |
| API modules | camelCase object | `ordersApi.list()` |
| Types/Interfaces | PascalCase | `OrderResponse`, `MenuItem` |
| CSS classes | kebab-case | `card-header`, `menu-grid` |
| Store names | camelCase | `appState`, `orderStore` |

### 3.9 Frontend Configuration (Environment Variables)

> Tất cả cấu hình frontend đều qua `PUBLIC_*` env vars, đọc ở runtime bằng `$env/dynamic/public`.

#### Nguyên tắc

1. **Single source of truth** — `.env` ở project root, không có file `.env` riêng trong `ChipmeoPOS/`.
2. **Runtime, không build-time** — dùng `$env/dynamic/public` thay vì `$env/static/public` để Docker build không cần `.env`.
3. **Không hardcode fallback** — mọi `PUBLIC_*` đều required trong `.env`, config không có `|| defaultValue`.
4. **Docker** — biến môi trường được truyền qua `environment:` trong `docker-compose.yml`.

#### Cấu trúc

```typescript
// src/lib/config/index.ts
import { env } from '$env/dynamic/public';

const apiBaseUrl = env.PUBLIC_API_URL;
if (!apiBaseUrl) throw new Error('PUBLIC_API_URL is required');
export const API_BASE_URL = apiBaseUrl;

const storagePrefix = env.PUBLIC_AUTH_STORAGE_PREFIX || '';
export const STORAGE_KEYS = {
  TOKEN: storagePrefix + 'token',
  USER: storagePrefix + 'user'
};

export const SIGNALR_HUB_PATH = env.PUBLIC_SIGNALR_HUB;
export const API_HOST_URL = API_BASE_URL.replace(/\/v2\/?$/, '');
// ...etc
```

#### Danh sách `PUBLIC_*` vars

| Var | Mục đích |
|---|---|
| `PUBLIC_API_URL` | Base URL của API (vd: `http://api.localhost/v2`) |
| `PUBLIC_AUTH_STORAGE_PREFIX` | Prefix cho localStorage keys (tránh conflict domain) |
| `PUBLIC_SIGNALR_HUB` | SignalR hub path (vd: `/hubs/app`) |
| `PUBLIC_SIGNALR_WS_ONLY` | `true` = WebSockets only (skipNegotiation) |
| `PUBLIC_VIETQR_API_URL` | URL API VietQR (vd: `https://api.vietqr.io/v2/banks`) |
| `PUBLIC_DEFAULT_QR_TEMPLATE` | Template QR mặc định (vd: `compact2`) |
| `PUBLIC_SITE_DOMAIN` | Domain cho SEO internal link detection |
| `PUBLIC_DEFAULT_CUSTOMER_PASSWORD` | Password mặc định cho customer POS |

#### Lưu ý

- `PUBLIC_SIGNALR_WS_ONLY=false` → dùng negotiation + fallback transport.
- `PUBLIC_AUTH_STORAGE_PREFIX` rỗng → không prefix, key là `token`/`user`.
- `API_HOST_URL` = `API_BASE_URL` bỏ suffix `/v2` — dùng cho SignalR WebSocket (hub ở root, không dưới `/v2`).

### 3.10 Flowbite Interactive Components

> Flowbite is used for interactive UI components that require JavaScript. It is used via its **vanilla JS API** (data-* attributes + `initFlowbite()`), NOT via Svelte wrappers.

#### Import Pattern

```typescript
// In any component that needs Flowbite interactivity
import { onMount } from 'svelte';
import 'flowbite';

onMount(() => {
  // Must call after DOM is rendered to activate data-* behaviors
  initFlowbite();
});
```

#### What Uses Flowbite

- **Dropdowns** — `data-dropdown-toggle`
- **Modals** — `data-modal-target` / `data-modal-toggle`
- **Tooltips** — `data-tooltip-target`
- **Collapse / Accordion** — `data-collapse-toggle`
- **Tabs** — `data-tabs-toggle`
- **Datepicker** — `datepicker` class + `data-datepicker`

#### DO ✅

```svelte
<!-- ✅ Correct: Flowbite via data-* attributes + initFlowbite() -->
<script lang="ts">
  import { onMount } from 'svelte';
  import 'flowbite';

  onMount(() => { initFlowbite(); });
</script>

<button data-dropdown-toggle="myDropdown">Open</button>
<div id="myDropdown" class="hidden">...</div>
```

#### DON'T ❌

- ❌ Don't use Flowbite Svelte wrappers (flowbite-svelte) — not installed
- ❌ Don't import Flowbite CSS directly — Tailwind plugin handles it
- ❌ Don't call `initFlowbite()` in `$effect` — use `onMount`

#### Component Library (`src/lib/components/ui/`)

Reusable UI components live in `src/lib/components/ui/`:
- `Icon.svelte` — unified icon component (wraps `@iconify/svelte`)
- `Modal.svelte` — modal wrapper (uses Flowbite data-* internally)
- `Accordion.svelte` — accordion component
- `Breadcrumb.svelte` — breadcrumb navigation
- `Sidebar.svelte` — admin sidebar
- `Button.svelte` — button component
- `Pagination.svelte` — pagination component
- `Table.svelte` — data table component
- `Badge.svelte` — status badges

These components are NOT Flowbite wrappers — they use Flowbite JS for interactivity where needed and write their own markup/Tailwind styles.

### 3.11 Icons (Iconify + Tabler)

> All icons use **Tabler Icons** via `@iconify/svelte`. No other icon set or inline `<svg>` should be used for static icons.

#### Icon Component

```svelte
<script lang="ts">
  import Icon from '$lib/components/ui/Icon.svelte';
</script>

<!-- ✅ Correct: Tabler icon via Icon component -->
<Icon name="tabler:x" class="size-5" />
<Icon name="tabler:user" class="size-4 text-gray-500" />
```

#### Icon Naming

All icon names follow the format `tabler:{icon-name}` where `{icon-name}` is the Tabler icon name in kebab-case. Browse available icons at [https://tabler.io/icons](https://tabler.io/icons).

| Icon | Usage |
|---|---|
| `tabler:x` | Close / delete / cancel |
| `tabler:plus` | Add / create |
| `tabler:check` | Confirm / save |
| `tabler:pencil` | Edit |
| `tabler:trash` | Delete |
| `tabler:arrow-left` `tabler:arrow-right` | Navigation arrows |
| `tabler:search` | Search |
| `tabler:loader-2` | Loading spinner (use with `animate-spin`) |
| `tabler:dots-vertical` | More / context menu |
| `tabler:menu-2` | Hamburger menu |

#### DO ✅

```svelte
<!-- ✅ Correct: static icon via Icon component -->
<Icon name="tabler:user" />
<button onclick={handleSave}>
  <Icon name="tabler:check" class="size-4" /> Save
</button>
```

#### DON'T ❌

- ❌ Don't use inline `<svg>` for static icons (exception: loading spinners in `Sidebar`, blog, media)
- ❌ Don't use `<Icon name="general/close">` or any non-`tabler:` prefix
- ❌ Don't import from `phosphor-svelte` or `flowbite-icons` — packages removed
- ❌ Don't reference `icons.ts` — file deleted; `@iconify/svelte` loads icons on demand

#### Exceptions (Inline SVGs Kept)

5 inline SVGs remain where dynamic icon sources prevent static `<Icon>` usage:

| File | Reason |
|---|---|
| `Sidebar.svelte` | 3 icons use dynamic `<use href={item.icon}>` — SVG sprite reference |
| `admin/blog/+page.svelte` | 1 loading `animate-spin` SVG |
| `media/MediaLibraryModal.svelte` | 1 loading `animate-spin` SVG |

---

## 4. Cross-Cutting Concerns

### 4.1 Security

#### Frontend

```typescript
// ✅ Correct: sanitize HTML before {@html ...}
import DOMPurify from 'dompurify';
const sanitized = DOMPurify.sanitize(unsafeHtml);

// ✅ Correct: store JWT in memory, not localStorage
// (token is in appState rune — lost on page reload, refresh via httpOnly cookie)

// ✅ Correct: validate API responses with Zod or manual checks
```

#### Backend

```csharp
// ✅ Correct: hash passwords with BCrypt
string hash = BCrypt.HashPassword(password);
bool valid = BCrypt.Verify(password, hash);

// ✅ Correct: validate file types server-side
var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
if (!allowedTypes.Contains(file.ContentType))
    return BadRequest("Invalid file type");

// ✅ Correct: use parameterized queries / EF Core (no SQL injection)
// ✅ Correct: validate all inputs with Data Annotations or FluentValidation
```

### 4.2 Git

#### Tag Naming Convention

> This project follows **[Semantic Versioning 2.0.0](https://semver.org/)** — tags use `MAJOR.MINOR.PATCH` format **without** `v` prefix.

```
1.0.0       # ✅ Correct (pure SemVer)
v1.0.0      # ❌ Wrong — SemVer spec says "v1.2.3" is NOT a semantic version
```

| Bump | When | Example |
|---|---|---|
| **MAJOR** | Breaking API/DB schema changes, architecture rewrite | `1.0.0` → `2.0.0` |
| **MINOR** | New features, endpoints, modules (backward compatible) | `1.0.0` → `1.1.0` |
| **PATCH** | Bug fixes, package updates, refactoring, docs, config | `1.0.0` → `1.0.1` |

**Rules:**
- Only the **project owner** decides when to tag — AI must ask before creating any tag
- Tags are applied to `master` branch only
- Each tag must have a corresponding [GitHub Release](https://docs.github.com/en/repositories/releasing-projects-on-github/managing-releases-in-a-repository)
- Never force-push tags (delete + recreate instead)

#### Commit Messages

```
type(scope): brief description

- Bullet points for details if needed
```

**Types:** `feat`, `fix`, `chore`, `docs`, `style`, `refactor`, `perf`, `test`

**Examples:**
```
feat(pos): add QR payment support

- Integrate VietQR bank transfer modal
- Add payment confirmation flow
- Update order status on payment callback
```

```
fix(admin): prevent double-click on save button

- Disable button during submission
- Show loading spinner
```

```
chore(deps): update all NuGet packages to 10.0.9
```

#### Branch Strategy

- `master` — production-ready
- Feature branches: `feat/{name}` — for new features
- Fix branches: `fix/{name}` — for bug fixes
- Always squash-merge feature branches

### 4.3 Testing

#### Backend (xUnit)

```csharp
// ✅ Test services in isolation (mock repositories)
public class OrderServiceTests
{
    [Fact]
    public async Task CreateOrder_WithValidData_ReturnsOrderResponse()
    {
        // Arrange
        var repo = new Mock<IOrderRepository>();
        var service = new OrderService(repo.Object, ...);

        // Act
        var result = await service.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }
}
```

#### Frontend (Vitest)

```typescript
// ✅ Test API utilities and stores
import { describe, it, expect } from 'vitest';

describe('cart calculations', () => {
  it('calculates total correctly', () => {
    const items = [{ price: 10000, quantity: 3 }];
    expect(calculateTotal(items)).toBe(30000);
  });
});
```

---

## Quick Reference Card

### C# / .NET Checklist

- [ ] Class is `sealed` unless designed for inheritance
- [ ] Async methods use `CancellationToken` parameter
- [ ] DTOs are `record` types (immutable)
- [ ] No EF Core attributes on Core entities
- [ ] Business logic in Usecase Services, not Controllers
- [ ] DI registered in each layer's `Extensions/`
- [ ] All endpoints use `[RequirePermission]` where needed
- [ ] `AsNoTracking()` for read-only queries
- [ ] Soft delete via `IsDeleted` + query filter

### Svelte 5 Checklist

- [ ] Use `$state()` / `$state.raw()` — not plain `let`
- [ ] Use `$derived()` — not `$effect()` for computed values
- [ ] Use `$props()` — not `export let`
- [ ] Use `onclick` — not `on:click`
- [ ] Use `{#snippet}` — not `<slot>` for new components
- [ ] Class-based stores in `.svelte.ts` — not module-level runes
- [ ] Event handlers are named functions or inline arrows
- [ ] `{#each}` blocks always have a `key`
- [ ] No `any` types (use `warn` level, aim to eliminate)
