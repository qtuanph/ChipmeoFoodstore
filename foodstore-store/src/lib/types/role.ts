export interface Role {
	id: number;
	name: string;
	description?: string;
	defaultRoute?: string;
	isActive: boolean;
	createdAt?: string;
	updatedAt?: string;
}

export interface RoleCreateDto {
	name: string;
	description?: string;
	defaultRoute?: string;
	isActive: boolean;
}

export interface RoleUpdateDto {
	name: string;
	description?: string;
	defaultRoute?: string;
	isActive: boolean;
}
