<script lang="ts">
	import { page } from '$app/stores';
	import { auth } from '$lib/utils/auth.ts';
	import { goto } from '$app/navigation';
	import { onMount } from 'svelte';

	import ProfileModal from '$lib/components/ProfileModal.svelte';
	import Icon from '$lib/components/ui/Icon.svelte';

	let { children } = $props();
	let showDropdown = $state(false);
	let showProfileModal = $state(false);
	let showFooterDropdown = $state(false);

	// Simple click outside action
	function clickOutsideAction(node: HTMLElement, callback: () => void) {
		const handleClick = (event: MouseEvent) => {
			if (node && !node.contains(event.target as Node) && !event.defaultPrevented) {
				callback();
			}
		};
		document.addEventListener('click', handleClick, true);
		return {
			destroy() {
				document.removeEventListener('click', handleClick, true);
			}
		};
	}

	onMount(() => {
		const unsubscribe = auth.subscribe((state) => {
			if (state.isAuthenticated && state.user?.defaultRoute) {
				// If user has a default route that is NOT /admin (e.g. /pos), redirect them there
				// Allow access if defaultRoute starts with /admin (e.g. /admin/orders)
				if (
					!state.user.defaultRoute.startsWith('/admin') &&
					!state.user.permissions.includes('admin.access')
				) {
					goto(state.user.defaultRoute);
				}
			}
		});
		return unsubscribe;
	});

	const navGroups = [
		{
			title: 'BÁO CÁO',
			items: [
				{ href: '/admin', label: 'Tổng quan', icon: '📊' },
				{ href: '/admin/analytics', label: 'Thống kê & AI', icon: '🤖' }
			]
		},
		{
			title: 'VẬN HÀNH',
			items: [
				{ href: '/admin/orders', label: 'Đơn hàng', icon: '📋' },
				{ href: '/admin/sources', label: 'Nguồn đơn', icon: '📍' }
			]
		},
		{
			title: 'THỰC ĐƠN',
			items: [
				{ href: '/admin/categories', label: 'Danh mục', icon: '📑' },
				{ href: '/admin/menu', label: 'Món ăn', icon: '🍜' },
				{ href: '/admin/addons', label: 'Món thêm', icon: '✨' },
				{ href: '/admin/combos', label: 'Combo', icon: '🎁' }
			]
		},
		{
			title: 'HỆ THỐNG',
			items: [
				{ href: '/admin/discounts', label: 'Khuyến mãi', icon: '🎫' },
				{ href: '/admin/employees', label: 'Nhân viên', icon: '👥' },
				{ href: '/admin/roles', label: 'Vai trò', icon: '🔐' },
				{ href: '/admin/role-permissions', label: 'Quản lý quyền', icon: '🛡️' },
				{ href: '/admin/payment-settings', label: 'Thanh toán', icon: '💳' }
			]
		},
		{
			title: 'NỘI DUNG',
			items: [
				{ href: '/admin/blog', label: 'Quản lý Blog', icon: '📝' },
				{ href: '/admin/tags', label: 'Quản lý Tags', icon: '🏷️' },
				{ href: '/admin/customers', label: 'Khách hàng', icon: '👥' }
			]
		}
	];

	const isActive = (href: string) => {
		if (href === '/admin') {
			return $page.url.pathname === '/admin';
		}
		return $page.url.pathname.startsWith(href);
	};

	let isSidebarOpen = $state(false);

	function toggleSidebar() {
		isSidebarOpen = !isSidebarOpen;
	}

	function handleLogout() {
		auth.logout();
	}
</script>

<div class="flex h-screen overflow-hidden bg-gray-50">
	<!-- Mobile Header -->
	<div
		class="fixed top-0 right-0 left-0 z-20 flex h-16 items-center justify-between border-b bg-white px-4 shadow-sm lg:hidden"
	>
		<div class="flex items-center gap-3">
			<button
				onclick={toggleSidebar}
				class="-ml-2 rounded-lg p-2 text-gray-600 hover:bg-gray-100"
				aria-label="Toggle sidebar"
			>
				<Icon name="tabler:menu" class="h-6 w-6" />
			</button>
			<h1 class="text-lg font-bold text-gray-900">Foodstore Admin</h1>
		</div>

		<!-- User Profile Dropdown (Mobile) -->
		<div class="relative" use:clickOutsideAction={() => (showDropdown = false)}>
			<button
				class="flex items-center gap-3 rounded-lg p-1 transition-colors hover:bg-gray-50"
				onclick={() => (showDropdown = !showDropdown)}
			>
				<div class="hidden text-right sm:block">
					<div class="text-sm font-semibold text-gray-900">{$auth.user?.fullName}</div>
					<div class="text-xs text-gray-500">{$auth.user?.roleName}</div>
				</div>
				<div
					class="flex h-9 w-9 items-center justify-center overflow-hidden rounded-full border border-indigo-200 bg-indigo-100 font-bold text-indigo-600"
				>
					{#if $auth.user?.avatarUrl}
						<img src={$auth.user.avatarUrl} alt="Avatar" class="h-full w-full object-cover" />
					{:else}
						{$auth.user?.fullName?.charAt(0).toUpperCase() || 'U'}
					{/if}
				</div>
			</button>

			<!-- Dropdown -->
			{#if showDropdown}
				<div
					class="absolute right-0 z-50 mt-2 w-56 rounded-xl border border-gray-100 bg-white py-2 shadow-lg duration-200 animate-in fade-in slide-in-from-top-2"
				>
					<div class="border-b border-gray-50 px-4 py-2 sm:hidden">
						<div class="text-sm font-medium text-gray-900">{$auth.user?.fullName}</div>
						<div class="text-xs text-gray-500">{$auth.user?.roleName}</div>
					</div>

					<button
						class="flex w-full items-center gap-2 px-4 py-2 text-left text-sm text-gray-700 hover:bg-gray-50 hover:text-indigo-600"
						onclick={() => {
							showProfileModal = true;
							showDropdown = false;
						}}
					>
						👤 Thông tin cá nhân
					</button>

					<button
						class="flex w-full items-center gap-2 px-4 py-2 text-left text-sm text-gray-700 hover:bg-gray-50 hover:text-indigo-600"
						onclick={() => (window.location.href = '/pos')}
					>
						🖥️ Vào POS
					</button>

					{#if ['Admin', 'Manager', 'Chef', 'Kitchen', 'Cook', 'Bếp'].includes($auth.user?.roleName || '')}
						<button
							class="flex w-full items-center gap-2 px-4 py-2 text-left text-sm text-gray-700 hover:bg-gray-50 hover:text-indigo-600"
							onclick={() => (window.location.href = '/kitchen')}
						>
							👨‍🍳 Vào Bếp
						</button>
					{/if}

					<div class="my-1 border-t border-gray-50"></div>

					<button
						class="flex w-full items-center gap-2 px-4 py-2 text-left text-sm text-red-600 hover:bg-red-50"
						onclick={handleLogout}
					>
						🚪 Đăng xuất
					</button>
				</div>
			{/if}
		</div>
	</div>

	<!-- Sidebar Overlay -->
	{#if isSidebarOpen}
		<div
			class="fixed inset-0 z-30 bg-black/50 backdrop-blur-sm lg:hidden"
			onclick={() => (isSidebarOpen = false)}
			role="button"
			tabindex="0"
			onkeydown={(e) => e.key === 'Escape' && (isSidebarOpen = false)}
		></div>
	{/if}

	<!-- Sidebar -->
	<aside
		class="
		fixed inset-y-0 left-0 z-40 flex w-64 transform flex-col border-r border-gray-200 bg-white shadow-xl transition-transform duration-300
		lg:relative lg:translate-x-0
		{isSidebarOpen ? 'translate-x-0' : '-translate-x-full'}
	"
	>
		<div class="hidden flex-shrink-0 border-b border-gray-200 p-4 lg:block">
			<div class="flex items-center gap-3">
				<div
					class="flex h-10 w-10 items-center justify-center rounded-lg bg-indigo-600 text-xl text-white shadow-lg shadow-indigo-200"
				>
					🍔
				</div>
				<div>
					<h1 class="text-xl leading-none font-bold text-gray-900">Foodstore</h1>
					<p class="mt-0.5 text-[10px] font-medium tracking-wider text-gray-500 uppercase">Admin</p>
				</div>
			</div>
		</div>

		<div
			class="flex flex-shrink-0 items-center justify-between border-b border-gray-200 bg-gray-50 p-4 lg:hidden"
		>
			<span class="text-lg font-bold text-gray-900">Menu</span>
			<button
				onclick={() => (isSidebarOpen = false)}
				class="rounded-lg p-2 text-gray-500 hover:bg-gray-200"
				aria-label="Close sidebar"
			>
				<Icon name="tabler:x" class="h-6 w-6" />
			</button>
		</div>

		<nav class="flex-1 overflow-y-auto py-4">
			{#each navGroups as group, i (group.title)}
				{#if i > 0}
					<div class="my-2 px-3">
						<div class="border-t border-gray-100"></div>
					</div>
				{/if}
				<div class="mb-2 px-3">
					<p class="px-3 text-[11px] font-semibold tracking-wider text-gray-400 uppercase">
						{group.title}
					</p>
				</div>

				<div class="space-y-1 px-3">
					{#each group.items as item (item.href)}
						<a
							href={item.href}
							onclick={() => (isSidebarOpen = false)}
							class="group flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-all
								{isActive(item.href)
								? 'bg-indigo-50 text-indigo-700'
								: 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'}"
						>
							<span class="text-lg">{item.icon}</span>
							<span class="flex-1">{item.label}</span>
							{#if isActive(item.href)}
								<div class="h-1.5 w-1.5 rounded-full bg-indigo-600"></div>
							{/if}
						</a>
					{/each}
				</div>
			{/each}
		</nav>
	</aside>

	<!-- Main Content -->
	<main class="flex-1 overflow-auto pt-16 lg:pt-0">
		<!-- Desktop Header Bar -->
		<header class="hidden h-16 items-center justify-between border-b bg-white px-6 lg:flex">
			<div></div>

			<div class="relative" use:clickOutsideAction={() => (showDropdown = false)}>
				<button
					class="flex items-center gap-3 rounded-lg p-1.5 transition-all hover:bg-gray-50"
					onclick={() => (showDropdown = !showDropdown)}
				>
					<div class="text-right">
						<div class="text-xs font-bold text-gray-900">{$auth.user?.fullName}</div>
						<div class="text-[10px] font-medium text-gray-500 italic">{$auth.user?.roleName}</div>
					</div>
					<div
						class="flex h-9 w-9 items-center justify-center overflow-hidden rounded-full border border-indigo-200 bg-indigo-50 font-bold text-indigo-600 shadow-sm"
					>
						{#if $auth.user?.avatarUrl}
							<img src={$auth.user.avatarUrl} alt="Avatar" class="h-full w-full object-cover" />
						{:else}
							{$auth.user?.fullName?.charAt(0).toUpperCase() || 'U'}
						{/if}
					</div>
					<Icon
						name="tabler:chevron-down"
						class="h-4 w-4 text-gray-400 {showDropdown ? 'rotate-180' : ''}"
					/>
				</button>

				{#if showDropdown}
					<div
						class="absolute right-0 z-50 mt-2 w-56 rounded-xl border border-gray-100 bg-white py-2 shadow-xl duration-200 animate-in fade-in slide-in-from-top-2"
					>
						<div class="border-b border-gray-50 px-4 py-2">
							<div class="text-xs font-semibold tracking-wider text-gray-400 uppercase">
								Hệ thống
							</div>
						</div>

						<button
							class="flex w-full items-center gap-2 px-4 py-2 text-left text-sm text-gray-700 hover:bg-gray-50 hover:text-indigo-600"
							onclick={() => {
								showProfileModal = true;
								showDropdown = false;
							}}
						>
							<span class="text-lg">👤</span> Thông tin cá nhân
						</button>

						<button
							class="flex w-full items-center gap-2 px-4 py-2 text-left text-sm text-gray-700 hover:bg-gray-50 hover:text-indigo-600"
							onclick={() => (window.location.href = '/pos')}
						>
							<span class="text-lg">🖥️</span> Vào bán hàng (POS)
						</button>

						{#if ['Admin', 'Manager', 'Chef', 'Kitchen', 'Cook', 'Bếp'].includes($auth.user?.roleName || '')}
							<button
								class="flex w-full items-center gap-2 px-4 py-2 text-left text-sm text-gray-700 hover:bg-gray-50 hover:text-indigo-600"
								onclick={() => (window.location.href = '/kitchen')}
							>
								<span class="text-lg">👨‍🍳</span> Vào màn hình Bếp
							</button>
						{/if}

						<div class="my-1 border-t border-gray-50"></div>

						<button
							class="flex w-full items-center gap-2 px-4 py-2 text-left text-sm font-medium text-red-600 hover:bg-red-50"
							onclick={handleLogout}
						>
							<span class="text-lg">🚪</span> Đăng xuất
						</button>
					</div>
				{/if}
			</div>
		</header>

		<div class="container mx-auto">
			{@render children()}
		</div>
	</main>
</div>

<ProfileModal bind:open={showProfileModal} />
