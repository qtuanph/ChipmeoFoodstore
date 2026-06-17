import { apiRequest } from '$lib/api/utils.js';
import { API_ENDPOINTS } from '$lib/config/index.ts';
import type { Permission, Role } from '$lib/types/index.ts';

// Get all permissions grouped by module
export async function getPermissions(): Promise<Record<string, any[]>> {
	return apiRequest<Record<string, any[]>>(API_ENDPOINTS.permissions.list);
}

// Get all roles
export async function getRoles(): Promise<Role[]> {
	return apiRequest<Role[]>(API_ENDPOINTS.roles.list);
}

// Get permissions for a specific role
export async function getRolePermissions(roleId: number): Promise<number[]> {
	return apiRequest<number[]>(API_ENDPOINTS.roles.assignPermissions(roleId));
}

// Update role permissions (bulk)
export async function updateRolePermissions(
	roleId: number,
	permissionIds: number[]
): Promise<void> {
	return apiRequest<void>(API_ENDPOINTS.roles.assignPermissions(roleId), {
		method: 'PUT',
		body: JSON.stringify(permissionIds)
	});
}

// Permissions API object (for consistency)
export const permissionsAPI = {
	async getAll(): Promise<Permission[]> {
		const response = await apiRequest<Record<string, Permission[]>>(API_ENDPOINTS.permissions.list);
		return Object.values(response).flat();
	}
};
