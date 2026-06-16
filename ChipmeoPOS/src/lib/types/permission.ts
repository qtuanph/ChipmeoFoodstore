export interface Permission {
	id: number;
	code: string;
	name: string;
	description?: string;
	module: string;
	createdAt?: string;
	updatedAt?: string;
}

export interface PermissionsByModule {
	[module: string]: Permission[];
}
