export interface Addon {
	id: number;
	name: string;
	price: number;
	isActive: boolean;
	createdAt?: string;
	updatedAt?: string;
}

export interface AddonCreateDto {
	name: string;
	price: number;
	isActive: boolean;
}

export interface AddonUpdateDto {
	name: string;
	price: number;
	isActive: boolean;
}
