import { apiRequest } from '$lib/api/utils.js';
import { API_ENDPOINTS } from '$lib/config/index.ts';
import type { Combo, ComboCreateDto, ComboUpdateDto } from '$lib/types/index.ts';

export const combosAPI = {
	async getAll(): Promise<Combo[]> {
		return apiRequest<Combo[]>(API_ENDPOINTS.combos.list);
	},

	async getById(id: number): Promise<Combo> {
		return apiRequest<Combo>(API_ENDPOINTS.combos.detail(id));
	},

	async create(data: ComboCreateDto): Promise<Combo> {
		return apiRequest<Combo>(API_ENDPOINTS.combos.create, {
			method: 'POST',
			body: JSON.stringify(data)
		});
	},

	async update(id: number, data: ComboUpdateDto): Promise<void> {
		return apiRequest<void>(API_ENDPOINTS.combos.update(id), {
			method: 'PUT',
			body: JSON.stringify(data)
		});
	},

	async delete(id: number): Promise<void> {
		return apiRequest<void>(API_ENDPOINTS.combos.delete(id), {
			method: 'DELETE'
		});
	}
};
