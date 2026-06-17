# AGENTS.md — Foodstore AI Context

> ⚠️ **AI agents MUST read all files in `docs/` before performing any task.**

## Required Context Loading

Before any code generation, refactoring, or answering questions:

1. **Read `docs/5_coding_standards.md`** — master coding conventions (Svelte 5 runes, .NET Clean Architecture, naming, patterns)
2. **Read `docs/2_flows_and_project_structure.md`** — project structure, architecture diagrams, data flows
3. **Read `docs/3_api.md`** — API reference (endpoints, auth, payloads)
4. **Read `docs/4_known_errors.json`** — known issues registry (do not reintroduce fixed issues)
5. **Read `docs/1_tech_stack.md`** — full tech stack with versions
6. **Read `docs/0_quick_reference.json`** — dev commands, ports, env configs

## Project Overview

- **Monorepo**: 3 sub-projects under `Foodstore/`
  - `FoodstoreApi/` — .NET 10 Clean Architecture backend
  - `Store/` — SvelteKit 2 + Svelte 5 frontend
  - `MediaStorageManagement/` — .NET 10 media file server
- **Database**: SQL Server 2022+ with EF Core 10

## Critical Rules for AI

### .NET Backend
- Follow Clean Architecture: Core → Usecase → Infrastructure → Web
- Dependencies point inward. Never reference Infrastructure from Core.
- Business logic in Usecase Services, NOT in Controllers
- Use `sealed record` for DTOs, primary constructors, `CancellationToken` in async methods
- Register DI in each layer's `Extensions/DependencyInjection.cs`
- All API endpoints require `[RequirePermission("module.action")]` except auth/public

### Svelte 5 Frontend
- Use runes: `$state()`, `$derived()`, `$effect()`, `$props()`, `$bindable()`
- Use `$state.raw()` for API data (replaced, not mutated)
- Use `onclick` NOT `on:click`
- Use `{#snippet}` NOT `<slot>`
- Class-based stores in `.svelte.ts` (NOT module-level runes — SSR leak)
- Event handlers: named functions or inline arrows

### General
- `{#each}` blocks MUST have a `key`
- No `any` types — if urgent, use `warn` level in eslint config
- Sanitize HTML before `{@html ...}` (XSS prevention)
- Prefer Tailwind utility classes over custom CSS

## Build & Verify Commands

| Project | Build | Lint | Type Check |
|---|---|---|---|
| Store | `npm run build` | `npm run lint` | `npm run check` |
| FoodstoreApi | `dotnet build FoodstoreApi` | — | — |
| MediaStorage | `dotnet build MediaStorageManagement` | — | — |

Run build after every change. Fix errors, not warnings (warnings are accepted tech debt).

## Commit Convention

```
type(scope): brief description

- Bullet list of details
```

Types: `feat`, `fix`, `chore`, `docs`, `style`, `refactor`, `perf`, `test`

## Key Files

| File | Purpose |
|---|---|
| `FoodstoreApi/FoodstoreApi.Web/Program.cs` | API composition root |
| `FoodstoreApi/FoodstoreApi.Web/Controllers/` | API endpoints |
| `FoodstoreApi/FoodstoreApi.Infrastructure/Data/StoreDbContext.cs` | EF Core DbContext |
| `Store/src/lib/api/utils.ts` | API request helper (JWT auth) |
| `Store/src/lib/config/index.ts` | Environment-aware config |
| `Store/src/routes/` | SvelteKit file-based routes |
| `Store/vite.config.ts` | Vite 8 config (Rolldown bundler) |
| `Store/eslint.config.js` | ESLint 10 flat config (warnings for tech debt) |

## Tag Versioning Convention

This project follows **[Semantic Versioning](https://semver.org/)** (SemVer):

```
MAJOR.MINOR.PATCH  (e.g., 1.0.0, 2.3.1)
```

| Bump | When | Example |
|---|---|---|
| **MAJOR** | Breaking API changes, breaking DB schema changes, major architecture rewrite | `1.0.0` → `2.0.0` |
| **MINOR** | New features, new API endpoints, new modules (backward compatible) | `1.0.0` → `1.1.0` |
| **PATCH** | Bug fixes, package updates, refactoring, docs, config changes (no new features) | `1.0.0` → `1.0.1` |

### Rules
- Only the **project owner** decides when to tag. AI must **ask** before creating any tag.
- Tags are applied to `master` branch only (never on feature branches).
- Each tag must have a corresponding [GitHub Release](https://docs.github.com/en/repositories/releasing-projects-on-github/managing-releases-in-a-repository).
- For **squashing**: AI must get explicit user confirmation before force-pushing squashed commits.

### Current Tags

| Tag | Description |
|---|---|
| `1.0.0` | Initial project state |
| `1.0.1` | Package upgrades + docs + coding standards |
| `1.1.0` | Docker Compose stack, API v2, env var config |
| `1.2.0` | Icon migration (Iconify + Tabler), UI refactor, docs update |

## Important Constraints

- **NO git commands** (`commit`, `push`, `tag`, `checkout`, `rebase`, etc.) unless explicitly requested by the user
- Do NOT modify `docs/` files without explicit user request
- Do NOT remove eslint rule overrides without fixing all corresponding violations
- Do NOT downgrade packages (all are at latest compatible versions)
- Do NOT add new dependencies without user approval
- Do NOT modify `package-lock.json` manually (use `npm install <pkg>`)
