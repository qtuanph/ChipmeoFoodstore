Mô tả cấu trúc chuẩn mình đã triển khai trong từng project để giữ nguyên tắc Clean Architecture và thuận tiện cho việc phát triển/triển khai.

A. `ChipmeoApis.Core`
- `Entities/` : Các entity (POCO) do scaffolding hoặc viết tay (ví dụ `Category.cs`, `MenuItem.cs`, `Order.cs`, ...).
- `Interfaces/` : Gồm các interface của repository.
  - `ICategoryRepository.cs`
  - `IMenuItemRepository.cs`

B. `ChipmeoApis.Usecase`
- `DTOs/` : Tách theo domain aggregate để rõ ràng.
  - `DTOs/Category/CategoryDto.cs`
  - `DTOs/Category/CreateCategoryDto.cs`
  - `DTOs/MenuItem/MenuItemDto.cs`
  - `DTOs/MenuItem/CreateMenuItemDto.cs`
- `Interfaces/` : Interface cho service layer.
  - `ICategoryService.cs`
  - `IMenuItemService.cs`
- `Services/` : Implementation của service interfaces.
  - `CategoryService.cs` (implements `ICategoryService`)
  - `MenuItemService.cs` (implements `IMenuItemService`)
- `Extensions/` : DI registration for application services.
  - `ServiceCollectionExtensions.cs` (register `ICategoryService` -> `CategoryService`, etc.)

C. `ChipmeoApis.Infrastructure`
- `Data/` : `StoreDbContext.cs` (EF Core DbContext)
- `Repositories/` : Implementation của repositories (đặt tên theo aggregate)
  - `CategoryRepository.cs` (implements `ICategoryRepository`)
  - `MenuItemRepository.cs` (implements `IMenuItemRepository`)
- `Extensions/` : DI registration for infra implementations.
  - `ServiceCollectionExtensions.cs` (register `ICategoryRepository` -> `CategoryRepository`, etc.)

D. `ChipmeoApis.Web`
- `Controllers/` : API controllers.
  - `CategoriesController.cs` (uses `ICategoryService`)
  - `MenuItemsController.cs` (uses `IMenuItemService`)
  - `HealthController.cs` (`GET /health`)
- `Program.cs` : Wiring DbContext, Redis, DI extensions (`AddInfrastructureServices`, `AddApplicationServices`), CORS, Auth, Swagger.

---

## 3. 📝 HƯỚNG DẪN NHANH ĐỂ CHẠY

1. Điền `DefaultConnection` trong `appsettings.Development.json`.
2. (Nếu cần) Tạo migration và cập nhật DB: `dotnet ef migrations add Init` và `dotnet ef database update` (đảm bảo project startup là `ChipmeoApis.Web`).
3. Chạy API: `dotnet run --project ChipmeoApis.Web`.
4. Kiểm tra endpoints:
   - `GET /health`
   - `GET /api/categories`
   - `POST /api/categories` (body: CreateCategoryDto)
   - `GET /api/menuitems`

---

## 4. 🔧 Triển khai trên Windows Server (IIS)

Mô tả cấu trúc chuẩn mình đã triển khai trong từng project để giữ nguyên tắc Clean Architecture và thuận tiện cho việc phát triển/triển khai.

A. `ChipmeoApis.Core`
- `Entities/` : Các entity (POCO) do scaffolding hoặc viết tay (ví dụ `Category.cs`, `MenuItem.cs`, `Order.cs`, ...).
- `Interfaces/` : Gồm các interface của repository.
  - `ICategoryRepository.cs`
  - `IMenuItemRepository.cs`

B. `ChipmeoApis.Usecase`
- `DTOs/` : Tách theo domain aggregate để rõ ràng.
  - `DTOs/Category/CategoryDto.cs`
  - `DTOs/Category/CreateCategoryDto.cs`
  - `DTOs/MenuItem/MenuItemDto.cs`
  - `DTOs/MenuItem/CreateMenuItemDto.cs`
- `Interfaces/` : Interface cho service layer.
  - `ICategoryService.cs`
  - `IMenuItemService.cs`
- `Services/` : Implementation của service interfaces.
  - `CategoryService.cs` (implements `ICategoryService`)
  - `MenuItemService.cs` (implements `IMenuItemService`)
- `Extensions/` : DI registration for application services.
  - `ServiceCollectionExtensions.cs` (register `ICategoryService` -> `CategoryService`, etc.)

C. `ChipmeoApis.Infrastructure`
- `Data/` : `StoreDbContext.cs` (EF Core DbContext)
- `Repositories/` : Implementation của repositories (đặt tên theo aggregate)
  - `CategoryRepository.cs` (implements `ICategoryRepository`)
  - `MenuItemRepository.cs` (implements `IMenuItemRepository`)
- `Extensions/` : DI registration for infra implementations.
  - `ServiceCollectionExtensions.cs` (register `ICategoryRepository` -> `CategoryRepository`, etc.)

D. `ChipmeoApis.Web`
- `Controllers/` : API controllers.
  - `CategoriesController.cs` (uses `ICategoryService`)
  - `MenuItemsController.cs` (uses `IMenuItemService`)
  - `HealthController.cs` (`GET /health`)
- `Program.cs` : Wiring DbContext, Redis, DI extensions (`AddInfrastructureServices`, `AddApplicationServices`), CORS, Auth, Swagger.

---

## 3. 📝 HƯỚNG DẪN NHANH ĐỂ CHẠY

1. Điền `DefaultConnection` trong `appsettings.Development.json`.
2. (Nếu cần) Tạo migration và cập nhật DB: `dotnet ef migrations add Init` và `dotnet ef database update` (đảm bảo project startup là `ChipmeoApis.Web`).
3. Chạy API: `dotnet run --project ChipmeoApis.Web`.
4. Kiểm tra endpoints:
   - `GET /health`
   - `GET /api/categories`
   - `POST /api/categories` (body: CreateCategoryDto)
   - `GET /api/menuitems`

---

## 4. 🔧 Triển khai trên Windows Server (IIS)

- Bạn có thể publish ứng dụng và host bằng IIS (Reverse Proxy). Một vài lưu ý:
  - Trong `Program.cs` giữ `UseHttpsRedirection()` và cấu hình Kestrel nếu cần.
  - Trên IIS, cấu hình Application Pool (No Managed Code) và URL Rewrite để forward traffic tới ứng dụng Kestrel.
  - Sử dụng `dotnet publish -c Release` để tạo package, copy lên server, cấu hình site trong IIS.
  - Cấu hình `ConnectionStrings__DefaultConnection` và `RedisSettings__ConnectionString` qua Environment Variables trên server thay vì lưu trong `appsettings.json`.

---

## 5. 💾 In-Memory Caching (Best Practices)

Tại sao dùng In-Memory Cache:
- `IMemoryCache` là cache built-in của .NET, đơn giản và hiệu quả cho ứng dụng đơn server.
- Phù hợp cho cache dữ liệu đọc nhiều như `categories`, `menu_items`.

Đã được cấu hình trong `Program.cs`:

```csharp
builder.Services.AddMemoryCache();
```

Sử dụng cache trong Service/Repository:
- Inject `IMemoryCache` vào Service hoặc Repository.
- Cache các query GET với TTL hợp lý (60-300 giây).
- Invalidate cache khi tạo/cập nhật/xóa dữ liệu.

Ví dụ pseudo-code (CategoryService):

```csharp
// constructor: IMemoryCache _cache injected
if (_cache.TryGetValue("categories:all", out List<CategoryDto> cached))
    return cached;

var items = await _repo.GetAllAsync();
_cache.Set("categories:all", items, TimeSpan.FromSeconds(120));
return items;
```

Ghi chú:
- Cache chỉ phù hợp cho dữ liệu đọc nhiều, cập nhật ít.
- Nếu scale multi-server, cân nhắc Redis hoặc distributed cache.

---

Nếu bạn muốn mình tích hợp Redis caching ngay vào code (ví dụ cache `GetAll` cho categories/menuitems với `IDistributedCache` và invalidate khi thay đổi), mình sẽ thêm nhanh vào repository/service và chạy kiểm tra. Nói mình biết là thêm cache vào `Service` hay `Repository` (mình khuyên vào `Service`).



