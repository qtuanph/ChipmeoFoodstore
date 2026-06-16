import { apiRequest } from '$lib/api/utils.js';
import { API_ENDPOINTS } from '$lib/config/index.ts';
import type { Category, CategoryCreateDto, CategoryUpdateDto } from '$lib/types/index.ts';

export const categoriesAPI = {
	async getAll(): Promise<Category[]> {
		return apiRequest<Category[]>(API_ENDPOINTS.categories.list);
	},

	async getById(id: number): Promise<Category> {
		return apiRequest<Category>(API_ENDPOINTS.categories.detail(id));
	},

	async create(data: CategoryCreateDto): Promise<Category> {
		return apiRequest<Category>(API_ENDPOINTS.categories.create, {
			method: 'POST',
			body: JSON.stringify(data)
		});
	},

	async update(id: number, data: CategoryUpdateDto): Promise<void> {
		return apiRequest<void>(API_ENDPOINTS.categories.update(id), {
			method: 'PUT',
			body: JSON.stringify(data)
		});
	},

	async delete(id: number): Promise<void> {
		return apiRequest<void>(API_ENDPOINTS.categories.delete(id), {
			method: 'DELETE'
		});
	}
};
