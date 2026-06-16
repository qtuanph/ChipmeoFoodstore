// Svelte stores for POS data - NO MOCK DATA, all from backend
import { writable, derived } from 'svelte/store';
import type {
	Category,
	MenuItem,
	Addon,
	Source,
	CartItem,
	Discount,
	Order,
	Combo,
	CartState
} from '$lib/types/index.js';
import { calculateCartTotals } from './cart.js';

// ============================================
// DATA STORES (populated from backend API)
// ============================================

// Categories store - loaded from backend
export const categories = writable<Category[]>([]);

// Menu items store - loaded from backend
export const menuItems = writable<MenuItem[]>([]);

// Combos store - loaded from backend
export const combos = writable<Combo[]>([]);

// Addons store - loaded from backend
export const addons = writable<Addon[]>([]);

// Sources store - loaded from backend
export const sources = writable<Source[]>([]);

// Discounts store - loaded from backend
export const discounts = writable<Discount[]>([]);

// Orders store - loaded from backend (Added for Admin Dashboard)
export const orders = writable<Order[]>([]);

// ============================================
// CART STORE
// ============================================

function createCartStore() {
	const { subscribe, set, update } = writable<CartState>({
		items: [],
		selectedSource: null,
		discount: null,
		voucherCode: ''
	});

	return {
		subscribe,

		addItem: (item: CartItem) => {
			update((state) => {
				// Check if item already exists (same menu item/combo and same addons)
				const existingIndex = state.items.findIndex((i) => {
					if (item.menuItem && i.menuItem) {
						const sameItem = i.menuItem.id === item.menuItem.id;
						const sameAddons =
							JSON.stringify(i.selectedAddons) === JSON.stringify(item.selectedAddons);
						const sameNote = i.note === item.note;
						return sameItem && sameAddons && sameNote;
					}
					if (item.combo && i.combo) {
						return i.combo.id === item.combo.id;
					}
					return false;
				});

				if (existingIndex >= 0) {
					// Update quantity of existing item
					state.items[existingIndex].quantity += item.quantity;
					state.items[existingIndex].subtotal += item.subtotal;
				} else {
					// Add new item
					state.items.push(item);
				}

				return state;
			});
		},

		removeItem: (index: number) => {
			update((state) => {
				state.items.splice(index, 1);
				return state;
			});
		},

		updateQuantity: (index: number, quantity: number) => {
			if (quantity <= 0) {
				return;
			}

			update((state) => {
				const item = state.items[index];
				if (!item) return state;

				// Recalculate subtotal based on new quantity
				const basePrice = item.menuItem?.price || item.combo?.comboPrice || 0;
				const addonsTotal = item.selectedAddons.reduce(
					(sum, a) => sum + a.addon.price * a.quantity,
					0
				);

				item.quantity = quantity;
				item.subtotal = (basePrice + addonsTotal) * quantity;

				return state;
			});
		},

		setSource: (source: Source) => {
			update((state) => {
				state.selectedSource = source;
				return state;
			});
		},

		setDiscount: (discount: Discount | null) => {
			update((state) => {
				state.discount = discount;
				return state;
			});
		},

		setCustomerInfo: (name?: string, phone?: string) => {
			update((state) => {
				state.customerName = name;
				state.customerPhone = phone;
				return state;
			});
		},

		setNote: (note?: string) => {
			update((state) => {
				state.note = note;
				return state;
			});
		},

		clearCart: () => {
			set({
				items: [],
				selectedSource: null,
				discount: null,
				voucherCode: ''
			});
		},

		reset: () => {
			set({
				items: [],
				selectedSource: null,
				discount: null,
				voucherCode: ''
			});
		}
	};
}

export const cart = createCartStore();

// ============================================
// CART TOTALS (Derived Store)
// ============================================

export const cartTotals = derived(cart, ($cart) => calculateCartTotals($cart));

// ============================================
// CART ACTIONS (for easier imports)
// ============================================

export const cartActions = {
	addItem: cart.addItem,
	removeItem: cart.removeItem,
	updateQuantity: cart.updateQuantity,
	setSource: cart.setSource,
	setDiscount: cart.setDiscount,
	setCustomerInfo: cart.setCustomerInfo,
	setNote: cart.setNote,
	clearCart: cart.clearCart,
	reset: cart.reset
};
