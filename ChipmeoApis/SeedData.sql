-- ============================================
-- SEED DATA - CHỈ ADMIN ROOT VÀ PERMISSIONS
-- ============================================

USE [pos_shop];
GO

-- 1. Insert Admin Role
SET IDENTITY_INSERT roles ON;

INSERT INTO roles (id, name, description, is_active, created_at)
VALUES (1, N'Admin', N'Quản trị viên Root - Toàn quyền hệ thống', 1, GETUTCDATE());

SET IDENTITY_INSERT roles OFF;

-- 2. Insert Admin User
-- Username: admin
-- Password: admin123
-- BCrypt hash for password "admin123"
SET IDENTITY_INSERT employees ON;

INSERT INTO employees (id, full_name, username, password_hash, email, phone, role_id, is_active, created_at)
VALUES (
    1, 
    N'Root Administrator', 
    'admin', 
    '$2a$12$89g1rmrXs66X5vlgbx92SuY51m.jizW3DTnsys9q9OUbtIBANrl7q', 
    'chipmeo2304@gmail.com', 
    '0903282169', 
    1, 
    1, 
    GETUTCDATE()
);

SET IDENTITY_INSERT employees OFF;

-- 3. Insert All Permissions (44 permissions - 12 modules)
SET IDENTITY_INSERT permissions ON;

INSERT INTO permissions (id, code, name, description, module, created_at) VALUES
-- Category Module (1-4)
(1, 'category.view', N'Xem danh mục', N'Có thể xem danh sách danh mục', 'category', GETUTCDATE()),
(2, 'category.create', N'Tạo danh mục', N'Có thể tạo danh mục mới', 'category', GETUTCDATE()),
(3, 'category.update', N'Sửa danh mục', N'Có thể sửa danh mục', 'category', GETUTCDATE()),
(4, 'category.delete', N'Xóa danh mục', N'Có thể xóa danh mục', 'category', GETUTCDATE()),
-- Menu Module (5-8)
(5, 'menu.view', N'Xem món ăn', N'Có thể xem danh sách món', 'menu', GETUTCDATE()),
(6, 'menu.create', N'Tạo món ăn', N'Có thể tạo món mới', 'menu', GETUTCDATE()),
(7, 'menu.update', N'Sửa món ăn', N'Có thể sửa món', 'menu', GETUTCDATE()),
(8, 'menu.delete', N'Xóa món ăn', N'Có thể xóa món', 'menu', GETUTCDATE()),
-- Order Module (9-12)
(9, 'order.view', N'Xem đơn hàng', N'Có thể xem đơn', 'order', GETUTCDATE()),
(10, 'order.create', N'Tạo đơn hàng', N'Có thể tạo đơn', 'order', GETUTCDATE()),
(11, 'order.update', N'Cập nhật đơn', N'Có thể cập nhật trạng thái đơn', 'order', GETUTCDATE()),
(12, 'order.delete', N'Hủy đơn hàng', N'Có thể hủy đơn', 'order', GETUTCDATE()),
-- Employee Module (13-16)
(13, 'employee.view', N'Xem nhân viên', N'Có thể xem danh sách nhân viên', 'employee', GETUTCDATE()),
(14, 'employee.create', N'Tạo nhân viên', N'Có thể tạo nhân viên mới', 'employee', GETUTCDATE()),
(15, 'employee.update', N'Sửa nhân viên', N'Có thể sửa thông tin nhân viên', 'employee', GETUTCDATE()),
(16, 'employee.delete', N'Xóa nhân viên', N'Có thể xóa nhân viên', 'employee', GETUTCDATE()),
-- Role Module (17-20)
(17, 'role.view', N'Xem vai trò', N'Có thể xem vai trò', 'role', GETUTCDATE()),
(18, 'role.create', N'Tạo vai trò', N'Có thể tạo vai trò mới', 'role', GETUTCDATE()),
(19, 'role.update', N'Sửa vai trò', N'Có thể sửa vai trò', 'role', GETUTCDATE()),
(20, 'role.delete', N'Xóa vai trò', N'Có thể xóa vai trò', 'role', GETUTCDATE()),
-- Discount Module (21-24)
(21, 'discount.view', N'Xem giảm giá', N'Có thể xem mã giảm giá', 'discount', GETUTCDATE()),
(22, 'discount.create', N'Tạo giảm giá', N'Có thể tạo mã giảm giá', 'discount', GETUTCDATE()),
(23, 'discount.update', N'Sửa giảm giá', N'Có thể sửa mã giảm giá', 'discount', GETUTCDATE()),
(24, 'discount.delete', N'Xóa giảm giá', N'Có thể xóa mã giảm giá', 'discount', GETUTCDATE()),
-- Source Module (25-28)
(25, 'source.view', N'Xem nguồn đơn', N'Có thể xem danh sách nguồn đơn', 'source', GETUTCDATE()),
(26, 'source.create', N'Tạo nguồn đơn', N'Có thể tạo nguồn đơn mới', 'source', GETUTCDATE()),
(27, 'source.update', N'Sửa nguồn đơn', N'Có thể sửa thông tin nguồn đơn', 'source', GETUTCDATE()),
(28, 'source.delete', N'Xóa nguồn đơn', N'Có thể xóa nguồn đơn', 'source', GETUTCDATE()),
-- Addon Module (29-32)
(29, 'addon.view', N'Xem topping', N'Có thể xem topping', 'addon', GETUTCDATE()),
(30, 'addon.create', N'Tạo topping', N'Có thể tạo topping', 'addon', GETUTCDATE()),
(31, 'addon.update', N'Sửa topping', N'Có thể sửa topping', 'addon', GETUTCDATE()),
(32, 'addon.delete', N'Xóa topping', N'Có thể xóa topping', 'addon', GETUTCDATE()),
-- Combo Module (33-36)
(33, 'combo.view', N'Xem combo', N'Có thể xem combo', 'combo', GETUTCDATE()),
(34, 'combo.create', N'Tạo combo', N'Có thể tạo combo', 'combo', GETUTCDATE()),
(35, 'combo.update', N'Sửa combo', N'Có thể sửa combo', 'combo', GETUTCDATE()),
(36, 'combo.delete', N'Xóa combo', N'Có thể xóa combo', 'combo', GETUTCDATE()),
-- Dashboard Module (37-38)
(37, 'dashboard.view', N'Xem dashboard', N'Có thể xem trang tổng quan', 'dashboard', GETUTCDATE()),
(38, 'analytics.view', N'Xem phân tích', N'Có thể xem báo cáo và dự báo', 'analytics', GETUTCDATE()),
-- Analytics Export (39)
(39, 'analytics.export', N'Xuất báo cáo', N'Có thể xuất file báo cáo', 'analytics', GETUTCDATE()),
-- Payment Module (40-42)
(40, 'payment.view', N'Xem cài đặt thanh toán', N'Có thể xem thông tin tài khoản ngân hàng', 'payment', GETUTCDATE()),
(41, 'payment.update', N'Cập nhật thanh toán', N'Có thể thêm/sửa tài khoản ngân hàng', 'payment', GETUTCDATE()),
(42, 'payment.delete', N'Xóa cài đặt thanh toán', N'Có thể xóa tài khoản ngân hàng', 'payment', GETUTCDATE()),
-- System Module (43-44)
(43, 'system.view', N'Xem cài đặt hệ thống', N'Có thể xem cấu hình hệ thống', 'system', GETUTCDATE()),
(44, 'system.update', N'Cập nhật cài đặt hệ thống', N'Có thể thay đổi cấu hình hệ thống', 'system', GETUTCDATE()),

-- Blog Module (Using IDs 45-48)
(45, 'blog.view', N'Xem bài viết', N'Có thể xem danh sách bài viết', 'blog', GETUTCDATE()),
(46, 'blog.create', N'Tạo bài viết', N'Có thể tạo bài viết mới', 'blog', GETUTCDATE()),
(47, 'blog.update', N'Sửa bài viết', N'Có thể sửa bài viết', 'blog', GETUTCDATE()),
(48, 'blog.delete', N'Xóa bài viết', N'Có thể xóa bài viết', 'blog', GETUTCDATE()),

-- Customer Module (Using IDs 49-52)
(49, 'customer.view', N'Xem khách hàng', N'Có thể xem danh sách khách hàng', 'customer', GETUTCDATE()),
(50, 'customer.create', N'Tạo khách hàng', N'Có thể tạo khách hàng', 'customer', GETUTCDATE()),
(51, 'customer.update', N'Sửa khách hàng', N'Có thể sửa thông tin khách hàng', 'customer', GETUTCDATE()),
(52, 'customer.delete', N'Xóa khách hàng', N'Có thể xóa khách hàng', 'customer', GETUTCDATE()),

-- Media Module (Using IDs 53-54)
(53, 'media.upload', N'Tải ảnh lên', N'Có thể tải ảnh lên hệ thống', 'media', GETUTCDATE()),
(54, 'media.delete', N'Xóa ảnh', N'Có thể xóa ảnh khỏi hệ thống', 'media', GETUTCDATE());
SET IDENTITY_INSERT permissions OFF;

-- 4. Assign All Permissions to Admin Role
-- 4. Assign All Permissions to Admin Role
INSERT INTO role_permissions (role_id, permission_id, is_active, created_at)
SELECT 1, id, 1, GETUTCDATE() FROM permissions;

-- ============================================
-- VERIFICATION
-- ============================================

PRINT '=================================================';
PRINT 'SEED DATA COMPLETED - ROOT ADMIN SETUP';
PRINT '=================================================';
PRINT '';
PRINT 'ROOT ADMIN LOGIN:';
PRINT 'Username: admin';
PRINT 'Password: admin123';
PRINT '';

SELECT 'Admin User' AS Info, id, username, full_name, email FROM employees WHERE username = 'admin';
SELECT 'Roles' AS Info, COUNT(*) AS total FROM roles;
SELECT 'Permissions' AS Info, COUNT(*) AS total FROM permissions;
SELECT 'Role Permissions' AS Info, COUNT(*) AS total FROM role_permissions;

PRINT '';
PRINT '=================================================';
PRINT 'Root Admin có 44 permissions đầy đủ (12 modules)';
PRINT 'Bạn có thể thêm dữ liệu quán ăn qua Admin UI';
PRINT '=================================================';

GO