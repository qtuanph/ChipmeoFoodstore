export interface PaymentSetting {
	id: number;
	bankName: string;
	accountNumber: string;
	accountName: string;
	qrCodeUrl?: string;
	isDefault: boolean;
	isActive: boolean;
	createdAt?: string;
	updatedAt?: string;
}

export interface PaymentSettingCreateDto {
	bankName: string;
	accountNumber: string;
	accountName: string;
	qrCodeUrl?: string;
	isDefault: boolean;
	isActive: boolean;
}

export interface PaymentSettingUpdateDto {
	bankName: string;
	accountNumber: string;
	accountName: string;
	qrCodeUrl?: string;
	isDefault: boolean;
	isActive: boolean;
}
