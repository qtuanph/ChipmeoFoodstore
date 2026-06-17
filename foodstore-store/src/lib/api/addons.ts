import { apiRequest } from '$lib/api/utils.js';
import { API_ENDPOINTS } from '$lib/config/index.ts';
import type { Addon, AddonCreateDto, AddonUpdateDto } from '$lib/types/index.ts';

export const addonsAPI = {
	async getAll(): Promise<Addon[]> {
		return apiRequest<Addon[]>(API_ENDPOINTS.addons.list);
	},

	async getById(id: number): Promise<Addon> {
		return apiRequest<Addon>(API_ENDPOINTS.addons.detail(id));
	},

	async create(data: AddonCreateDto): Promise<Addon> {
		return apiRequest<Addon>(API_ENDPOINTS.addons.create, {
			method: 'POST',
			body: JSON.stringify(data)
		});
	},

	async update(id: number, data: AddonUpdateDto): Promise<void> {
		return apiRequest<void>(API_ENDPOINTS.addons.update(id), {
			method: 'PUT',
			body: JSON.stringify(data)
		});
	},

	async delete(id: number): Promise<void> {
		return apiRequest<void>(API_ENDPOINTS.addons.delete(id), {
			method: 'DELETE'
		});
	}
};
