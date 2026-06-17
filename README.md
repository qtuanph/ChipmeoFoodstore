<div align="center">
  <h1>Foodstore 🍽️</h1>
  <p>
    <strong>Modern Restaurant Management & POS System</strong>
  </p>
  <p>
    A full-stack monorepo featuring a real-time Point of Sale, admin dashboard, kitchen display system, and machine learning-powered analytics.
  </p>

  <p>
    <img src="https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet" alt=".NET 10">
    <img src="https://img.shields.io/badge/Svelte-5-FF3E00?logo=svelte" alt="Svelte 5">
    <img src="https://img.shields.io/badge/SvelteKit-2-FF3E00?logo=svelte" alt="SvelteKit 2">
    <img src="https://img.shields.io/badge/TypeScript-6-3178C6?logo=typescript" alt="TypeScript 6">
    <img src="https://img.shields.io/badge/Vite-8-646CFF?logo=vite" alt="Vite 8">
    <img src="https://img.shields.io/badge/Tailwind_CSS-4-06B6D4?logo=tailwindcss" alt="Tailwind CSS 4">
    <img src="https://img.shields.io/badge/PostgreSQL-18-4169E1?logo=postgresql" alt="PostgreSQL 18">
    <img src="https://img.shields.io/badge/Redis-8-DC382D?logo=redis" alt="Redis 8">
    <img src="https://img.shields.io/badge/Traefik-latest-24D1C4?logo=traefik" alt="Traefik">
    <img src="https://img.shields.io/badge/Docker-2496ED?logo=docker" alt="Docker">
  </p>

  <br>
</div>

## Overview

Foodstore is a production-ready restaurant management platform designed for small to medium food & beverage businesses. It handles the entire order lifecycle — from POS ordering and kitchen display to admin management and analytics — with real-time updates via SignalR.

## Features

### Point of Sale (`/pos`)
- Intuitive menu browsing with categories, combos, and add-ons
- Cart management with discount code application
- Multiple payment methods: cash, QR (VietQR), ZaloPay, Momo, card
- Customer lookup by phone number
- Order history per session

### Kitchen Display System (`/kitchen`)
- Real-time order queue with automatic updates
- Status transitions: paid → preparing → served
- Sound notifications for new orders
- Kiosk-friendly layout for tablet/mounted displays

### Admin Dashboard (`/admin`)
- **Dashboard KPIs**: revenue, orders, customers, popular items
- **Analytics**: time-series charts, ML-powered sales forecasting, combo recommendations
- **Menu management**: categories, menu items, add-ons, combos with image upload
- **Order management**: full order lifecycle, status tracking, payment processing, refunds
- **Employee management**: staff accounts, role assignment, login tracking
- **Role-Based Access Control**: granular permissions per module/action
- **Customer management**: profiles, loyalty points, order history
- **Discount codes**: percentage/fixed, usage limits, date ranges
- **Blog & SEO**: rich text editor (TipTap), meta fields, Open Graph, tag management
- **Media gallery**: upload, usage tracking, unused image cleanup
- **Payment settings**: VietQR bank configuration
- **Sources/Tables**: order source management for dine-in

### Real-Time
- **SignalR hub** for live order updates, menu changes, table status, and kitchen notifications
- MessagePack protocol for efficient binary transport

### Machine Learning
- **Sales forecasting** using `Microsoft.ML.TimeSeries` (SSA estimation)
- **Combo recommendations** based on order co-occurrence analysis

## Tech Stack

### Frontend — Store (`Store/`)
| Technology | Purpose |
|---|---|
| **Svelte 5** + **SvelteKit 2** | UI framework with runes reactivity |
| **TypeScript 6** | Type-safe development |
| **Vite 8** (Rolldown/Oxc) | Build tool & dev server |
| **Tailwind CSS 4** | Utility-first styling |
| **@iconify/svelte** | Tabler Icons via Iconify (275k+ icons, on-demand) |
| **Flowbite** | Vanilla JS UI components via data-* API + initFlowbite() |
| **TipTap** | Rich text editor for blog posts |
| **ApexCharts** | Interactive analytics charts |
| **Croppie** | Client-side image cropping |
| **SignalR** | Real-time WebSocket communication |
| **@sveltejs/adapter-node** | Node.js production server (Docker) |

### Backend — FoodstoreApi (`FoodstoreApi/`)
| Layer | Technology |
|---|---|
| **Core** | .NET 10, EF Core Abstractions |
| **Usecase** | BCrypt, JWT, ML.NET, StackExchangeRedis |
| **Infrastructure** | EF Core (PostgreSQL/Npgsql), AWS S3 SDK, Redis |
| **Web** | ASP.NET Core, SignalR, JWT Bearer, Rate Limiting |

### Database
- **PostgreSQL 18** with Vietnamese collation (`vi-VN-x-ICU`)
- 23 tables covering the full restaurant domain model
- Entity Framework Core 10 with Npgsql provider

### Caching
- **Redis 8** — distributed cache (replaces legacy `IMemoryCache`)

### Media Storage
- **RustFS** (S3-compatible object storage)
- **AWS SDK for .NET** (`AWSSDK.S3`) for S3 API calls

### Reverse Proxy
- **Traefik** (latest) — single entry point, routes `/api/*` & `/hubs/*` to API, everything else to webapp

## Architecture

```
Internet (port 80)
     │
     ▼
┌──────────┐
│  Traefik  │  ← Reverse Proxy (latest)
└────┬─────┘
     │
     ├── /api/* , /hubs/* ────────► api:8080 (ASP.NET Core 10)
     │
     └── Host(localhost) ─────────► webapp:3000 (SvelteKit Node)
                                        │
                                        ▼
                                   api:8080 (SSR internal)
                                        │
                               ┌────────┼────────┐
                               ▼        ▼        ▼
                            db:5432  redis:6379  rustfs:9000
                          (PG 18)   (Redis 8)   (S3 Storage)
```

The backend follows **Clean Architecture** with 4 layers:
1. **Core** — domain entities (no dependencies)
2. **Usecase** — business logic, DTOs, service interfaces
3. **Infrastructure** — EF Core DbContext, repositories, S3, Redis
4. **Web** — controllers, middleware, SignalR hub

## Project Structure

```
Foodstore/
├── FoodstoreApi/                    # Backend (.NET 10)
│   ├── FoodstoreApi.Core/           #   Domain entities
│   ├── FoodstoreApi.Usecase/        #   Business logic & DTOs
│   ├── FoodstoreApi.Infrastructure/ #   EF Core, S3, Redis
│   ├── Dockerfile                  #   Docker image
│   └── FoodstoreApi.Web/            #   API controllers, hubs
├── Store/                     # Frontend (SvelteKit)
│   ├── Dockerfile                  #   Docker image
│   ├── src/
│   │   ├── lib/
│   │   │   ├── api/                #   API client modules
│   │   │   ├── components/         #   Reusable UI components
│   │   │   │   ├── ui/             #     Base components: Icon, Modal, Button, Sidebar, Table, etc.
│   │   │   │   ├── editor/         #     TipTap rich text editor
│   │   │   │   └── media/          #     Media gallery modal
│   │   │   ├── services/           #   SignalR client
│   │   │   ├── types/              #   TypeScript interfaces
│   │   │   ├── utils/              #   Stores & helpers
│   │   │   └── config/             #   Environment config
│   │   └── routes/
│   │       ├── pos/                #   Point of Sale
│   │       ├── kitchen/            #   Kitchen display
│   │       ├── admin/              #   Admin dashboard
│   │       ├── logout/
│   │       └── error/
│   └── ...
├── scripts/                        # DB init scripts
│   └── init.sql                    # PostgreSQL schema + seed
├── docker-compose.yml              # Full stack orchestration
├── .env                            # Environment variables
└── docs/
```

## Getting Started

### Prerequisites
- [Docker](https://www.docker.com) + Docker Compose
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (local dev)
- [Node.js 22+](https://nodejs.org) (local dev)

### 1. Quick Start (Docker — Recommended)

```bash
# Clone & start full stack
git clone https://github.com/qtuanph/Foodstore.git
cd Foodstore

# Full stack: traefik + db + redis + rustfs + api + webapp
docker compose up -d
```

Open **http://localhost** → Traefik routes to webapp.

Default admin login: **admin** / **admin123**

### 2. Local Development (without Docker)

```bash
# Terminal 1 — Dependencies (PostgreSQL + Redis + RustFS)
docker compose up -d db redis rustfs

# Terminal 2 — Backend API
cd FoodstoreApi
dotnet run --project FoodstoreApi.Web   # http://localhost:5142

# Terminal 3 — Frontend
cd Store
npm install
npm run dev                            # http://localhost:5173
```

> The Vite dev server proxies `/api` and `/hubs` to `localhost:5142` automatically.

### 3. Build Docker Images

```bash
docker compose build
```

## Configuration

### Environment (`.env`)
```env
# PostgreSQL
POSTGRES_USER=foodstore
POSTGRES_DB=pos_shop
DB_PASSWORD=REMOVED

# Redis
REDIS_PASSWORD=

# S3 (RustFS)
S3_ACCESS_KEY=foodstore
S3_SECRET_KEY=REMOVED
S3_BUCKET=food-media

# JWT
JWT_SECRET=REMOVED

# Frontend
PUBLIC_API_URL=http://api:8080
```

### Backend (`appsettings.json`)
Settings are overridden by environment variables in Docker (via `docker-compose.yml`). The committed `appsettings.json` contains dummy placeholders — real secrets are in `.env` and passed as env vars to containers. Key config sections:
- `ConnectionStrings:DefaultConnection` — PostgreSQL
- `ConnectionStrings:Redis` — Redis
- `S3:Endpoint` / `S3:Bucket` — S3 object storage
- `JwtSettings:SecretKey` — JWT signing key

### Frontend (`src/lib/config/index.ts`)
Auto-detects environment. In Docker, server-side uses `PUBLIC_API_URL=http://api:8080`; browser uses relative URLs via Traefik proxy.

## API Overview

All API requests go through Traefik at `http://localhost/api/*`.

### Authentication
- `POST /api/auth/login` — Employee login (JWT)
- `POST /api/auth/refresh` — Refresh token
- `POST /api/customers/register` — Customer registration
- `POST /api/customers/login` — Customer login

### POS
- `GET /pos/menu` — Full menu data
- `POST /pos/orders` — Create order
- `PUT /pos/orders/{id}/status` — Update order status
- `POST /pos/orders/{id}/payment` — Process payment

### Admin
- Full CRUD for categories, menu items, add-ons, combos, discounts, sources, employees, roles, customers, blog posts, tags, payment settings

### Kitchen
- `GET /api/kitchen/orders` — Order queue
- `PUT /api/kitchen/orders/{id}/start` — Start preparing
- `PUT /api/kitchen/orders/{id}/complete` — Complete order

### Media
- `POST /api/media/upload` — Upload file (saved to S3)
- `GET /api/media` — List media
- `DELETE /api/media/{id}` — Delete media

### SignalR Hub
- **Endpoint**: `/hubs/app`
- **Events**: order updates, menu updates, table updates, kitchen notifications

## Docker Compose Services

| Service | Image | Internal Port | External | Depends On |
|---------|-------|--------------|----------|------------|
| **traefik** | `traefik:latest` | `:8080` (web) | **`:80`** | — |
| **db** | `postgres:18-alpine` | `:5432` | — | — |
| **redis** | `redis:8-alpine` | `:6379` | — | — |
| **rustfs** | `rustfs/rustfs:latest` | `:9000` (S3), `:9001` (console) | — | — |
| **api** | `chipmeofoodstore-api` (build) | `:8080` | — | db, redis, rustfs (healthy) |
| **webapp** | `chipmeofoodstore-webapp` (build) | `:3000` | — | api |

### Data Volumes
| Volume | Mount | Purpose |
|--------|-------|---------|
| `postgres_data` | `/var/lib/postgresql` | PostgreSQL data |
| `redis_data` | `/data` | Redis persistence |
| `s3_data` | `/data` | RustFS object storage |

## Deployment

```bash
docker compose up -d
```

### Roadmap
- [x] Docker Compose (Traefik + PostgreSQL + Redis + RustFS + API + Frontend)
- [ ] Customer mobile ordering via QR code
- [ ] Multi-branch/outlet support
- [ ] Offline POS mode
- [ ] Inventory management
- [ ] Supplier management
- [ ] E-invoice integration
- [ ] i18n (Vietnamese/English)
- [ ] PWA support

## License

This project is private software. All rights reserved.
