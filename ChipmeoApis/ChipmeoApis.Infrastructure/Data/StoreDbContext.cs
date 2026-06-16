using ChipmeoApis.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChipmeoApis.Infrastructure.Data;

public partial class StoreDbContext : DbContext
{
    public StoreDbContext()
    {
    }

    public StoreDbContext(DbContextOptions<StoreDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Addon> Addons { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Combo> Combos { get; set; }

    public virtual DbSet<ComboItem> ComboItems { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<BlogPost> BlogPosts { get; set; }

    public virtual DbSet<Media> Media { get; set; }

    public virtual DbSet<MenuItem> MenuItems { get; set; }

    public virtual DbSet<MenuItemAddon> MenuItemAddons { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderItemAddon> OrderItemAddons { get; set; }

    public virtual DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentSetting> PaymentSettings { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<Source> Sources { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<BlogPostTag> BlogPostTags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AI");

        modelBuilder.Entity<Addon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__addons__3213E83F24DDF8BC");

            entity.ToTable("addons");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 0)")
                .HasColumnName("price");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__categori__3213E83F0968880A");

            entity.ToTable("categories");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.ImageUrl).HasMaxLength(500).HasColumnName("image_url");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Combo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__combos__3213E83FF8E5B9EC");

            entity.ToTable("combos");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ComboPrice)
                .HasColumnType("decimal(10, 0)")
                .HasColumnName("combo_price");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.ImageUrl).HasMaxLength(500).HasColumnName("image_url");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<ComboItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__combo_it__3213E83FF7F6D2A5");

            entity.ToTable("combo_items");

            entity.HasIndex(e => new { e.ComboId, e.MenuItemId }, "UQ_combo_item").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ComboId).HasColumnName("combo_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.MenuItemId).HasColumnName("menu_item_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Combo).WithMany(p => p.ComboItems)
                .HasForeignKey(d => d.ComboId)
                .HasConstraintName("FK__combo_ite__combo__656C112C");

            entity.HasOne(d => d.MenuItem).WithMany(p => p.ComboItems)
                .HasForeignKey(d => d.MenuItemId)
                .HasConstraintName("FK__combo_ite__menu___66603565");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__discount__3213E83FEE7AC483");

            entity.ToTable("discounts");

            entity.HasIndex(e => e.Code, "IX_discounts_code");

            entity.HasIndex(e => e.IsActive, "IX_discounts_is_active");

            entity.HasIndex(e => e.Code, "UQ__discount__357D4CF940F1F217").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.MaxDiscountAmount)
                .HasColumnType("decimal(10, 0)")
                .HasColumnName("max_discount_amount");
            entity.Property(e => e.MinOrderAmount)
                .HasColumnType("decimal(10, 0)")
                .HasColumnName("min_order_amount");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Type)
                .HasMaxLength(10)
                .HasColumnName("type");
            entity.Property(e => e.UsageLimit).HasColumnName("usage_limit");
            entity.Property(e => e.UsedCount).HasColumnName("used_count");
            entity.Property(e => e.Value)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("value");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__employee__3213E83FF1A23F2C");

            entity.ToTable("employees");

            entity.HasIndex(e => e.RoleId, "IX_employees_role_id");

            entity.HasIndex(e => e.Username, "IX_employees_username");

            entity.HasIndex(e => e.Username, "UQ__employee__F3DBC57267A7BC3B").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.LastLogin).HasColumnName("last_login");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(500)
                .HasColumnName("avatar_url");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.Employees)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__employees__role___08B54D69");
        });

        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__menu_ite__3213E83F6301D595");

            entity.ToTable("menu_items");

            entity.HasIndex(e => e.CategoryId, "IX_menu_items_category_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Description).HasMaxLength(500).HasColumnName("description");
            entity.Property(e => e.ImageUrl).HasMaxLength(500).HasColumnName("image_url");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 0)")
                .HasColumnName("price");

            entity.HasOne(d => d.Category).WithMany(p => p.MenuItems)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__menu_item__categ__5070F446");
        });

        modelBuilder.Entity<MenuItemAddon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__menu_ite__3213E83F3B9F2A60");

            entity.ToTable("menu_item_addons");

            entity.HasIndex(e => new { e.MenuItemId, e.AddonId }, "UQ_menu_item_addon").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AddonId).HasColumnName("addon_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.MenuItemId).HasColumnName("menu_item_id");
            entity.Property(e => e.PriceOverride)
                .HasColumnType("decimal(10, 0)")
                .HasColumnName("price_override");

            entity.HasOne(d => d.Addon).WithMany(p => p.MenuItemAddons)
                .HasForeignKey(d => d.AddonId)
                .HasConstraintName("FK__menu_item__addon__5BE2A6F2");

            entity.HasOne(d => d.MenuItem).WithMany(p => p.MenuItemAddons)
                .HasForeignKey(d => d.MenuItemId)
                .HasConstraintName("FK__menu_item__menu___5AEE82B9");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__orders__3213E83FC728A4D0");

            entity.ToTable("orders");

            entity.HasIndex(e => e.CreatedAt, "IX_orders_created_at");

            entity.HasIndex(e => e.EmployeeId, "IX_orders_employee_id");

            entity.HasIndex(e => e.Status, "IX_orders_status");

            entity.HasIndex(e => e.SourceId, "IX_orders_source_id");

            entity.HasIndex(e => e.OrderCode, "UQ__orders__99D12D3FECDACF00").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.DiscountAmount)
                .HasColumnType("decimal(10, 0)")
                .HasColumnName("discount_amount");
            entity.Property(e => e.DiscountId).HasColumnName("discount_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.Note)
                .HasMaxLength(500)
                .HasColumnName("note");
            entity.Property(e => e.OrderCode)
                .HasMaxLength(20)
                .HasColumnName("order_code");
            entity.Property(e => e.PaidAt).HasColumnName("paid_at");
            entity.Property(e => e.PrintedAt).HasColumnName("printed_at");
            entity.Property(e => e.QrPaymentUrl).HasColumnName("qr_payment_url");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.SubtotalAmount)
                .HasColumnType("decimal(10, 0)")
                .HasColumnName("subtotal_amount");
            entity.Property(e => e.SourceId).HasColumnName("source_id");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(10, 0)")
                .HasColumnName("total_amount");
            entity.Property(e => e.VatAmount)
                .HasColumnType("decimal(10, 0)")
                .HasColumnName("vat_amount");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.Discount).WithMany(p => p.Orders)
                .HasForeignKey(d => d.DiscountId)
                .HasConstraintName("FK__orders__discount__151B244E");

            entity.HasOne(d => d.Employee).WithMany(p => p.OrderEmployees)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__orders__employee__14270015");

            entity.HasOne(d => d.Source).WithMany(p => p.Orders)
                .HasForeignKey(d => d.SourceId)
                .HasConstraintName("FK__orders__source_id__1332DBDC");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.OrderUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__orders__updated___160F4887");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_orders_customer");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__order_it__3213E83F562D9943");

            entity.ToTable("order_items");

            entity.HasIndex(e => e.MenuItemId, "IX_order_items_menu_item_id");

            entity.HasIndex(e => e.OrderId, "IX_order_items_order_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ComboId).HasColumnName("combo_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.MenuItemId).HasColumnName("menu_item_id");
            entity.Property(e => e.MenuItemName)
                .HasMaxLength(255)
                .HasColumnName("menu_item_name");
            entity.Property(e => e.Note)
                .HasMaxLength(255)
                .HasColumnName("note");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(10, 0)")
                .HasColumnName("total_price");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(10, 0)")
                .HasColumnName("unit_price");

            entity.HasOne(d => d.Combo).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ComboId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__order_ite__combo__1CBC4616");

            entity.HasOne(d => d.MenuItem).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.MenuItemId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__order_ite__menu___1BC821DD");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__order_ite__order__1AD3FDA4");
        });

        modelBuilder.Entity<OrderItemAddon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__order_it__3213E83FAF3A2D21");

            entity.ToTable("order_item_addons");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AddonId).HasColumnName("addon_id");
            entity.Property(e => e.AddonName)
                .HasMaxLength(100)
                .HasColumnName("addon_name");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.OrderItemId).HasColumnName("order_item_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(10, 0)")
                .HasColumnName("total_price");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(10, 0)")
                .HasColumnName("unit_price");

            entity.HasOne(d => d.Addon).WithMany(p => p.OrderItemAddons)
                .HasForeignKey(d => d.AddonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__order_ite__addon__2739D489");

            entity.HasOne(d => d.OrderItem).WithMany(p => p.OrderItemAddons)
                .HasForeignKey(d => d.OrderItemId)
                .HasConstraintName("FK__order_ite__order__2645B050");
        });

        modelBuilder.Entity<OrderStatusHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__order_st__3213E83FCE1D9451");

            entity.ToTable("order_status_history");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChangedAt).HasColumnName("changed_at");
            entity.Property(e => e.ChangedBy).HasColumnName("changed_by");
            entity.Property(e => e.FromStatus)
                .HasMaxLength(20)
                .HasColumnName("from_status");
            entity.Property(e => e.Note)
                .HasMaxLength(500)
                .HasColumnName("note");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ToStatus)
                .HasMaxLength(20)
                .HasColumnName("to_status");

            entity.HasOne(d => d.ChangedByNavigation).WithMany(p => p.OrderStatusHistories)
                .HasForeignKey(d => d.ChangedBy)
                .HasConstraintName("FK__order_sta__chang__2180FB33");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderStatusHistories)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__order_sta__order__208CD6FA");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__payments__3213E83F83E7B0B7");

            entity.ToTable("payments");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 0)")
                .HasColumnName("amount");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Method)
                .HasMaxLength(20)
                .HasColumnName("method");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.PaidAt).HasColumnName("paid_at");
            entity.Property(e => e.ReferenceCode)
                .HasMaxLength(100)
                .HasColumnName("reference_code");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__payments__order___2EDAF651");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__permissi__3213E83FCB30748C");

            entity.ToTable("permissions");

            entity.HasIndex(e => e.Code, "UQ__permissi__357D4CF98F9F6620").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(100)
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Module)
                .HasMaxLength(50)
                .HasColumnName("module");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__roles__3213E83F9FA1ADC9");

            entity.ToTable("roles");

            entity.HasIndex(e => e.Name, "UQ__roles__72E12F1BF07239B0").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.DefaultRoute)
                .HasMaxLength(100)
                .HasColumnName("default_route");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__role_per__3213E83FD8315518");

            entity.ToTable("role_permissions");

            entity.HasIndex(e => new { e.RoleId, e.PermissionId }, "UQ_role_permission").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .HasConstraintName("FK__role_perm__permi__02FC7413");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__role_perm__role___02084FDA");
        });

        modelBuilder.Entity<Source>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__sources__3213E83FEE006DAA");

            entity.ToTable("sources");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<PaymentSetting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_payment_settings");

            entity.ToTable("payment_settings");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BankId)
                .HasMaxLength(50)
                .HasColumnName("bank_id");
            entity.Property(e => e.BankAccount)
                .HasMaxLength(50)
                .HasColumnName("bank_account");
            entity.Property(e => e.BankName)
                .HasMaxLength(100)
                .HasColumnName("bank_name");
            entity.Property(e => e.Template)
                .HasMaxLength(20)
                .HasColumnName("template")
                .HasDefaultValue("compact2");
            entity.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(true);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_customers");
            entity.ToTable("customers");
            entity.HasIndex(e => e.Phone, "UQ_customers_phone").IsUnique();
            entity.HasIndex(e => e.Email, "UQ_customers_email").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FullName).HasMaxLength(100).HasColumnName("full_name");
            entity.Property(e => e.Phone).HasMaxLength(20).HasColumnName("phone");
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100).HasColumnName("email");
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255).HasColumnName("password_hash");
            entity.Property(e => e.AvatarUrl).HasMaxLength(500).HasColumnName("avatar_url");
            entity.Property(e => e.Points).HasColumnName("points").HasDefaultValue(0);
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        });

        modelBuilder.Entity<BlogPost>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_blog_posts");
            entity.ToTable("blog_posts");
            entity.HasIndex(e => e.Slug, "IX_blog_posts_slug");
            entity.HasIndex(e => e.Status, "IX_blog_posts_status");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasMaxLength(255).HasColumnName("title");
            entity.Property(e => e.Slug).HasMaxLength(255).HasColumnName("slug");
            entity.Property(e => e.Excerpt).HasMaxLength(500).HasColumnName("excerpt");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.ThumbnailUrl).HasMaxLength(500).HasColumnName("thumbnail_url");
            entity.Property(e => e.Status).HasMaxLength(20).HasColumnName("status").HasDefaultValue("draft");
            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.PublishedAt).HasColumnName("published_at");
            // SEO Fields
            entity.Property(e => e.MetaTitle).HasMaxLength(100).HasColumnName("meta_title");
            entity.Property(e => e.MetaDescription).HasMaxLength(200).HasColumnName("meta_description");
            entity.Property(e => e.FocusKeyword).HasMaxLength(100).HasColumnName("focus_keyword");
            entity.Property(e => e.Keywords).HasMaxLength(500).HasColumnName("keywords");
            entity.Property(e => e.CanonicalUrl).HasMaxLength(500).HasColumnName("canonical_url");
            entity.Property(e => e.OgImageUrl).HasMaxLength(500).HasColumnName("og_image_url");
            entity.Property(e => e.ReadingTime).HasColumnName("reading_time").HasDefaultValue(0);
            entity.Property(e => e.WordCount).HasColumnName("word_count").HasDefaultValue(0);
            entity.Property(e => e.SeoScore).HasColumnName("seo_score").HasDefaultValue(0);
            // Timestamps
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.Author).WithMany(p => p.BlogPosts)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_blog_posts_author");
        });

        modelBuilder.Entity<Media>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_media");
            entity.ToTable("media");

            entity.HasIndex(e => e.Folder, "IX_media_folder");
            entity.HasIndex(e => new { e.EntityType, e.EntityId }, "IX_media_entity");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FileName).HasMaxLength(255).HasColumnName("file_name");
            entity.Property(e => e.Folder).HasMaxLength(100).HasDefaultValue("misc").HasColumnName("folder");
            entity.Property(e => e.FileUrl).HasMaxLength(500).HasColumnName("file_url");
            entity.Property(e => e.FileType).HasMaxLength(50).HasColumnName("file_type");
            entity.Property(e => e.FileSize).HasColumnName("file_size");
            entity.Property(e => e.AltText).HasMaxLength(255).HasColumnName("alt_text");
            entity.Property(e => e.UploadedByEmployee).HasColumnName("uploaded_by_employee");
            entity.Property(e => e.UploadedByCustomer).HasColumnName("uploaded_by_customer");
            entity.Property(e => e.EntityType).HasMaxLength(50).HasColumnName("entity_type");
            entity.Property(e => e.EntityId).HasColumnName("entity_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");

            entity.HasOne(d => d.UploadedByEmployeeNavigation).WithMany()
                .HasForeignKey(d => d.UploadedByEmployee)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_media_uploaded_by_employee");
            entity.HasOne(d => d.UploadedByCustomerNavigation).WithMany()
                .HasForeignKey(d => d.UploadedByCustomer)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_media_uploaded_by_customer");
        });

        // Tag entity configuration
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tags");
            entity.ToTable("tags");

            entity.HasIndex(e => e.Slug, "IX_tags_slug").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasMaxLength(100).HasColumnName("name");
            entity.Property(e => e.Slug).HasMaxLength(100).HasColumnName("slug");
            entity.Property(e => e.Description).HasMaxLength(255).HasColumnName("description");
            entity.Property(e => e.Color).HasMaxLength(7).HasDefaultValue("#f59e0b").HasColumnName("color");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        });

        // BlogPostTag junction entity configuration
        modelBuilder.Entity<BlogPostTag>(entity =>
        {
            entity.HasKey(e => new { e.PostId, e.TagId });
            entity.ToTable("blog_post_tags");

            entity.HasIndex(e => e.TagId, "IX_blog_post_tags_tag");

            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.TagId).HasColumnName("tag_id");

            entity.HasOne(d => d.Post)
                .WithMany(p => p.BlogPostTags)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Tag)
                .WithMany(t => t.BlogPostTags)
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}




