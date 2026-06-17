export interface Employee {
	id: number;
	fullName: string;
	username: string;
	email?: string;
	phone?: string;
	avatarUrl?: string | null;
	roleId: number;
	roleName?: string;
	isActive: boolean;
	createdAt?: string;
	updatedAt?: string;
}

export interface EmployeeCreateDto {
	fullName: string;
	username: string;
	password: string;
	email?: string;
	phone?: string;
	avatarUrl?: string | null;
	roleId: number;
	isActive: boolean;
}

export interface EmployeeUpdateDto {
	fullName: string;
	username: string;
	email?: string;
	phone?: string;
	avatarUrl?: string | null;
	roleId: number;
	isActive: boolean;
}
