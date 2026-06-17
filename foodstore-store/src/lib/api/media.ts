import { api } from '$lib/api/utils.js';
import { API_ENDPOINTS } from '$lib/config/index.js';

export const mediaAPI = {
	getAll: (folder?: string) => api.get(API_ENDPOINTS.media + (folder ? `?folder=${folder}` : '')),
	delete: (id: number) => api.delete(`${API_ENDPOINTS.media}/${id}`),
	upload: (formData: FormData) => api.upload(`${API_ENDPOINTS.media}/upload`, formData)
};
