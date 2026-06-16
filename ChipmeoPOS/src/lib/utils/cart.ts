import type { CartState } from '$lib/types/index.js';

export function calculateCartTotals(cart: CartState) {
	const subtotal = cart.items.reduce((sum, item) => sum + item.subtotal, 0);
	const itemCount = cart.items.reduce((sum, item) => sum + item.quantity, 0);

	let discountAmount = 0;
	if (cart.discount) {
		if (cart.discount.type === 'percent') {
			discountAmount = subtotal * (cart.discount.value / 100);
			if (cart.discount.maxDiscountAmount) {
				discountAmount = Math.min(discountAmount, cart.discount.maxDiscountAmount);
			}
		} else if (cart.discount.type === 'amount') {
			discountAmount = cart.discount.value;
		}
	}

	const vatAmount = subtotal * 0.1;
	const total = subtotal - discountAmount + vatAmount;

	return {
		subtotal,
		discountAmount,
		vatAmount,
		total,
		itemCount
	};
}
