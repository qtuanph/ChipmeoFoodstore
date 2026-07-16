# Tech Stack

> Detailed breakdown of every technology used across the Foodstore monorepo.

## Frontend — Store

| Category | Technology | Version | Purpose |
|---|---|---|---|
| Language | TypeScript | 6.0.3 | Type-safe development |
| UI Framework | Svelte | 5.56.5 | Reactive component framework with runes |
| Meta-Framework | SvelteKit | 2.69.3 | Routing, SSR, server load functions, form actions |
| Build Tool | Vite | 8.1.4 | Dev server & production bundler (Rolldown/Oxc) |
| CSS | Tailwind CSS | 4.3.2 | Utility-first styling |
| Tailwind Plugins | forms, typography, animate | — | Form resets, prose styles, animations |
| Rich Text Editor | TipTap (core + 8 extensions) | 3.28.0 | Blog post editing with images, links, SEO |
| Charts | ApexCharts | 5.16.0 | Analytics dashboards |
| Icons (Unified) | @iconify/svelte | 5.2.2 | Tabler Icons via Iconify API (275k+ icons) |
| Image Cropping | Croppie | 2.6.5 | Avatar & menu image cropping |
| Real-Time | @microsoft/signalr | 10.0.0 | WebSocket client for order/kitchen updates |
| Markdown | mdsvex | 0.12.7 | Markdown rendering in Svelte |
| Linter | ESLint | 10.7.0 | Code quality enforcement (flat config) |
| TS Lint Plugin | typescript-eslint | 8.64.0 | TypeScript-aware lint rules |
| Svelte Lint Plugin | eslint-plugin-svelte | 3.20.0 | Svelte-specific lint rules |
| Formatter | Prettier | 3.9.5 | Code formatting |
| Prettier Svelte | prettier-plugin-svelte | 4.1.1 | Svelte file formatting |
| Prettier Tailwind | prettier-plugin-tailwindcss | 0.8.1 | Tailwind class sorting |
| Type Checker | svelte-check | 4.7.3 | Svelte + TS type validation |
| Adapter | @sveltejs/adapter-node | 5.5.7 | Node.js production server (Docker) |

### Key Frontend Dependencies (production)

```json
{
  "@iconify/svelte": "^5.2.2",
  "@microsoft/signalr": "^10.0.0",
  "@tiptap/core": "^3.28.0",
  "@tiptap/extension-bubble-menu": "^3.28.0",
  "@tiptap/extension-floating-menu": "^3.28.0",
  "@tiptap/extension-image": "^3.28.0",
  "@tiptap/extension-link": "^3.28.0",
  "@tiptap/extension-placeholder": "^3.28.0",
  "@tiptap/extension-text-align": "^3.28.0",
  "@tiptap/extension-underline": "^3.28.0",
  "@tiptap/pm": "^3.28.0",
  "@tiptap/starter-kit": "^3.28.0",
  "apexcharts": "^5.16.0",
  "croppie": "^2.6.5"
}
```

## Backend — FoodstoreApi

### Layer Dependency Tree

```
FoodstoreApi.Web
  └── FoodstoreApi.Usecase
        └── FoodstoreApi.Core
FoodstoreApi.Infrastructure
  └── FoodstoreApi.Usecase (via DI registration)
```

### Core Layer

| Package | Version | Purpose |
|---|---|---|
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 10.0.10 | Identity + EF Core integration |
| Microsoft.EntityFrameworkCore.Abstractions | 10.0.10 | EF Core base abstractions (entity base types) |

### Usecase Layer

| Package | Version | Purpose |
|---|---|---|
| Microsoft.Extensions.Caching.Abstractions | 10.0.10 | Cache abstraction |
| Microsoft.Extensions.Configuration.Abstractions | 10.0.10 | Configuration binding |
| Microsoft.Extensions.DependencyInjection.Abstractions | 10.0.10 | DI container abstractions |
| Microsoft.Extensions.Http | 10.0.10 | HTTP client factory |
| Microsoft.ML | 5.0.0 | ML framework core |
| Microsoft.ML.TimeSeries | 5.0.0 | Time series forecasting (SSA) |
| System.IdentityModel.Tokens.Jwt | 8.19.2 | JWT token generation & validation |

### Infrastructure Layer

| Package | Version | Purpose |
|---|---|---|
| Npgsql.EntityFrameworkCore.PostgreSQL | 10.0.3 | PostgreSQL EF Core provider |
| Microsoft.Extensions.Caching.StackExchangeRedis | 10.0.10 | Redis distributed cache client |
| AWSSDK.S3 | 4.0.101.1 | AWS S3 SDK for object storage |
| AWSSDK.Extensions.NETCore.Setup | 4.0.100.4 | S3 DI integration |
| Microsoft.EntityFrameworkCore.Tools | 10.0.10 | EF Core CLI tooling (migrations) |

### Web Layer

| Package | Version | Purpose |
|---|---|---|
| Microsoft.AspNetCore.Authentication.JwtBearer | 10.0.10 | JWT Bearer token authentication |
| Microsoft.AspNetCore.OpenApi | 10.0.10 | OpenAPI/Swagger generation |
| Microsoft.AspNetCore.SignalR.Protocols.MessagePack | 10.0.10 | Binary SignalR protocol |
| Microsoft.EntityFrameworkCore.Design | 10.0.10 | EF Core design-time (migrations) |

## Database — PostgreSQL

- **Engine**: PostgreSQL 18 with ICU Vietnamese collation
- **Provider**: Entity Framework Core 10 + Npgsql
- **Database name**: `foodstore_shop`
- **Collation**: `vi_VN_ci_ai` (accent-insensitive, case-insensitive Vietnamese)
- **Tables**: 33+ (users, sessions, accounts, verifications, employees, customers, roles, permissions, role_permissions, categories, menu_items, addons, menu_item_addons, combos, combo_items, discounts, sources, orders, order_items, order_item_addons, order_status_history, payments, payment_settings, blog_posts, blog_categories, blog_post_categories, blog_post_tags, blog_post_revisions, blog_post_blocks, blog_settings, tags, media, refresh_tokens, sources, **e_invoices**, **e_invoice_providers**, **e_invoice_settings**)

### Database Layers

```
Better Auth (4 tables)     ───  Core Auth: users, sessions, accounts, verifications
  └── users (UUID PK)          Email/password + OAuth, banned support, UUID primary key
  └── sessions                 Server-side session tokens (optional, JWT-first otherwise)
  └── accounts                 OAuth providers (Google, Facebook...), 1 user → many providers
  └── verifications            OTP, magic links, reset-password tokens

Identity (2 tables)        ───  Employee & Customer profiles referencing users
  ├── employees                user_id → users, role_id → roles, employee_code, phone, avatar
  └── customers                user_id → users, customer_code, loyalty_points, membership_level

RBAC (3 tables)            ───  Role-based access control
  ├── roles                   name UNIQUE, is_system (true = not deletable: root, customer)
  ├── permissions             59 default permissions (13 modules: +einvoice)
  └── role_permissions        roles M:N permissions

E-Invoice (3 tables)       ───  Electronic invoice system
  ├── e_invoices              order FK, provider FK, amounts, buyer info, status lifecycle (draft/issued/failed/cancelled)
  ├── e_invoice_providers     name, provider_type, config JSON, active flag
  └── e_invoice_settings      single-row: default_provider, auto-issue, template, digital signature

Business + CMS (22+ tables) ───  Menu, orders, payments, blog, media
  └── categories / menu_items / addons / menu_item_addons
  └── combos / combo_items
  └── discounts / sources
  └── orders / order_items / order_item_addons / order_status_history
  └── payments / payment_settings
  └── blog_posts / blog_categories / blog_post_categories / blog_post_tags
  └── blog_post_revisions / blog_post_blocks / blog_settings
  └── tags / media / refresh_tokens
```

### Key Database Patterns

- `created_at` / `updated_at` timestamp columns on most entities (via `IAuditableEntity` interface + AuditSaveChangesInterceptor)
- `is_deleted` soft-delete flag (planned, not yet implemented)
- `status` enum-style string columns (order status flow: `pending → confirmed → preparing → ready → served → paid → cancelled`)
- UUID primary keys on all tables (`gen_random_uuid()`)
- Many-to-many via junction tables: `menu_item_addons`, `role_permissions`, `blog_post_categories`, `blog_post_tags`
- Entity image URLs stored as strings (images hosted on RustFS S3)
- SEO metadata stored directly on `blog_posts` table
- Order-payment relationship: 1 order has many payments (supports split payments)
- RBAC: roles → role_permissions → permissions (flat list of `"module.action"` strings), `is_system` flag prevents deletion of built-in roles

## Infrastructure & Integrations

| Feature | Technology |
|---|---|
| Real-Time | SignalR + MessagePack binary protocol |
| Caching | Redis 8 (distributed, via StackExchangeRedis) |
| Rate Limiting | System.Threading.RateLimiting (fixed window) |
| Authentication (API) | JWT (HS256, access + refresh tokens) |
| Authentication (Admin) | Better Auth (Next.js BFF) with Drizzle ORM — session cookies, HttpOnly, no client token exposure |
| Authorization | Custom permission-based RBAC with claims |
| Media Storage | RustFS (S3-compatible object storage) via AWS SDK |
| ML | ML.NET SSA forecasting + co-occurrence recommendations |
| E-Invoice | Multi-provider via factory pattern (Viettel, MISA) with `IEInvoiceProvider` abstraction |
| File Validation | Server-side MIME type + extension check |
| Deployment | Docker Compose (8 services) |
| Proxy | Vite dev proxy (localhost:5173 → localhost:5142) |

## Landing Page — foodstore-landingpage (Astro)

| Category | Technology | Version | Purpose |
|---|---|---|---|
| Language | TypeScript | 6.0.3 | Type-safe development |
| Meta-Framework | Astro | 7.0.9 | Static site generation, content-driven pages |
| Build Tool | Vite | (bundled with Astro) | Dev server & production bundler (Rust compiler) |
| Markdown Processor | Sätteri (built-in) | — | Native Markdown pipeline (replaces remark/rehype) |
| Compiler | @astrojs/compiler-rs (Rust) | — | Faster builds, stricter HTML validation |

## Admin Frontend — foodstore-admin (Better Auth BFF)

| Category | Technology | Version | Purpose |
|---|---|---|---|
| Language | TypeScript | 7.0.2 | Type-safe development |
| UI Framework | React | 19.2.7 | Component library |
| Meta-Framework | Next.js | 16.2.10 | App Router, server components, BFF |
| CSS | Tailwind CSS | 4.x | Utility-first styling |
| Component Lib | shadcn/ui + Base UI | latest | Accessible headless components |
| Auth | Better Auth | 1.6.19 | Authentication, session management, DB-backed |
| ORM | Drizzle | (bundled with Better Auth) | Database access via Better Auth |
| Charts | Recharts | 3.9.2 | Analytics dashboards |
| Icons | lucide-react | latest | Icon library |
| Styling Util | tailwind-merge + clsx | latest | Conditional classes merging |
| Toast/Sonner | sonner | 2.0.7 | Toast notifications |
| Date Picker | react-day-picker | 10.x | Date range selection |
| Build Tool | Next.js (Turbopack) | — | Dev server & production build |

### Auth Architecture (BFF)

```
[Browser] ← HttpOnly session cookie → [Next.js Server]
                                          ├── Better Auth (session verify)
                                          ├── Drizzle ORM → PostgreSQL
                                          └── Proxy ↓ .NET API (internal header)
```

- Browser NEVER sees access/refresh tokens — only HttpOnly session cookie
- Better Auth tự ghi `users`, `sessions`, `accounts`, `verifications` vào DB qua Drizzle
- .NET API không can thiệp auth flow — chỉ nhận identity từ Next.js proxy

### Admin E-Invoice Module

| Category | Technology | Purpose |
|---|---|---|
| Backend | Custom `IEInvoiceProvider` factory | Pluggable invoice provider abstraction (Viettel, MISA) |
| Frontend | 4 Next.js pages (`/admin/food/e-invoice/*`) | Dashboard, Transactions, Providers, Settings |
| DB Tables | `e_invoices`, `e_invoice_providers`, `e_invoice_settings` | Full invoice lifecycle + provider management |
| Permissions | 5 new (`einvoice.view`, `.providers`, `.issue`, `.cancel`, `.settings`) | Role-based access to e-invoice features |

## DevOps & Tools

| Tool | Purpose |
|---|---|
| Git | Version control |
| GitHub | Repository hosting |
| Visual Studio 2026 | Backend development |
| VS Code | Frontend development |
| dotnet-ef | Migration management |
| npm | Package management |
| Docker | Container runtime |
| Docker Compose | Multi-service orchestration |
| Traefik | Reverse proxy (ingress controller) |
