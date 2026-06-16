import type { Category } from '$lib/types/index.js';
import {
	loadCategories,
	createEmptyFormData,
	createFormDataFromCategory,
	createCategory,
	updateCategory,
	deleteCategory,
	type CategoryFormData
} from '$lib/utils/index.js';
import { API_BASE_URL, STORAGE_KEYS } from '$lib/config/index.js';
import { api } from '$lib/api/utils.js';

export class CategoriesState {
	categories = $state<Category[]>([]);
	loading = $state(true);
	showModal = $state(false);
	editingItem = $state<Category | null>(null);

	// Toast state
	showToast = $state(false);
	toastMessage = $state('');
	toastType = $state<'success' | 'error'>('success');

	formData = $state<CategoryFormData>(createEmptyFormData());

	// Image state
	pendingImageFile = $state<File | null>(null);
	imagePreview = $state<string | null>(null);
	cropImageSrc = $state<string | null>(null);

	async init() {
		await this.loadData();
	}

	async loadData() {
		try {
			this.loading = true;
			this.categories = await loadCategories();
		} catch (error: any) {
			this.showError('Lỗi tải dữ liệu: ' + (error.message || ''));
		} finally {
			this.loading = false;
		}
	}

	openCreateModal() {
		this.editingItem = null;
		this.pendingImageFile = null;
		this.imagePreview = null;
		this.formData = createEmptyFormData();
		this.showModal = true;
	}

	openEditModal(item: Category) {
		this.editingItem = item;
		this.pendingImageFile = null;
		this.imagePreview = null;
		this.formData = createFormDataFromCategory(item);
		this.showModal = true;
	}

	async handleSubmit() {
		try {
			const token = localStorage.getItem(STORAGE_KEYS.TOKEN);
			const oldImageUrl = this.editingItem?.imageUrl;

			// Upload new image if pending
			if (this.pendingImageFile) {
				const uploadData = new FormData();
				uploadData.append('file', this.pendingImageFile);
				uploadData.append('folder', 'categories');

				const result = await api.upload<{ fileUrl: string }>(
					`${API_BASE_URL}/api/media/upload`,
					uploadData
				);
				this.formData.imageUrl = result.fileUrl;
			}

			if (this.editingItem) {
				await updateCategory(this.editingItem.id, this.formData);

				// Delete old image if changed
				if (oldImageUrl && this.formData.imageUrl !== oldImageUrl) {
					try {
						const url = new URL(oldImageUrl);
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
				this.showSuccess('Cập nhật danh mục thành công!');
			} else {
				await createCategory(this.formData);
				this.showSuccess('Thêm danh mục thành công!');
			}

			this.pendingImageFile = null;
			this.imagePreview = null;
			this.showModal = false;
			await this.loadData();
		} catch (error: any) {
			this.showError('Lỗi: ' + (error.message || ''));
		}
	}

	async handleDelete(id: number) {
		try {
			await deleteCategory(id);
			this.showSuccess('Xóa danh mục thành công!');
			await this.loadData();
		} catch (error: any) {
			this.showError('Lỗi xóa: ' + (error.message || ''));
		}
	}

	handleImageSelect(file: File) {
		// Read for cropper
		const reader = new FileReader();
		reader.onload = (e) => {
			if (e.target?.result) {
				this.cropImageSrc = e.target.result as string;
			}
		};
		reader.readAsDataURL(file);
	}

	handleCrop(blob: Blob) {
		this.cropImageSrc = null;
		this.pendingImageFile = new File([blob], 'category.jpg', { type: 'image/jpeg' });
		this.imagePreview = URL.createObjectURL(blob);
	}

	handleCancelCrop() {
		this.cropImageSrc = null;
	}

	clearImage() {
		this.pendingImageFile = null;
		this.imagePreview = null;
		this.formData.imageUrl = null;
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
