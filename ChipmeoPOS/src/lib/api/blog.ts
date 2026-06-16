import { apiRequest } from '$lib/api/utils.js';
import { API_ENDPOINTS } from '$lib/config/index.ts';

import type { Tag } from '$lib/api/tags.js';

export interface BlogPost {
	id: number;
	title: string;
	slug: string;
	excerpt?: string;
	content?: string;
	thumbnailUrl?: string;
	status: string;
	authorId?: number;
	authorName?: string;
	publishedAt?: string;
	// SEO Fields
	metaTitle?: string;
	metaDescription?: string;
	focusKeyword?: string;
	keywords?: string;
	canonicalUrl?: string;
	ogImageUrl?: string;
	readingTime?: number;
	wordCount?: number;
	seoScore?: number;
	createdAt?: string;
	updatedAt?: string;
	tags?: Tag[];
}

export interface CreateBlogPostDto {
	title: string;
	excerpt?: string;
	content?: string;
	thumbnailUrl?: string;
	status?: string;
	// SEO Fields
	metaTitle?: string;
	metaDescription?: string;
	focusKeyword?: string;
	keywords?: string;
	canonicalUrl?: string;
	ogImageUrl?: string;
	seoScore?: number;
	tagIds?: number[];
}

export interface UpdateBlogPostDto {
	title?: string;
	excerpt?: string;
	content?: string;
	thumbnailUrl?: string;
	status?: string;
	// SEO Fields
	metaTitle?: string;
	metaDescription?: string;
	focusKeyword?: string;
	keywords?: string;
	canonicalUrl?: string;
	ogImageUrl?: string;
	seoScore?: number;
	tagIds?: number[];
}

export const blogAPI = {
	async getPosts(status?: string): Promise<BlogPost[]> {
		const url = status ? `${API_ENDPOINTS.blog}?status=${status}` : API_ENDPOINTS.blog;
		return apiRequest(url);
	},

	async getPostBySlug(slug: string): Promise<BlogPost> {
		return apiRequest(`${API_ENDPOINTS.blog}/${slug}`);
	},

	async getPostById(id: number): Promise<BlogPost> {
		return apiRequest(`${API_ENDPOINTS.blog}/${id}`);
	},

	async createPost(data: CreateBlogPostDto): Promise<BlogPost> {
		return apiRequest(API_ENDPOINTS.blog, {
			method: 'POST',
			body: JSON.stringify(data)
		});
	},

	async updatePost(id: number, data: UpdateBlogPostDto): Promise<BlogPost> {
		return apiRequest(`${API_ENDPOINTS.blog}/${id}`, {
			method: 'PUT',
			body: JSON.stringify(data)
		});
	},

	async deletePost(id: number): Promise<void> {
		return apiRequest(`${API_ENDPOINTS.blog}/${id}`, {
			method: 'DELETE'
		});
	}
};
