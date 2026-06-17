import { rolesAPI, permissionsAPI } from '$lib/api/index.js';
import type { Role, Permission } from '$lib/types/index.js';

export class RolesState {
	roles = $state<Role[]>([]);
	permissions = $state<Permission[]>([]);
	loading = $state(true);
	showModal = $state(false);
	editingItem = $state<Role | null>(null);

	// Toast state
	showToast = $state(false);
	toastMessage = $state('');
	toastType = $state<'success' | 'error'>('success');

	formData = $state({
		name: '',
		description: '',
		defaultRoute: '/admin',
		isActive: true
	});

	async init() {
		await this.loadData();
	}

	async loadData() {
		try {
			this.loading = true;
			[this.roles, this.permissions] = await Promise.all([
				rolesAPI.getAll(),
				permissionsAPI.getAll()
			]);
		} catch (error: any) {
			this.showError('Lỗi tải dữ liệu: ' + error.message);
		} finally {
			this.loading = false;
		}
	}

	openCreateModal() {
		this.editingItem = null;
		this.formData = { name: '', description: '', defaultRoute: '/admin', isActive: true };
		this.showModal = true;
	}

	openEditModal(item: Role) {
		this.editingItem = item;
		this.formData = {
			name: item.name,
			description: item.description || '',
			defaultRoute: item.defaultRoute || '/admin',
			isActive: item.isActive
		};
		this.showModal = true;
	}

	async handleSubmit() {
		try {
			if (this.editingItem) {
				await rolesAPI.update(this.editingItem.id, this.formData);
				this.showSuccess('Cập nhật vai trò thành công!');
			} else {
				await rolesAPI.create(this.formData);
				this.showSuccess('Thêm vai trò thành công!');
			}
			this.showModal = false;
			await this.loadData();
		} catch (error: any) {
			this.showError('Lỗi: ' + error.message);
		}
	}

	async handleDelete(id: number) {
		// if (!confirm('Bạn có chắc muốn xóa?')) return;
		try {
			await rolesAPI.delete(id);
			this.showSuccess('Xóa thành công!');
			await this.loadData();
		} catch (error: any) {
			this.showError('Lỗi xóa: ' + error.message);
		}
	}

	private showSuccess(message: string) {
		this.toastMessage = message;
		this.toastType = 'success';
		this.showToast = true;
	}

	private showError(message: string) {
		this.toastMessage = message;
		this.toastType = 'error';
		this.showToast = true;
	}
}
