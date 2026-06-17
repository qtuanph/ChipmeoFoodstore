import { apiRequest } from '$lib/api/utils.js';
import { API_ENDPOINTS } from '$lib/config/index.ts';
import type { Discount, DiscountCreateDto, DiscountUpdateDto } from '$lib/types/index.ts';

export const discountsAPI = {
	async getAll(): Promise<Discount[]> {
		return apiRequest<Discount[]>(API_ENDPOINTS.discounts.list);
	},

	async getById(id: number): Promise<Discount> {
		return apiRequest<Discount>(API_ENDPOINTS.discounts.detail(id));
	},

	async create(data: DiscountCreateDto): Promise<Discount> {
		return apiRequest<Discount>(API_ENDPOINTS.discounts.create, {
			method: 'POST',
			body: JSON.stringify(data)
		});
	},

	async update(id: number, data: DiscountUpdateDto): Promise<void> {
		return apiRequest<void>(API_ENDPOINTS.discounts.update(id), {
			method: 'PUT',
			body: JSON.stringify(data)
		});
	},

	async delete(id: number): Promise<void> {
		return apiRequest<void>(API_ENDPOINTS.discounts.delete(id), {
			method: 'DELETE'
		});
	}
};
