import type { Category } from '$lib/types/index.ts';
import { categoriesAPI } from '$lib/api/index.ts';

export interface CategoryFormData {
	name: string;
	description: string;
	imageUrl?: string | null;
	isActive: boolean;
}

export interface CategoryState {
	categories: Category[];
	loading: boolean;
	showModal: boolean;
	editingItem: Category | null;
	formData: CategoryFormData;
}

export interface ToastState {
	show: boolean;
	message: string;
	type: 'success' | 'error';
}

/**
 * Load all categories from API
 */
export async function loadCategories(): Promise<Category[]> {
	return await categoriesAPI.getAll();
}

/**
 * Create empty form data
 */
export function createEmptyFormData(): CategoryFormData {
	return {
		name: '',
		description: '',
		imageUrl: null,
		isActive: true
	};
}

/**
 * Create form data from existing category
 */
export function createFormDataFromCategory(category: Category): CategoryFormData {
	return {
		name: category.name,
		description: category.description || '',
		imageUrl: category.imageUrl,
		isActive: category.isActive
	};
}

/**
 * Create a new category
 */
export async function createCategory(formData: CategoryFormData): Promise<void> {
	await categoriesAPI.create(formData);
}

/**
 * Update an existing category
 */
export async function updateCategory(id: number, formData: CategoryFormData): Promise<void> {
	await categoriesAPI.update(id, formData);
}

/**
 * Delete a category
 */
export async function deleteCategory(id: number): Promise<void> {
	await categoriesAPI.delete(id);
}
