import { apiRequest } from '$lib/api/utils.js';
import { API_ENDPOINTS } from '$lib/config/index.ts';
import type { DashboardStats, Analytics, Forecast, Recommendation } from '$lib/types/index.ts';

export const reportsAPI = {
	async getDashboardStats(): Promise<DashboardStats> {
		return apiRequest<DashboardStats>(API_ENDPOINTS.reports.dashboardStats);
	},

	async getAnalytics(
		fromDate?: string,
		toDate?: string,
		groupBy: 'day' | 'month' = 'day'
	): Promise<Analytics> {
		const params = new URLSearchParams();
		if (fromDate) params.append('fromDate', fromDate);
		if (toDate) params.append('toDate', toDate);
		params.append('groupBy', groupBy);
		return apiRequest<Analytics>(API_ENDPOINTS.reports.analytics + '?' + params.toString());
	},

	async getForecast(horizon: number = 7): Promise<Forecast> {
		return apiRequest<Forecast>(API_ENDPOINTS.reports.forecast + `?horizon=${horizon}`);
	},

	async getRecommendations(): Promise<Recommendation[]> {
		return apiRequest<Recommendation[]>(API_ENDPOINTS.reports.recommendations);
	}
};
