<script lang="ts">
	import { onMount } from 'svelte';
	import Button from '$lib/components/Button.svelte';
	import Card from '$lib/components/Card.svelte';
	import Modal from '$lib/components/Modal.svelte';
	import Toast from '$lib/components/Toast.svelte';
	import { SourcesState } from './sources.svelte.js';

	const vm = new SourcesState();

	onMount(() => {
		vm.init();
	});
</script>

<svelte:head><title>Quản lý Nguồn đơn - Admin</title></svelte:head>

<div class="p-6">
	<div class="mb-6 flex items-center justify-between">
		<div>
			<h1 class="text-2xl font-bold">Quản lý Nguồn đơn</h1>
			<p class="text-gray-600">Danh sách nguồn đơn hàng (Tại quán, App, Mang về...)</p>
		</div>
		<Button variant="primary" onclick={() => vm.openCreateModal()}>+ Thêm nguồn</Button>
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
							<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">ID</th>
							<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase"
								>Tên nguồn</th
							>
							<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase"
								>Hoạt động</th
							>
							<th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase"
								>Thao tác</th
							>
						</tr>
					</thead>
					<tbody class="divide-y">
						{#each vm.sources as item (item.id)}
							<tr class="hover:bg-gray-50">
								<td class="px-6 py-4 text-sm">{item.id}</td>
								<td class="px-6 py-4 text-sm font-medium">{item.name}</td>
								<td class="px-6 py-4 text-sm">
									{#if item.isActive}
										<span class="rounded-full bg-green-100 px-2 py-1 text-xs text-green-800"
											>Có</span
										>
									{:else}
										<span class="rounded-full bg-gray-100 px-2 py-1 text-xs text-gray-800"
											>Không</span
										>
									{/if}
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
	title={vm.editingItem ? 'Sửa nguồn' : 'Thêm nguồn'}
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
			<label for="sourceName" class="mb-2 block text-sm font-medium">Tên nguồn</label>
			<input
				id="sourceName"
				type="text"
				bind:value={vm.formData.name}
				required
				class="w-full rounded-lg border px-4 py-2 focus:ring-2 focus:ring-indigo-500"
			/>
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
