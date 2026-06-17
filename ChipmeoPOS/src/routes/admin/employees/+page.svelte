<script lang="ts">
	import { onMount } from 'svelte';
	import Button from '$lib/components/ui/Button.svelte';
	import Card from '$lib/components/ui/Card.svelte';
	import Modal from '$lib/components/ui/Modal.svelte';
	import Toast from '$lib/components/ui/Toast.svelte';
	import { EmployeesState } from './employees.svelte.js';

	const vm = new EmployeesState();
	let avatarInput: HTMLInputElement | null = $state(null);

	onMount(() => {
		vm.init();
	});

	function handleAvatarSelect(event: Event) {
		const input = event.target as HTMLInputElement;
		const file = input.files?.[0];
		if (!file) return;
		vm.handleAvatarSelect(file);
		input.value = '';
	}
</script>

<svelte:head><title>Quản lý Nhân viên - Admin</title></svelte:head>

<div class="p-6">
	<div class="mb-6 flex items-center justify-between">
		<div>
			<h1 class="text-2xl font-bold">Quản lý Nhân viên</h1>
			<p class="text-gray-600">Danh sách nhân viên</p>
		</div>
		<Button variant="primary" onclick={() => vm.openCreateModal()}>+ Thêm nhân viên</Button>
	</div>

	{#if vm.loading}
		<div class="py-12 text-center">
			<div
				class="inline-block h-12 w-12 animate-spin rounded-full border-b-2 border-indigo-600"
			></div>
		</div>
	{:else}
		<Card>
			<div class="overflow-x-auto">
				<table class="w-full">
					<thead class="border-b bg-gray-50">
						<tr>
							<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Avatar</th
							>
							<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Họ tên</th
							>
							<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase"
								>Username</th
							>
							<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Email</th>
							<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase"
								>Vai trò</th
							>
							<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase"
								>Trạng thái</th
							>
							<th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase"
								>Thao tác</th
							>
						</tr>
					</thead>
					<tbody class="divide-y">
						{#each vm.employees as item (item.id)}
							<tr class="hover:bg-gray-50">
								<td class="px-6 py-4">
									{#if item.avatarUrl}
										<img
											src={item.avatarUrl}
											alt={item.fullName}
											class="h-10 w-10 rounded-full object-cover"
										/>
									{:else}
										<div
											class="flex h-10 w-10 items-center justify-center rounded-full bg-gray-200 text-gray-500"
										>
											👤
										</div>
									{/if}
								</td>
								<td class="px-6 py-4 text-sm font-medium">{item.fullName}</td>
								<td class="px-6 py-4 text-sm">{item.username}</td>
								<td class="px-6 py-4 text-sm">{item.email || 'N/A'}</td>
								<td class="px-6 py-4 text-sm">{item.roleName}</td>
								<td class="px-6 py-4">
									<span
										class="rounded-full px-2 py-1 text-xs {item.isActive
											? 'bg-green-100 text-green-800'
											: 'bg-gray-100 text-gray-800'}"
									>
										{item.isActive ? 'Hoạt động' : 'Tắt'}
									</span>
								</td>
								<td class="space-x-2 px-6 py-4 text-right">
									<button
										onclick={() => vm.openEditModal(item)}
										class="text-indigo-600 hover:text-indigo-900">Sửa</button
									>
									<button
										onclick={() => vm.handleDelete(item.id)}
										class="text-red-600 hover:text-red-900">Xóa</button
									>
								</td>
							</tr>
						{/each}
					</tbody>
				</table>
			</div>
		</Card>
	{/if}
</div>

<Modal
	open={vm.showModal}
	title={vm.editingItem ? 'Sửa nhân viên' : 'Thêm nhân viên'}
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
			<label for="fullName" class="mb-2 block text-sm font-medium">Họ tên</label>
			<input
				id="fullName"
				type="text"
				bind:value={vm.formData.fullName}
				required
				class="w-full rounded-lg border px-4 py-2"
			/>
		</div>
		<div>
			<label for="username" class="mb-2 block text-sm font-medium">Username</label>
			<input
				id="username"
				type="text"
				bind:value={vm.formData.username}
				required
				class="w-full rounded-lg border px-4 py-2"
			/>
		</div>
		<div>
			<label for="password" class="mb-2 block text-sm font-medium"
				>Password {vm.editingItem ? '(để trống nếu không đổi)' : ''}</label
			>
			<input
				id="password"
				type="password"
				bind:value={vm.formData.password}
				required={!vm.editingItem}
				class="w-full rounded-lg border px-4 py-2"
			/>
		</div>
		<div>
			<label for="email" class="mb-2 block text-sm font-medium">Email</label>
			<input
				id="email"
				type="email"
				bind:value={vm.formData.email}
				class="w-full rounded-lg border px-4 py-2"
			/>
		</div>
		<div>
			<label for="phone" class="mb-2 block text-sm font-medium">Số điện thoại</label>
			<input
				id="phone"
				type="tel"
				bind:value={vm.formData.phone}
				class="w-full rounded-lg border px-4 py-2"
			/>
		</div>
		<!-- Avatar Upload -->
		<div>
			<p class="mb-2 block text-sm font-medium">Avatar</p>
			<div class="flex items-center gap-4">
				{#if vm.imagePreview || vm.formData.avatarUrl}
					<img
						src={vm.imagePreview || vm.formData.avatarUrl}
						alt="Avatar"
						class="h-16 w-16 rounded-full border object-cover"
					/>
				{:else}
					<div
						class="flex h-16 w-16 items-center justify-center rounded-full border bg-gray-100 text-gray-400"
					>
						👤
					</div>
				{/if}
				<div class="flex flex-col gap-2">
					<button
						type="button"
						onclick={() => avatarInput?.click()}
						class="rounded-lg bg-amber-100 px-3 py-1.5 text-sm text-amber-700 hover:bg-amber-200"
					>
						📷 Chọn ảnh
					</button>
					{#if vm.imagePreview || vm.formData.avatarUrl}
						<button
							type="button"
							onclick={() => vm.clearAvatar()}
							class="px-3 py-1.5 text-sm text-red-600 hover:underline"
						>
							Xóa avatar
						</button>
					{/if}
				</div>
			</div>
		</div>
		<div>
			<label for="roleId" class="mb-2 block text-sm font-medium">Vai trò</label>
			<select
				id="roleId"
				bind:value={vm.formData.roleId}
				required
				class="w-full rounded-lg border px-4 py-2"
			>
				{#each vm.roles as role (role.id)}
					<option value={role.id}>{role.name}</option>
				{/each}
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

<!-- Hidden File Input -->
<input
	type="file"
	accept="image/*"
	bind:this={avatarInput}
	onchange={handleAvatarSelect}
	class="hidden"
/>
