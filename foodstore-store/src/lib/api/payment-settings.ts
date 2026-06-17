import { apiRequest } from '$lib/api/utils.js';
import { API_ENDPOINTS } from '$lib/config/index.ts';
import type {
	PaymentSetting,
	PaymentSettingCreateDto,
	PaymentSettingUpdateDto
} from '$lib/types/index.ts';

export const paymentSettingsAPI = {
	// Get default payment setting (for POS/public)
	async getDefault(): Promise<PaymentSetting> {
		return apiRequest<PaymentSetting>(API_ENDPOINTS.paymentSettings);
	},

	// Get all payment settings (admin only)
	async getAll(): Promise<PaymentSetting[]> {
		return apiRequest<PaymentSetting[]>(`${API_ENDPOINTS.paymentSettings}/all`);
	},

	// Get payment setting by ID
	async getById(id: number): Promise<PaymentSetting> {
		return apiRequest<PaymentSetting>(`${API_ENDPOINTS.paymentSettings}/${id}`);
	},

	// Create new payment setting
	async create(data: PaymentSettingCreateDto): Promise<PaymentSetting> {
		return apiRequest<PaymentSetting>(API_ENDPOINTS.paymentSettings, {
			method: 'POST',
			body: JSON.stringify(data)
		});
	},

	// Update payment setting
	async update(id: number, data: PaymentSettingUpdateDto): Promise<void> {
		return apiRequest<void>(`${API_ENDPOINTS.paymentSettings}/${id}`, {
			method: 'PUT',
			body: JSON.stringify(data)
		});
	},

	// Set as default
	async setDefault(id: number): Promise<void> {
		return apiRequest<void>(`${API_ENDPOINTS.paymentSettings}/${id}/set-default`, {
			method: 'PUT'
		});
	},

	// Delete payment setting
	async delete(id: number): Promise<void> {
		return apiRequest<void>(`${API_ENDPOINTS.paymentSettings}/${id}`, {
			method: 'DELETE'
		});
	}
};
