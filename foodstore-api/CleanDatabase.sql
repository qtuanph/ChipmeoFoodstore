-- ============================================
-- CLEAN DATABASE - XÓA TOÀN BỘ DỮ LIỆU
-- Reset về trạng thái ban đầu
-- ============================================

USE [pos_shop];
GO

PRINT '=================================================';
PRINT 'BẮT ĐẦU XÓA DỮ LIỆU...';
PRINT '=================================================';

-- Tắt foreign key constraints tạm thời
EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';

-- ============================================
-- XÓA DỮ LIỆU THEO THỨ TỰ (con trước, cha sau)
-- ============================================

-- 1. Xóa Order related tables (con nhất)
DELETE FROM order_item_addons;
PRINT 'Đã xóa order_item_addons';

DELETE FROM order_items;
PRINT 'Đã xóa order_items';

DELETE FROM order_status_history;
PRINT 'Đã xóa order_status_history';

DELETE FROM payments;
PRINT 'Đã xóa payments';

DELETE FROM orders;
PRINT 'Đã xóa orders';

-- 2. Xóa Combo related
DELETE FROM combo_items;
PRINT 'Đã xóa combo_items';

DELETE FROM combos;
PRINT 'Đã xóa combos';

-- 3. Xóa Menu Item Addons mapping
DELETE FROM menu_item_addons;
PRINT 'Đã xóa menu_item_addons';

-- 4. Xóa Menu Items
DELETE FROM menu_items;
PRINT 'Đã xóa menu_items';

-- 5. Xóa Addons
DELETE FROM addons;
PRINT 'Đã xóa addons';

-- 6. Xóa Categories
DELETE FROM categories;
PRINT 'Đã xóa categories';

-- 7. Xóa Discounts
DELETE FROM discounts;
PRINT 'Đã xóa discounts';

-- 8. Xóa Tables
DELETE FROM tables;
PRINT 'Đã xóa tables';

-- 9. Xóa RBAC related
DELETE FROM role_permissions;
PRINT 'Đã xóa role_permissions';

DELETE FROM employees;
PRINT 'Đã xóa employees';

DELETE FROM permissions;
PRINT 'Đã xóa permissions';

DELETE FROM roles;
PRINT 'Đã xóa roles';

-- 10. Xóa Payment Settings
DELETE FROM payment_settings;
PRINT 'Đã xóa payment_settings';

-- 11. Xóa Media
DELETE FROM media;
PRINT 'Đã xóa media';

-- 12. Xóa Blog Posts
DELETE FROM blog_posts;
PRINT 'Đã xóa blog_posts';

-- 13. Xóa Customers (phải xóa sau orders vì orders có FK tới customers)
DELETE FROM customers;
PRINT 'Đã xóa customers';

-- ============================================
-- RESET IDENTITY SEEDS VỀ 1
-- ============================================

DBCC CHECKIDENT ('order_item_addons', RESEED, 0);
DBCC CHECKIDENT ('order_items', RESEED, 0);
DBCC CHECKIDENT ('order_status_history', RESEED, 0);
DBCC CHECKIDENT ('payments', RESEED, 0);
DBCC CHECKIDENT ('orders', RESEED, 0);
DBCC CHECKIDENT ('combo_items', RESEED, 0);
DBCC CHECKIDENT ('combos', RESEED, 0);
DBCC CHECKIDENT ('menu_item_addons', RESEED, 0);
DBCC CHECKIDENT ('menu_items', RESEED, 0);
DBCC CHECKIDENT ('addons', RESEED, 0);
DBCC CHECKIDENT ('categories', RESEED, 0);
DBCC CHECKIDENT ('discounts', RESEED, 0);
DBCC CHECKIDENT ('tables', RESEED, 0);
DBCC CHECKIDENT ('role_permissions', RESEED, 0);
DBCC CHECKIDENT ('employees', RESEED, 0);
DBCC CHECKIDENT ('permissions', RESEED, 0);
DBCC CHECKIDENT ('roles', RESEED, 0);
DBCC CHECKIDENT ('payment_settings', RESEED, 0);
DBCC CHECKIDENT ('media', RESEED, 0);
DBCC CHECKIDENT ('blog_posts', RESEED, 0);
DBCC CHECKIDENT ('customers', RESEED, 0);

PRINT 'Đã reset tất cả identity seeds về 0';

-- Bật lại foreign key constraints
EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL';

PRINT '';
PRINT '=================================================';
PRINT 'HOÀN TẤT XÓA DỮ LIỆU!';
PRINT '=================================================';
PRINT '';
PRINT 'Database đã sạch. Bây giờ bạn có thể chạy SeedData.sql';
PRINT '';

-- ============================================
-- VERIFICATION - Kiểm tra tất cả tables đã trống
-- ============================================

SELECT 'categories' AS [Table], COUNT(*) AS [Count] FROM categories UNION ALL
SELECT 'menu_items', COUNT(*) FROM menu_items UNION ALL
SELECT 'addons', COUNT(*) FROM addons UNION ALL
SELECT 'menu_item_addons', COUNT(*) FROM menu_item_addons UNION ALL
SELECT 'combos', COUNT(*) FROM combos UNION ALL
SELECT 'combo_items', COUNT(*) FROM combo_items UNION ALL
SELECT 'discounts', COUNT(*) FROM discounts UNION ALL
SELECT 'tables', COUNT(*) FROM tables UNION ALL
SELECT 'roles', COUNT(*) FROM roles UNION ALL
SELECT 'permissions', COUNT(*) FROM permissions UNION ALL
SELECT 'role_permissions', COUNT(*) FROM role_permissions UNION ALL
SELECT 'employees', COUNT(*) FROM employees UNION ALL
SELECT 'orders', COUNT(*) FROM orders UNION ALL
SELECT 'order_items', COUNT(*) FROM order_items UNION ALL
SELECT 'order_item_addons', COUNT(*) FROM order_item_addons UNION ALL
SELECT 'order_status_history', COUNT(*) FROM order_status_history UNION ALL
SELECT 'payments', COUNT(*) FROM payments UNION ALL
SELECT 'payment_settings', COUNT(*) FROM payment_settings UNION ALL
SELECT 'media', COUNT(*) FROM media UNION ALL
SELECT 'blog_posts', COUNT(*) FROM blog_posts UNION ALL
SELECT 'customers', COUNT(*) FROM customers
ORDER BY [Table];

GO