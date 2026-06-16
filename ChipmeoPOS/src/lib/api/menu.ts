import { apiRequest } from '$lib/api/utils.js';
import { API_ENDPOINTS } from '$lib/config/index.ts';
import type { MenuItem, MenuItemCreateDto, MenuItemUpdateDto } from '$lib/types/index.ts';

export const menuItemsAPI = {
	async getAll(): Promise<MenuItem[]> {
		return apiRequest<MenuItem[]>(API_ENDPOINTS.menuItems.list);
	},

	async getById(id: number): Promise<MenuItem> {
		return apiRequest<MenuItem>(API_ENDPOINTS.menuItems.detail(id));
	},

	async create(data: MenuItemCreateDto): Promise<MenuItem> {
		return apiRequest<MenuItem>(API_ENDPOINTS.menuItems.create, {
			method: 'POST',
			body: JSON.stringify(data)
		});
	},

	async update(id: number, data: MenuItemUpdateDto): Promise<void> {
		return apiRequest<void>(API_ENDPOINTS.menuItems.update(id), {
			method: 'PUT',
			body: JSON.stringify(data)
		});
	},

	async delete(id: number): Promise<void> {
		return apiRequest<void>(API_ENDPOINTS.menuItems.delete(id), {
			method: 'DELETE'
		});
	}
};
