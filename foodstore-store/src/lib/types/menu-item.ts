export interface MenuItem {
	id: number;
	name: string;
	description?: string;
	price: number;
	categoryId: number;
	imageUrl?: string | null;
	isActive: boolean;
	createdAt?: string;
	updatedAt?: string;
	addons?: import('$lib/types/index.ts').Addon[];
}

export interface MenuItemCreateDto {
	name: string;
	description?: string;
	price: number;
	categoryId: number;
	imageUrl?: string | null;
	isActive: boolean;
	addonIds?: number[];
}

export interface MenuItemUpdateDto {
	name: string;
	description?: string;
	price: number;
	categoryId: number;
	imageUrl?: string | null;
	isActive: boolean;
	addonIds?: number[];
}
