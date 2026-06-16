import type { MenuItem } from './menu-item.js';
import type { Combo } from './combo.js';
import type { Addon } from './addon.js';

export interface CartItemAddon {
	addon: Addon;
	quantity: number;
}

export interface CartItem {
	menuItem?: MenuItem;
	combo?: Combo;
	selectedAddons: CartItemAddon[];
	quantity: number;
	subtotal: number;
	note?: string;
}

import type { Source } from './source.js';
import type { Discount } from './discount.js';

export interface CartState {
	items: CartItem[];
	selectedSource: Source | null;
	discount: Discount | null;
	customerName?: string;
	customerPhone?: string;
	note?: string;
	voucherCode?: string;
}
