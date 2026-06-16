<div align="center">
  <h1>Chipmeo Foodstore 🍽️</h1>
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
    <img src="https://img.shields.io/badge/SQL_Server-CC2927?logo=microsoftsqlserver" alt="SQL Server">
  </p>

  <br>
</div>

## Overview

Chipmeo Foodstore is a production-ready restaurant management platform designed for small to medium food & beverage businesses. It handles the entire order lifecycle — from POS ordering and kitchen display to admin management and analytics — with real-time updates via SignalR.

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
- BFF integration with MessagePack protocol for efficient binary transport

### Machine Learning
- **Sales forecasting** using `Microsoft.ML.TimeSeries` (SSA estimation)
- **Combo recommendations** based on order co-occurrence analysis

## Tech Stack

### Frontend — ChipmeoPOS (`ChipmeoPOS/`)
| Technology | Purpose |
|---|---|
| **Svelte 5** + **SvelteKit 2** | UI framework with runes reactivity |
| **TypeScript 6** | Type-safe development |
| **Vite 8** (Rolldown/Oxc) | Build tool & dev server |
| **Tailwind CSS 4** | Utility-first styling |
| **TipTap** | Rich text editor for blog posts |
| **ApexCharts** | Interactive analytics charts |
| **Croppie** | Client-side image cropping |
| **SignalR** | Real-time WebSocket communication |

### Backend — ChipmeoApis (`ChipmeoApis/`)
| Layer | Technology |
|---|---|
| **Core** | .NET 10, EF Core Abstractions |
| **Usecase** | BCrypt, JWT, ML.NET, IMemoryCache |
| **Infrastructure** | EF Core (SQL Server), FluentFTP |
| **Web** | ASP.NET Core, SignalR, JWT Bearer, Rate Limiting |

### Media Server — MediaStorageManagement (`MediaStorageManagement/`)
- Standalone .NET 10 Minimal API for file storage
- API-key authenticated uploads/deletes
- Serves static files directly from disk

### Database
- **SQL Server** with Entity Framework Core 10
- 23 tables covering the full restaurant domain model
- Collation: `SQL_Latin1_General_CP1_CI_AI`

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    ChipmeoFoodstore                          │
├─────────────────────┬───────────────────┬───────────────────┤
│    ChipmeoPOS       │   ChipmeoApis     │MediaStorageMgmt   │
│   (SvelteKit UI)    │  (.NET Clean API) │  (.NET File API)  │
│                     │                   │                   │
│  ┌───────────────┐  │ ┌─────────────┐   │ ┌─────────────┐   │
│  │  POS Module   │  │ │   Web/API   │   │ │Minimal API  │   │
│  │  Admin Module │  │ │   (JWT)     │   │ │(API Key)    │   │
│  │  Kitchen View │  │ ├─────────────┤   │ └─────────────┘   │
│  │  Blog Editor  │  │ │ Usecase     │   │                   │
│  └───────┬───────┘  │ ├─────────────┤   └───────────────────┘
│          │           │ │Infrastructure│          │
│          │  SignalR  │ ├─────────────┤          │
│          ◄──────────►│ │   Core      │          │
│          │  REST API │ └─────────────┘          │
│          ◄──────────►│         │                │
│          │           │    SQL Server │     Disk Storage
│          └───────────┴──────────┴────────────────┘
```

The backend follows **Clean Architecture** with 4 layers:
1. **Core** — domain entities (no dependencies)
2. **Usecase** — business logic, DTOs, service interfaces
3. **Infrastructure** — EF Core DbContext, repositories, FTP
4. **Web** — controllers, middleware, SignalR hub

## Project Structure

```
ChipmeoFoodstore/
├── ChipmeoApis/                    # Backend (.NET 10)
│   ├── ChipmeoApis.Core/           #   Domain entities
│   ├── ChipmeoApis.Usecase/        #   Business logic & DTOs
│   ├── ChipmeoApis.Infrastructure/ #   EF Core, repositories
│   └── ChipmeoApis.Web/            #   API controllers, hubs
├── ChipmeoPOS/                     # Frontend (SvelteKit)
│   ├── src/
│   │   ├── lib/
│   │   │   ├── api/                #   API client modules
│   │   │   ├── components/         #   Reusable UI components
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
├── MediaStorageManagement/         # Media file server
│   └── MediaStorageManagement/
└── docs/
```

## Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js 22+](https://nodejs.org)
- [SQL Server](https://www.microsoft.com/en-us/sql-server) (local or remote)
- [Git](https://git-scm.com)

### 1. Clone & Setup Database

```bash
git clone https://github.com/qtuanph/ChipmeoFoodstore.git
cd ChipmeoFoodstore
```

Create a SQL Server database named `pos_shop` and run the migration:

```bash
cd ChipmeoApis
dotnet ef database update --project ChipmeoApis.Web
```

> **Note**: Update the connection string in `ChipmeoApis/ChipmeoApis.Web/appsettings.json` to point to your SQL Server instance.

### 2. Run the Backend API

```bash
cd ChipmeoApis
dotnet run --project ChipmeoApis.Web
```

The API starts at `http://localhost:5142`.

### 3. Run the Media Server

```bash
cd MediaStorageManagement
dotnet run --project MediaStorageManagement
```

### 4. Run the Frontend

```bash
cd ChipmeoPOS
npm install
npm run dev
```

The frontend starts at `http://localhost:5173` with a Vite proxy forwarding `/api` and `/hubs` to the backend.

### Build for Production

```bash
# Frontend
cd ChipmeoPOS
npm run build          # Output: .svelte-kit/output/

# Backend
cd ChipmeoApis
dotnet publish -c Release
```

## Configuration

### Backend (`appsettings.json`)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=pos_shop;Integrated Security=True;TrustServerCertificate=True"
  },
  "JwtSettings": {
    "SecretKey": "your-256-bit-secret",
    "Issuer": "ChipmeoApisAPI",
    "Audience": "ChipmeoApisFrontend",
    "ExpiryInHours": 1
  }
}
```

### Frontend (`src/lib/config/index.ts`)
The frontend auto-detects its environment (localhost, Vercel demo, or production via `food.chipmeo.io.vn`) and selects the corresponding API URLs.

### Media Server (`appsettings.json`)
```json
{
  "Storage": {
    "Path": "D:\\MediaStorage"
  },
  "Security": {
    "ApiKey": "your-api-key"
  }
}
```

## API Overview

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
- `GET /admin/dashboard/overview` — KPIs
- `GET /admin/dashboard/analytics` — Time-series stats
- `GET /admin/dashboard/forecast` — ML sales forecast
- `GET /admin/dashboard/recommendations` — Combo recommendations

### Kitchen
- `GET /api/kitchen/orders` — Order queue
- `PUT /api/kitchen/orders/{id}/start` — Start preparing
- `PUT /api/kitchen/orders/{id}/complete` — Complete order

### Media
- `GET /api/media` — List media
- `POST /api/media/upload` — Upload file
- `DELETE /api/media/{id}` — Delete media

### SignalR Hub
- **Endpoint**: `/hubs/app`
- **Events**: order updates, menu updates, table updates, kitchen notifications

## Deployment

The project is designed for flexible deployment:

| Component | Hosting Options |
|---|---|
| **Frontend** | Vercel, Netlify, IIS, any Node.js host |
| **Backend API** | IIS, Azure App Service, Linux container |
| **Media Server** | IIS, Azure, any Windows/Linux server |
| **Database** | SQL Server (on-prem or cloud) |

Environment-specific domains:
- `food.chipmeo.io.vn` — Production frontend
- `api.chipmeo.io.vn` — Production API
- `media.chipmeo.io.vn` — Media server
- `foodchipmeo.vercel.app` — Vercel demo

## Roadmap

- [ ] Docker Compose setup (API + DB + Media Server)
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
