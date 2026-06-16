import { apiRequest } from '$lib/api/utils.js';
import { API_ENDPOINTS } from '$lib/config/index.ts';
import type { Role, RoleCreateDto, RoleUpdateDto } from '$lib/types/index.ts';

export const rolesAPI = {
	async getAll(): Promise<Role[]> {
		return apiRequest<Role[]>(API_ENDPOINTS.roles.list);
	},

	async getById(id: number): Promise<Role> {
		return apiRequest<Role>(API_ENDPOINTS.roles.detail(id));
	},

	async create(data: RoleCreateDto): Promise<Role> {
		return apiRequest<Role>(API_ENDPOINTS.roles.create, {
			method: 'POST',
			body: JSON.stringify(data)
		});
	},

	async update(id: number, data: RoleUpdateDto): Promise<void> {
		return apiRequest<void>(API_ENDPOINTS.roles.update(id), {
			method: 'PUT',
			body: JSON.stringify(data)
		});
	},

	async delete(id: number): Promise<void> {
		return apiRequest<void>(API_ENDPOINTS.roles.delete(id), {
			method: 'DELETE'
		});
	},

	async assignPermissions(id: number, permissionIds: number[]): Promise<void> {
		return apiRequest<void>(API_ENDPOINTS.roles.assignPermissions(id), {
			method: 'POST',
			body: JSON.stringify({ permissionIds })
		});
	}
};
