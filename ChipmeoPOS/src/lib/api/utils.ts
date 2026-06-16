import { get } from 'svelte/store';
import { auth } from '$lib/utils/auth.ts';

export async function apiRequest<T>(url: string, options: RequestInit = {}): Promise<T> {
	const authStore = get(auth);
	const token = authStore?.token;

	const headers: HeadersInit = {
		'Content-Type': 'application/json',
		...(token ? { Authorization: `Bearer ${token}` } : {}),
		...options.headers
	};

	const response = await fetch(url, {
		...options,
		headers
	});

	if (!response.ok) {
		if (response.status === 401) {
			// Handle unauthorized - redirect to login
			if (typeof window !== 'undefined') {
				// Only redirect if not already on login page
				if (!window.location.pathname.includes('/login')) {
					window.location.href = '/';
				}
			}
		}

		let errorData;
		try {
			const text = await response.text();
			errorData = text ? JSON.parse(text) : {};
		} catch {
			errorData = {};
		}

		const errorMessage = errorData.error || errorData.message || response.statusText;
		const error = new Error(errorMessage) as Error & { error?: string };
		error.error = errorData.error; // Attach raw error for handlers
		throw error;
	}

	// Handle 204 No Content
	if (response.status === 204) {
		return undefined as T;
	}

	return response.json();
}

// Generic API helper object
export const api = {
	get: <T>(url: string) => apiRequest<T>(url),
	post: <T>(url: string, body: unknown) =>
		apiRequest<T>(url, { method: 'POST', body: JSON.stringify(body) }),
	put: <T>(url: string, body: unknown) =>
		apiRequest<T>(url, { method: 'PUT', body: JSON.stringify(body) }),
	delete: <T>(url: string) => apiRequest<T>(url, { method: 'DELETE' }),
	upload: <T>(url: string, formData: FormData): Promise<T> => {
		// Special handler for FormData (no Content-Type header, browser sets it)
		const token = get(auth)?.token;
		return fetch(url, {
			method: 'POST',
			headers: token ? { Authorization: `Bearer ${token}` } : {},
			body: formData
		}).then(async (res) => {
			if (!res.ok) {
				const text = await res.text();
				throw new Error(text || res.statusText);
			}
			return res.json();
		});
	}
};
