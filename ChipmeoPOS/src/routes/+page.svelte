<script lang="ts">
	import { auth } from '$lib/utils/index.js';
	import AuthModal from '$lib/components/AuthModal.svelte';
	import ProfileModal from '$lib/components/ProfileModal.svelte';
	import Button from '$lib/components/Button.svelte';
	import { goto } from '$app/navigation';

	let showAuthModal = $state(false);
	let showProfileModal = $state(false);
	let showDropdown = $state(false);

	function handleLogout() {
		auth.logout();
		showDropdown = false;
	}

	async function navigateTo(path: string) {
		await goto(path);
		showDropdown = false;
	}

	function onLoginSuccess() {
		// Stay on page, auth store will update automatically
	}

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
</script>

<svelte:head>
	<title>Trang chủ - Chipmeo Foodstore</title>
</svelte:head>

<div class="flex min-h-screen flex-col bg-gray-50 md:h-screen md:overflow-hidden">
	<!-- Navbar -->
	<nav class="sticky top-0 z-50 border-b border-gray-100 bg-white shadow-sm">
		<div class="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
			<div class="flex h-16 items-center justify-between">
				<!-- Logo -->
				<div
					class="flex cursor-pointer items-center gap-2"
					onclick={async () => await goto('/')}
					role="button"
					tabindex="0"
					onkeydown={async (e) => e.key === 'Enter' && (await goto('/'))}
				>
					<img
						src="/cmfs_removed_bg.png"
						alt="Chipmeo Logo"
						class="h-10 w-10 rounded-full bg-white p-0.5 drop-shadow-md"
					/>
					<span class="font-brand text-xl font-bold tracking-wide text-brand-brown">
						Chipmeo Foodstore
					</span>
				</div>

				<!-- Right Side -->
				<div class="flex items-center gap-4">
					{#if $auth.isAuthenticated && $auth.user}
						<div class="relative" use:clickOutsideAction={() => (showDropdown = false)}>
							<button
								class="flex items-center gap-3 rounded-lg p-2 transition-colors hover:bg-gray-50"
								onclick={() => (showDropdown = !showDropdown)}
							>
								<div class="hidden text-right sm:block">
									<div class="text-sm font-medium text-gray-900">
										Xin chào, {$auth.user.fullName || $auth.user.username}
									</div>
									<div class="text-xs text-gray-500">{$auth.user.roleName}</div>
								</div>
								<div
									class="flex h-10 w-10 items-center justify-center rounded-full border-2 border-white bg-brand-yellow/20 font-bold text-brand-brown shadow-sm"
								>
									{($auth.user.fullName || $auth.user.username)[0].toUpperCase()}
								</div>
							</button>

							<!-- Dropdown -->
							{#if showDropdown}
								<div
									class="absolute right-0 mt-2 w-56 rounded-xl border border-gray-100 bg-white py-2 shadow-lg duration-200 animate-in fade-in slide-in-from-top-2"
								>
									<div class="border-b border-gray-50 px-4 py-2 sm:hidden">
										<div class="text-sm font-medium text-gray-900">
											{$auth.user.fullName || $auth.user.username}
										</div>
										<div class="text-xs text-gray-500">{$auth.user.roleName}</div>
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

									{#if $auth.user.roleName === 'Admin' || $auth.user.roleName === 'Manager'}
										<button
											class="flex w-full items-center gap-2 px-4 py-2 text-left text-sm text-gray-700 hover:bg-gray-50 hover:text-brand-brown"
											onclick={() => navigateTo('/admin')}
										>
											📊 Vào Dashboard
										</button>
									{/if}

									{#if $auth.user.roleName === 'Admin' || $auth.user.roleName === 'Manager' || $auth.user.roleName === 'Cashier'}
										<button
											class="flex w-full items-center gap-2 px-4 py-2 text-left text-sm text-gray-700 hover:bg-gray-50 hover:text-brand-brown"
											onclick={() => navigateTo('/pos')}
										>
											🖥️ Vào POS
										</button>
									{/if}

									{#if ['Admin', 'Manager', 'Chef', 'Kitchen', 'Cook', 'Bếp'].includes($auth.user.roleName)}
										<button
											class="flex w-full items-center gap-2 px-4 py-2 text-left text-sm text-gray-700 hover:bg-gray-50 hover:text-brand-brown"
											onclick={() => navigateTo('/kitchen')}
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
					{:else}
						<div class="flex gap-2">
							<Button
								variant="secondary"
								onclick={() => {
									showAuthModal = true;
								}}
							>
								Đăng nhập
							</Button>
							<!-- <Button variant="primary" onclick={() => { showAuthModal = true; }}>
								Đăng ký
							</Button> -->
						</div>
					{/if}
				</div>
			</div>
		</div>
	</nav>

	<!-- Hero Section -->
	<main class="flex flex-1 items-center justify-center p-4">
		<div class="w-full max-w-4xl space-y-8 text-center">
			<div class="relative inline-block">
				<div
					class="absolute -inset-4 animate-pulse rounded-full bg-gradient-to-r from-brand-brown via-brand-gold to-brand-yellow opacity-20 blur-xl"
				></div>
				<img
					src="/cmfs_removed_bg.png"
					alt="Chipmeo Logo"
					class="relative mx-auto h-32 w-32 rounded-full bg-white p-2 drop-shadow-2xl md:h-40 md:w-40"
				/>
			</div>

			<div
				class="space-y-2 duration-1000 ease-out animate-in fade-in slide-in-from-bottom-8 md:space-y-4"
			>
				<h1 class="text-4xl font-extrabold tracking-tight text-gray-900 md:text-5xl">
					Chào mừng đến với <br />
					<span class="font-brand tracking-wide text-brand-brown"> Chipmeo Foodstore </span>
				</h1>
				<p class="mx-auto max-w-2xl text-lg text-gray-600 md:text-xl">
					Hệ thống quản lý nhà hàng thông minh, hiện đại và tiện lợi.
				</p>
			</div>

			{#if !$auth.isAuthenticated}
				<div
					class="flex justify-center gap-4 pt-4 delay-200 duration-700 animate-in fade-in fill-mode-forwards zoom-in"
				>
					<button
						class="transform rounded-xl bg-brand-brown px-8 py-4 text-lg font-bold text-white shadow-lg transition-all hover:-translate-y-1 hover:scale-105 hover:bg-brand-brown/90 hover:shadow-xl active:scale-95"
						onclick={() => (showAuthModal = true)}
					>
						Bắt đầu ngay
					</button>
				</div>
			{:else}
				<div
					class="flex justify-center gap-4 pt-4 delay-200 duration-700 animate-in fade-in fill-mode-forwards zoom-in"
				>
					{#if $auth.user?.roleName === 'Admin' || $auth.user?.roleName === 'Manager'}
						<button
							class="rounded-xl border border-brand-brown/20 bg-white px-8 py-4 text-lg font-bold text-brand-brown shadow-sm transition-all hover:bg-brand-yellow/10"
							onclick={() => navigateTo('/admin')}
						>
							Quản lý
						</button>
					{/if}
					<button
						class="transform rounded-xl bg-brand-brown px-8 py-4 text-lg font-bold text-white shadow-lg transition-all hover:-translate-y-1 hover:bg-brand-brown/90 hover:shadow-xl"
						onclick={() => navigateTo('/pos')}
					>
						Đặt món ngay
					</button>
				</div>
			{/if}
		</div>
	</main>

	<!-- Footer -->
	<footer class="border-t border-gray-100 bg-white py-4 md:py-6">
		<div class="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
			<div class="mb-4 grid grid-cols-1 gap-4 md:grid-cols-3 md:gap-8">
				<!-- Column 1: About -->
				<div class="space-y-2">
					<div class="flex items-center justify-center gap-2 md:justify-start">
						<img
							src="/cmfs_removed_bg.png"
							alt="Chipmeo Logo"
							class="h-6 w-6 rounded-full bg-white p-0.5 drop-shadow-sm"
						/>
						<span class="font-brand text-base font-bold tracking-wide text-brand-brown">
							Chipmeo Foodstore
						</span>
					</div>
					<p class="hidden text-center text-xs leading-relaxed text-gray-500 md:block md:text-left">
						Mang đến trải nghiệm ẩm thực tuyệt vời.
					</p>
				</div>

				<!-- Column 2: Links -->
				<div class="hidden md:block">
					<h3 class="mb-2 text-sm font-bold text-gray-900">Liên kết nhanh</h3>
					<ul class="space-y-1 text-xs text-gray-600">
						<li>
							<button
								class="transition-colors hover:text-brand-brown"
								onclick={async () => await goto('/')}
							>
								Trang chủ
							</button>
						</li>
						<li>
							<button
								class="transition-colors hover:text-brand-brown"
								onclick={async () => await goto('/menu')}
							>
								Thực đơn
							</button>
						</li>
					</ul>
				</div>

				<!-- Column 3: Contact -->
				<div class="text-center md:text-left">
					<h3 class="mb-2 hidden text-sm font-bold text-gray-900 md:block">Liên hệ</h3>
					<ul class="flex flex-col items-center space-y-1 text-xs text-gray-600 md:items-start">
						<li class="flex items-center gap-2">
							<span>📞</span>
							<span>0903 282 169</span>
						</li>
						<li class="flex items-center gap-2">
							<span>✉️</span>
							<span>chipmeo2304@gmail.com</span>
						</li>
					</ul>
				</div>
			</div>

			<div class="border-t border-gray-100 pt-2 text-center text-[10px] text-gray-400">
				<p>Chipmeo Foodstore © 2025. All rights reserved.</p>
			</div>
		</div>
	</footer>
</div>

<AuthModal bind:open={showAuthModal} {onLoginSuccess} />
<ProfileModal bind:open={showProfileModal} />
