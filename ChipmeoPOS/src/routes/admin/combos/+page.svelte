<script lang="ts">
	import { onMount } from 'svelte';
	import { formatCurrency } from '$lib/utils/index.js';
	import Button from '$lib/components/ui/Button.svelte';
	import Card from '$lib/components/ui/Card.svelte';
	import Modal from '$lib/components/ui/Modal.svelte';
	import Toast from '$lib/components/ui/Toast.svelte';
	import ImageCropper from '$lib/components/ImageCropper.svelte';
	import { CombosState } from './combos.svelte.js';

	const vm = new CombosState();
	let imageInput: HTMLInputElement | null = $state(null);

	onMount(() => {
		vm.init();
	});

	function handleImageSelect(event: Event) {
		const input = event.target as HTMLInputElement;
		const file = input.files?.[0];
		if (!file) return;
		vm.handleImageSelect(file);
		input.value = '';
	}
</script>

<svelte:head><title>Quản lý Combo - Admin</title></svelte:head>

<div class="p-6">
	<div class="mb-6 flex items-center justify-between">
		<div>
			<h1 class="text-2xl font-bold">Quản lý Combo</h1>
			<p class="text-gray-600">Combo món ăn</p>
		</div>
		<Button variant="primary" onclick={() => vm.openCreateModal()}>+ Thêm combo</Button>
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
							<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Ảnh</th>
							<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase"
								>Tên combo</th
							>
							<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Mô tả</th>
							<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Giá</th>
							<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Số món</th
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
						{#each vm.combos as item (item.id)}
							<tr class="hover:bg-gray-50">
								<td class="px-6 py-4">
									{#if item.imageUrl}
										<img
											src={item.imageUrl}
											alt={item.name}
											class="h-12 w-12 rounded-lg object-cover"
										/>
									{:else}
										<div
											class="flex h-12 w-12 items-center justify-center rounded-lg bg-gray-200 text-gray-400"
										>
											🍱
										</div>
									{/if}
								</td>
								<td class="px-6 py-4 text-sm font-medium">{item.name}</td>
								<td
									class="max-w-xs truncate px-6 py-4 text-sm text-gray-500"
									title={item.description}>{item.description || '-'}</td
								>
								<td class="px-6 py-4 text-sm font-semibold">{formatCurrency(item.comboPrice)}</td>
								<td class="px-6 py-4 text-sm">{item.items?.length || 0} món</td>
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
	title={vm.editingItem ? 'Sửa combo' : 'Thêm combo'}
	onClose={() => (vm.showModal = false)}
	size="lg"
>
	<form
		onsubmit={(e) => {
			e.preventDefault();
			vm.handleSubmit();
		}}
		class="space-y-4"
	>
		<div>
			<label for="name" class="mb-2 block text-sm font-medium">Tên combo</label>
			<input
				id="name"
				type="text"
				bind:value={vm.formData.name}
				required
				class="w-full rounded-lg border px-4 py-2"
			/>
		</div>
		<div>
			<label for="price" class="mb-2 block text-sm font-medium">Giá combo</label>
			<input
				id="price"
				type="number"
				bind:value={vm.formData.comboPrice}
				required
				min="0"
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
		<!-- Image Upload -->
		<div>
			<p class="mb-2 block text-sm font-medium">Ảnh combo</p>
			<div class="flex items-center gap-4">
				{#if vm.imagePreview || vm.formData.imageUrl}
					<img
						src={vm.imagePreview || vm.formData.imageUrl}
						alt="Preview"
						class="h-16 w-16 rounded-lg border object-cover"
					/>
				{:else}
					<div
						class="flex h-16 w-16 items-center justify-center rounded-lg border bg-gray-100 text-gray-400"
					>
						🍱
					</div>
				{/if}
				<div class="flex flex-col gap-2">
					<button
						type="button"
						onclick={() => imageInput?.click()}
						class="rounded-lg bg-amber-100 px-3 py-1.5 text-sm text-amber-700 hover:bg-amber-200"
					>
						📷 Chọn ảnh
					</button>
					{#if vm.imagePreview || vm.formData.imageUrl}
						<button
							type="button"
							onclick={() => vm.clearImage()}
							class="px-3 py-1.5 text-sm text-red-600 hover:underline"
						>
							Xóa ảnh
						</button>
					{/if}
				</div>
			</div>
		</div>
		<div>
			<div class="mb-2 flex items-center justify-between">
				<span class="block text-sm font-medium">Món trong combo</span>
				<button type="button" onclick={() => vm.addComboItem()} class="text-sm text-indigo-600"
					>+ Thêm món</button
				>
			</div>
			{#each vm.formData.items as item, index (index)}
				<div class="mb-2 flex gap-2">
					<select
						value={item.menuItemId}
						onchange={(e) =>
							vm.updateComboItem(
								index,
								'menuItemId',
								parseInt((e.target as HTMLSelectElement).value)
							)}
						class="flex-1 rounded-lg border px-4 py-2"
					>
						{#each vm.menuItems as mi (mi.id)}
							<option value={mi.id}>{mi.name}</option>
						{/each}
					</select>
					<input
						type="number"
						value={item.quantity}
						onchange={(e) =>
							vm.updateComboItem(index, 'quantity', parseInt((e.target as HTMLInputElement).value))}
						min="1"
						placeholder="SL"
						class="w-20 rounded-lg border px-4 py-2"
					/>
					<button type="button" onclick={() => vm.removeComboItem(index)} class="text-red-600"
						>Xóa</button
					>
				</div>
			{/each}
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
	bind:this={imageInput}
	onchange={handleImageSelect}
	class="hidden"
/>

{#if vm.cropImageSrc}
	<ImageCropper
		imageSrc={vm.cropImageSrc}
		aspectRatio={1}
		onCrop={(blob) => vm.handleCrop(blob)}
		onCancel={() => vm.handleCancelCrop()}
	/>
{/if}
