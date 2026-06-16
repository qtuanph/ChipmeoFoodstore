export interface Combo {
	id: number;
	name: string;
	description?: string;
	imageUrl?: string | null;
	comboPrice: number;
	items: ComboItem[];
	isActive: boolean;
	createdAt?: string;
	updatedAt?: string;
}

export interface ComboItem {
	menuItemId: number;
	quantity: number;
}

export interface ComboCreateDto {
	name: string;
	description?: string;
	imageUrl?: string | null;
	comboPrice: number;
	items: ComboItem[];
	isActive: boolean;
}

export interface ComboUpdateDto {
	name: string;
	description?: string;
	imageUrl?: string | null;
	comboPrice: number;
	items: ComboItem[];
	isActive: boolean;
}
