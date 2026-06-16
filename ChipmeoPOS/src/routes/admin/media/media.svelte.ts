import { mediaAPI } from '$lib/api/index.js';

export class MediaState {
	files = $state<any[]>([]);
	loading = $state(true);
	folders = $state<string[]>(['All', 'blog_post', 'customer', 'employee', 'menu-items', 'misc']);
	selectedFolder = $state('All');

	// Toast state
	showToast = $state(false);
	toastMessage = $state('');
	toastType = $state<'success' | 'error'>('success');

	// Upload state
	uploading = $state(false);

	get filteredFiles() {
		return this.selectedFolder === 'All'
			? this.files
			: this.files.filter((f) => f.folder === this.selectedFolder);
	}

	async init() {
		await this.loadFiles();
	}

	async loadFiles() {
		try {
			this.loading = true;
			// API doesn't support folder filtering in GetAll yet (per my implementation plan check),
			// but I added support in the previous steps?
			// Wait, MediaHandler.GetAllMediaAsync doesn't accept args.
			// I need to filter client-side for now or rely on my earlier plan to update backend if I did.
			// I checked MediaHandler, it assumes 'getAll' returns list. I'll filter client side.
			const data = (await mediaAPI.getAll()) as any[];
			this.files = data;

			// Extract unique folders dynamically if needed
			const dynamicFolders = [...new Set(data.map((f: any) => f.folder).filter(Boolean))];
			dynamicFolders.sort();
			this.folders = ['All', ...dynamicFolders];
			if (!this.folders.includes('blog_post')) this.folders.push('blog_post'); // Ensure common ones exist
		} catch (error: any) {
			this.showError('Lỗi tải danh sách file: ' + (error.message || ''));
		} finally {
			this.loading = false;
		}
	}

	async handleDelete(id: number) {
		try {
			await mediaAPI.delete(id);
			this.files = this.files.filter((f) => f.id !== id);
			this.showSuccess('Xóa file thành công');
		} catch (error: any) {
			this.showError('Lỗi xóa file: ' + (error.message || ''));
		}
	}

	async handleUpload(file: File) {
		try {
			this.uploading = true;
			const formData = new FormData();
			formData.append('file', file);
			formData.append('folder', this.selectedFolder === 'All' ? 'misc' : this.selectedFolder);

			const result = await mediaAPI.upload(formData);
			// Add to list (mock since we don't refetch to save bandwidth, or refetch)
			// Ideally refetch or construct object.
			await this.loadFiles();

			this.showSuccess('Upload thành công');
		} catch (error: any) {
			this.showError('Lỗi upload: ' + (error.message || ''));
		} finally {
			this.uploading = false;
		}
	}

	private showSuccess(message: string) {
		this.toastMessage = message;
		this.toastType = 'success';
		this.showToast = true;
	}

	private showError(message: string) {
		this.toastMessage = message;
		this.toastType = 'error';
		this.showToast = true;
	}
}
