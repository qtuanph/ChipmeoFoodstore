import { writable, get } from 'svelte/store';
import { BROWSER as browser } from 'esm-env';
import { authAPI } from '$lib/api/auth.js';
import type { User, AuthState } from '$lib/types/index.js';

// Initial state
const initialState: AuthState = {
	isAuthenticated: false,
	user: null,
	token: null,
	loading: false,
	error: null
};

function createAuthStore() {
	const { subscribe, set, update } = writable<AuthState>(initialState);

	return {
		subscribe,

		// Initialize from localStorage
		init: () => {
			if (browser) {
				const token = localStorage.getItem('token');
				const userStr = localStorage.getItem('user');

				if (token && userStr) {
					try {
						const user = JSON.parse(userStr);
						update((s) => ({
							...s,
							isAuthenticated: true,
							token,
							user
						}));
					} catch (e) {
						console.error('Failed to parse user from localStorage', e);
						localStorage.removeItem('token');
						localStorage.removeItem('user');
					}
				}
			}
		},

		// Login action
		loginAPI: async (
			username: string,
			password: string
		): Promise<{ success: boolean; error?: string; user?: User }> => {
			update((s) => ({ ...s, loading: true, error: null }));

			try {
				// Use the API module for login
				const data = await authAPI.login(username, password);

				const { token, user } = data;

				// Save to localStorage
				if (browser) {
					localStorage.setItem('token', token);
					localStorage.setItem('user', JSON.stringify(user));
				}

				// Update store
				update((s) => ({
					...s,
					isAuthenticated: true,
					user,
					token,
					loading: false,
					error: null
				}));

				return { success: true, user };
			} catch (error: any) {
				console.error('Login error:', error);
				// Handle specific API error messages if available
				let errorMessage = error.message || 'Lỗi kết nối server';

				// If the error is an object with an error property (from backend)
				if (error.error) {
					errorMessage = error.error;
				}

				update((s) => ({
					...s,
					loading: false,
					error: errorMessage
				}));

				return { success: false, error: errorMessage };
			}
		},

		// Logout action
		logout: async () => {
			try {
				await authAPI.logout();
			} catch (e) {
				console.error('Logout API error:', e);
			}

			if (browser) {
				localStorage.removeItem('token');
				localStorage.removeItem('user');
			}

			set(initialState);

			if (browser) {
				window.location.href = '/';
			}
		},

		// Check auth status
		checkAuth: () => {
			const state = get({ subscribe });
			if (!state.isAuthenticated && browser) {
				const token = localStorage.getItem('token');
				if (!token) {
					// Redirect to login if needed, but be careful about infinite loops
					const path = window.location.pathname;
					// Only redirect for protected routes
					if (path.startsWith('/admin') || path.startsWith('/pos') || path.startsWith('/kitchen')) {
						window.location.href = '/';
					}
				}
			}
		},

		// Update user profile
		updateUser: (user: User) => {
			if (browser) {
				localStorage.setItem('user', JSON.stringify(user));
			}

			update((s) => ({
				...s,
				user
			}));
		},

		// Get profile
		getProfile: async () => {
			return await authAPI.getProfile();
		},

		// Update profile
		updateProfile: async (data: any) => {
			return await authAPI.updateProfile(data);
		}
	};
}

export const auth = createAuthStore();

// Initialize immediately if in browser
if (browser) {
	auth.init();
}
