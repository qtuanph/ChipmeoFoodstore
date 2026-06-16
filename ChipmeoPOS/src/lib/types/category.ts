export interface Category {
	id: number;
	name: string;
	description?: string;
	imageUrl?: string | null;
	isActive: boolean;
	createdAt?: string;
	updatedAt?: string;
}

export interface CategoryCreateDto {
	name: string;
	description?: string;
	imageUrl?: string | null;
	isActive: boolean;
}

export interface CategoryUpdateDto {
	name: string;
	description?: string;
	imageUrl?: string | null;
	isActive: boolean;
}
