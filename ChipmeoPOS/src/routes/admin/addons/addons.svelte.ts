import { addonsAPI } from '$lib/api/index.js';
import type { Addon } from '$lib/types/index.js';

export class AddonsState {
	addons = $state<Addon[]>([]);
	loading = $state(true);
	showModal = $state(false);
	editingItem = $state<Addon | null>(null);

	// Toast state
	showToast = $state(false);
	toastMessage = $state('');
	toastType = $state<'success' | 'error'>('success');

	formData = $state({ name: '', price: 0, isActive: true });

	async init() {
		await this.loadData();
	}

	async loadData() {
		try {
			this.loading = true;
			this.addons = await addonsAPI.getAll();
		} catch (error: any) {
			this.showError('Lỗi tải dữ liệu: ' + error.message);
		} finally {
			this.loading = false;
		}
	}

	openCreateModal() {
		this.editingItem = null;
		this.formData = { name: '', price: 0, isActive: true };
		this.showModal = true;
	}

	openEditModal(item: Addon) {
		this.editingItem = item;
		this.formData = { name: item.name, price: item.price, isActive: item.isActive };
		this.showModal = true;
	}

	async handleSubmit() {
		try {
			if (this.editingItem) {
				await addonsAPI.update(this.editingItem.id, this.formData);
				this.showSuccess('Cập nhật topping thành công!');
			} else {
				await addonsAPI.create(this.formData);
				this.showSuccess('Thêm topping thành công!');
			}
			this.showModal = false;
			await this.loadData();
		} catch (error: any) {
			this.showError('Lỗi: ' + error.message);
		}
	}

	async handleDelete(id: number) {
		try {
			await addonsAPI.delete(id);
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
