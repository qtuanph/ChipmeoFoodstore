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
			// Don't redirect if the failing request is to the login endpoint itself
			if (!url.includes('/auth/login')) {
				if (typeof window !== 'undefined') {
					window.location.href = '/';
					return Promise.reject(new Error('Unauthorized')) as never;
				}
			}
		}

		let errorData: Record<string, unknown>;
		try {
			const text = await response.text();
			errorData = text ? JSON.parse(text) : {};
		} catch {
			errorData = {};
		}

		// Handle ApiResult error envelope: { error: { code, message } }
		const errorObj = errorData?.error as { message?: string } | undefined;
		const errorMessage =
			errorObj?.message || (errorData?.message as string | undefined) || response.statusText;
		const error = new Error(errorMessage) as Error & { error?: string };
		error.error = typeof errorData.error === 'string' ? errorData.error : errorObj?.message;
		throw error;
	}

	// Handle 204 No Content
	if (response.status === 204) {
		return undefined as T;
	}

	const json = (await response.json()) as Record<string, unknown>;

	// Unwrap ApiResult envelope: { data, error, meta }
	if (json && typeof json === 'object' && 'data' in json) {
		if (json.error) {
			const errDetail = json.error as { message?: string };
			throw new Error(errDetail.message || 'API Error');
		}
		return json.data as T;
	}

	return json as T;
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
			return res.json().then((json: Record<string, unknown>) => {
				if (json && typeof json === 'object' && 'data' in json) {
					if (json.error)
						throw new Error((json.error as { message?: string }).message || 'API Error');
					return json.data as T;
				}
				return json as T;
			});
		});
	}
};
