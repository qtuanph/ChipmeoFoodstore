export interface Discount {
	id: number;
	code: string;
	name: string;
	description?: string;
	type: 'percent' | 'amount';
	value: number;
	maxDiscountAmount?: number;
	minOrderAmount?: number;
	usageLimit?: number;
	usedCount?: number;
	startDate?: string;
	endDate?: string;
	isActive: boolean;
	createdAt?: string;
	updatedAt?: string;
}

export interface DiscountCreateDto {
	code: string;
	name: string;
	description?: string;
	type: 'percent' | 'amount';
	value: number;
	maxDiscountAmount?: number | null;
	minOrderAmount?: number | null;
	usageLimit?: number | null;
	startDate?: string | null;
	endDate?: string | null;
	isActive: boolean;
}

export interface DiscountUpdateDto {
	code: string;
	name: string;
	description?: string;
	type: 'percent' | 'amount';
	value: number;
	maxDiscountAmount?: number | null;
	minOrderAmount?: number | null;
	usageLimit?: number | null;
	startDate?: string | null;
	endDate?: string | null;
	isActive: boolean;
}
