import { apiRequest } from '$lib/api/utils.js';
import { API_ENDPOINTS } from '$lib/config/index.ts';

import type {
	Customer,
	CustomerRegisterDto,
	CustomerLoginDto,
	CustomerLoginResult,
	CustomerUpdateDto
} from '$lib/types/index.js';

export type { Customer };

export const customerAPI = {
	async register(data: CustomerRegisterDto): Promise<Customer> {
		return apiRequest(`${API_ENDPOINTS.customers}/register`, {
			method: 'POST',
			body: JSON.stringify(data)
		});
	},

	async login(data: CustomerLoginDto): Promise<CustomerLoginResult> {
		return apiRequest(`${API_ENDPOINTS.customers}/login`, {
			method: 'POST',
			body: JSON.stringify(data)
		});
	},

	async getProfile(): Promise<Customer> {
		return apiRequest(`${API_ENDPOINTS.customers}/me`);
	},

	async updateProfile(data: CustomerUpdateDto): Promise<Customer> {
		return apiRequest(`${API_ENDPOINTS.customers}/me`, {
			method: 'PUT',
			body: JSON.stringify(data)
		});
	},

	async getAllCustomers(): Promise<Customer[]> {
		return apiRequest(API_ENDPOINTS.customers);
	},

	async getCustomerById(id: number): Promise<Customer> {
		return apiRequest(`${API_ENDPOINTS.customers}/${id}`);
	},

	async lookupByPhone(phone: string): Promise<Customer> {
		return apiRequest(`${API_ENDPOINTS.customers}/lookup/${phone}`);
	}
};
