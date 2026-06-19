# AGENTS.md — Foodstore AI Context

> **AI agents MUST read all files in `docs/` before performing any task.**

## Required Context Loading

Before any code generation, refactoring, or answering questions:

1. **Read `docs/5_coding_standards.md`** — master coding conventions
2. **Read `docs/2_flows_and_project_structure.md`** — project structure, architecture, data flows
3. **Read `docs/3_api.md`** — API reference (endpoints, auth, payloads)
4. **Read `docs/4_known_errors.json`** — known issues registry
5. **Read `docs/1_tech_stack.md`** — full tech stack with versions
6. **Read `docs/0_quick_reference.json`** — dev commands, ports, env configs

## Project Overview

- **Monorepo**: 4 sub-projects
  - `foodstore-api/` — .NET 10 Clean Architecture backend
  - `foodstore-store/` — SvelteKit 2 + Svelte 5 (POS, Kitchen, Admin)
  - `foodstore-admin/` — Next.js 16 + shadcn/ui (Admin Dashboard)
  - `foodstore-landingpage/` — Astro 6 (Landing Page)
- **Database**: PostgreSQL 18 with EF Core 10 + Npgsql
- **Deployment**: Docker Compose (8 containers: traefik, db, redis, rustfs, api, store, admin, landingpage)

## Critical Rules

### Auth Architecture (Admin)
- `foodstore-admin/` dùng **Better Auth** BFF pattern — login qua Next.js server, session HttpOnly cookie
- Better Auth tự ghi `users`, `sessions`, `accounts`, `verifications` vào PostgreSQL qua Drizzle ORM
- .NET API không can thiệp auth flow — chỉ làm business logic
- API proxy: Next.js server gọi .NET API với internal header, không fetch từ browser

### .NET Backend
- Clean Architecture: Core → Usecase → Infrastructure → Web; dependencies point inward
- Business logic trong Usecase Services, không trong Controllers
- Register DI trong mỗi layer: `Extensions/DependencyInjection.cs`
- Endpoints yêu cầu `[RequirePermission("module.action")]` ngoại trừ auth/public
- S3 config đọc từ env var `S3:AccessKey`, `S3:SecretKey` — không hardcode trong `appsettings.json`

### Next.js / Admin (foodstore-admin)
- Default Server Components — `"use client"` chỉ khi cần
- UI từ `src/components/ui/` (shadcn), `cn()` cho conditional classes, `lucide-react` icons
- `next-themes` cho dark mode
- Server Actions cho form mutations
- **Route convention**: module root → `/admin/{module}/dashboard` (e.g. `/admin/cms/dashboard`)
- **Notable bug patterns**: React #418 → mounted guard `useState`+`useEffect`+`null` return
- **DataTable** cho danh sách, **Sheet** cho CRUD, native `<select>`, không shadcn Select

### Svelte 5 (foodstore-store)
- Runes: `$state()`, `$derived()`, `$effect()`, `$props()`, `$bindable()`
- `$state.raw()` cho API data, `onclick` NOT `on:click`, `{#snippet}` NOT `<slot>`
- Class-based stores `.svelte.ts` (tránh SSR leak)
- Programmatic Flowbite API — không `initFlowbite()` + `data-*`

### Security
- `.env` gitignored — secrets chỉ trong `.env`, docker-compose dùng `${VAR}` interpolation
- `appsettings.json` dùng placeholder `"__CHANGE_ME__"` hoặc `"overridden_by_env"`
- Frontend proxy qua `/api/proxy/:path*` — không expose API trực tiếp

## Build Commands

| Project | Build | Lint | Type Check |
|---------|-------|------|------------|
| foodstore-store | `npm run build` | `npm run lint` | `npm run check` |
| foodstore-admin | `npm run build` | `npm run lint` | `npx tsc --noEmit` |
| foodstore-landingpage | `npm run build` | — | — |
| foodstore-api | `dotnet build foodstore-api` | — | — |

Run build sau mỗi thay đổi. Fix errors, warnings là tech debt chấp nhận được.

## Key Files

| File | Purpose |
|------|---------|
| `foodstore-api/FoodstoreApi.Web/Program.cs` | API composition root |
| `foodstore-api/FoodstoreApi.Web/Controllers/` | 25 API controllers |
| `foodstore-api/FoodstoreApi.Infrastructure/Data/StoreDbContext.cs` | EF Core DbContext (30+ DbSets) |
| `scripts/init.sql` | PostgreSQL schema — 28+ tables |
| `foodstore-admin/next.config.ts` | Next.js config + API proxy rules |
| `foodstore-admin/src/app/admin/` | CMS, CRM, Employees, Food pages |
| `foodstore-admin/src/components/editor/tiptap.tsx` | TipTap rich text editor |
| `foodstore-admin/src/lib/services/` | Blog, CRM, Employee services |
| `foodstore-admin/src/lib/api-client.ts` | Fetch proxy helper |

## Current Version

Latest tag: `1.3.0` — CMS system, CRM module, TipTap editor, Next.js admin expansion
Next tag: `2.0.0` — planned (major update with full CMS + CRM)
