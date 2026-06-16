import { apiRequest } from '$lib/api/utils.js';
import { API_ENDPOINTS } from '$lib/config/index.js';
import type { Source, SourceCreateDto, SourceUpdateDto } from '$lib/types/index.js';

export const sourcesAPI = {
	async getAll(): Promise<Source[]> {
		return apiRequest<Source[]>(API_ENDPOINTS.sources.list);
	},

	async getById(id: number): Promise<Source> {
		return apiRequest<Source>(API_ENDPOINTS.sources.detail(id));
	},

	async create(data: SourceCreateDto): Promise<Source> {
		return apiRequest<Source>(API_ENDPOINTS.sources.create, {
			method: 'POST',
			body: JSON.stringify(data)
		});
	},

	async update(id: number, data: SourceUpdateDto): Promise<void> {
		return apiRequest<void>(API_ENDPOINTS.sources.update(id), {
			method: 'PUT',
			body: JSON.stringify(data)
		});
	},

	async delete(id: number): Promise<void> {
		return apiRequest<void>(API_ENDPOINTS.sources.delete(id), {
			method: 'DELETE'
		});
	}
};
