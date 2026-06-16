import { employeesAPI, rolesAPI } from '$lib/api/index.js';
import type { Employee, Role } from '$lib/types/index.js';
import { API_BASE_URL } from '$lib/config/index.js';

export class EmployeesState {
	employees = $state<Employee[]>([]);
	roles = $state<Role[]>([]);
	loading = $state(true);
	showModal = $state(false);
	editingItem = $state<Employee | null>(null);

	// Toast state
	showToast = $state(false);
	toastMessage = $state('');
	toastType = $state<'success' | 'error'>('success');

	formData = $state({
		fullName: '',
		username: '',
		password: '',
		email: '',
		phone: '',
		avatarUrl: null as string | null,
		roleId: 0,
		isActive: true
	});

	// Image state
	pendingImageFile = $state<File | null>(null);
	imagePreview = $state<string | null>(null);

	async init() {
		await this.loadData();
	}

	async loadData() {
		try {
			this.loading = true;
			[this.employees, this.roles] = await Promise.all([employeesAPI.getAll(), rolesAPI.getAll()]);
		} catch (error: any) {
			this.showError('Lỗi tải dữ liệu: ' + error.message);
		} finally {
			this.loading = false;
		}
	}

	openCreateModal() {
		this.editingItem = null;
		this.pendingImageFile = null;
		this.imagePreview = null;
		this.formData = {
			fullName: '',
			username: '',
			password: '',
			email: '',
			phone: '',
			avatarUrl: null,
			roleId: this.roles[0]?.id || 0,
			isActive: true
		};
		this.showModal = true;
	}

	openEditModal(item: Employee) {
		this.editingItem = item;
		this.pendingImageFile = null;
		this.imagePreview = null;
		this.formData = {
			fullName: item.fullName,
			username: item.username,
			password: '',
			email: item.email || '',
			phone: item.phone || '',
			avatarUrl: item.avatarUrl || null,
			roleId: item.roleId,
			isActive: item.isActive
		};
		this.showModal = true;
	}

	async handleSubmit() {
		try {
			const token = localStorage.getItem('token');
			const oldAvatarUrl = this.editingItem?.avatarUrl;

			// Upload new avatar if pending
			if (this.pendingImageFile) {
				const uploadData = new FormData();
				uploadData.append('file', this.pendingImageFile);
				uploadData.append('folder', 'avatars');

				const uploadRes = await fetch(`${API_BASE_URL}/api/media/upload`, {
					method: 'POST',
					headers: { Authorization: `Bearer ${token}` },
					body: uploadData
				});

				if (!uploadRes.ok) throw new Error('Upload avatar thất bại');
				const result = await uploadRes.json();
				this.formData.avatarUrl = result.fileUrl;
			}

			if (this.editingItem) {
				await employeesAPI.update(this.editingItem.id, this.formData);

				// Delete old avatar if changed
				if (oldAvatarUrl && this.formData.avatarUrl !== oldAvatarUrl) {
					try {
						const url = new URL(oldAvatarUrl);
						const parts = url.pathname.split('/');
						const folder = parts[parts.length - 2];
						const filename = parts[parts.length - 1];
						await fetch(`${API_BASE_URL}/api/media/${folder}/${filename}`, {
							method: 'DELETE',
							headers: { Authorization: `Bearer ${token}` }
						});
					} catch (e) {
						/* ignore delete errors */
					}
				}
				this.showSuccess('Cập nhật nhân viên thành công!');
			} else {
				await employeesAPI.create(this.formData);
				this.showSuccess('Thêm nhân viên thành công!');
			}

			this.pendingImageFile = null;
			this.imagePreview = null;
			this.showModal = false;
			await this.loadData();
		} catch (error: any) {
			this.showError('Lỗi: ' + error.message);
		}
	}

	async handleDelete(id: number) {
		try {
			await employeesAPI.delete(id);
			this.showSuccess('Xóa thành công!');
			await this.loadData();
		} catch (error: any) {
			this.showError('Lỗi xóa: ' + error.message);
		}
	}

	handleAvatarSelect(file: File) {
		this.pendingImageFile = file;
		this.imagePreview = URL.createObjectURL(file);
	}

	clearAvatar() {
		this.pendingImageFile = null;
		this.imagePreview = null;
		this.formData.avatarUrl = null;
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
