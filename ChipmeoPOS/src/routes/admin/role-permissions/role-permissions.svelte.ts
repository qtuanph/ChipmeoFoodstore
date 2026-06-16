import {
	getPermissions,
	getRolePermissions,
	updateRolePermissions,
	getRoles
} from '$lib/api/index.js';
import type { Role, Permission } from '$lib/types/index.js';

export class RolePermissionsState {
	roles = $state<Role[]>([]);
	permissionsByModule = $state<Record<string, Permission[]>>({});
	selectedRoleId = $state<number | null>(null);
	rolePermissionIds = $state<number[]>([]);
	loading = $state(false);
	saving = $state(false);

	// Permission Templates
	permissionTemplates = {
		admin: {
			name: 'Quản trị viên (Super Admin)',
			description: 'Tự động chọn TẤT CẢ các quyền (bao gồm cả quyền mới)',
			permissions: [] // Special case handled in applyTemplate
		},
		cashier: {
			name: 'Thu ngân (POS)',
			description: 'Tạo đơn, xem menu, quản lý đơn hàng',
			permissions: [
				'order.view',
				'order.create',
				'order.update',
				'menu.view',
				'addon.view',
				'combo.view',
				'table.view',
				'discount.view',
				'customer.view',
				'customer.create'
			]
		},
		kitchen: {
			name: 'Bếp',
			description: 'Xem đơn hàng và menu',
			permissions: ['order.view', 'order.update', 'menu.view']
		}
	};

	get selectedRole() {
		return this.roles.find((r) => r.id === this.selectedRoleId) || null;
	}

	async init() {
		await this.loadData();
	}

	async loadData() {
		this.loading = true;
		try {
			const [rolesData, permissionsData] = await Promise.all([getRoles(), getPermissions()]);
			this.roles = rolesData;
			this.permissionsByModule = permissionsData;
			if (this.roles.length > 0) {
				this.selectRole(this.roles[0].id);
			}
		} catch (error) {
			console.error('Failed to load data:', error);
		} finally {
			this.loading = false;
		}
	}

	async selectRole(roleId: number) {
		this.selectedRoleId = roleId;
		try {
			this.rolePermissionIds = await getRolePermissions(roleId);
		} catch (error) {
			console.error('Failed to load role permissions:', error);
		}
	}

	togglePermission(permissionId: number) {
		if (this.rolePermissionIds.includes(permissionId)) {
			this.rolePermissionIds = this.rolePermissionIds.filter((id) => id !== permissionId);
		} else {
			this.rolePermissionIds = [...this.rolePermissionIds, permissionId];
		}
	}

	toggleModule(moduleName: string) {
		const modulePermissions = this.permissionsByModule[moduleName];
		const modulePermissionIds = modulePermissions.map((p) => p.id);
		const allSelected = modulePermissionIds.every((id) => this.rolePermissionIds.includes(id));

		if (allSelected) {
			// Deselect all
			this.rolePermissionIds = this.rolePermissionIds.filter(
				(id) => !modulePermissionIds.includes(id)
			);
		} else {
			// Select all
			const newIds = modulePermissionIds.filter((id) => !this.rolePermissionIds.includes(id));
			this.rolePermissionIds = [...this.rolePermissionIds, ...newIds];
		}
	}

	async savePermissions() {
		if (!this.selectedRoleId) return;

		this.saving = true;
		try {
			await updateRolePermissions(this.selectedRoleId, this.rolePermissionIds);
			alert('Cập nhật quyền thành công!');
		} catch (error) {
			console.error('Failed to save permissions:', error);
			alert('Lỗi khi cập nhật quyền');
		} finally {
			this.saving = false;
		}
	}

	getModuleProgress(moduleName: string): string {
		const modulePermissions = this.permissionsByModule[moduleName];
		const selected = modulePermissions.filter((p) => this.rolePermissionIds.includes(p.id)).length;
		return `${selected}/${modulePermissions.length}`;
	}

	isModuleFullySelected(moduleName: string): boolean {
		const modulePermissions = this.permissionsByModule[moduleName];
		return modulePermissions.every((p) => this.rolePermissionIds.includes(p.id));
	}

	applyTemplate(templateKey: keyof typeof this.permissionTemplates) {
		const allPermissions = Object.values(this.permissionsByModule).flat();

		// Special logic for Admin: Select ALL permissions
		if (templateKey === 'admin') {
			this.rolePermissionIds = allPermissions.map((p) => p.id);
			return;
		}

		const template = this.permissionTemplates[templateKey];

		// Get permission IDs based on codes
		const templatePermissionIds = template.permissions
			.map((code) => allPermissions.find((p) => p.code === code)?.id)
			.filter((id): id is number => id !== undefined);

		this.rolePermissionIds = templatePermissionIds;
	}
}
