// Media Storage API client
// Calls the dedicated media storage server

const MEDIA_API_URL = 'https://media.chipmeo.io.vn';
const MEDIA_API_KEY = 'chipmeo-media-api-key-2024';

export interface MediaFile {
	fileName: string;
	folder: string;
	fileUrl: string;
	fileSize: number;
	fileType: string;
	createdAt: string;
}

export async function getMediaFiles(folder?: string): Promise<MediaFile[]> {
	const url = folder ? `${MEDIA_API_URL}/api/media?folder=${folder}` : `${MEDIA_API_URL}/api/media`;

	const response = await fetch(url);
	if (!response.ok) throw new Error('Failed to fetch media');

	const files = await response.json();
	// Convert relative URLs to absolute
	return files.map((f: MediaFile) => ({
		...f,
		fileUrl: f.fileUrl.startsWith('http') ? f.fileUrl : `${MEDIA_API_URL}${f.fileUrl}`
	}));
}

export async function getFolders(): Promise<string[]> {
	const response = await fetch(`${MEDIA_API_URL}/api/media/folders`);
	if (!response.ok) throw new Error('Failed to fetch folders');
	return response.json();
}

export async function uploadMedia(file: File, folder: string = 'misc'): Promise<MediaFile> {
	const formData = new FormData();
	formData.append('file', file);
	formData.append('folder', folder);

	const response = await fetch(`${MEDIA_API_URL}/api/media/upload`, {
		method: 'POST',
		headers: {
			'X-Api-Key': MEDIA_API_KEY
		},
		body: formData
	});

	if (!response.ok) {
		const error = await response.json();
		throw new Error(error.error || 'Upload failed');
	}

	const result = await response.json();
	return {
		...result,
		fileUrl: result.fileUrl.startsWith('http')
			? result.fileUrl
			: `${MEDIA_API_URL}${result.fileUrl}`
	};
}

export async function deleteMedia(folder: string, filename: string): Promise<void> {
	const response = await fetch(`${MEDIA_API_URL}/api/media/${folder}/${filename}`, {
		method: 'DELETE',
		headers: {
			'X-Api-Key': MEDIA_API_KEY
		}
	});

	if (!response.ok) {
		const error = await response.json();
		throw new Error(error.error || 'Delete failed');
	}
}

export function formatFileSize(bytes: number): string {
	if (bytes < 1024) return bytes + ' B';
	if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(1) + ' KB';
	return (bytes / (1024 * 1024)).toFixed(1) + ' MB';
}
