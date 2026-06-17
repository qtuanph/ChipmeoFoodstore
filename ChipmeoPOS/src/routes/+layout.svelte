<script lang="ts">
	import './layout.css';
	import { onMount } from 'svelte';
	import { auth } from '$lib/utils/index.js';
	import LandscapePrompt from '$lib/components/LandscapePrompt.svelte';
	import { STORAGE_KEYS } from '$lib/config/index.js';
	import { initFlowbite } from 'flowbite';

	onMount(() => {
		auth.checkAuth();
		initFlowbite();
	});

	import { page } from '$app/stores';
	import { goto } from '$app/navigation';

	let { children } = $props();
	let isAuthorized = $state(false);
	let checking = $state(true);

	const protectedPrefixes = ['/admin', '/pos', '/kitchen'];

	$effect(() => {
		(async () => {
			const path = $page.url.pathname;
			const isProtected = protectedPrefixes.some((prefix) => path.startsWith(prefix));

			if (isProtected) {
				if (!$auth.isAuthenticated) {
					// Wait for auth check if loading
					if (!$auth.loading) {
						// Double check token in storage
						const token = localStorage.getItem(STORAGE_KEYS.TOKEN);
						if (!token) {
							isAuthorized = false;
							await goto('/');
						} else {
							// Token exists, wait for auth store to update
						}
					}
				} else {
					// Authenticated, check permissions
					const role = $auth.user?.roleName;
					let hasPermission = false;

					if (path.startsWith('/admin')) {
						hasPermission = role === 'Admin' || role === 'Manager';
					} else if (path.startsWith('/pos')) {
						hasPermission = ['Admin', 'Manager', 'Cashier', 'Thu ngân'].includes(role || '');
					} else if (path.startsWith('/kitchen')) {
						hasPermission = ['Admin', 'Manager', 'Chef', 'Kitchen', 'Cook', 'Bếp'].includes(
							role || ''
						);
					}

					if (!hasPermission) {
						isAuthorized = false;
						await goto('/error');
					} else {
						isAuthorized = true;
					}
				}
			} else {
				isAuthorized = true;
			}

			// Simple timeout to prevent infinite loading state if auth check fails silently
			setTimeout(() => {
				if (checking) checking = false;
			}, 1000);
		})().catch(console.error);
	});

	// Reactive check for auth state changes
	$effect(() => {
		(async () => {
			if ($auth.isAuthenticated) {
				isAuthorized = true;
				checking = false;
			} else if (!$auth.loading && checking) {
				// If auth finished loading and still not authenticated
				const path = $page.url.pathname;
				if (protectedPrefixes.some((prefix) => path.startsWith(prefix))) {
					const token = localStorage.getItem(STORAGE_KEYS.TOKEN);
					if (!token) {
						isAuthorized = false;
						checking = false;
						await goto('/');
					} else {
						// Check permissions after late auth load
						const role = $auth.user?.roleName;
						let hasPermission = false;

						if (path.startsWith('/admin')) {
							hasPermission = role === 'Admin' || role === 'Manager';
						} else if (path.startsWith('/pos')) {
							hasPermission = ['Admin', 'Manager', 'Cashier', 'Thu ngân'].includes(role || '');
						} else if (path.startsWith('/kitchen')) {
							hasPermission = ['Admin', 'Manager', 'Chef', 'Kitchen', 'Cook', 'Bếp'].includes(
								role || ''
							);
						}

						if (!hasPermission) {
							isAuthorized = false;
							checking = false;
							await goto('/error');
						} else {
							isAuthorized = true;
							checking = false;
						}
					}
				} else {
					isAuthorized = true;
					checking = false;
				}
			} else if (checking) {
				const token = localStorage.getItem(STORAGE_KEYS.TOKEN);
				const path = $page.url.pathname;
				if (!token && protectedPrefixes.some((prefix) => path.startsWith(prefix))) {
					isAuthorized = false;
					checking = false;
					await goto('/');
				}
			}
		})().catch(console.error);
	});
</script>

<LandscapePrompt />

{#if isAuthorized}
	{@render children()}
{:else if checking}
	<div class="flex min-h-screen items-center justify-center bg-gray-50">
		<div class="flex flex-col items-center gap-4">
			<div
				class="h-12 w-12 animate-spin rounded-full border-4 border-indigo-600 border-t-transparent"
			></div>
			<p class="font-medium text-gray-500">Đang kiểm tra bảo mật...</p>
		</div>
	</div>
{/if}
