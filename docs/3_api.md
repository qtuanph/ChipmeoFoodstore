# API Reference v2

> RESTful API với response envelope chuẩn. Tất cả routes đều có prefix `/v2/`.

## Base URLs

| Environment | Base URL |
|---|---|
| **Docker (Traefik)** | `http://api.localhost/v2` |

## Response Envelope

Mọi response đều theo format:

```json
// ✅ Success — single resource (HTTP 200)
{
  "data": { "id": 1, "name": "Alice", "createdAt": "2026-01-15T10:30:00Z" },
  "error": null,
  "meta": { "requestId": "...", "timestamp": "2026-01-15T10:30:00Z" }
}

// ✅ Success — collection (HTTP 200)
{
  "data": [ ... ],
  "error": null,
  "meta": {
    "page": 1,
    "pageSize": 20,
    "totalCount": 87,
    "totalPages": 5
  }
}

// ✅ Error (HTTP 4xx/5xx)
{
  "data": null,
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Username and password are required",
    "details": [
      { "field": "email", "message": "must not be empty" }
    ]
  },
  "meta": { "requestId": "...", "timestamp": "..." }
}
```

### HTTP Status Codes

| Code | Meaning |
|---|---|
| 200 | Success |
| 201 | Created (POST) |
| 204 | No Content (DELETE) |
| 400 | Bad Request / Validation Error |
| 401 | Unauthorized |
| 403 | Forbidden |
| 404 | Not Found |
| 429 | Too Many Requests |
| 500 | Internal Server Error |

### Error Codes

| Code | Description |
|---|---|
| `VALIDATION_ERROR` | Input validation failed |
| `NOT_FOUND` | Resource not found |
| `INTERNAL_ERROR` | Server error |
| `UNAUTHORIZED` | Authentication required |
| `FORBIDDEN` | Insufficient permissions |

---

## Authentication

### Employee Login

```
POST /v2/api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "password123"
}

Response 200:
{
  "data": {
    "token": "eyJhbGciOiJI...",
    "refreshToken": "dGhpcyBpcyBh...",
    "user": {
      "id": 1,
      "username": "admin",
      "fullName": "Admin",
      "roleId": 1,
      "roleName": "Admin",
      "avatarUrl": null,
      "permissions": ["order.view", "menu.edit", ...]
    }
  },
  "error": null,
  "meta": { "timestamp": "..." }
}
```

### Refresh Token

```
POST /v2/api/auth/refresh
Content-Type: application/json

{
  "refreshToken": "dGhpcyBpcyBh..."
}
```

### Logout

```
POST /v2/api/auth/logout
Authorization: Bearer <token>
```

### Profile

```
GET /v2/api/auth/profile
Authorization: Bearer <token>

PUT /v2/api/auth/profile
Authorization: Bearer <token>
Content-Type: application/json

{
  "fullName": "New Name",
  "email": "new@email.com"
}
```

### Customer Login

```
POST /v2/api/customers/login
Content-Type: application/json

{
  "phone": "0901234567",
  "password": "customerpass"
}
```

### Customer Registration

```
POST /v2/api/customers/register
Content-Type: application/json

{
  "phone": "0901234567",
  "fullName": "Nguyen Van A",
  "password": "customerpass"
}
```

### Authentication Header

```
Authorization: Bearer <jwt_token>
```

---

## Health

| Method | Route | Description |
|---|---|---|
| `GET` | `/v2/api/health` | Server health check |

---

## POS Endpoints

| Method | Route | Description | Auth |
|---|---|---|---|
| `GET` | `/v2/api/pos/menu` | Full menu (categories + items + addons + combos + discounts) | Employee |
| `GET` | `/v2/api/pos/sources` | Active order sources/tables | Employee |
| `GET` | `/v2/api/pos/addons` | All add-ons | Employee |
| `POST` | `/v2/api/pos/orders` | Create new order | Employee |
| `PUT` | `/v2/api/pos/orders/{id}/status` | Update order status | Employee |
| `POST` | `/v2/api/pos/orders/{id}/payment` | Process payment | Employee |

### POS Menu Response

```json
{
  "data": {
    "categories": [
      {
        "id": 1,
        "name": "Coffee",
        "imageUrl": "http://localhost/uploads/food-media/categories/coffee.jpg",
        "items": [...]
      }
    ],
    "combos": [...],
    "addons": [...],
    "discounts": [...]
  },
  "error": null,
  "meta": { "timestamp": "..." }
}
```

---

## Kitchen Endpoints

| Method | Route | Description | Auth |
|---|---|---|---|
| `GET` | `/v2/api/kitchen/orders` | Get kitchen order queue | Employee |
| `PUT` | `/v2/api/kitchen/orders/{id}/start` | Start preparing | Employee |
| `PUT` | `/v2/api/kitchen/orders/{id}/complete` | Complete → served | Employee |

---

## Admin Endpoints

### CRUD Pattern

| Method | Route | Description |
|---|---|---|
| `GET` | `/v2/api/admin/{resource}` | List all |
| `GET` | `/v2/api/admin/{resource}/{id}` | Get by ID |
| `POST` | `/v2/api/admin/{resource}` | Create |
| `PUT` | `/v2/api/admin/{resource}/{id}` | Update |
| `DELETE` | `/v2/api/admin/{resource}/{id}` | Delete |

### Resources

| Resource | Route Prefix | Permission |
|---|---|---|
| Categories | `/v2/api/admin/categories` | `category.*` |
| Menu Items | `/v2/api/admin/menuitems` | `menu.*` |
| Addons | `/v2/api/admin/addons` | `addon.*` |
| Combos | `/v2/api/admin/combos` | `menu.*` |
| Discounts | `/v2/api/admin/discounts` | `discount.*` |
| Sources (tables) | `/v2/api/admin/sources` | `source.*` |
| Orders | `/v2/api/admin/orders` | `order.*` |
| Employees | `/v2/api/admin/employees` | `employee.*` |
| Roles | `/v2/api/admin/roles` | `role.*` |
| Role Permissions | `/v2/api/admin/roles/{roleId}/permissions` | `role.*` |
| Permissions | `/v2/api/admin/permissions` | `role.*` |
| Payment Settings | `/v2/api/admin/payment-settings` | `payment.*` |

### Special Admin Endpoints

| Method | Route | Description |
|---|---|---|
| `GET` | `/v2/api/admin/dashboard/stats` | KPIs (revenue, orders, customers) |
| `GET` | `/v2/api/admin/dashboard/analytics` | Time-series data |
| `GET` | `/v2/api/admin/dashboard/forecast` | Sales forecast (7 days) |
| `GET` | `/v2/api/admin/dashboard/recommendations` | Combo recommendations |
| `GET` | `/v2/api/admin/orders/paged?page=1&pageSize=20` | Paginated orders |
| `GET` | `/v2/api/admin/orders/status/{status}` | Orders by status |
| `PUT` | `/v2/api/admin/orders/{id}/set-unpaid` | Revert payment |
| `GET` | `/v2/api/admin/orders/date-range?fromDate=...&toDate=...` | Orders by date range |

### Paginated Response Example

```json
GET /v2/api/admin/orders/paged?page=1&pageSize=10

{
  "data": [
    { "id": 42, "finalAmount": 78300, "status": "paid", ... }
  ],
  "error": null,
  "meta": {
    "page": 1,
    "pageSize": 10,
    "totalCount": 87,
    "totalPages": 9
  }
}
```

---

## Customer Endpoints

| Method | Route | Description | Auth |
|---|---|---|---|
| `POST` | `/v2/api/customers/register` | Register | None |
| `POST` | `/v2/api/customers/login` | Login | None |
| `GET` | `/v2/api/customers/me` | Get profile | Customer |
| `PUT` | `/v2/api/customers/me` | Update profile | Customer |
| `GET` | `/v2/api/customers/lookup/{phone}` | POS phone lookup | Employee |
| `GET` | `/v2/api/customers` | List all (admin) | Employee |
| `GET` | `/v2/api/customers/{id}` | Get by ID | Employee |
| `POST` | `/v2/api/customers` | Create (admin) | Employee |
| `PUT` | `/v2/api/customers/{id}` | Update | Employee |
| `DELETE` | `/v2/api/customers/{id}` | Delete | Employee |

---

## Blog & Tags

| Method | Route | Description | Auth |
|---|---|---|---|
| `GET` | `/v2/api/blog` | Public blog list | None |
| `GET` | `/v2/api/blog/{slug}` | Public blog detail | None |
| `POST` | `/v2/api/blog` | Create post | Employee |
| `PUT` | `/v2/api/blog/{id}` | Update post | Employee |
| `DELETE` | `/v2/api/blog/{id}` | Delete post | Employee |
| `GET` | `/v2/api/tags` | List tags | None |
| `GET` | `/v2/api/tags/{id}` | Get tag | None |
| `GET` | `/v2/api/tags/slug/{slug}` | Get tag by slug | None |
| `POST` | `/v2/api/tags` | Create tag | Employee |
| `PUT` | `/v2/api/tags/{id}` | Update tag | Employee |
| `DELETE` | `/v2/api/tags/{id}` | Delete tag | Employee |

---

## Reports

| Method | Route | Description | Auth |
|---|---|---|---|
| `GET` | `/v2/api/reports/dashboard-stats` | Dashboard stats | Employee |

---

## Media

| Method | Route | Description | Auth |
|---|---|---|---|
| `POST` | `/v2/api/media/upload` | Upload file (multipart) | Employee |
| `GET` | `/v2/api/media` | List media (paginated) | Employee |
| `DELETE` | `/v2/api/media/{id}` | Delete media | Employee |
| `GET` | `/v2/api/media/check-usage` | Check URL usage | Employee |
| `GET` | `/v2/api/media/unused` | Find unused media | Employee |
| `DELETE` | `/v2/api/media/cleanup` | Bulk delete unused | Employee |

### Media Upload

```
POST /v2/api/media/upload
Content-Type: multipart/form-data
Authorization: Bearer <token>

file: <binary>
folder: "menu-items" | "categories" | "combos" | "blog" | "avatars" | "misc"

Response 201:
{
  "data": {
    "id": 1,
    "url": "http://localhost/uploads/food-media/menu-items/espresso.jpg",
    "fileName": "espresso.jpg",
    "folder": "menu-items",
    "contentType": "image/jpeg",
    "size": 24576
  },
  "error": null,
  "meta": { "timestamp": "..." }
}
```

---

## SignalR Hub

**Endpoint**: `/hubs/app`
**Protocol**: MessagePack (binary) with JSON fallback

### Client → Server

| Method | Description |
|---|---|
| `JoinGroup(groupName)` | Join notification group |
| `LeaveGroup(groupName)` | Leave notification group |

### Server → Client

| Event | Payload | Description |
|---|---|---|
| `ReceiveOrderUpdate` | `{ id, status, ... }` | Order changed |
| `ReceiveTableUpdate` | `{ ... }` | Table/source changed |
| `ReceiveNewOrder` | `{ ... }` | New order for kitchen |
| `ReceiveSourceUpdate` | `{ ... }` | Source updated |

---

## Order Status Values

| Status | Description |
|---|---|
| `pending` | Created, awaiting payment |
| `confirmed` | Payment confirmed |
| `preparing` | Kitchen started preparing |
| `ready` | Ready to serve |
| `served` | Delivered to customer |
| `paid` | Payment completed |
| `cancelled` | Order cancelled |

## Payment Methods

| Value | Description |
|---|---|
| `cash` | Cash payment |
| `qr` | VietQR bank transfer |
| `zalopay` | ZaloPay wallet |
| `momo` | Momo wallet |
| `card` | Bank card (POS terminal) |