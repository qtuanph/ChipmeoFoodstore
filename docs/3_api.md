# API Reference

## Base URLs

| Environment | Base URL |
|---|---|
| **Development** | `http://localhost:5142` |
| **Production** | `https://api.chipmeo.io.vn` |
| **Media Server (dev)** | `http://localhost:5000` |
| **Media Server (prod)** | `https://media.chipmeo.io.vn` |

## Authentication

### Employee Login

```
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "password123"
}

Response 200:
{
  "token": "eyJhbGciOiJI...",
  "refreshToken": "dGhpcyBpcyBh...",
  "employee": {
    "id": 1,
    "username": "admin",
    "fullName": "Admin",
    "roleId": 1,
    "roleName": "Admin",
    "avatarUrl": null
  }
}
```

### Customer Login

```
POST /api/customers/login
Content-Type: application/json

{
  "phone": "0901234567",
  "password": "customerpass"
}

Response 200:
{
  "token": "eyJhbGciOiJI...",
  "customer": { ... }
}
```

### Customer Registration

```
POST /api/customers/register
Content-Type: application/json

{
  "phone": "0901234567",
  "fullName": "Nguyen Van A",
  "password": "customerpass"
}

Response 201:
{
  "token": "eyJhbGciOiJI...",
  "customer": { ... }
}
```

### All API calls (except auth) require:

```
Authorization: Bearer <jwt_token>
```

---

## Health

| Method | Route | Description |
|---|---|---|
| `GET` | `/health` | Server health check |

---

## POS Endpoints

| Method | Route | Description | Auth |
|---|---|---|---|
| `GET` | `/pos/menu` | Full menu (categories + items + addons + combos + discounts) | Employee |
| `GET` | `/pos/sources` | Active order sources/tables | Employee |
| `GET` | `/pos/addons` | All add-ons (for POS ordering) | Employee |
| `POST` | `/pos/orders` | Create new order | Employee |
| `PUT` | `/pos/orders/{id}` | Update order (items, discounts) | Employee |
| `PUT` | `/pos/orders/{id}/status` | Update order status | Employee |
| `POST` | `/pos/orders/{id}/payment` | Process payment | Employee |

### POS Menu Response

```json
{
  "categories": [
    {
      "id": 1,
      "name": "Coffee",
      "imageUrl": "https://media.chipmeo.io.vn/categories/coffee.jpg",
      "items": [
        {
          "id": 1,
          "name": "Espresso",
          "price": 29000,
          "imageUrl": "https://media.chipmeo.io.vn/menu-items/espresso.jpg",
          "status": "available",
          "allowAddons": true,
          "addonIds": [1, 2]
        }
      ]
    }
  ],
  "combos": [
    {
      "id": 1,
      "name": "Breakfast Combo",
      "price": 59000,
      "imageUrl": "...",
      "items": [{ "id": 1, "quantity": 1 }]
    }
  ],
  "discounts": [
    {
      "id": 1,
      "code": "WELCOME10",
      "type": "percentage",
      "value": 10
    }
  ]
}
```

### Create Order

```
POST /pos/orders
Content-Type: application/json

{
  "sourceId": 1,
  "customerPhone": "0901234567",
  "note": "Less ice",
  "discountCode": "WELCOME10",
  "items": [
    {
      "menuItemId": 1,
      "quantity": 2,
      "addonIds": [1, 2],
      "note": "No sugar"
    },
    {
      "comboId": 1,
      "quantity": 1
    }
  ]
}

Response 201:
{
  "id": 42,
  "totalAmount": 87000,
  "discountAmount": 8700,
  "finalAmount": 78300,
  "status": "pending"
}
```

### Process Payment

```
POST /pos/orders/{id}/payment
Content-Type: application/json

{
  "method": "cash" | "qr" | "zalopay" | "momo" | "card",
  "amount": 78300,
  "bankTransactionId": "optional"
}

Response 200:
{
  "orderId": 42,
  "paymentId": 1,
  "status": "paid",
  "changeAmount": 21700
}
```

---

## Kitchen Endpoints

| Method | Route | Description | Auth |
|---|---|---|---|
| `GET` | `/api/kitchen/orders` | Get kitchen order queue (paid orders) | Employee |
| `PUT` | `/api/kitchen/orders/{id}/start` | Start preparing → status `preparing` | Employee |
| `PUT` | `/api/kitchen/orders/{id}/complete` | Complete → status `served` | Employee |

---

## Admin Endpoints (Full CRUD)

Each admin endpoint follows the same pattern:

| Method | Route | Description |
|---|---|---|
| `GET` | `/admin/{resource}` | List all |
| `GET` | `/admin/{resource}/{id}` | Get by ID |
| `POST` | `/admin/{resource}` | Create |
| `PUT` | `/admin/{resource}/{id}` | Update |
| `DELETE` | `/admin/{resource}/{id}` | Delete |

### Admin Resources

| Resource | Route Prefix | Auth Permission |
|---|---|---|
| Categories | `/admin/categories` | `categories.*` |
| Menu Items | `/admin/menuitems` | `menu.*` |
| Addons | `/admin/addons` | `menu.*` |
| Combos | `/admin/combos` | `menu.*` |
| Discounts | `/admin/discounts` | `discounts.*` |
| Sources (tables) | `/admin/sources` | `sources.*` |
| Orders | `/admin/orders` | `orders.*` |
| Employees | `/admin/employees` | `employees.*` |
| Roles | `/admin/roles` | `roles.*` |
| Role Permissions | `/admin/roles/{roleId}/permissions` | `roles.*` |
| Customers | `/api/customers` | `customers.*` |
| Blog Posts | `/api/blog` | `blog.*` |
| Tags | `/api/tags` | `blog.*` |
| Payment Settings | `/admin/payment-settings` | `settings.*` |
| Media | `/api/media` | `media.*` |

### Special Admin Endpoints

| Method | Route | Description |
|---|---|---|
| `GET` | `/admin/dashboard/overview` | KPIs (revenue, orders, customers, popular items) |
| `GET` | `/admin/dashboard/stats` | Detailed statistics |
| `GET` | `/admin/dashboard/analytics` | Time-series data (daily/weekly/monthly) |
| `GET` | `/admin/dashboard/forecast` | ML sales forecast (next 7 days) |
| `GET` | `/admin/dashboard/recommendations` | Combo recommendations (co-occurrence) |
| `GET` | `/admin/orders/paged` | Paginated orders with filters |
| `GET` | `/admin/orders/status/{status}` | Orders filtered by status |
| `PUT` | `/admin/orders/{id}/set-unpaid` | Revert payment to unpaid |
| `GET` | `/api/reports/dashboard-stats` | Dashboard statistics report |

---

## Customer Endpoints

| Method | Route | Description | Auth |
|---|---|---|---|
| `POST` | `/api/customers/register` | Register | None |
| `POST` | `/api/customers/login` | Login | None |
| `GET` | `/api/customers/me` | Get profile | Customer |
| `PUT` | `/api/customers/me` | Update profile | Customer |
| `GET` | `/api/customers/lookup/{phone}` | POS phone lookup | Employee |
| `GET` | `/api/customers` | List all (admin) | Employee |
| `GET` | `/api/customers/{id}` | Get by ID (admin) | Employee |
| `POST` | `/api/customers` | Create (admin) | Employee |
| `PUT` | `/api/customers/{id}` | Update (admin) | Employee |
| `DELETE` | `/api/customers/{id}` | Delete (admin) | Employee |

---

## Blog & Tags

| Method | Route | Description | Auth |
|---|---|---|---|
| `GET` | `/api/blog` | Public blog list | None |
| `GET` | `/api/blog/{slug}` | Public blog detail | None |
| `POST` | `/api/blog` | Create post | Employee |
| `PUT` | `/api/blog/{id}` | Update post | Employee |
| `DELETE` | `/api/blog/{id}` | Delete post | Employee |
| `GET` | `/api/tags` | List tags | None |
| `GET` | `/api/tags/{id}` | Get tag | None |
| `GET` | `/api/tags/slug/{slug}` | Get tag by slug | None |
| `POST` | `/api/tags` | Create tag | Employee |
| `PUT` | `/api/tags/{id}` | Update tag | Employee |
| `DELETE` | `/api/tags/{id}` | Delete tag | Employee |

---

## Media Endpoints

| Method | Route | Description | Auth |
|---|---|---|---|
| `POST` | `/api/media/upload` | Upload file (multipart) | Employee |
| `GET` | `/api/media` | List all media (paginated) | Employee |
| `DELETE` | `/api/media/{id}` | Delete media | Employee |
| `GET` | `/api/media/check-usage` | Check if URL is referenced | Employee |
| `GET` | `/api/media/unused` | Find unused media | Employee |
| `DELETE` | `/api/media/cleanup` | Bulk delete unused media | Employee |

### Media Upload

```
POST /api/media/upload
Content-Type: multipart/form-data

file: <binary>
folder: "menu-items" | "categories" | "combos" | "blog" | "avatars" | "misc"

Response 201:
{
  "id": 1,
  "url": "https://media.chipmeo.io.vn/menu-items/espresso.jpg",
  "fileName": "espresso.jpg",
  "folder": "menu-items",
  "contentType": "image/jpeg",
  "size": 24576
}
```

---

## Media Storage Server Endpoints

**Base**: `http://localhost:5000` (dev) | `https://media.chipmeo.io.vn` (prod)
**Auth**: `X-Api-Key: <your-api-key>` (write operations only)

| Method | Route | Auth | Description |
|---|---|---|---|
| `GET` | `/api/media/folders` | None | List available folders |
| `GET` | `/api/media?folder={name}` | None | List files in folder |
| `POST` | `/api/media/upload` | X-Api-Key | Upload file (max 10MB, images only) |
| `DELETE` | `/api/media/{folder}/{filename}` | X-Api-Key | Delete file |

---

## SignalR Hub

**Endpoint**: `/hubs/app`
**Protocol**: MessagePack (binary) with JSON fallback

### Client → Server

| Method | Description |
|---|---|
| `JoinGroup(groupName)` | Join a notification group |
| `LeaveGroup(groupName)` | Leave a notification group |

### Server → Client

| Event | Payload | Description |
|---|---|---|
| `OrderStatusChanged` | `{ orderId, status, updatedAt }` | Order status changed |
| `MenuUpdated` | `{ entity, action }` | Menu/category/addon changed |
| `TableUpdated` | `{ sourceId, action }` | Table/source changed |
| `KitchenNotification` | `{ orderId, items }` | New order for kitchen |

---

## Error Response Format

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Validation Error",
  "status": 400,
  "errors": {
    "fieldName": ["The field is required."]
  }
}

// Unauthorized
{
  "status": 401,
  "title": "Unauthorized",
  "detail": "Invalid credentials"
}

// Forbidden (permission denied)
{
  "status": 403,
  "title": "Forbidden",
  "detail": "You do not have permission: orders.delete"
}
```

---

## Order Status Values

| Status | Description |
|---|---|
| `pending` | Created, awaiting payment |
| `confirmed` | Payment confirmed |
| `preparing` | Kitchen started preparing |
| `ready` | Ready to serve |
| `served` | Delivered to customer |
| `paid` | Payment completed (after POS payment) |
| `cancelled` | Order cancelled |

## Payment Methods

| Value | Description |
|---|---|
| `cash` | Cash payment |
| `qr` | VietQR bank transfer |
| `zalopay` | ZaloPay wallet |
| `momo` | Momo wallet |
| `card` | Bank card (POS terminal) |
