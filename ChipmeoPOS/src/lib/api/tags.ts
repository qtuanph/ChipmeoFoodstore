import { apiRequest } from '$lib/api/utils.js';
import { API_BASE_URL } from '$lib/config/index.ts';

export interface Tag {
	id: number;
	name: string;
	slug: string;
	description?: string;
	color: string;
	postCount: number;
}

export interface CreateTagDto {
	name: string;
	description?: string;
	color?: string;
}

export interface UpdateTagDto {
	name?: string;
	description?: string;
	color?: string;
}

export const tagsAPI = {
	async getAll(): Promise<Tag[]> {
		return apiRequest(`${API_BASE_URL}/api/tags`);
	},

	async getById(id: number): Promise<Tag> {
		return apiRequest(`${API_BASE_URL}/api/tags/${id}`);
	},

	async getBySlug(slug: string): Promise<Tag> {
		return apiRequest(`${API_BASE_URL}/api/tags/slug/${slug}`);
	},

	async create(data: CreateTagDto): Promise<Tag> {
		return apiRequest(`${API_BASE_URL}/api/tags`, {
			method: 'POST',
			body: JSON.stringify(data)
		});
	},

	async update(id: number, data: UpdateTagDto): Promise<Tag> {
		return apiRequest(`${API_BASE_URL}/api/tags/${id}`, {
			method: 'PUT',
			body: JSON.stringify(data)
		});
	},

	async delete(id: number): Promise<void> {
		return apiRequest(`${API_BASE_URL}/api/tags/${id}`, {
			method: 'DELETE'
		});
	},

	async getByPost(postId: number): Promise<Tag[]> {
		return apiRequest(`${API_BASE_URL}/api/tags/post/${postId}`);
	},

	async setPostTags(postId: number, tagIds: number[]): Promise<void> {
		return apiRequest(`${API_BASE_URL}/api/tags/post/${postId}`, {
			method: 'PUT',
			body: JSON.stringify(tagIds)
		});
	}
};
