import { discountsAPI } from '$lib/api/index.js';
import type { Discount } from '$lib/types/index.js';

interface DiscountFormData {
	code: string;
	name: string;
	type: 'percent' | 'amount';
	value: number;
	maxDiscountAmount: number;
	minOrderAmount: number;
	usageLimit: number;
	startDate: string;
	endDate: string;
	isActive: boolean;
}

export class DiscountsState {
	discounts = $state<Discount[]>([]);
	loading = $state(true);
	showModal = $state(false);
	editingItem = $state<Discount | null>(null);

	// Toast state
	showToast = $state(false);
	toastMessage = $state('');
	toastType = $state<'success' | 'error'>('success');

	formData = $state<DiscountFormData>({
		code: '',
		name: '',
		type: 'percent',
		value: 0,
		maxDiscountAmount: 0,
		minOrderAmount: 0,
		usageLimit: 0,
		startDate: '',
		endDate: '',
		isActive: true
	});

	async init() {
		await this.loadData();
	}

	async loadData() {
		try {
			this.loading = true;
			this.discounts = await discountsAPI.getAll();
		} catch (error: any) {
			this.showError('Lỗi tải dữ liệu: ' + (error.message || ''));
		} finally {
			this.loading = false;
		}
	}

	openCreateModal() {
		this.editingItem = null;
		this.formData = {
			code: '',
			name: '',
			type: 'percent',
			value: 0,
			maxDiscountAmount: 0,
			minOrderAmount: 0,
			usageLimit: 0,
			startDate: '',
			endDate: '',
			isActive: true
		};
		this.showModal = true;
	}

	openEditModal(item: Discount) {
		this.editingItem = item;
		this.formData = {
			code: item.code,
			name: item.name,
			type: item.type,
			value: item.value,
			maxDiscountAmount: item.maxDiscountAmount || 0,
			minOrderAmount: item.minOrderAmount || 0,
			usageLimit: item.usageLimit || 0,
			startDate: item.startDate ? new Date(item.startDate).toISOString().slice(0, 16) : '',
			endDate: item.endDate ? new Date(item.endDate).toISOString().slice(0, 16) : '',
			isActive: item.isActive
		};
		this.showModal = true;
	}

	async handleSubmit() {
		try {
			const payload = {
				...this.formData,
				usageLimit: this.formData.usageLimit || null,
				startDate: this.formData.startDate || null,
				endDate: this.formData.endDate || null,
				maxDiscountAmount: this.formData.maxDiscountAmount || null,
				minOrderAmount: this.formData.minOrderAmount || null
			};

			if (this.editingItem) {
				await discountsAPI.update(this.editingItem.id, payload);
				this.showSuccess('Cập nhật mã giảm giá thành công!');
			} else {
				await discountsAPI.create(payload);
				this.showSuccess('Thêm mã giảm giá thành công!');
			}
			this.showModal = false;
			await this.loadData();
		} catch (error: any) {
			this.showError('Lỗi: ' + (error.message || ''));
		}
	}

	async handleDelete(id: number) {
		try {
			await discountsAPI.delete(id);
			this.showSuccess('Xóa thành công!');
			await this.loadData();
		} catch (error: any) {
			this.showError('Lỗi xóa: ' + (error.message || ''));
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
