-- ============================================
-- DROP ALL TABLES - XÓA TOÀN BỘ BẢNG
-- Giữ lại database, xóa tất cả tables
-- ============================================

USE [pos_shop];
GO

PRINT '=================================================';
PRINT 'BẮT ĐẦU XÓA TOÀN BỘ BẢNG...';
PRINT '=================================================';

-- ============================================
-- BƯỚC 1: Tắt tất cả foreign key constraints
-- ============================================
PRINT 'Đang tắt foreign key constraints...';

DECLARE @sql NVARCHAR(MAX) = N'';

SELECT @sql += N'ALTER TABLE ' + QUOTENAME(s.name) + '.' + QUOTENAME(t.name) 
    + ' DROP CONSTRAINT ' + QUOTENAME(f.name) + ';' + CHAR(13)
FROM sys.foreign_keys f
INNER JOIN sys.tables t ON f.parent_object_id = t.object_id
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id;

EXEC sp_executesql @sql;
PRINT 'Đã xóa tất cả foreign key constraints';

-- ============================================
-- BƯỚC 2: Drop tất cả tables
-- ============================================
PRINT 'Đang xóa tất cả tables...';

SET @sql = N'';

SELECT @sql += N'DROP TABLE ' + QUOTENAME(s.name) + '.' + QUOTENAME(t.name) + ';' + CHAR(13)
FROM sys.tables t
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
WHERE t.type = 'U';

EXEC sp_executesql @sql;

PRINT 'Đã xóa tất cả tables';

-- ============================================
-- VERIFICATION - Kiểm tra không còn table nào
-- ============================================
PRINT '';
PRINT '=================================================';
PRINT 'HOÀN TẤT XÓA TOÀN BỘ BẢNG!';
PRINT '=================================================';
PRINT '';

SELECT 'Số tables còn lại: ' + CAST(COUNT(*) AS VARCHAR(10)) AS [Result]
FROM sys.tables;

PRINT '';
PRINT 'Database [pos_shop] đã sạch hoàn toàn.';
PRINT 'Bây giờ bạn có thể chạy Database.sql để tạo lại schema.';
PRINT '';

GO