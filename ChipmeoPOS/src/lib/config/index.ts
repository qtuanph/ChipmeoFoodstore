// API configuration - Backend endpoints
import { DEV } from 'esm-env';
export const isDev = DEV;

// Determine API URL based on environment and hostname
export const API_BASE_URL = (() => {
	// 1. Local Development
	if (isDev) return 'http://localhost:5142';

	if (typeof window !== 'undefined') {
		const host = window.location.hostname;

		// 2. Frontend Vercel (Demo) -> Backend Demo (Tunnel)
		if (host === 'foodchipmeo.vercel.app') return 'https://demo.chipmeo.io.vn';

		// 3. Frontend Production -> Backend Production
		// Khi deploy lên food.chipmeo.io.vn, code này sẽ tự động trỏ về api.chipmeo.io.vn
		// Bạn KHÔNG CẦN sửa gì cả nếu domain đúng như này.
		if (host === 'food.chipmeo.io.vn') return 'https://api.chipmeo.io.vn';

		// 4. Frontend Tunnel Dev (nếu bạn chạy dev qua tunnel) -> Proxy
		if (host === 'demo.chipmeo.io.vn') return '';
	}

	// Default Fallback (Production)
	// Nếu không khớp case nào ở trên, mặc định trỏ về Production API
	return 'https://api.chipmeo.io.vn';
})();

// API Endpoints matching backend controllers
export const API_ENDPOINTS = {
	// Auth
	auth: {
		base: `${API_BASE_URL}/api/auth`,
		login: `${API_BASE_URL}/api/auth/login`,
		logout: `${API_BASE_URL}/api/auth/logout`,
		refresh: `${API_BASE_URL}/api/auth/refresh`
	},
	// POS/Orders
	orders: {
		list: `${API_BASE_URL}/admin/orders`,
		create: `${API_BASE_URL}/pos/orders`, // POS uses this
		detail: (id: number) => `${API_BASE_URL}/admin/orders/${id}`,
		update: (id: number) => `${API_BASE_URL}/admin/orders/${id}`,
		updateStatus: (id: number) => `${API_BASE_URL}/admin/orders/${id}/status`, // Default to admin
		setUnpaid: (id: number) => `${API_BASE_URL}/admin/orders/${id}/set-unpaid`,
		byStatus: (status: string) => `${API_BASE_URL}/admin/orders/status/${status}`,
		payment: (id: number) => `${API_BASE_URL}/pos/orders/${id}/payment`,
		paged: `${API_BASE_URL}/admin/orders/paged`,
		delete: (id: number) => `${API_BASE_URL}/admin/orders/${id}`
	},
	posOrders: {
		create: `${API_BASE_URL}/pos/orders`,
		updateStatus: (id: number) => `${API_BASE_URL}/pos/orders/${id}/status`
	},
	pos: {
		menu: `${API_BASE_URL}/pos/menu`,
		tables: `${API_BASE_URL}/pos/tables`,
		addons: `${API_BASE_URL}/pos/addons`
	},
	// Admin endpoints
	categories: {
		list: `${API_BASE_URL}/admin/categories`,
		create: `${API_BASE_URL}/admin/categories`,
		detail: (id: number) => `${API_BASE_URL}/admin/categories/${id}`,
		update: (id: number) => `${API_BASE_URL}/admin/categories/${id}`,
		delete: (id: number) => `${API_BASE_URL}/admin/categories/${id}`
	},
	menuItems: {
		list: `${API_BASE_URL}/admin/menuitems`,
		create: `${API_BASE_URL}/admin/menuitems`,
		detail: (id: number) => `${API_BASE_URL}/admin/menuitems/${id}`,
		update: (id: number) => `${API_BASE_URL}/admin/menuitems/${id}`,
		delete: (id: number) => `${API_BASE_URL}/admin/menuitems/${id}`
	},
	addons: {
		list: `${API_BASE_URL}/admin/addons`,
		create: `${API_BASE_URL}/admin/addons`,
		detail: (id: number) => `${API_BASE_URL}/admin/addons/${id}`,
		update: (id: number) => `${API_BASE_URL}/admin/addons/${id}`,
		delete: (id: number) => `${API_BASE_URL}/admin/addons/${id}`
	},
	sources: {
		list: `${API_BASE_URL}/admin/sources`,
		create: `${API_BASE_URL}/admin/sources`,
		detail: (id: number) => `${API_BASE_URL}/admin/sources/${id}`,
		update: (id: number) => `${API_BASE_URL}/admin/sources/${id}`,
		delete: (id: number) => `${API_BASE_URL}/admin/sources/${id}`
	},
	combos: {
		list: `${API_BASE_URL}/admin/combos`,
		create: `${API_BASE_URL}/admin/combos`,
		detail: (id: number) => `${API_BASE_URL}/admin/combos/${id}`,
		update: (id: number) => `${API_BASE_URL}/admin/combos/${id}`,
		delete: (id: number) => `${API_BASE_URL}/admin/combos/${id}`
	},
	discounts: {
		list: `${API_BASE_URL}/admin/discounts`,
		create: `${API_BASE_URL}/admin/discounts`,
		detail: (id: number) => `${API_BASE_URL}/admin/discounts/${id}`,
		update: (id: number) => `${API_BASE_URL}/admin/discounts/${id}`,
		delete: (id: number) => `${API_BASE_URL}/admin/discounts/${id}`
	},
	employees: {
		list: `${API_BASE_URL}/admin/employees`,
		create: `${API_BASE_URL}/admin/employees`,
		detail: (id: number) => `${API_BASE_URL}/admin/employees/${id}`,
		update: (id: number) => `${API_BASE_URL}/admin/employees/${id}`,
		delete: (id: number) => `${API_BASE_URL}/admin/employees/${id}`
	},
	roles: {
		list: `${API_BASE_URL}/admin/roles`,
		create: `${API_BASE_URL}/admin/roles`,
		detail: (id: number) => `${API_BASE_URL}/admin/roles/${id}`,
		update: (id: number) => `${API_BASE_URL}/admin/roles/${id}`,
		delete: (id: number) => `${API_BASE_URL}/admin/roles/${id}`,
		assignPermissions: (id: number) => `${API_BASE_URL}/admin/roles/${id}/permissions`
	},
	permissions: {
		list: `${API_BASE_URL}/admin/permissions`
	},
	reports: {
		dashboardStats: `${API_BASE_URL}/admin/dashboard/stats`,
		analytics: `${API_BASE_URL}/admin/dashboard/analytics`,
		forecast: `${API_BASE_URL}/admin/dashboard/forecast`,
		recommendations: `${API_BASE_URL}/admin/dashboard/recommendations`
	},
	paymentSettings: `${API_BASE_URL}/admin/payment-settings`,
	kitchen: {
		list: `${API_BASE_URL}/api/kitchen/orders`,
		start: (id: number) => `${API_BASE_URL}/api/kitchen/orders/${id}/start`,
		complete: (id: number) => `${API_BASE_URL}/api/kitchen/orders/${id}/complete`
	},
	// Blog (Public + Admin)
	blog: `${API_BASE_URL}/api/blog`,
	// Customers (Public auth + Admin management)
	customers: `${API_BASE_URL}/api/customers`,
	// Media
	media: `${API_BASE_URL}/api/media`
};
