import { apiRequest } from '$lib/api/utils.js';
import { API_ENDPOINTS } from '$lib/config/index.ts';

import type { User } from '$lib/types/index.js';

export const authAPI = {
	async login(username: string, password: string): Promise<{ token: string; user: User }> {
		return apiRequest(`${API_ENDPOINTS.auth.login}`, {
			method: 'POST',
			body: JSON.stringify({ username, password })
		});
	},

	async logout(): Promise<void> {
		return apiRequest(`${API_ENDPOINTS.auth.logout}`, {
			method: 'POST'
		});
	},

	async getProfile(): Promise<User> {
		return apiRequest(`${API_ENDPOINTS.auth.base}/profile`);
	},

	async updateProfile(data: Partial<User>): Promise<User> {
		return apiRequest(`${API_ENDPOINTS.auth.base}/profile`, {
			method: 'PUT',
			body: JSON.stringify(data)
		});
	}
};
