export interface Customer {
	id: number;
	fullName: string;
	phone?: string;
	email: string;
	avatarUrl?: string;
	points: number;
	isActive: boolean;
	createdAt?: string;
}

export interface CustomerRegisterDto {
	fullName: string;
	email: string;
	password: string;
	phone?: string;
}

export interface CustomerLoginDto {
	email: string;
	password: string;
}

export interface CustomerLoginResult {
	customer: Customer;
	token: string;
}

export interface CustomerUpdateDto {
	fullName?: string;
	phone?: string;
	avatarUrl?: string;
}
