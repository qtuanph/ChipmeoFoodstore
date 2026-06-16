# Tech Stack

> Detailed breakdown of every technology used across the ChipmeoFoodstore monorepo.

## Frontend — ChipmeoPOS

| Category | Technology | Version | Purpose |
|---|---|---|---|
| Language | TypeScript | 6.0.3 | Type-safe development |
| UI Framework | Svelte | 5.56.3 | Reactive component framework with runes |
| Meta-Framework | SvelteKit | 2.65.1 | Routing, SSR, server load functions, form actions |
| Build Tool | Vite | 8.0.16 | Dev server & production bundler (Rolldown/Oxc) |
| CSS | Tailwind CSS | 4.3.1 | Utility-first styling |
| Tailwind Plugins | forms, typography, animate | — | Form resets, prose styles, animations |
| Rich Text Editor | TipTap (core + 8 extensions) | 3.26.1 | Blog post editing with images, links, SEO |
| Charts | ApexCharts | 5.15.0 | Analytics dashboards |
| Image Cropping | Croppie | 2.6.5 | Avatar & menu image cropping |
| Real-Time | @microsoft/signalr | 10.0.0 | WebSocket client for order/kitchen updates |
| Markdown | mdsvex | 0.12.7 | Markdown rendering in Svelte |
| Linter | ESLint | 10.5.0 | Code quality enforcement (flat config) |
| TS Lint Plugin | typescript-eslint | 8.61.1 | TypeScript-aware lint rules |
| Svelte Lint Plugin | eslint-plugin-svelte | 3.19.0 | Svelte-specific lint rules |
| Formatter | Prettier | 3.8.4 | Code formatting |
| Prettier Svelte | prettier-plugin-svelte | 4.1.1 | Svelte file formatting |
| Prettier Tailwind | prettier-plugin-tailwindcss | 0.8.0 | Tailwind class sorting |
| Type Checker | svelte-check | 4.6.0 | Svelte + TS type validation |
| Adapter | @sveltejs/adapter-node | 5.5.4 | Node.js production server (Docker) |

### Key Frontend Dependencies (production)

```json
{
  "@microsoft/signalr": "^10.0.0",
  "@tiptap/core": "^3.26.1",
  "@tiptap/extension-bubble-menu": "^3.26.1",
  "@tiptap/extension-floating-menu": "^3.26.1",
  "@tiptap/extension-image": "^3.26.1",
  "@tiptap/extension-link": "^3.26.1",
  "@tiptap/extension-placeholder": "^3.26.1",
  "@tiptap/extension-text-align": "^3.26.1",
  "@tiptap/extension-underline": "^3.26.1",
  "@tiptap/pm": "^3.26.1",
  "@tiptap/starter-kit": "^3.26.1",
  "apexcharts": "^5.15.0",
  "croppie": "^2.6.5"
}
```

## Backend — ChipmeoApis

### Layer Dependency Tree

```
ChipmeoApis.Web
  └── ChipmeoApis.Usecase
        └── ChipmeoApis.Core
ChipmeoApis.Infrastructure
  └── ChipmeoApis.Usecase (via DI registration)
```

### Core Layer

| Package | Version | Purpose |
|---|---|---|
| Microsoft.EntityFrameworkCore.Abstractions | 10.0.9 | EF Core base abstractions (entity base types) |

### Usecase Layer

| Package | Version | Purpose |
|---|---|---|
| BCrypt.Net-Next | 4.2.0 | Password hashing & verification |
| Microsoft.Extensions.Caching.Memory | 10.0.9 | In-memory cache abstraction |
| Microsoft.Extensions.Configuration.Abstractions | 10.0.9 | Configuration binding |
| Microsoft.Extensions.DependencyInjection.Abstractions | 10.0.9 | DI container abstractions |
| Microsoft.Extensions.Http | 10.0.9 | HTTP client factory |
| Microsoft.ML | 5.0.0 | ML framework core |
| Microsoft.ML.TimeSeries | 5.0.0 | Time series forecasting (SSA) |
| System.IdentityModel.Tokens.Jwt | 8.19.1 | JWT token generation & validation |

### Infrastructure Layer

| Package | Version | Purpose |
|---|---|---|
| Npgsql.EntityFrameworkCore.PostgreSQL | 10.0.2 | PostgreSQL EF Core provider |
| Microsoft.Extensions.Caching.StackExchangeRedis | 10.0.9 | Redis distributed cache client |
| AWSSDK.S3 | 4.0.24.5 | AWS S3 SDK for object storage |
| AWSSDK.Extensions.NETCore.Setup | 4.0.4.8 | S3 DI integration |
| Microsoft.EntityFrameworkCore.Tools | 10.0.9 | EF Core CLI tooling (migrations) |

### Web Layer

| Package | Version | Purpose |
|---|---|---|
| Microsoft.AspNetCore.Authentication.JwtBearer | 10.0.9 | JWT Bearer token authentication |
| Microsoft.AspNetCore.OpenApi | 10.0.9 | OpenAPI/Swagger generation |
| Microsoft.AspNetCore.SignalR.Protocols.MessagePack | 10.0.9 | Binary SignalR protocol |
| Microsoft.EntityFrameworkCore.Design | 10.0.9 | EF Core design-time (migrations) |

## Database — PostgreSQL

- **Engine**: PostgreSQL 18 with ICU Vietnamese collation
- **Provider**: Entity Framework Core 10 + Npgsql
- **Database name**: `pos_shop`
- **Collation**: `vi_VN_ci_ai` (accent-insensitive, case-insensitive Vietnamese)
- **Tables**: 23 (categories, menu_items, addons, combos, orders, order_items, customers, employees, roles, permissions, blog_posts, media, discounts, sources, payments, payment_settings, tags, etc.)

### Key Database Patterns

- `created_at` / `updated_at` timestamp columns on most entities
- `is_deleted` soft-delete flag
- `status` enum-style string columns (order status flow: `pending → confirmed → preparing → ready → served → paid → cancelled`)
- Many-to-many via junction tables: `menu_item_addons`, `role_permissions`, `blog_post_tags`
- Entity image URLs stored as strings (images hosted on RustFS S3)
- SEO metadata stored directly on `blog_posts` table
- Order-payment relationship: 1 order has many payments (supports split payments)
- RBAC: roles → role_permissions → permissions (flat list of `"module.action"` strings)

## Infrastructure & Integrations

| Feature | Technology |
|---|---|
| Real-Time | SignalR + MessagePack binary protocol |
| Caching | Redis 8 (distributed, via StackExchangeRedis) |
| Rate Limiting | System.Threading.RateLimiting (fixed window) |
| Authentication | JWT (HS256, access + refresh tokens) |
| Authorization | Custom permission-based RBAC with claims |
| Media Storage | RustFS (S3-compatible object storage) via AWS SDK |
| ML | ML.NET SSA forecasting + co-occurrence recommendations |
| File Validation | Server-side MIME type + extension check |
| Deployment | Docker Compose (5 services) |
| Proxy | Vite dev proxy (localhost:5173 → localhost:5142) |

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
