import { categoriesAPI, menuItemsAPI, addonsAPI } from '$lib/api/index.js';
import { categories } from '$lib/utils/index.js';
import type { MenuItem, Addon } from '$lib/types/index.js';
import { API_BASE_URL } from '$lib/config/index.js';
import { get } from 'svelte/store';

interface MenuFormData {
	name: string;
	price: number;
	categoryId: number;
	imageUrl: string | null;
	isActive: boolean;
	addonIds: number[];
}

export class MenuState {
	menuItems = $state<MenuItem[]>([]);
	allAddons = $state<Addon[]>([]);
	loading = $state(true);
	showModal = $state(false);
	editingItem = $state<MenuItem | null>(null);

	// Toast state
	showToast = $state(false);
	toastMessage = $state('');
	toastType = $state<'success' | 'error'>('success');

	formData = $state<MenuFormData>({
		name: '',
		price: 0,
		categoryId: 0,
		imageUrl: null,
		isActive: true,
		addonIds: []
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
			const [items, cats, addons] = await Promise.all([
				menuItemsAPI.getAll(),
				categoriesAPI.getAll(),
				addonsAPI.getAll()
			]);
			this.menuItems = items;
			categories.set(cats);
			this.allAddons = addons;
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
		const cats = get(categories);
		this.formData = {
			name: '',
			price: 0,
			categoryId: cats[0]?.id || 0,
			imageUrl: null,
			isActive: true,
			addonIds: []
		};
		this.showModal = true;
	}

	openEditModal(item: MenuItem) {
		this.editingItem = item;
		this.pendingImageFile = null;
		this.imagePreview = null;
		this.formData = {
			name: item.name,
			price: item.price,
			categoryId: item.categoryId || 0,
			imageUrl: item.imageUrl || null,
			isActive: item.isActive,
			addonIds: item.addons?.map((a) => a.id) || []
		};
		this.showModal = true;
	}

	async handleSubmit() {
		try {
			const token = localStorage.getItem('token');
			const oldImageUrl = this.editingItem?.imageUrl;

			// Upload new image if pending
			if (this.pendingImageFile) {
				const uploadData = new FormData();
				uploadData.append('file', this.pendingImageFile);
				uploadData.append('folder', 'menu-items');

				const uploadRes = await fetch(`${API_BASE_URL}/api/media/upload`, {
					method: 'POST',
					headers: { Authorization: `Bearer ${token}` },
					body: uploadData
				});

				if (!uploadRes.ok) throw new Error('Upload ảnh thất bại');
				const result = await uploadRes.json();
				this.formData.imageUrl = result.fileUrl;
			}

			if (this.editingItem) {
				await menuItemsAPI.update(this.editingItem.id, this.formData);

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
				this.showSuccess('Cập nhật món thành công!');
			} else {
				await menuItemsAPI.create(this.formData);
				this.showSuccess('Thêm món thành công!');
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
			await menuItemsAPI.delete(id);
			this.showSuccess('Xóa món thành công!');
			await this.loadData();
		} catch (error: any) {
			this.showError('Lỗi xóa: ' + (error.message || ''));
		}
	}

	getCategoryName(categoryId: number) {
		const cats = get(categories);
		return cats.find((c) => c.id === categoryId)?.name || 'N/A';
	}

	toggleAddon(addonId: number) {
		if (this.formData.addonIds.includes(addonId)) {
			this.formData.addonIds = this.formData.addonIds.filter((id) => id !== addonId);
		} else {
			this.formData.addonIds = [...this.formData.addonIds, addonId];
		}
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
		this.pendingImageFile = new File([blob], 'menu_item.jpg', { type: 'image/jpeg' });
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
