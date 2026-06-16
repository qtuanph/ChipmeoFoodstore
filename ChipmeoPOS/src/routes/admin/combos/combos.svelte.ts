import { combosAPI, menuItemsAPI } from '$lib/api/index.js';
import type { Combo, MenuItem } from '$lib/types/index.js';
import { API_BASE_URL, STORAGE_KEYS } from '$lib/config/index.js';
import { api } from '$lib/api/utils.js';

interface ComboFormData {
	name: string;
	comboPrice: number;
	description: string;
	imageUrl: string | null;
	isActive: boolean;
	items: { menuItemId: number; quantity: number }[];
}

export class CombosState {
	combos = $state<Combo[]>([]);
	menuItems = $state<MenuItem[]>([]);
	loading = $state(true);
	showModal = $state(false);
	editingItem = $state<Combo | null>(null);

	// Toast state
	showToast = $state(false);
	toastMessage = $state('');
	toastType = $state<'success' | 'error'>('success');

	formData = $state<ComboFormData>({
		name: '',
		comboPrice: 0,
		description: '',
		imageUrl: null,
		isActive: true,
		items: []
	});

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
			[this.combos, this.menuItems] = await Promise.all([
				combosAPI.getAll(),
				menuItemsAPI.getAll()
			]);
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
		this.formData = {
			name: '',
			comboPrice: 0,
			description: '',
			imageUrl: null,
			isActive: true,
			items: []
		};
		this.showModal = true;
	}

	openEditModal(item: Combo) {
		this.editingItem = item;
		this.pendingImageFile = null;
		this.imagePreview = null;
		this.formData = {
			name: item.name,
			comboPrice: item.comboPrice,
			description: item.description || '',
			imageUrl: item.imageUrl || null,
			isActive: item.isActive,
			items:
				item.items?.map((ci: any) => ({ menuItemId: ci.menuItemId, quantity: ci.quantity })) || []
		};
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
				uploadData.append('folder', 'combos');

				const result = await api.upload<{ fileUrl: string }>(
					`${API_BASE_URL}/api/media/upload`,
					uploadData
				);
				this.formData.imageUrl = result.fileUrl;
			}

			if (this.editingItem) {
				await combosAPI.update(this.editingItem.id, this.formData);

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
				this.showSuccess('Cập nhật combo thành công!');
			} else {
				await combosAPI.create(this.formData);
				this.showSuccess('Thêm combo thành công!');
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
			await combosAPI.delete(id);
			this.showSuccess('Xóa thành công!');
			await this.loadData();
		} catch (error: any) {
			this.showError('Lỗi xóa: ' + (error.message || ''));
		}
	}

	addComboItem() {
		this.formData.items = [
			...this.formData.items,
			{ menuItemId: this.menuItems[0]?.id || 0, quantity: 1 }
		];
	}

	removeComboItem(index: number) {
		this.formData.items = this.formData.items.filter((_, i) => i !== index);
	}

	updateComboItem(index: number, field: 'menuItemId' | 'quantity', value: number) {
		this.formData.items = this.formData.items.map((item, i) =>
			i === index ? { ...item, [field]: value } : item
		);
	}

	handleImageSelect(file: File) {
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
		this.pendingImageFile = new File([blob], 'combo.jpg', { type: 'image/jpeg' });
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
