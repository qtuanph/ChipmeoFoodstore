import { apiRequest } from '$lib/api/utils.js';
import { API_ENDPOINTS } from '$lib/config/index.ts';
import type { Employee, EmployeeCreateDto, EmployeeUpdateDto } from '$lib/types/index.ts';

export const employeesAPI = {
	async getAll(): Promise<Employee[]> {
		return apiRequest<Employee[]>(API_ENDPOINTS.employees.list);
	},

	async getById(id: number): Promise<Employee> {
		return apiRequest<Employee>(API_ENDPOINTS.employees.detail(id));
	},

	async create(data: EmployeeCreateDto): Promise<Employee> {
		return apiRequest<Employee>(API_ENDPOINTS.employees.create, {
			method: 'POST',
			body: JSON.stringify(data)
		});
	},

	async update(id: number, data: EmployeeUpdateDto): Promise<void> {
		return apiRequest<void>(API_ENDPOINTS.employees.update(id), {
			method: 'PUT',
			body: JSON.stringify(data)
		});
	},

	async delete(id: number): Promise<void> {
		return apiRequest<void>(API_ENDPOINTS.employees.delete(id), {
			method: 'DELETE'
		});
	}
};
