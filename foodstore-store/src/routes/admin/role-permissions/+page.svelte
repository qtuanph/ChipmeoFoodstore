<script lang="ts">
	import { onMount } from 'svelte';
	import Button from '$lib/components/ui/Button.svelte';
	import Card from '$lib/components/ui/Card.svelte';
	import { RolePermissionsState } from './role-permissions.svelte.js';

	const vm = new RolePermissionsState();

	onMount(() => {
		vm.init();
	});
</script>

<svelte:head>
	<title>Quản lý phân quyền - Admin</title>
</svelte:head>

<div class="p-6">
	<div class="mb-6">
		<h1 class="text-2xl font-bold text-gray-900">Quản lý phân quyền</h1>
		<p class="text-gray-600">Gán quyền truy cập cho các vai trò trong hệ thống</p>
	</div>

	{#if vm.loading}
		<div class="py-12 text-center">
			<div
				class="inline-block h-12 w-12 animate-spin rounded-full border-b-2 border-indigo-600"
			></div>
		</div>
	{:else}
		<div class="grid grid-cols-1 gap-6 lg:grid-cols-[280px_1fr]">
			<!-- Left Sidebar: Role List -->
			<Card>
				<h2 class="mb-4 text-lg font-semibold text-gray-800">Vai trò</h2>
				<div class="space-y-3">
					{#each vm.roles as role (role.id)}
						<button
							class="w-full rounded-lg border-2 p-3 text-left transition-all duration-200 {vm.selectedRoleId ===
							role.id
								? 'border-indigo-500 bg-indigo-50'
								: 'border-gray-100 hover:border-indigo-200 hover:bg-gray-50'}"
							onclick={() => vm.selectRole(role.id)}
						>
							<div class="mb-1 font-medium text-gray-900">{role.name}</div>
							<div class="text-sm text-gray-500">{role.description}</div>
						</button>
					{/each}
				</div>
			</Card>

			<!-- Main Area: Permission Matrix -->
			{#if vm.selectedRole}
				<div class="space-y-6">
					<!-- Header & Actions -->
					<Card>
						<div class="flex items-center justify-between">
							<div>
								<h2 class="text-xl font-bold text-gray-900">Quyền của {vm.selectedRole.name}</h2>
								<div class="mt-1 text-sm text-gray-500">
									Đang chọn {vm.rolePermissionIds.length} quyền
								</div>
							</div>
							<Button variant="primary" onclick={() => vm.savePermissions()} disabled={vm.saving}>
								{vm.saving ? 'Đang lưu...' : 'Lưu thay đổi'}
							</Button>
						</div>
					</Card>

					<!-- Quick Templates -->
					<div
						class="rounded-xl bg-gradient-to-r from-indigo-500 to-purple-600 p-6 text-white shadow-lg"
					>
						<h3 class="mb-2 flex items-center gap-2 text-lg font-bold">⚡ Phân quyền nhanh</h3>
						<p class="mb-4 text-sm text-white/90">
							Chọn mẫu quyền phổ biến để gán nhanh cho vai trò này
						</p>
						<div class="grid grid-cols-1 gap-3 sm:grid-cols-2 xl:grid-cols-4">
							{#each Object.entries(vm.permissionTemplates) as [key, template] (key)}
								<button
									class="rounded-lg border border-white/20 bg-white/10 p-3 text-left backdrop-blur-sm transition-all hover:bg-white/20 active:scale-95"
									onclick={() => vm.applyTemplate(key as keyof typeof vm.permissionTemplates)}
								>
									<div class="mb-1 text-sm font-semibold">{template.name}</div>
									<div class="line-clamp-2 text-xs text-white/80">{template.description}</div>
								</button>
							{/each}
						</div>
					</div>

					<!-- Permissions Grid -->
					<div class="grid gap-6">
						{#each Object.entries(vm.permissionsByModule) as [moduleName, permissions] (moduleName)}
							<Card>
								<div class="mb-4 flex items-center justify-between border-b pb-3">
									<button
										class="-ml-2 flex items-center gap-3 rounded-lg p-2 transition-colors hover:bg-gray-50"
										onclick={() => vm.toggleModule(moduleName)}
									>
										<div
											class="flex h-5 w-5 items-center justify-center rounded border {vm.isModuleFullySelected(
												moduleName
											)
												? 'border-indigo-600 bg-indigo-600 text-white'
												: 'border-gray-300 text-transparent'}"
										>
											✓
										</div>
										<span class="text-lg font-semibold text-gray-800 capitalize">{moduleName}</span>
									</button>
									<span
										class="rounded-full bg-gray-100 px-2.5 py-1 text-xs font-medium text-gray-600"
									>
										{vm.getModuleProgress(moduleName)}
									</span>
								</div>

								<div class="grid grid-cols-1 gap-3 md:grid-cols-2 xl:grid-cols-3">
									{#each permissions as permission (permission.id)}
										<label
											class="group flex cursor-pointer items-start gap-3 rounded-lg border border-gray-100 p-3 transition-colors hover:bg-gray-50"
										>
											<input
												type="checkbox"
												checked={vm.rolePermissionIds.includes(permission.id)}
												onchange={() => vm.togglePermission(permission.id)}
												class="mt-1 h-4 w-4 rounded border-gray-300 text-indigo-600 focus:ring-indigo-500"
											/>
											<div>
												<div
													class="font-medium text-gray-900 transition-colors group-hover:text-indigo-700"
												>
													{permission.name}
												</div>
												<div class="mt-0.5 font-mono text-xs text-gray-400">
													{permission.code}
												</div>
											</div>
										</label>
									{/each}
								</div>
							</Card>
						{/each}
					</div>
				</div>
			{:else}
				<div
					class="flex flex-col items-center justify-center rounded-xl border-2 border-dashed border-gray-200 bg-white p-12 text-gray-400"
				>
					<div class="mb-3 text-4xl">👈</div>
					<div class="text-lg">Chọn một vai trò bên trái để bắt đầu phân quyền</div>
				</div>
			{/if}
		</div>
	{/if}
</div>
