# Flows & Project Structure

## Monorepo Structure

```
ChipmeoFoodstore/
├── docker-compose.yml                    # 🐳 Full stack (8 services)
├── .env                                  # 🔒 Environment variables
├── foodstore-api/                         # 🖥️ Backend (.NET 10 Clean Architecture)
│   ├── FoodstoreApi.slnx                  # Solution file
│   │
│   ├── FoodstoreApi.Core/                 # 🎯 Domain Layer (zero dependencies)
│   │   ├── Entities/                     #   POCO entities (35+ files)
│   │   │   ├── Identity/
│   │   │   │   ├── ApplicationUser.cs     #     Better Auth user entity
│   │   │   │   └── ApplicationRole.cs     #     Better Auth role entity
│   │   │   ├── Employee.cs               #     user_id → users, role_id → roles
│   │   │   ├── Customer.cs               #     user_id → users, loyalty_points
│   │   │   ├── Role.cs                   #     is_system flag
│   │   │   ├── Permission.cs
│   │   │   ├── RolePermission.cs
│   │   │   ├── RefreshToken.cs
│   │   │   ├── IAuditableEntity.cs       #     Auditable interface
│   │   │   ├── Category.cs
│   │   │   ├── MenuItem.cs
│   │   │   ├── Addon.cs
│   │   │   ├── MenuItemAddon.cs
│   │   │   ├── Combo.cs
│   │   │   ├── ComboItem.cs
│   │   │   ├── Discount.cs
│   │   │   ├── Source.cs
│   │   │   ├── Order.cs
│   │   │   ├── OrderItem.cs
│   │   │   ├── OrderItemAddon.cs
│   │   │   ├── OrderStatusHistory.cs
│   │   │   ├── Payment.cs
│   │   │   ├── PaymentSetting.cs
│   │   │   ├── Media.cs
│   │   │   ├── Tag.cs
│   │   │   ├── BlogPost.cs               #     Extended: scheduledAt, reviewedBy, isFeatured, etc.
│   │   │   ├── BlogCategory.cs
│   │   │   ├── BlogPostCategory.cs
│   │   │   ├── BlogPostTag.cs
│   │   │   ├── BlogPostRevision.cs
│   │   │   ├── BlogPostBlock.cs
│   │   │   ├── BlogSetting.cs
│   │   │   ├── EInvoice.cs               #     E-Invoice (order FK, provider FK, status lifecycle)
│   │   │   ├── EInvoiceProvider.cs       #     Provider config, type, active flag
│   │   │   └── EInvoiceSetting.cs        #     Global settings (single-row)
│   │   ├── Configuration/
│   │   ├── Constants/
│   │   └── Utils/
│   │
│   ├── FoodstoreApi.Usecase/             # 🧠 Application Layer
│   │   ├── DTOs/                         #   21 DTO subdirectories
│   │   │   ├── EInvoice/                 #     EInvoiceDto, IssueInvoiceDto, CancelInvoiceDto
│   │   │   ├── EInvoiceProvider/         #     Provider DTOs
│   │   │   └── EInvoiceSetting/          #     Setting DTOs + Dashboard DTO
│   │   ├── Interfaces/                   #   Service & Repository interfaces
│   │   │   ├── IBlogService.cs           #     + IBlogBlockService, IBlogCategoryService,
│   │   │   ├── IBlogBlockService.cs      #       IBlogRevisionService, IBlogSettingService
│   │   │   ├── IEInvoiceService.cs       #     E-Invoice service
│   │   │   ├── IEInvoiceRepository.cs    #     E-Invoice repository
│   │   │   ├── IEInvoiceProvider.cs      #     Provider abstraction (Viettel, MISA...)
│   │   │   ├── IEInvoiceProviderFactory.cs#    Factory → resolve provider by type
│   │   │   └── ...                       #     25+ interfaces total
│   │   ├── Services/                     #   Service implementations (25+)
│   │   │   ├── EInvoiceService.cs        #     E-Invoice CRUD + issue/cancel
│   │   │   ├── EInvoiceProviderFactory.cs#     Provider resolution via DI
│   │   │   ├── ViettelProvider.cs        #     Viettel e-invoice integration
│   │   │   └── MisaProvider.cs            #     MISA e-invoice integration
│   │   ├── Extensions/                   #   DI registration
│   │   └── Utils/                        #   Shared utilities
│   │
│   ├── FoodstoreApi.Infrastructure/       # 📀 Infrastructure Layer
│   │   ├── Data/                         #   EF Core DbContext + Configurations + Migrations
│   │   ├── Repositories/                 #   23 repository implementations
│   │   │   ├── ...                       #     + EInvoiceRepository
│   │   ├── Handlers/                     #   Media upload handler (S3/AWS SDK)
│   │   ├── Caching/                      #   Redis caching implementation
│   │   └── Extensions/                   #   DI registration
│   │
│   ├── Dockerfile                        #   🐳 API Docker image
│   └── FoodstoreApi.Web/                  # 🌐 Presentation Layer
│       ├── Controllers/                  #   25 API controllers
│       ├── Hubs/                         #   SignalR hub
│       ├── Middleware/                   #   Security headers, rate limiting
│       ├── Authorization/               #   Custom RBAC (policy provider + handler)
│       ├── Seed/                         #   Database seeding
│       ├── Program.cs                    #   App startup / composition root
│       └── appsettings.json             #   Configuration
│
├── foodstore-admin/                     # 🛡️ Admin Dashboard (Next.js 16)
│   ├── src/
│   │   ├── app/
│   │   │   ├── admin/
│   │   │   │   ├── cms/                  #   /admin/cms — CMS module
│   │   │   │   │   ├── dashboard/        #     Dashboard stats
│   │   │   │   │   ├── posts/            #     Bài viết (list + editor [id])
│   │   │   │   │   ├── categories/       #     Danh mục
│   │   │   │   │   ├── tags/             #     Thẻ
│   │   │   │   │   └── settings/         #     Cài đặt CMS
│   │   │   │   ├── crm/                  #   /admin/crm — CRM module
│   │   │   │   │   ├── dashboard/        #     Dashboard stats
│   │   │   │   │   ├── customers/        #     Khách hàng (DataTable + Sheet CRUD)
│   │   │   │   │   └── leaderboard/      #     Bảng xếp hạng điểm
│   │   │   │   ├── employees/            #   /admin/employees — Nhân viên
│   │   │   │   │   ├── dashboard/        #     Dashboard stats
│   │   │   │   │   ├── all/              #     Danh sách nhân viên
│   │   │   │   │   ├── roles/            #     Vai trò
│   │   │   │   │   └── role-permissions/ #     Phân quyền
│   │   │   │   ├── food/                 #   /admin/food — Thực đơn (legacy)
│   │   │   │   │   ├── e-invoice/        #     /admin/food/e-invoice — Hóa đơn điện tử
│   │   │   │   │   │   ├── dashboard/    #       Tổng quan stats
│   │   │   │   │   │   ├── transactions/ #       Danh sách giao dịch
│   │   │   │   │   │   ├── providers/    #       Quản lý nhà cung cấp
│   │   │   │   │   │   └── settings/     #       Cài đặt HDDT
│   │   │   │   ├── layout.tsx            #   SidebarProvider + AppSidebar + Auth guard
│   │   │   │   └── page.tsx              #   Redirect → /admin/cms/dashboard
│   │   │   ├── login/                   #   /login — Login page
│   │   │   ├── layout.tsx               #   Root layout + AuthProvider + ThemeProvider
│   │   │   └── page.tsx                 #   Redirect → /admin
│   │   ├── components/
│   │   │   ├── app-sidebar.tsx          #   Sidebar + module switcher
│   │   │   ├── team-switcher.tsx        #   Dropdown chọn phân hệ
│   │   │   ├── nav-user.tsx             #   Avatar dropdown
│   │   │   ├── nav-main.tsx             #   Sidebar navigation
│   │   │   ├── nav-projects.tsx         #   Project navigation
│   │   │   ├── settings-dialog.tsx      #   Modal: profile edit + avatar upload + theme
│   │   │   ├── data-table.tsx           #   Generic DataTable (sort, search, pagination)
│   │   │   ├── crud-sheet.tsx           #   CRUD Sheet component
│   │   │   ├── confirm-dialog.tsx       #   DeleteConfirmDialog
│   │   │   ├── status-badge.tsx         #   Status badge component
│   │   │   ├── search-input.tsx         #   Search input
│   │   │   ├── skeleton-table.tsx       #   Loading skeleton
│   │   │   ├── image-upload.tsx         #   Image upload component
│   │   │   ├── editor/
│   │   │   │   └── tiptap.tsx           #   TipTap rich text editor
│   │   │   └── ui/                      #   shadcn/ui components (55+)
│   │   ├── lib/
│   │   │   ├── auth-context.tsx         #   AuthProvider + useAuth() hook
│   │   │   ├── auth-service.ts           #   Login/logout/profile
│   │   │   ├── api-client.ts            #   Fetch proxy → .NET API
│   │   │   ├── services/                #   Blog, CRM, Employee, Media services
│   │   │   ├── types/                   #   TypeScript interfaces
│   │   │   └── utils.ts                 #   Date utils (UTC → GMT+7)
│   │   ├── hooks/                       #   Custom React hooks
│   │   └── proxy.ts                    #   Next.js rewrites proxy config
│   ├── next.config.ts
│   ├── package.json
│   └── tsconfig.json
│
├── foodstore-store/                      # 🎨 Frontend (SvelteKit)
│   ├── src/
│   │   ├── lib/
│   │   │   ├── api/                     #   API client (21 modules)
│   │   │   ├── components/              #   UI: Icon, Modal, Accordion, Sidebar, Table...
│   │   │   │   ├── ui/
│   │   │   │   ├── editor/
│   │   │   │   └── media/
│   │   │   ├── services/                #   SignalR connection manager
│   │   │   ├── types/                   #   16 TypeScript interface files
│   │   │   ├── utils/                   #   State stores, cart, auth, helpers
│   │   │   └── config/                  #   Environment-aware config
│   │   └── routes/                      #   File-based routing
│   │       ├── +layout.svelte
│   │       ├── +page.svelte
│   │       ├── pos/                     #   /pos — Point of Sale
│   │       ├── kitchen/                 #   /kitchen — KDS
│   │       ├── logout/
│   │       └── error/
│   ├── Dockerfile
│   ├── svelte.config.js
│   ├── vite.config.ts
│   └── package.json
│
├── foodstore-landingpage/               # 🌐 Landing Page (Astro 7)
│   ├── src/
│   │   ├── components/
│   │   ├── layouts/
│   │   └── pages/
│   ├── Dockerfile
│   └── astro.config.mjs
│
├── scripts/                              # 📜 Database
│   └── init.sql                          #   PostgreSQL schema (28+ tables)
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
├── AGENTS.md                             # 🤖 AI context file
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

- POS creates order → status = `pending`, payment processed → `paid`
- Kitchen sees `paid` orders → starts preparing → `preparing`
- Kitchen completes → `served`
- Admin can cancel at any point
- SignalR broadcasts every status change

### 2a. Authentication Flow — Next.js Admin (BFF)

```
┌──────────┐     ┌──────────────────┐     ┌──────────────────────────┐
│  Browser │     │  Next.js Server  │     │  .NET API               │
│          │     │  (foodstore-admin)│    │                          │
├──────────┤     ├──────────────────┤     ├──────────────────────────┤
│  1. Form │────►│  Better Auth     │────►│  /api/auth/login        │
│  POST    │     │  validate        │     │  Validate credentials   │
│  /login  │     │  set HttpOnly    │     │  Return user + perms    │
│          │◄────│  session cookie  │────│                          │
│          │     │                  │     │                          │
│  2. GET  │────►│  middleware      │────►│  /api/proxy/:path*      │
│  /admin  │     │  verify session  │     │  .NET API proxy         │
│          │◄────│  Return page     │────│                          │
└──────────┘     └──────────────────┘     └──────────────────────────┘
```

- Better Auth sets HttpOnly session cookie on login
- Next.js middleware verifies session on each request
- API proxy: `/api/proxy/:path*` → `http://api:8080/v2/api/:path*`
- SignalR proxy: `/api/proxy/hubs/:path*` → `http://api:8080/hubs/:path*`

### 2b. Authentication Flow — Store / POS (.NET API JWT)

```
┌──────────┐     ┌──────────┐     ┌──────────────────┐
│  Client  │     │ Traefik  │     │  FoodstoreApi.Web │
├──────────┤     ├──────────┤     ├──────────────────┤
│  1. POST │────►│ :80/     │────►│  /api/auth/login  │
│          │     │ /api/*   │     │  JWT response    │
│          │◄────│          │◄────│  Bearer token    │
│          │     │          │     │                  │
│  2. GET  │────►│ :80/     │────►│  /api/xxx        │
│          │     │ /api/*   │     │  Bearer <jwt>    │
│          │◄────│          │◄────│  Return data     │
└──────────┘     └──────────┘     └──────────────────┘
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
│   UI     │     │ Traefik  │     │  FoodstoreApi.Web │     │  RustFS (S3 Object Store)│
├──────────┤     ├──────────┤     ├──────────────────┤     ├──────────────────────────┤
│  POST/   │────►│ :80/     │────►│  /api/media/     │────►│  PutObjectAsync (AWS SDK)│
│  upload  │     │ /api/*   │     │  upload           │     │  Validate & save         │
│          │     │          │     │                   │◄────│  Return public URL      │
│          │◄────│          │◄────│  {id, url}       │     │                          │
└──────────┘     └──────────┘     └──────────────────┘     └──────────────────────────┘
```

### 5. CMS Post Lifecycle

```
┌─────────┐    ┌───────────┐    ┌──────────┐    ┌───────────┐    ┌────────────┐
│  Draft  │──► │  Reviewed  │──► │Scheduled │──► │ Published  │──► │ Increment  │
│         │    │  (duyệt)   │    │(lên lịch)│    │ (xuất bản) │    │ view count │
└─────────┘    └───────────┘    └──────────┘    └───────────┘    └────────────┘
     │              │               │                │
     └──────────────┴───────────────┴────────────────┘
                            │
                       ┌────▼────┐
                       │ Deleted │
                       └─────────┘
```

- Posts support full workflow: draft → reviewed → scheduled/published
- Revisions auto-created on publish, view count tracking
- SEO metadata: meta title, description, focus keyword, OG image
- Categories + Tags via many-to-many relationships

### 6. Data Fetching — Next.js Admin (Proxy Pattern)

```
┌──────────────┐     ┌──────────────┐     ┌──────────────┐
│ Client Page  │     │  Next.js     │     │  .NET API    │
│ ("use client")│    │  Server      │     │  Controller  │
├──────────────┤     ├──────────────┤     ├──────────────┤
│ apiClient()  │────►│ fetch proxy  │────►│ Auth + perm  │
│ /api/proxy/  │     │ /api/proxy/  │     │ check        │
│ blog/posts   │     │ → api:8080/  │     │              │
│              │◄────│ parsed JSON  │◄────│ JSON result  │
└──────────────┘     └──────────────┘     └──────────────┘
```

### 7. Authorization (RBAC)

```
Request ──► JWT Middleware (validate token)
               └── ClaimsPrincipal with userId, roleId, permissions
                    └── [RequirePermission("module.action")] attribute
                         └── PermissionPolicyProvider
                              └── PermissionAuthorizationHandler
                                   └── bool (allow/deny)
```

Permissions follow `"{module}.{action}"`: `menu.view`, `blog.create`, `crm.view`, `dashboard.view`, etc.

### 8. Dashboard ML Flow

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

### 9. E-Invoice Lifecycle

```
                  ┌──────────┐
                  │  Draft   │
                  └────┬─────┘
                       │ Issue invoice
                  ┌────▼─────┐
                  │  Issued   │
                  └────┬─────┘
                       │
              ┌────────┴────────┐
              │                 │
         ┌────▼─────┐    ┌─────▼──────┐
         │  Failed   │    │ Cancelled  │
         └──────────┘    └────────────┘
```

- POS creates order → admin issues e-invoice via Viettel/MISA provider
- Supports multiple providers via factory pattern (`IEInvoiceProviderFactory`)
- Provider config stored as JSON in `e_invoice_providers` table
- Global settings: auto-issue flag, default template/serial, digital signature
- PDF/XML invoice files stored on RustFS S3
- 5 RBAC permissions: `einvoice.view`, `.providers`, `.issue`, `.cancel`, `.settings`

---

## Key Design Decisions

| Decision | Rationale |
|---|---|
| **Clean Architecture** | Isolates business rules from frameworks; swappable DB/UI |
| **RBAC with flat permissions** | Simpler than hierarchical roles; each action explicitly checked |
| **RustFS for media** | S3-compatible object storage (MinIO/RustFS/Garage) via AWS SDK |
| **SignalR + MessagePack** | Binary protocol reduces payload size for real-time updates |
| **TipTap for blog editor** | HTML output (compatible with Astro `set:html`), extensible |
| **Better Auth BFF** | HttpOnly cookie, no client-side token exposure |
| **CMS workflow** | Draft → reviewed → scheduled/published, revision history, SEO metadata |
| **React #418 guard** | `useState(mounted)` + `useEffect` + `return null` pattern for Dialog/Sheet/TipTap |
