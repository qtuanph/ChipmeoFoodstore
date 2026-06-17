// ============================================
// CENTRALIZED API EXPORTS
// ============================================
// All API modules in one place - import from '$lib/api'

export { API_BASE_URL, API_ENDPOINTS } from '$lib/config/index.js';
export { apiRequest, api } from '$lib/api/utils.js';

// Auth
// Auth
export { authAPI } from '$lib/api/auth.js';
export { customerAPI } from '$lib/api/customer.js';

// Admin - Menu Management
export { categoriesAPI } from '$lib/api/categories.js';
export { menuItemsAPI } from '$lib/api/menu.js';
export { addonsAPI } from '$lib/api/addons.js';
export { combosAPI } from '$lib/api/combos.js';

// Admin - Operations
export { sourcesAPI } from '$lib/api/sources.js';
export { ordersAPI } from '$lib/api/orders.js';
export { discountsAPI } from '$lib/api/discounts.js';

// Admin - System
export { employeesAPI } from '$lib/api/employees.js';
export { rolesAPI } from '$lib/api/roles.js';
export {
	getPermissions,
	getRoles,
	getRolePermissions,
	updateRolePermissions,
	permissionsAPI
} from '$lib/api/permissions.js';
export { paymentSettingsAPI } from '$lib/api/payment-settings.js';

// POS
export { posAPI } from '$lib/api/pos.js';

// Reports
export { reportsAPI } from '$lib/api/reports.js';

// Media
export { mediaAPI } from '$lib/api/media.js';
