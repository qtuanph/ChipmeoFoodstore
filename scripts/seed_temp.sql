-- ============================================================
-- Seed data: Chipmeo Foodstore — restaurant infrastructure
-- (Categories, Menu Items, Addons, Combos, Discounts, Payments)
-- Ad-hoc seed, NOT in DataSeeder.cs
-- ============================================================

DO $$
DECLARE
  now_utc TIMESTAMP := NOW() AT TIME ZONE 'UTC';

  cat_cf    UUID; cat_tea UUID; cat_blend UUID; cat_bakery UUID;
  cat_juice UUID; cat_smoothie UUID;

  add_pearl   UUID; add_jelly UUID; add_condensed UUID; add_cheese UUID;
  add_coffee  UUID; add_coconut UUID; add_aloe UUID;

  mi_den     UUID; mi_sua    UUID; mi_bac    UUID; mi_latte  UUID;
  mi_caphe   UUID; mi_matcha UUID; mi_olong  UUID; mi_dao   UUID;
  mi_chanh   UUID; mi_cam_ep UUID; mi_tao    UUID;
  mi_xoai    UUID; mi_dau    UUID;
  mi_banhmi  UUID; mi_crois  UUID; mi_tirami UUID; mi_brownie UUID;

  cb_sang    UUID; cb_trua   UUID; cb_chieu  UUID;
BEGIN
  -- ============ CATEGORIES ============
  INSERT INTO categories (id, name, description, is_active, created_at, updated_at)
  VALUES (gen_random_uuid(), 'Cà phê', 'Cà phê Việt Nam & Ý — phin, espresso, latte', true, now_utc, now_utc)
  RETURNING id INTO cat_cf;

  INSERT INTO categories (id, name, description, is_active, created_at, updated_at) VALUES
    (gen_random_uuid(), 'Trà', 'Trà xanh, trà ô long & trà trái cây', true, now_utc, now_utc),
    (gen_random_uuid(), 'Đồ uống đá xay', 'Frappé, smoothie, đá xay các loại', true, now_utc, now_utc),
    (gen_random_uuid(), 'Bánh ngọt', 'Bánh ngọt Âu — croissant, brownie, tiramisu', true, now_utc, now_utc),
    (gen_random_uuid(), 'Nước ép', 'Nước ép trái cây tươi', true, now_utc, now_utc),
    (gen_random_uuid(), 'Sinh tố', 'Sinh tố trái cây tươi', true, now_utc, now_utc);

  SELECT id INTO cat_tea     FROM categories WHERE name = 'Trà';
  SELECT id INTO cat_blend   FROM categories WHERE name = 'Đồ uống đá xay';
  SELECT id INTO cat_bakery  FROM categories WHERE name = 'Bánh ngọt';
  SELECT id INTO cat_juice   FROM categories WHERE name = 'Nước ép';
  SELECT id INTO cat_smoothie FROM categories WHERE name = 'Sinh tố';

  -- ============ ADDONS ============
  INSERT INTO addons (id, name, price, is_active, created_at, updated_at) VALUES
    (gen_random_uuid(), 'Trân châu đen', 5000, true, now_utc, now_utc),
    (gen_random_uuid(), 'Thạch rau câu', 5000, true, now_utc, now_utc),
    (gen_random_uuid(), 'Sữa đặc', 3000, true, now_utc, now_utc),
    (gen_random_uuid(), 'Phô mai viên', 7000, true, now_utc, now_utc),
    (gen_random_uuid(), 'Cà phê đen', 4000, true, now_utc, now_utc),
    (gen_random_uuid(), 'Nước cốt dừa', 5000, true, now_utc, now_utc),
    (gen_random_uuid(), 'Thạch lô hội', 5000, true, now_utc, now_utc);

  SELECT id INTO add_pearl     FROM addons WHERE name = 'Trân châu đen';
  SELECT id INTO add_jelly     FROM addons WHERE name = 'Thạch rau câu';
  SELECT id INTO add_condensed FROM addons WHERE name = 'Sữa đặc';
  SELECT id INTO add_cheese    FROM addons WHERE name = 'Phô mai viên';
  SELECT id INTO add_coffee    FROM addons WHERE name = 'Cà phê đen';
  SELECT id INTO add_coconut   FROM addons WHERE name = 'Nước cốt dừa';
  SELECT id INTO add_aloe      FROM addons WHERE name = 'Thạch lô hội';

  -- ============ MENU ITEMS ============
  -- Cà phê
  INSERT INTO menu_items (id, category_id, name, description, price, is_active, created_at, updated_at) VALUES
    (gen_random_uuid(), cat_cf, 'Cà phê đen', 'Cà phê đen truyền thống — đậm đà Việt Nam', 20000, true, now_utc, now_utc),
    (gen_random_uuid(), cat_cf, 'Cà phê sữa', 'Cà phê sữa đá — huyền thoại Sài Gòn', 25000, true, now_utc, now_utc),
    (gen_random_uuid(), cat_cf, 'Bạc xỉu', 'Cà phê sữa ít cà phê, nhiều sữa — dịu nhẹ', 22000, true, now_utc, now_utc),
    (gen_random_uuid(), cat_cf, 'Latte', 'Espresso + sữa nóng — phong cách Ý', 35000, true, now_utc, now_utc),
    (gen_random_uuid(), cat_cf, 'Cà phê trứng', 'Cà phê đen + lòng đỏ trứng đánh bông — đặc sản Hà Nội', 32000, true, now_utc, now_utc);

  SELECT id INTO mi_den   FROM menu_items WHERE name = 'Cà phê đen';
  SELECT id INTO mi_sua   FROM menu_items WHERE name = 'Cà phê sữa';
  SELECT id INTO mi_bac   FROM menu_items WHERE name = 'Bạc xỉu';
  SELECT id INTO mi_latte FROM menu_items WHERE name = 'Latte';
  SELECT id INTO mi_caphe FROM menu_items WHERE name = 'Cà phê trứng';

  -- Trà
  INSERT INTO menu_items (id, category_id, name, description, price, is_active, created_at, updated_at) VALUES
    (gen_random_uuid(), cat_tea, 'Trà matcha', 'Matcha Nhật Bản — xanh mướt, béo ngậy', 30000, true, now_utc, now_utc),
    (gen_random_uuid(), cat_tea, 'Trà ô long', 'Trà ô long Đài Loan — hương nhẹ, hậu ngọt', 25000, true, now_utc, now_utc),
    (gen_random_uuid(), cat_tea, 'Trà đào', 'Trà đào cam sả — thanh mát, thơm ngon', 28000, true, now_utc, now_utc),
    (gen_random_uuid(), cat_tea, 'Trà chanh', 'Trà chanh tươi — giải nhiệt mùa hè', 20000, true, now_utc, now_utc);

  SELECT id INTO mi_matcha FROM menu_items WHERE name = 'Trà matcha';
  SELECT id INTO mi_olong  FROM menu_items WHERE name = 'Trà ô long';
  SELECT id INTO mi_dao    FROM menu_items WHERE name = 'Trà đào';
  SELECT id INTO mi_chanh  FROM menu_items WHERE name = 'Trà chanh';

  -- Đồ uống đá xay
  INSERT INTO menu_items (id, category_id, name, description, price, is_active, created_at, updated_at) VALUES
    (gen_random_uuid(), cat_blend, 'Bạc xỉu đá xay', 'Bạc xỉu phiên bản frappé — mát lạnh', 35000, true, now_utc, now_utc),
    (gen_random_uuid(), cat_blend, 'Cookies & Cream', 'Đá xay kem cookies — béo ngậy', 38000, true, now_utc, now_utc),
    (gen_random_uuid(), cat_blend, 'Matcha đá xay', 'Matcha đá xay kem sữa — thơm béo', 38000, true, now_utc, now_utc);

  -- Bánh ngọt
  INSERT INTO menu_items (id, category_id, name, description, price, is_active, created_at, updated_at) VALUES
    (gen_random_uuid(), cat_bakery, 'Bánh mì que pate', 'Bánh mì que nóng giòn — pate gan truyền thống', 15000, true, now_utc, now_utc),
    (gen_random_uuid(), cat_bakery, 'Croissant bơ', 'Croissant Pháp — 7 lớp bơ nguyên chất', 25000, true, now_utc, now_utc),
    (gen_random_uuid(), cat_bakery, 'Tiramisu', 'Bánh Tiramisu Ý — cà phê, mascarpone, cacao', 35000, true, now_utc, now_utc),
    (gen_random_uuid(), cat_bakery, 'Brownie socola', 'Brownie socola đen — ẩm, đậm vị cacao', 22000, true, now_utc, now_utc);

  SELECT id INTO mi_banhmi  FROM menu_items WHERE name = 'Bánh mì que pate';
  SELECT id INTO mi_crois   FROM menu_items WHERE name = 'Croissant bơ';
  SELECT id INTO mi_tirami  FROM menu_items WHERE name = 'Tiramisu';
  SELECT id INTO mi_brownie FROM menu_items WHERE name = 'Brownie socola';

  -- Nước ép
  INSERT INTO menu_items (id, category_id, name, description, price, is_active, created_at, updated_at) VALUES
    (gen_random_uuid(), cat_juice, 'Nước ép cam', 'Cam tươi nguyên chất — không đường, không nước', 25000, true, now_utc, now_utc),
    (gen_random_uuid(), cat_juice, 'Nước ép táo', 'Táo xanh nhập khẩu — ép tươi tại quán', 30000, true, now_utc, now_utc),
    (gen_random_uuid(), cat_juice, 'Nước ép cà rốt', 'Cà rốt Đà Lạt — giàu vitamin A', 25000, true, now_utc, now_utc);

  SELECT id INTO mi_cam_ep FROM menu_items WHERE name = 'Nước ép cam';
  SELECT id INTO mi_tao    FROM menu_items WHERE name = 'Nước ép táo';

  -- Sinh tố
  INSERT INTO menu_items (id, category_id, name, description, price, is_active, created_at, updated_at) VALUES
    (gen_random_uuid(), cat_smoothie, 'Sinh tố xoài', 'Xoài cát Hòa Lộc — đặc sản miền Tây', 30000, true, now_utc, now_utc),
    (gen_random_uuid(), cat_smoothie, 'Sinh tố dâu', 'Dâu tây Đà Lạt — chua ngọt, thơm lừng', 32000, true, now_utc, now_utc),
    (gen_random_uuid(), cat_smoothie, 'Sinh tố bơ', 'Bơ sáp Đăk Lăk — béo ngậy, siêu mịn', 35000, true, now_utc, now_utc);

  SELECT id INTO mi_xoai FROM menu_items WHERE name = 'Sinh tố xoài';
  SELECT id INTO mi_dau  FROM menu_items WHERE name = 'Sinh tố dâu';

  -- ============ MENU ITEM — ADDON MAPPING ============
  INSERT INTO menu_item_addons (id, menu_item_id, addon_id, is_active, created_at, updated_at) VALUES
    (gen_random_uuid(), mi_den, add_condensed, true, now_utc, now_utc),
    (gen_random_uuid(), mi_den, add_coffee, true, now_utc, now_utc),
    (gen_random_uuid(), mi_sua, add_pearl, true, now_utc, now_utc),
    (gen_random_uuid(), mi_sua, add_jelly, true, now_utc, now_utc),
    (gen_random_uuid(), mi_sua, add_condensed, true, now_utc, now_utc),
    (gen_random_uuid(), mi_sua, add_cheese, true, now_utc, now_utc),
    (gen_random_uuid(), mi_matcha, add_pearl, true, now_utc, now_utc),
    (gen_random_uuid(), mi_matcha, add_jelly, true, now_utc, now_utc),
    (gen_random_uuid(), mi_matcha, add_coconut, true, now_utc, now_utc),
    (gen_random_uuid(), mi_dao, add_jelly, true, now_utc, now_utc),
    (gen_random_uuid(), mi_dao, add_aloe, true, now_utc, now_utc),
    (gen_random_uuid(), mi_xoai, add_jelly, true, now_utc, now_utc),
    (gen_random_uuid(), mi_dau, add_jelly, true, now_utc, now_utc);

  -- ============ COMBOS ============
  INSERT INTO combos (id, name, description, combo_price, is_active, created_at, updated_at) VALUES
    (gen_random_uuid(), 'Combo Sáng', 'Cà phê đen + Bánh mì que', 30000, true, now_utc, now_utc),
    (gen_random_uuid(), 'Combo Trưa', 'Cà phê sữa + Croissant', 40000, true, now_utc, now_utc),
    (gen_random_uuid(), 'Combo Chiều', 'Trà đào + Tiramisu', 55000, true, now_utc, now_utc);

  SELECT id INTO cb_sang  FROM combos WHERE name = 'Combo Sáng';
  SELECT id INTO cb_trua  FROM combos WHERE name = 'Combo Trưa';
  SELECT id INTO cb_chieu FROM combos WHERE name = 'Combo Chiều';

  INSERT INTO combo_items (id, combo_id, menu_item_id, quantity, created_at, updated_at) VALUES
    (gen_random_uuid(), cb_sang, mi_den, 1, now_utc, now_utc),
    (gen_random_uuid(), cb_sang, mi_banhmi, 1, now_utc, now_utc),
    (gen_random_uuid(), cb_trua, mi_sua, 1, now_utc, now_utc),
    (gen_random_uuid(), cb_trua, mi_crois, 1, now_utc, now_utc),
    (gen_random_uuid(), cb_chieu, mi_dao, 1, now_utc, now_utc),
    (gen_random_uuid(), cb_chieu, mi_tirami, 1, now_utc, now_utc);

  -- ============ DISCOUNTS ============
  INSERT INTO discounts (id, code, name, type, value, max_discount_amount, min_order_amount, usage_limit, used_count, is_active, start_date, end_date, created_at, updated_at) VALUES
    (gen_random_uuid(), 'KHAITRUONG', 'Giảm 20% khai trương', 'percent', 20, 50000, 50000, 1000, 0, true, now_utc, now_utc + INTERVAL '90 days', now_utc, now_utc),
    (gen_random_uuid(), 'THANHVIEN', 'Giảm 10% cho hội viên', 'percent', 10, 30000, 50000, 999999, 0, true, now_utc, now_utc + INTERVAL '365 days', now_utc, now_utc),
    (gen_random_uuid(), 'SINHNHAT', 'Giảm 50K sinh nhật', 'amount', 50000, NULL, 100000, 1, 0, true, now_utc, now_utc + INTERVAL '365 days', now_utc, now_utc),
    (gen_random_uuid(), 'GIOVANG', 'Giảm 15% giờ vàng (14-17h)', 'percent', 15, 30000, 30000, 500, 0, true, now_utc, now_utc + INTERVAL '60 days', now_utc, now_utc),
    (gen_random_uuid(), 'CODERED', 'Giảm 30K đơn trên 150K', 'amount', 30000, NULL, 150000, 200, 0, true, now_utc, now_utc + INTERVAL '30 days', now_utc, now_utc);

  -- ============ PAYMENT SETTINGS ============
  INSERT INTO payment_settings (id, bank_id, bank_account, bank_name, bank_account_name, template, is_default, is_active, created_at, updated_at) VALUES
    (gen_random_uuid(), '970436', '0123456789', 'Ngân hàng TMCP Ngoại thương Việt Nam (Vietcombank)', 'CHIPMEO FOODSTORE', 'compact2', true, true, now_utc, now_utc);

  RAISE NOTICE '✓ Seed completed: 6 categories, 22 items, 7 addons, 13 item-addon links, 3 combos, 5 discounts, 1 payment setting';
END $$;
