export interface User {
	id: number;
	username: string;
	fullName: string;
	roleId: number;
	roleName: string;
	defaultRoute?: string;
	permissions: string[];
	avatarUrl?: string;
	email?: string;
	phone?: string;
}

export interface AuthState {
	isAuthenticated: boolean;
	user: User | null;
	token: string | null;
	loading: boolean;
	error: string | null;
}
