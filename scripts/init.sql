-- ==========================================================
-- INIT SCRIPT — PostgreSQL 18 + Vietnamese Collation (ICU)
-- EF Core Code-First sẽ tự tạo toàn bộ tables qua migration
-- Chỉ tạo collation + extension ở đây
-- ==========================================================

-- Tạo ICU collation cho tiếng Việt (không dấu, không phân biệt hoa/thường)
CREATE COLLATION IF NOT EXISTS vi_ci_ai (
    PROVIDER = icu,
    LOCALE = 'vi-VN-x-icu',
    DETERMINISTIC = false
);

-- Extension pgcrypto cho gen_random_uuid()
CREATE EXTENSION IF NOT EXISTS "pgcrypto";
