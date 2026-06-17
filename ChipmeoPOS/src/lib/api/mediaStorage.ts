import { api } from '$lib/api/utils.js';
import { API_ENDPOINTS } from '$lib/config/index.js';

export interface MediaFile {
	fileName: string;
	folder: string;
	fileUrl: string;
	fileSize: number;
	fileType: string;
	createdAt: string;
}

export async function getMediaFiles(folder?: string): Promise<MediaFile[]> {
	const url = folder ? `${API_ENDPOINTS.media}?folder=${folder}` : API_ENDPOINTS.media;
	return api.get<MediaFile[]>(url);
}

export async function getFolders(): Promise<string[]> {
	return ['blog', 'avatars', 'menu-items', 'combos', 'categories', 'misc'];
}

export async function uploadMedia(file: File, folder: string = 'misc'): Promise<MediaFile> {
	const formData = new FormData();
	formData.append('file', file);
	formData.append('folder', folder);
	return api.upload<MediaFile>(`${API_ENDPOINTS.media}/upload`, formData);
}

export async function deleteMedia(id: number | string): Promise<void> {
	await api.delete(`${API_ENDPOINTS.media}/${id}`);
}

export function formatFileSize(bytes: number): string {
	if (bytes < 1024) return bytes + ' B';
	if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(1) + ' KB';
	return (bytes / (1024 * 1024)).toFixed(1) + ' MB';
}
