import { API_BASE_URL } from '$lib/config/index.js';
import { api } from '$lib/api/index.js';

export interface PaymentSetting {
	id: number;
	bankId: string;
	bankAccount: string;
	bankName: string;
	bankAccountName: string;
	template: string;
	isActive: boolean;
	isDefault: boolean;
}

export interface VietQRBank {
	id: number;
	name: string;
	code: string;
	bin: string;
	shortName: string;
	logo: string;
}

export class PaymentSettingsState {
	settings = $state<PaymentSetting[]>([]);
	banks = $state<VietQRBank[]>([]);
	loading = $state(false);
	loadingBanks = $state(false);
	showDialog = $state(false);
	editingId = $state<number | null>(null);

	formData = $state<Partial<PaymentSetting>>({
		bankId: '',
		bankAccount: '',
		bankName: '',
		bankAccountName: '',
		template: 'compact2',
		isActive: true,
		isDefault: false
	});

	async init() {
		await Promise.all([this.loadSettings(), this.loadBanks()]);
	}

	async loadBanks() {
		this.loadingBanks = true;
		try {
			const response = await fetch('https://api.vietqr.io/v2/banks');
			const data = await response.json();
			if (data.code === '00') {
				this.banks = data.data;
			}
		} catch (error) {
			console.error('Failed to load banks:', error);
		} finally {
			this.loadingBanks = false;
		}
	}

	async loadSettings() {
		this.loading = true;
		try {
			this.settings = await api.get<PaymentSetting[]>(`${API_BASE_URL}/admin/payment-settings/all`);
		} catch (error) {
			console.error('Failed to load payment settings:', error);
			alert('Lỗi khi tải danh sách tài khoản');
		} finally {
			this.loading = false;
		}
	}

	openCreateDialog() {
		this.editingId = null;
		this.formData = {
			bankId: '',
			bankAccount: '',
			bankName: '',
			bankAccountName: '',
			template: 'compact2',
			isActive: true,
			isDefault: false
		};
		this.showDialog = true;
	}

	openEditDialog(setting: PaymentSetting) {
		this.editingId = setting.id;
		this.formData = { ...setting };
		this.showDialog = true;
	}

	onBankSelect() {
		const selectedBank = this.banks.find((b) => b.bin === this.formData.bankId);
		if (selectedBank) {
			this.formData.bankName = selectedBank.shortName;
		}
	}

	async handleSave() {
		try {
			if (this.editingId) {
				await api.put(`${API_BASE_URL}/admin/payment-settings/${this.editingId}`, this.formData);
			} else {
				await api.post(`${API_BASE_URL}/admin/payment-settings`, this.formData);
			}
			this.showDialog = false;
			await this.loadSettings();
			// alert(this.editingId ? 'Cập nhật thành công!' : 'Thêm tài khoản thành công!');
		} catch (error) {
			console.error('Save failed:', error);
			alert('Lỗi khi lưu tài khoản');
		}
	}

	async toggleDefault(setting: PaymentSetting) {
		if (setting.isDefault) return; // Already default

		try {
			await api.put(`${API_BASE_URL}/admin/payment-settings/${setting.id}/set-default`, {});
			await this.loadSettings();
		} catch (error) {
			console.error('Set default failed:', error);
			alert('Lỗi khi đặt mặc định');
		}
	}

	async deleteSetting(id: number) {
		if (!confirm('Xóa tài khoản này? Không thể xóa tài khoản mặc định.')) return;

		try {
			await api.delete(`${API_BASE_URL}/admin/payment-settings/${id}`);
			await this.loadSettings();
			// alert('Đã xóa tài khoản!');
		} catch (error: any) {
			console.error('Delete failed:', error);
			alert(error.message || 'Lỗi khi xóa tài khoản');
		}
	}
}
