# Flows & Project Structure

## Monorepo Structure

```
Foodstore/
├── docker-compose.yml                    # 🐳 Full stack (6 services)
├── .env                                  # 🔒 Environment variables
├── FoodstoreApi/                          # 🖥️ Backend (.NET 10 Clean Architecture)
│   ├── FoodstoreApi.slnx                  # Solution file
│   │
│   ├── FoodstoreApi.Core/                 # 🎯 Domain Layer (zero dependencies)
│   │   ├── Entities/                     #   POCO entities (23 files)
│   │   │   ├── Category.cs
│   │   │   ├── MenuItem.cs
│   │   │   ├── Addon.cs
│   │   │   ├── Combo.cs
│   │   │   ├── ComboItem.cs
│   │   │   ├── Discount.cs
│   │   │   ├── Source.cs
│   │   │   ├── Role.cs
│   │   │   ├── Permission.cs
│   │   │   ├── RolePermission.cs
│   │   │   ├── Employee.cs
│   │   │   ├── Customer.cs
│   │   │   ├── Order.cs
│   │   │   ├── OrderItem.cs
│   │   │   ├── OrderItemAddon.cs
│   │   │   ├── OrderStatusHistory.cs
│   │   │   ├── Payment.cs
│   │   │   ├── PaymentSetting.cs
│   │   │   ├── BlogPost.cs
│   │   │   ├── Tag.cs
│   │   │   ├── BlogPostTag.cs
│   │   │   ├── MenuItemAddon.cs
│   │   │   └── Media.cs
│   │   └── FoodstoreApi.Core.csproj
│   │
│   ├── FoodstoreApi.Usecase/             # 🧠 Application Layer
│   │   ├── DTOs/                         #   Data Transfer Objects (16 subdirectories)
│   │   ├── Interfaces/                   #   Service & Repository interfaces
│   │   │   ├── Repositories/             #     17 repository interfaces
│   │   │   └── Services/                 #     16 service interfaces
│   │   ├── Services/                     #   Service implementations
│   │   ├── Extensions/                   #   DI registration
│   │   └── Utils/                        #   Shared utilities
│   │
│   ├── FoodstoreApi.Infrastructure/       # 📀 Infrastructure Layer
│   │   ├── Data/                         #   EF Core DbContext + Configurations
│   │   ├── Repositories/                 #   17 repository implementations
│   │   ├── Handlers/                     #   Media upload handler (S3/AWS SDK)
│   │   ├── Extensions/                   #   DI registration
│   │   ├── Caching/                      #   (empty — planned)
│   │   └── Mappings/                     #   (future: AutoMapper profiles)
│   │
│   ├── Dockerfile                        #   🐳 API Docker image
│   └── FoodstoreApi.Web/                  # 🌐 Presentation Layer
│       ├── Controllers/                  #   21 API controllers
│       ├── Hubs/                         #   SignalR hub
│       ├── Middleware/                   #   Security headers, rate limiting
│       ├── Authorization/               #   Custom RBAC (policy provider + handler)
│       ├── Program.cs                    #   App startup / composition root
│       └── appsettings.json             #   Configuration
│
├── Store/                           # 🎨 Frontend (SvelteKit)
│   ├── src/
│   │   ├── lib/
│   │   │   ├── api/                      #   API client (21 modules)
│   │   │   │   ├── index.ts             #     Central exports
│   │   │   │   └── utils.ts             #     Request helper (JWT auth)
│   │   │   ├── components/               #   Reusable UI components
│   │   │   │   ├── ui/                  #     Base UI: Icon, Modal, Accordion, Breadcrumb, Sidebar, Button, Pagination, Table, Badge
│   │   │   │   ├── editor/              #     TipTap rich text editor
│   │   │   │   └── media/               #     Media gallery modal
│   │   │   ├── services/                #   SignalR connection manager
│   │   │   ├── types/                    #   TypeScript interfaces (16 files)
│   │   │   ├── utils/                    #   State stores, cart, auth, helpers
│   │   │   └── config/                  #   Environment-aware API URL config
│   │   └── routes/                       #   File-based routing
│   │       ├── +layout.svelte           #     Root layout
│   │       ├── +page.svelte             #     Landing page
│   │       ├── admin/                    #     /admin — Dashboard
│   │       │   ├── +layout.svelte
│   │       │   ├── +page.svelte
│   │       │   ├── dashboard.svelte.ts
│   │       │   ├── analytics/
│   │       │   ├── menu/
│   │       │   ├── categories/
│   │       │   ├── addons/
│   │       │   ├── combos/
│   │       │   ├── discounts/
│   │       │   ├── orders/
│   │       │   ├── employees/
│   │       │   ├── roles/
│   │       │   ├── role-permissions/
│   │       │   ├── customers/
│   │       │   ├── blog/
│   │       │   ├── media/
│   │       │   ├── sources/
│   │       │   ├── tags/
│   │       │   └── payment-settings/
│   │       ├── pos/                      #     /pos — Point of Sale
│   │       │   ├── +layout.svelte
│   │       │   ├── +page.svelte
│   │       │   ├── store.svelte.ts
│   │       │   └── checkout/
│   │       ├── kitchen/                  #     /kitchen — KDS
│   │       │   ├── +layout.svelte
│   │       │   ├── +page.svelte
│   │       │   └── kitchen.svelte.ts
│   │       ├── logout/
│   │       └── error/
│   ├── Dockerfile                        #   🐳 Frontend Docker image
│   ├── static/
│   ├── svelte.config.js
│   ├── vite.config.ts
│   ├── tsconfig.json
│   ├── eslint.config.js
│   └── package.json
│
├── scripts/                              # 📜 Database
│   └── init.sql                          #   PostgreSQL schema + seed data
│
├── docker-compose.yml                    # 🐳 Full stack orchestration
├── .env                                  # 🔒 Environment variables
│
├── docs/                                 # 📚 Documentation
│   ├── 0_quick_reference.json
│   ├── 1_tech_stack.md
│   ├── 2_flows_and_project_structure.md
│   ├── 3_api.md
│   ├── 4_known_errors.json
│   └── 5_coding_standards.md
│
├── README.md
└── .gitignore
```

---

## Core Flows

### 1. Order Lifecycle

```
┌──────────┐     ┌───────────┐     ┌───────────┐     ┌───────┐     ┌────────┐     ┌──────┐
│  Pending  │ ──► │ Confirmed │ ──► │ Preparing │ ──► │ Ready │ ──► │ Served │ ──► │ Paid │
└──────────┘     └───────────┘     └───────────┘     └───────┘     └────────┘     └──────┘
                                                                                      │
                                                                                      ▼
                                                                                 ┌─────────┐
                                                                                 │Cancelled│
                                                                                 └─────────┘
```

- **POS** creates order → status = `pending`
- Payment processed → status = `paid`
- Kitchen sees `paid` orders → starts preparing → status = `preparing`
- Kitchen completes → status = `served`
- Admin can cancel at any point
- SignalR broadcasts every status change to all connected clients

### 2. Authentication Flow

```
┌──────────┐     ┌──────────┐     ┌──────────────────┐     ┌────────────────┐
│  Client  │     │ Traefik  │     │  FoodstoreApi.Web │     │  PostgreSQL    │
├──────────┤     ├──────────┤     ├──────────────────┤     ├────────────────┤
│  1. POST │────►│ :80/     │────►│  /api/auth/login │────►│  Find employee │
│          │     │ /api/*   │     │  (username,pass)  │     │  Verify bcrypt │
│          │◄────│          │◄────│  JWT + Refresh   │     │                │
│          │     │          │     │  (in response)   │     │                │
│          │     │          │     │                  │     │                │
│  2. GET  │────►│ :80/     │────►│  /admin/xxx      │     │                │
│   /admin │     │ /admin/* │     │  Authorization:  │     │                │
│          │     │          │     │  Bearer <jwt>    │     │                │
│          │◄────│          │◄────│  Check JWT sig   │     │                │
│          │     │          │     │  Check permission│────►│  role_perms    │
│          │     │          │     │  Return data     │     │                │
└──────────┘     └──────────┘     └──────────────────┘     └────────────────┘
```

### 3. Real-Time Updates (SignalR)

**Hub**: `/hubs/app` | **Protocol**: MessagePack (binary)

| Event | Trigger | Consumers |
|---|---|---|
| `OrderStatusChanged` | Payment / Kitchen status update | POS, Kitchen Display |
| `MenuUpdated` | Admin edits menu/categories/addons | POS |
| `TableUpdated` | Admin edits sources/tables | POS |
| `KitchenNotification` | New paid order | Kitchen Display |

### 4. Media Upload Flow

```
┌──────────┐     ┌──────────┐     ┌──────────────────┐     ┌──────────────────────────┐
│   POS UI │     │ Traefik  │     │  FoodstoreApi.Web │     │  RustFS (S3 Object Store)│
├──────────┤     ├──────────┤     ├──────────────────┤     ├──────────────────────────┤
│  1. POST │────►│ :80/     │────►│  /api/media/     │────►│  PutObjectAsync (AWS SDK)│
│  (file)  │     │ /api/*   │     │  upload           │     │  Validate file type     │
│          │     │          │     │                   │     │  Save to S3 bucket      │
│          │     │          │     │◄──────────────────│────│  Return public URL      │
│          │◄────│          │◄────│  {id, url}        │     │                          │
└──────────┘     └──────────┘     └──────────────────┘     └──────────────────────────┘
```

### 5. Data Fetching (Frontend → Backend)

```
┌──────────────┐     ┌──────────────┐     ┌──────────────┐
│ Svelte Route  │     │  API Module   │     │  .NET API    │
│ (+page.svelte)│     │ (lib/api/*)  │     │  Controller  │
├──────────────┤     ├──────────────┤     ├──────────────┤
│ onMount      │────►│ fetch()      │────►│ Auth check   │
│ or load fn   │     │ + JWT header │     │ (JWT + perm) │
│              │     │              │     │              │
│              │◄────│ parsed JSON  │◄────│ JSON result  │
│              │     │  (typed)     │     │              │
│ Renders      │     │              │     │              │
└──────────────┘     └──────────────┘     └──────────────┘
```

### 6. Authorization (RBAC)

```
Request ──► JWT Middleware (validate token)
               └── ClaimsPrincipal with userId, roleId, permissions
                    └── [RequirePermission("orders.view")] attribute
                         └── PermissionPolicyProvider
                              └── PermissionAuthorizationHandler
                                   └── bool (allow/deny)
```

Permissions follow the pattern `"{module}.{action}"`:
- `menu.view`, `menu.create`, `menu.edit`, `menu.delete`
- `orders.view`, `orders.edit`, `orders.delete`
- `employees.manage`
- `roles.manage`
- `dashboard.view`
- `settings.manage`

### 7. Dashboard ML Flow

```
┌──────────────┐     ┌───────────────┐     ┌──────────────────┐
│  Admin Panel  │     │ DashboardCtrl  │     │  ML.NET Service  │
├──────────────┤     ├───────────────┤     ├──────────────────┤
│  GET /admin  │────►│  GetForecast  │────►│  SSA Estimation  │
│  /analytics  │     │               │     │  (Microsoft.ML)  │
│              │     │               │     │                  │
│              │◄────│  Chart data   │◄────│  Forecast values │
│  Render      │     │  Forecast     │     │  + confidence    │
│  ApexCharts  │     │  Recommends   │     │  + recommendations│
└──────────────┘     └───────────────┘     └──────────────────┘
```

---

## Key Design Decisions

| Decision | Rationale |
|---|---|
| **Clean Architecture** | Isolates business rules from frameworks; swappable DB/UI |
| **RBAC with flat permissions** | Simpler than hierarchical roles; each action explicitly checked |
| **RustFS for media** | S3-compatible object storage (MinIO/RustFS/Garage) via AWS SDK — vendor-neutral |
| **SignalR + MessagePack** | Binary protocol reduces payload size for real-time updates |
| **Vite dev proxy** | Avoids CORS issues during local development |
| **Environment-aware config** | Frontend auto-detects local/demo/production without manual switches |
| **ML.NET (not Python API)** | Keeps ML in-process with .NET; no extra infrastructure |
| **Redis (not IMemoryCache)** | Distributed caching suitable for multi-instance Docker deployments |
| **Traefik ingress** | Single entry point (`:80`), path-based routing: `/api/*` & `/hubs/*` → API, rest → webapp |
| **Build cache mounts** | NuGet (`/root/.nuget/packages`) & npm (`/root/.npm`) cached via Docker BuildKit — rebuild nhanh hơn |
