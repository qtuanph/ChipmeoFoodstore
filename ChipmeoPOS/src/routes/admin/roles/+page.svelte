<script lang="ts">
	import { onMount } from 'svelte';
	import Button from '$lib/components/Button.svelte';
	import Card from '$lib/components/Card.svelte';
	import Modal from '$lib/components/Modal.svelte';
	import Toast from '$lib/components/Toast.svelte';
	import { RolesState } from './roles.svelte.js';

	const vm = new RolesState();

	onMount(() => {
		vm.init();
	});
</script>

<svelte:head><title>Quản lý Vai trò - Admin</title></svelte:head>

<div class="p-6">
	<div class="mb-6 flex items-center justify-between">
		<div>
			<h1 class="text-2xl font-bold">Quản lý Vai trò</h1>
			<p class="text-gray-600">Vai trò & phân quyền</p>
		</div>
		<Button variant="primary" onclick={() => vm.openCreateModal()}>+ Thêm vai trò</Button>
	</div>

	{#if vm.loading}
		<div class="py-12 text-center">
			<div
				class="inline-block h-12 w-12 animate-spin rounded-full border-b-2 border-indigo-600"
			></div>
		</div>
	{:else}
		<div class="grid grid-cols-1 gap-6 lg:grid-cols-2">
			<!-- Roles -->
			<Card>
				<h2 class="mb-4 text-lg font-bold">Danh sách vai trò</h2>
				<div class="space-y-3">
					{#each vm.roles as role (role.id)}
						<div class="rounded-lg border p-4 hover:bg-gray-50">
							<div class="flex items-start justify-between">
								<div>
									<h3 class="font-semibold">{role.name}</h3>
									<p class="text-sm text-gray-600">{role.description || 'Không có mô tả'}</p>
								</div>
								<div class="space-x-2">
									<button onclick={() => vm.openEditModal(role)} class="text-sm text-indigo-600"
										>Sửa</button
									>
									<button onclick={() => vm.handleDelete(role.id)} class="text-sm text-red-600"
										>Xóa</button
									>
								</div>
							</div>
						</div>
					{/each}
				</div>
			</Card>

			<!-- Permissions -->
			<Card>
				<h2 class="mb-4 text-lg font-bold">Danh sách quyền ({vm.permissions.length})</h2>
				<div class="max-h-[600px] space-y-2 overflow-y-auto">
					{#each vm.permissions as perm (perm.id || perm.code)}
						<div class="rounded bg-gray-50 p-3">
							<div class="text-sm font-medium">{perm.name}</div>
							<div class="text-xs text-gray-500">{perm.code}</div>
						</div>
					{/each}
				</div>
			</Card>
		</div>
	{/if}
</div>

<Modal
	bind:open={vm.showModal}
	title={vm.editingItem ? 'Sửa vai trò' : 'Thêm vai trò'}
	onClose={() => (vm.showModal = false)}
>
	<form
		onsubmit={(e) => {
			e.preventDefault();
			vm.handleSubmit();
		}}
		class="space-y-4"
	>
		<div>
			<label for="name" class="mb-2 block text-sm font-medium">Tên vai trò</label>
			<input
				id="name"
				type="text"
				bind:value={vm.formData.name}
				required
				class="w-full rounded-lg border px-4 py-2"
			/>
		</div>
		<div>
			<label for="desc" class="mb-2 block text-sm font-medium">Mô tả</label>
			<input
				id="desc"
				type="text"
				bind:value={vm.formData.description}
				class="w-full rounded-lg border px-4 py-2"
			/>
		</div>
		<div>
			<label for="defaultRoute" class="mb-2 block text-sm font-medium">Trang mặc định</label>
			<select
				id="defaultRoute"
				bind:value={vm.formData.defaultRoute}
				class="w-full rounded-lg border px-4 py-2"
			>
				<option value="/admin">Admin Dashboard (/admin)</option>
				<option value="/pos">POS Bán hàng (/pos)</option>
				<option value="/kitchen">Bếp (/kitchen)</option>
			</select>
		</div>
		<div class="flex items-center">
			<input type="checkbox" bind:checked={vm.formData.isActive} id="isActive" class="mr-2" />
			<label for="isActive" class="text-sm">Hoạt động</label>
		</div>
		<div class="flex gap-3">
			<Button variant="primary" type="submit" fullWidth={true}
				>{vm.editingItem ? 'Cập nhật' : 'Tạo mới'}</Button
			>
			<Button variant="secondary" onclick={() => (vm.showModal = false)} fullWidth={true}
				>Hủy</Button
			>
		</div>
	</form>
</Modal>

<Toast bind:show={vm.showToast} message={vm.toastMessage} type={vm.toastType} />
