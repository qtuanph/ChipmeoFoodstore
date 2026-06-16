import { sourcesAPI } from '$lib/api/index.js';
import type { Source } from '$lib/types/index.js';

interface SourceFormData {
	name: string;
	isActive: boolean;
}

export class SourcesState {
	sources = $state<Source[]>([]);
	loading = $state(true);
	showModal = $state(false);
	editingItem = $state<Source | null>(null);

	// Toast state
	showToast = $state(false);
	toastMessage = $state('');
	toastType = $state<'success' | 'error'>('success');

	formData = $state<SourceFormData>({ name: '', isActive: true });

	async init() {
		await this.loadData();
	}

	async loadData() {
		try {
			this.loading = true;
			this.sources = await sourcesAPI.getAll();
		} catch (error: any) {
			this.showError('Lỗi tải dữ liệu: ' + (error.message || ''));
		} finally {
			this.loading = false;
		}
	}

	openCreateModal() {
		this.editingItem = null;
		this.formData = { name: '', isActive: true };
		this.showModal = true;
	}

	openEditModal(item: Source) {
		this.editingItem = item;
		this.formData = { name: item.name, isActive: item.isActive };
		this.showModal = true;
	}

	async handleSubmit() {
		try {
			if (this.editingItem) {
				await sourcesAPI.update(this.editingItem.id, this.formData);
				this.showSuccess('Cập nhật nguồn thành công!');
			} else {
				await sourcesAPI.create(this.formData);
				this.showSuccess('Thêm nguồn thành công!');
			}
			this.showModal = false;
			await this.loadData();
		} catch (error: any) {
			this.showError('Lỗi: ' + (error.message || ''));
		}
	}

	async handleDelete(id: number) {
		try {
			await sourcesAPI.delete(id);
			this.showSuccess('Xóa thành công!');
			await this.loadData();
		} catch (error: any) {
			this.showError('Lỗi xóa: ' + (error.message || ''));
		}
	}

	// Removed getStatusBadge as generic status is removed

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
