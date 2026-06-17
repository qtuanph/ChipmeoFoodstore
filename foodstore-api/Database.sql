-- ==========================================================
-- 1. THIáº¾T Láº¬P DATABASE VÃ€ MÃ”I TRÆ¯á»œNG
-- ==========================================================

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'pos_shop')
BEGIN
    CREATE DATABASE [pos_shop] 
    COLLATE SQL_Latin1_General_CP1_CI_AI;
END
GO

USE [pos_shop];
GO

ALTER DATABASE [pos_shop] COLLATE SQL_Latin1_General_CP1_CI_AI;
GO

-- ==========================================================
-- 2. CATEGORY / MENU / ADDONS
-- ==========================================================

CREATE TABLE categories (
    id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(100) NOT NULL,
    description NVARCHAR(255),
    image_url NVARCHAR(500), -- https://media.foodstore.local/categories/xxx.jpg
    is_active BIT DEFAULT 1,
    created_at DATETIME2 DEFAULT GETDATE()
);

CREATE TABLE menu_items (
    id INT PRIMARY KEY IDENTITY(1,1),
    category_id INT,
    name NVARCHAR(100) NOT NULL,
    description NVARCHAR(500),
    price DECIMAL(10,0) NOT NULL,
    image_url NVARCHAR(500), -- https://media.foodstore.local/menu-items/xxx.jpg
    is_active BIT DEFAULT 1,
    created_at DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (category_id) REFERENCES categories(id) ON DELETE SET NULL
);

CREATE TABLE addons (
    id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(100) NOT NULL,
    price DECIMAL(10,0) NOT NULL DEFAULT 0,
    is_active BIT DEFAULT 1,
    created_at DATETIME2 DEFAULT GETDATE()
);

-- ==========================================================
-- 3. MENU ITEM â†” ADDONS
-- ==========================================================

CREATE TABLE menu_item_addons (
    id INT PRIMARY KEY IDENTITY(1,1),
    menu_item_id INT NOT NULL,
    addon_id INT NOT NULL,
    price_override DECIMAL(10,0) NULL,
    is_active BIT DEFAULT 1,
    created_at DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (menu_item_id) REFERENCES menu_items(id) ON DELETE CASCADE,
    FOREIGN KEY (addon_id) REFERENCES addons(id) ON DELETE CASCADE,
    CONSTRAINT UQ_menu_item_addon UNIQUE (menu_item_id, addon_id)
);

-- ==========================================================
-- 4. COMBO
-- ==========================================================

CREATE TABLE combos (
    id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(100) NOT NULL,
    combo_price DECIMAL(10,0) NOT NULL,
    description NVARCHAR(500),
    image_url NVARCHAR(500), -- https://media.foodstore.local/combos/xxx.jpg
    is_active BIT DEFAULT 1,
    created_at DATETIME2 DEFAULT GETDATE()
);

CREATE TABLE combo_items (
    id INT PRIMARY KEY IDENTITY(1,1),
    combo_id INT NOT NULL,
    menu_item_id INT NOT NULL,
    quantity INT DEFAULT 1,
    created_at DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (combo_id) REFERENCES combos(id) ON DELETE CASCADE,
    FOREIGN KEY (menu_item_id) REFERENCES menu_items(id) ON DELETE CASCADE,
    CONSTRAINT UQ_combo_item UNIQUE (combo_id, menu_item_id)
);

-- ==========================================================
-- 5. DISCOUNTS
-- ==========================================================

CREATE TABLE discounts (
    id INT PRIMARY KEY IDENTITY(1,1),
    code NVARCHAR(50) NOT NULL UNIQUE,
    name NVARCHAR(100) NOT NULL,
    type NVARCHAR(10) NOT NULL CHECK (type IN (N'percent', N'amount')),
    value DECIMAL(10,2) NOT NULL,
    max_discount_amount DECIMAL(10,0),
    min_order_amount DECIMAL(10,0) DEFAULT 0,
    usage_limit INT,
    used_count INT DEFAULT 0,
    is_active BIT DEFAULT 1,
    start_date DATETIME2,
    end_date DATETIME2,
    created_at DATETIME2 DEFAULT GETDATE()
);

-- ==========================================================
-- 6. TABLES
-- ==========================================================

CREATE TABLE sources (
    id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(50) NOT NULL,
    is_active BIT DEFAULT 1,
    created_at DATETIME2 DEFAULT GETDATE()
);

-- ==========================================================
-- 7. RBAC
-- ==========================================================

CREATE TABLE roles (
    id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(50) NOT NULL UNIQUE,
    description NVARCHAR(255),
    default_route NVARCHAR(100) DEFAULT '/admin',
    is_active BIT DEFAULT 1,
    created_at DATETIME2 DEFAULT GETDATE()
);

CREATE TABLE permissions (
    id INT PRIMARY KEY IDENTITY(1,1),
    code NVARCHAR(100) NOT NULL UNIQUE,
    name NVARCHAR(100) NOT NULL,
    description NVARCHAR(255),
    module NVARCHAR(50),
    created_at DATETIME2 DEFAULT GETDATE()
);

CREATE TABLE role_permissions (
    id INT PRIMARY KEY IDENTITY(1,1),
    role_id INT NOT NULL,
    permission_id INT NOT NULL,
    is_active BIT DEFAULT 1,
    created_at DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (role_id) REFERENCES roles(id) ON DELETE CASCADE,
    FOREIGN KEY (permission_id) REFERENCES permissions(id) ON DELETE CASCADE,
    CONSTRAINT UQ_role_permission UNIQUE (role_id, permission_id)
);

-- ==========================================================
-- 8. EMPLOYEES
-- ==========================================================

CREATE TABLE employees (
    id INT PRIMARY KEY IDENTITY(1,1),
    full_name NVARCHAR(100) NOT NULL,
    username NVARCHAR(50) NOT NULL UNIQUE,
    password_hash NVARCHAR(255) NOT NULL,
    email NVARCHAR(100),
    phone NVARCHAR(20),
    avatar_url NVARCHAR(500), -- https://media.foodstore.local/avatars/xxx.jpg
    role_id INT NOT NULL,
    is_active BIT DEFAULT 1,
    last_login DATETIME2,
    created_at DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (role_id) REFERENCES roles(id)
);

-- ==========================================================
-- 14. CUSTOMERS
-- ==========================================================

CREATE TABLE customers (
    id INT PRIMARY KEY IDENTITY(1,1),
    full_name NVARCHAR(100) NOT NULL,
    phone NVARCHAR(20),
    email NVARCHAR(100) NOT NULL,
    password_hash NVARCHAR(255) NOT NULL,
    avatar_url NVARCHAR(500),
    points INT DEFAULT 0,
    is_active BIT DEFAULT 1,
    created_at DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT UQ_customers_phone UNIQUE (phone),
    CONSTRAINT UQ_customers_email UNIQUE (email)
);

-- ==========================================================
-- 9. ORDERS
-- ==========================================================

CREATE TABLE orders (
    id INT PRIMARY KEY IDENTITY(1,1),
    order_code NVARCHAR(20) NOT NULL UNIQUE,
    source_id INT,
    employee_id INT, -- Nullable because online orders might not have an employee initially
    customer_id INT, -- Link to registered customer
    discount_id INT,
    subtotal_amount DECIMAL(10,0) DEFAULT 0,
    discount_amount DECIMAL(10,0) DEFAULT 0,
    vat_amount DECIMAL(10,0) DEFAULT 0,
    total_amount DECIMAL(10,0) DEFAULT 0,
    qr_payment_url NVARCHAR(MAX),
    paid_at DATETIME2,
    status NVARCHAR(20) DEFAULT N'pending'
        CHECK (status IN (N'pending', N'confirmed', N'preparing', N'ready', N'served', N'paid', N'cancelled')),
    note NVARCHAR(500),
    printed_at DATETIME2,
    updated_by INT,
    created_at DATETIME2 DEFAULT GETDATE(),
    updated_at DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (source_id) REFERENCES sources(id),
    FOREIGN KEY (employee_id) REFERENCES employees(id),
    FOREIGN KEY (customer_id) REFERENCES customers(id),
    FOREIGN KEY (discount_id) REFERENCES discounts(id),
    FOREIGN KEY (updated_by) REFERENCES employees(id)
);

-- ==========================================================
-- 10. ORDER ITEMS
-- ==========================================================

CREATE TABLE order_items (
    id INT PRIMARY KEY IDENTITY(1,1),
    order_id INT NOT NULL,
    menu_item_id INT,
    combo_id INT,
    menu_item_name NVARCHAR(255) NOT NULL,
    unit_price DECIMAL(10,0) NOT NULL,
    quantity INT DEFAULT 1,
    total_price DECIMAL(10,0) NOT NULL,
    note NVARCHAR(255),
    created_at DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (order_id) REFERENCES orders(id) ON DELETE CASCADE,
    FOREIGN KEY (menu_item_id) REFERENCES menu_items(id) ON DELETE SET NULL,
    FOREIGN KEY (combo_id) REFERENCES combos(id) ON DELETE SET NULL
);
-- ==========================================================
-- 11. ORDER STATUS HISTORY
-- ==========================================================

CREATE TABLE order_status_history (
    id INT PRIMARY KEY IDENTITY(1,1),
    order_id INT NOT NULL,
    from_status NVARCHAR(20),
    to_status NVARCHAR(20) NOT NULL,
    changed_by INT,
    changed_at DATETIME2 DEFAULT GETDATE(),
    note NVARCHAR(500),
    FOREIGN KEY (order_id) REFERENCES orders(id) ON DELETE CASCADE,
    FOREIGN KEY (changed_by) REFERENCES employees(id)
);

CREATE TABLE order_item_addons (
    id INT PRIMARY KEY IDENTITY(1,1),
    order_item_id INT NOT NULL,
    addon_id INT NOT NULL,
    addon_name NVARCHAR(100) NOT NULL,
    quantity INT DEFAULT 1,
    unit_price DECIMAL(10,0) NOT NULL,
    total_price DECIMAL(10,0) NOT NULL,
    created_at DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (order_item_id) REFERENCES order_items(id) ON DELETE CASCADE,
    FOREIGN KEY (addon_id) REFERENCES addons(id)
);

-- ==========================================================
-- 12. PAYMENTS
-- ==========================================================

CREATE TABLE payments (
    id INT PRIMARY KEY IDENTITY(1,1),
    order_id INT NOT NULL,
    amount DECIMAL(10,0) NOT NULL,
    method NVARCHAR(20) NOT NULL CHECK (method IN (N'cash', N'qr', N'zalopay', N'momo', N'card')),
    reference_code NVARCHAR(100),
    status NVARCHAR(20) DEFAULT N'success' 
        CHECK (status IN (N'pending', N'success', N'failed', N'refunded')),
    paid_at DATETIME2 DEFAULT GETDATE(),
    created_at DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (order_id) REFERENCES orders(id)
);

-- ==========================================================
-- 13. PAYMENT SETTINGS
-- ==========================================================

CREATE TABLE payment_settings (
    id INT PRIMARY KEY IDENTITY(1,1),
    bank_id NVARCHAR(50) NOT NULL,
    bank_account NVARCHAR(50) NOT NULL,
    bank_name NVARCHAR(100) NOT NULL,
    bank_account_name NVARCHAR(200),
    template NVARCHAR(20) DEFAULT N'compact2',
    is_active BIT DEFAULT 1,
    is_default BIT DEFAULT 0
);

CREATE UNIQUE INDEX UQ_payment_settings_default
ON payment_settings(is_default)
WHERE is_default = 1;



-- ==========================================================
-- 15. BLOG
-- ==========================================================

CREATE TABLE blog_posts (
    id INT PRIMARY KEY IDENTITY(1,1),
    title NVARCHAR(255) NOT NULL,
    slug NVARCHAR(255) NOT NULL UNIQUE,
    excerpt NVARCHAR(500),
    content NVARCHAR(MAX),
    thumbnail_url NVARCHAR(500),
    status NVARCHAR(20) DEFAULT N'draft' CHECK (status IN (N'draft', N'published', N'archived')),
    author_id INT,
    published_at DATETIME2,
    -- SEO Fields
    meta_title NVARCHAR(100),           -- SEO title (max 60 chars recommended)
    meta_description NVARCHAR(200),     -- Meta description (max 160 chars)
    focus_keyword NVARCHAR(100),        -- Primary keyword for SEO scoring
    keywords NVARCHAR(500),             -- Comma-separated keywords
    canonical_url NVARCHAR(500),        -- Canonical URL if different
    og_image_url NVARCHAR(500),         -- Open Graph image
    reading_time INT DEFAULT 0,         -- Estimated reading time in minutes
    word_count INT DEFAULT 0,           -- Word count
    seo_score INT DEFAULT 0,            -- SEO score 0-100
    -- Timestamps
    created_at DATETIME2 DEFAULT GETDATE(),
    updated_at DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (author_id) REFERENCES employees(id) ON DELETE SET NULL
);

-- ==========================================================
-- 15B. BLOG TAGS
-- ==========================================================

CREATE TABLE tags (
    id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(100) NOT NULL,
    slug NVARCHAR(100) NOT NULL UNIQUE,
    description NVARCHAR(255),
    color NVARCHAR(7) DEFAULT '#f59e0b', -- Hex color for UI
    created_at DATETIME2 DEFAULT GETDATE()
);

CREATE TABLE blog_post_tags (
    post_id INT NOT NULL,
    tag_id INT NOT NULL,
    PRIMARY KEY (post_id, tag_id),
    FOREIGN KEY (post_id) REFERENCES blog_posts(id) ON DELETE CASCADE,
    FOREIGN KEY (tag_id) REFERENCES tags(id) ON DELETE CASCADE
);



-- ==========================================================
-- 16. MEDIA (IMAGES/FILES)
-- Centralized storage at https://media.foodstore.local/
-- Folder structure:
--   /avatars/       - Customer avatars
--   /menu-items/    - Menu item images
--   /combos/        - Combo images
--   /categories/    - Category images  
--   /blog/          - Blog post images & thumbnails
--   /misc/          - Other files
-- ==========================================================

CREATE TABLE media (
    id INT PRIMARY KEY IDENTITY(1,1),
    file_name NVARCHAR(255) NOT NULL,
    folder NVARCHAR(100) NOT NULL DEFAULT 'misc', -- avatars, menu-items, combos, categories, blog, misc
    file_url NVARCHAR(500) NOT NULL, -- Full URL: https://media.foodstore.local/menu-items/abc.jpg
    file_type NVARCHAR(50) NOT NULL, -- image/jpeg, image/png, etc.
    file_size BIGINT,
    alt_text NVARCHAR(255), -- SEO alt text
    uploaded_by_employee INT, -- Employee who uploaded (nullable)
    uploaded_by_customer INT, -- Customer who uploaded (nullable)
    entity_type NVARCHAR(50), -- menu_item, combo, category, blog_post, customer, etc.
    entity_id INT, -- ID of the related entity
    created_at DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (uploaded_by_employee) REFERENCES employees(id) ON DELETE SET NULL,
    FOREIGN KEY (uploaded_by_customer) REFERENCES customers(id) ON DELETE SET NULL
);

CREATE INDEX IX_media_folder ON media(folder);
CREATE INDEX IX_media_entity ON media(entity_type, entity_id);

-- ==========================================================
-- 17. INDEXES
-- ==========================================================

CREATE INDEX IX_orders_status ON orders(status);
CREATE INDEX IX_orders_created_at ON orders(created_at);
CREATE INDEX IX_orders_source_id ON orders(source_id);
CREATE INDEX IX_orders_employee_id ON orders(employee_id);

CREATE INDEX IX_order_items_order_id ON order_items(order_id);
CREATE INDEX IX_order_items_menu_item_id ON order_items(menu_item_id);

CREATE INDEX IX_menu_items_category_id ON menu_items(category_id);

CREATE INDEX IX_employees_username ON employees(username);
CREATE INDEX IX_employees_role_id ON employees(role_id);

CREATE INDEX IX_discounts_code ON discounts(code);
CREATE INDEX IX_discounts_is_active ON discounts(is_active);



-- Composite Indexes for Performance
CREATE INDEX IX_orders_status_created_at ON orders(status, created_at);
CREATE INDEX IX_orders_source_status ON orders(source_id, status);
CREATE INDEX IX_order_items_order_menu ON order_items(order_id, menu_item_id);
CREATE INDEX IX_menu_items_category_active ON menu_items(category_id, is_active);
CREATE INDEX IX_order_status_history_order ON order_status_history(order_id, changed_at);
CREATE INDEX IX_employees_role_active ON employees(role_id, is_active);
CREATE INDEX IX_payments_order_status ON payments(order_id, status);

-- Filtered Indexes
CREATE INDEX IX_orders_active ON orders(id) WHERE status IN (N'pending', N'preparing', N'paid');
CREATE INDEX IX_payments_success ON payments(order_id) WHERE status = N'success';
CREATE INDEX IX_menu_items_active_price ON menu_items(price) WHERE is_active = 1;

-- Blog index
CREATE INDEX IX_blog_posts_slug ON blog_posts(slug);
CREATE INDEX IX_blog_posts_status ON blog_posts(status);
CREATE INDEX IX_customers_phone ON customers(phone);
CREATE INDEX IX_customers_email ON customers(email);
CREATE INDEX IX_tags_slug ON tags(slug);
CREATE INDEX IX_blog_post_tags_tag ON blog_post_tags(tag_id);


