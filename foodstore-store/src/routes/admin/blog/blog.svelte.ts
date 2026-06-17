import { blogAPI, type BlogPost } from '$lib/api/blog.js';
import { tagsAPI, type Tag } from '$lib/api/tags.js';
import { API_BASE_URL } from '$lib/config/index.js';
import { api } from '$lib/api/utils.js';
import { calculateSeoChecks, calculateTotalSeoScore } from '$lib/utils/seo.js';

export class BlogState {
	posts = $state<BlogPost[]>([]);
	loading = $state(true);
	editingPost = $state<BlogPost | null>(null);
	showModal = $state(false);
	activeSidebarTab = $state<'info' | 'seo' | 'settings' | 'analytics'>('info');
	availableTags = $state<Tag[]>([]);
	showPreview = $state(false);
	saving = $state(false);
	uploadingThumbnail = $state(false);

	formData = $state<{
		title: string;
		excerpt: string;
		content: string;
		thumbnailUrl: string;
		status: string;
		metaTitle: string;
		metaDescription: string;
		focusKeyword: string;
		keywords: string;
		canonicalUrl: string;
		ogImageUrl: string;
		seoScore: number;
		tagIds: number[];
	}>({
		title: '',
		excerpt: '',
		content: '',
		thumbnailUrl: '',
		status: 'draft',
		metaTitle: '',
		metaDescription: '',
		focusKeyword: '',
		keywords: '',
		canonicalUrl: '',
		ogImageUrl: '',
		seoScore: 0,
		tagIds: []
	});

	constructor() {
		// Auto-calculate SEO score on form change
		$effect.root(() => {
			$effect(() => {
				const checks = calculateSeoChecks(
					this.formData.title,
					this.formData.content,
					this.formData.excerpt,
					this.formData.focusKeyword,
					this.formData.metaTitle,
					this.formData.metaDescription
				);
				this.formData.seoScore = calculateTotalSeoScore(checks);
			});
		});
	}

	async init() {
		await Promise.all([this.loadPosts(), this.loadTags()]);
	}

	async loadPosts() {
		this.loading = true;
		try {
			this.posts = await blogAPI.getPosts();
		} catch (err) {
			console.error('Failed to load posts:', err);
		} finally {
			this.loading = false;
		}
	}

	async loadTags() {
		try {
			this.availableTags = await tagsAPI.getAll();
		} catch (err) {
			console.error('Failed to load tags:', err);
		}
	}

	openNewPost() {
		this.editingPost = null;
		this.resetForm();
		this.activeSidebarTab = 'info';
		this.showModal = true;
	}

	openEditPost(post: BlogPost) {
		this.editingPost = post;
		this.formData = {
			title: post.title,
			excerpt: post.excerpt || '',
			content: post.content || '',
			thumbnailUrl: post.thumbnailUrl || '',
			status: post.status || 'draft',
			metaTitle: post.metaTitle || '',
			metaDescription: post.metaDescription || '',
			focusKeyword: post.focusKeyword || '',
			keywords: post.keywords || '',
			canonicalUrl: post.canonicalUrl || '',
			ogImageUrl: post.ogImageUrl || '',
			seoScore: post.seoScore || 0,
			tagIds: post.tags?.map((t) => t.id) || []
		};
		this.activeSidebarTab = 'info';
		this.showModal = true;
	}

	resetForm() {
		this.formData = {
			title: '',
			excerpt: '',
			content: '',
			thumbnailUrl: '',
			status: 'draft',
			metaTitle: '',
			metaDescription: '',
			focusKeyword: '',
			keywords: '',
			canonicalUrl: '',
			ogImageUrl: '',
			seoScore: 0,
			tagIds: []
		};
	}

	toggleTag(tagId: number) {
		if (this.formData.tagIds.includes(tagId)) {
			this.formData.tagIds = this.formData.tagIds.filter((id) => id !== tagId);
		} else {
			this.formData.tagIds = [...this.formData.tagIds, tagId];
		}
	}

	async uploadThumbnail(file: File): Promise<void> {
		this.uploadingThumbnail = true;
		try {
			const data = new FormData();
			data.append('file', file);
			data.append('folder', 'blog');

			const result = await api.upload<{ fileUrl: string }>(
				`${API_BASE_URL}/api/media/upload`,
				data
			);
			this.formData.thumbnailUrl = result.fileUrl;
		} catch (err) {
			console.error(err);
			throw err;
		} finally {
			this.uploadingThumbnail = false;
		}
	}

	async uploadEditorImage(file: File): Promise<string> {
		const data = new FormData();
		data.append('file', file);
		data.append('folder', 'blog-content');

		const result = await api.upload<{ fileUrl: string }>(`${API_BASE_URL}/api/media/upload`, data);
		return result.fileUrl;
	}

	async handleSave(editorComponent?: any) {
		this.saving = true;
		try {
			// 1. Process pending images in editor
			if (editorComponent) {
				const newContent = await editorComponent.uploadImages(this.uploadEditorImage);
				this.formData.content = newContent;
			}

			// 2. Save post
			if (this.editingPost) {
				await blogAPI.updatePost(this.editingPost.id, this.formData);
			} else {
				await blogAPI.createPost(this.formData);
			}
			this.showModal = false;
			await this.loadPosts();
		} catch (err: any) {
			console.error(err);
			throw err; // Re-throw for UI to handle alert
		} finally {
			this.saving = false;
		}
	}

	async handleDelete(post: BlogPost) {
		try {
			await blogAPI.deletePost(post.id);
			await this.loadPosts();
		} catch (err: any) {
			console.error(err);
			throw err;
		}
	}

	// Helper methods
	formatDate(dateString?: string) {
		if (!dateString) return '-';
		return new Date(dateString).toLocaleDateString('vi-VN');
	}

	getStatusBadge(status: string) {
		switch (status) {
			case 'published':
				return 'bg-green-100 text-green-800';
			case 'draft':
				return 'bg-yellow-100 text-yellow-800';
			case 'archived':
				return 'bg-gray-100 text-gray-800';
			default:
				return 'bg-gray-100 text-gray-800';
		}
	}

	getStatusText(status: string) {
		switch (status) {
			case 'published':
				return 'Đã đăng';
			case 'draft':
				return 'Nháp';
			case 'archived':
				return 'Lưu trữ';
			default:
				return status;
		}
	}

	getSeoScoreColor(score?: number) {
		if (!score) return 'text-gray-400';
		if (score >= 80) return 'text-green-600';
		if (score >= 50) return 'text-yellow-600';
		return 'text-red-600';
	}
}
